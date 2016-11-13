#define CALCULATE_HARALICK
#define CALCULATE_GABOR
#define CALCULATE_MARKOV

using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using openDicom.File;

namespace BRISC.Core
{
    /// <summary>
    /// Converts LIDC data into .srs and .xml files for series and nodule viewers
    /// </summary>
    public class LIDCImport
    {
        /// <summary>
        /// Distance nodule centroids have to be from each other to be considered different nodules
        /// </summary>
        public static double CENTROID_THRESHOLD = 10.0;

        /// <summary>
        /// Next unique nodule number
        /// </summary>
        private static int nodule_no = 0;

        /// <summary>
        /// Import all studies in a root folder (ie. "C:\LIDC")
        /// </summary>
        /// <param name="folder">Path to root folder</param>
        public static void ImportAllStudies(string folder)
        {
            // initialize dialog
            BRISC.GUI.Splash dialog = new BRISC.GUI.Splash();
            dialog.FirstCheck.Text = "Import LIDC data";
            dialog.SecondCheck.Visible = false;
            dialog.ThirdCheck.Visible = false;
            dialog.FourthCheck.Visible = false;

            // main nodule database
            LIDCNoduleDB noduleDB = new LIDCNoduleDB();

            // get list of studies in folder
            DirectoryInfo dir = new DirectoryInfo(folder);
            DirectoryInfo[] subdirs = dir.GetDirectories();

            // update dialog
            dialog.Progress.Minimum = 0;
            dialog.Progress.Maximum = subdirs.Length;
            dialog.Progress.Value = 0;
            dialog.Show();

            //int seriesDone = 0;
            foreach (DirectoryInfo subdir in subdirs)
            {
                foreach (DirectoryInfo studydir in subdir.GetDirectories())
                {
                    foreach (DirectoryInfo seriesdir in studydir.GetDirectories())
                    {
                        /*Dictionary<string, Dictionary<string, bool>>physicianAgreement = new Dictionary<string, Dictionary<string, bool>>();
                            foreach (LIDCNodule nodule2 in nodules)
                            {
                                if (nodule1.NUID != nodule2.NUID)
                                {
                                    if(!physicianAgreement.ContainsKey(nodule2.Nodule_no))
                                    {
                                        physicianAgreement.Add(nodule2.Nodule_no, new Dictionary<string, bool>());
                                        physicianAgreement[nodule2.Nodule_no].Add(nodule2.NoduleID, false);
                                    }
                                    if(!physicianAgreement[nodule2.Nodule_no].ContainsKey(nodule2.NoduleID))
                                    {
                                        physicianAgreement[nodule2.Nodule_no].Add(nodule2.NoduleID, false);
                                    }
                                    if (nodule1.Nodule_no == nodule2.Nodule_no &&nodule1.NoduleID != nodule2.NoduleID &&!physicianAgreement[nodule2.Nodule_no][nodule2.NoduleID])
                                    {
                                        bool flag = true;
                                        if (nodule1.Annotations["texture"] != nodule2.Annotations["texture"])
                                        {
                                            flag = false;
                                        }
                                        if (flag)
                                        {
                                            nodule1.physicianCounter++;
                                        }
                                        physicianAgreement[nodule2.Nodule_no][nodule2.NoduleID] = true;
                                    }
                                }
                            }
                        }
                        foreach (LIDCNodule nodule in nodules)
                        {
                            if (nodule.physicianCounter >= 1)
                            {
                                noduleDB.AddNodule(nodule);
                            }
                        }
                        nodules.Clear();
                        */
                        dialog.Status.Text = "Loading series: " + seriesdir.Name;
                        dialog.Refresh();
                        Application.DoEvents();

                        // load each study
                        List<LIDCNodule> nodules = LIDCImport.ImportStudy(folder, folder + "\\" + subdir.Name, studydir.Name, seriesdir.Name,
                            dialog);
                        foreach (LIDCNodule nodule in nodules)
                            noduleDB.AddNodule(nodule);
                        nodules.Clear();
                    }
                }

                // update dialog
                dialog.Progress.Value++;
            }
            foreach (LIDCNodule nodule in noduleDB)
            {
                nodule.localHist = getHistogram(nodule, noduleDB);
                nodule.LocalHaralick.Clear();
            }
            // save xml files
            LIDCNodule.SAVE_XML_BOUNDS = true;
            noduleDB.SaveToXML(Util.DATA_PATH + "nodules-primary.xml");
            LIDCNodule.SAVE_XML_ANNOTATIONS = true;
            noduleDB.SaveToXML(Util.DATA_PATH + "nodules-annotations.xml");
            LIDCNodule.SAVE_XML_ANNOTATIONS = false;
            LIDCNodule.SAVE_XML_HARALICK = true;
            noduleDB.SaveToXML(Util.DATA_PATH + "nodules-haralick.xml");
            LIDCNodule.SAVE_XML_HARALICK = false;
            LIDCNodule.SAVE_XML_GABOR = true;
            noduleDB.SaveToXML(Util.DATA_PATH + "nodules-gabor.xml");
            LIDCNodule.SAVE_XML_GABOR = false;
            LIDCNodule.SAVE_XML_MARKOV = true;
            noduleDB.SaveToXML(Util.DATA_PATH + "nodules-markov.xml");
            LIDCNodule.SAVE_XML_MARKOV = false;
            LIDCNodule.SAVE_XML_POINTS = true;
            noduleDB.SaveToXML(Util.DATA_PATH + "nodules-points.xml");
            LIDCNodule.SAVE_XML_LOCAL = true;
            noduleDB.SaveToXML(Util.DATA_PATH + "nodules-local.xml");
            LIDCNodule.SAVE_XML_LOCAL = false;
            dialog.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lungNodule"></param>
        /// <param name="noduleList"></param>
        /// <returns></returns>
        public static ArrayList getHistogram(LIDCNodule lungNodule, LIDCNoduleDB noduleList)
        {
            int numberOfFeatures = 11;
            int numBins = 96;
            int numDist = 4;
            int numDir = 4;
            Dictionary<int, ArrayList> allHists = new Dictionary<int, ArrayList>();
            Dictionary<int, double> orderedNodules = new Dictionary<int, double>();
            Dictionary<string, double> min = noduleList.getlocalMinDictionary();
            Dictionary<string, double> max = noduleList.getlocalMaxDictionary();
            int countFeatureTotal = 0;
            int countDistDirPixels = 0;

            double[,] tempHist = null;
            int[] locationPerFeature = new int[numberOfFeatures];
            for (int i = 0; i < numberOfFeatures; i++)
            {
                locationPerFeature[i] = 0;
            }
            //combine all of the feature information per feature for the entire nodule to have a histogram done on it
            foreach (ArrayList arrayOfHist in lungNodule.LocalHaralick)
            {
                if (tempHist == null)
                {
                    int maxSize = 0;
                    foreach (ArrayList tempArrayofHist in lungNodule.LocalHaralick)
                    {
                        if (tempArrayofHist.Count > maxSize)
                        {
                            maxSize = tempArrayofHist.Count;
                        }
                    }
                    tempHist = new double[numberOfFeatures, lungNodule.LocalHaralick.Count*maxSize*numDist*numDir + 1];
                }
                foreach (Dictionary<string, double[,]> i in arrayOfHist)
                {
                    foreach (string s in i.Keys)
                    {
                        double[,] singlePixelsingleFeature = i[s];
                        //check to see if the minimum is negative - skew the results to positive if true.
                        //normalize feature vectors;
                        countDistDirPixels = locationPerFeature[countFeatureTotal];
                        for (int p = 0; p < singlePixelsingleFeature.GetLength(0); p++)
                        {
                            for (int j = 0; j < singlePixelsingleFeature.GetLength(1); j++)
                            {
                                if (max[s] - min[s] != 0)
                                {
                                    tempHist[countFeatureTotal, countDistDirPixels] = (singlePixelsingleFeature[p, j] - min[s])/
                                                                                      (max[s] - min[s]);
                                    countDistDirPixels++;
                                }
                            }
                        }
                        locationPerFeature[countFeatureTotal] = countDistDirPixels;
                        tempHist[countFeatureTotal, countDistDirPixels] = double.NaN;
                        countFeatureTotal++;
                    }
                    countFeatureTotal = 0;
                }
            }
            ArrayList AllFeatureHist = new ArrayList();
            if (tempHist != null)
            {
                //allHists is a Dictionary - each contains an array that represents the histogram for a specific 
                //nodule (the key is the nodule ID).
                double[] hist = new double[numBins];
                double[,] tempFeatureHistogram = new double[1, tempHist.GetLength(1)];
                for (int i = 0; i < numberOfFeatures; i++)
                {
                    for (int k = 0; k < tempHist.GetLength(1); k++)
                    {
                        tempFeatureHistogram[0, k] = tempHist[i, k];
                    }
                    hist = Util.ComputeHistogram(tempFeatureHistogram, numBins, 0, 1);
                    AllFeatureHist.Add(hist);
                }
            }
            return AllFeatureHist;
        }

        /// <summary>
        /// Import an LIDC study
        /// </summary>
        /// <param name="root">Main "LIDC" folder containing all the study folders</param>
        /// <param name="folder">Subfolder name</param>
        /// <param name="study">Study folder name</param>
        /// <param name="series">Series folder name</param>
        /// <returns>List of LIDCNodule objects</returns>
        public static List<LIDCNodule> ImportStudy(string root, string folder, string study, string series)
        {
            return ImportStudy(root, folder, study, series, null);
        }

        /// <summary>
        /// Import an LIDC study with a progress dialog
        /// </summary>
        /// <param name="root">Main "LIDC" folder containing all the study folders</param>
        /// <param name="folder">Subfolder name</param>
        /// <param name="study">Study folder name</param>
        /// <param name="series">Series folder name</param>
        /// <param name="dialog">Progress dialog</param>
        /// <returns>List of LIDCNodule objects</returns>
        public static List<LIDCNodule> ImportStudy(string root, string folder, string study, string series, BRISC.GUI.Splash dialog)
        {
            // list of imported nodule objects
            List<LIDCNodule> nodules = new List<LIDCNodule>();

            FeatureExtractor haralick = new GlobalCoOccurrence();
            FeatureExtractor gabor = new GaborFilter();
            FeatureExtractor markov = new MarkovRandom();
            FeatureExtractor LocalHaralick = new LocalCoOccurrence();

            // get lists of xml and dicom files
            DirectoryInfo dir = new DirectoryInfo(folder + "\\" + study + "\\" + series);
            FileInfo[] xmls = dir.GetFiles("*.xml");
            FileInfo[] dcms = dir.GetFiles("*.dcm");
            Dictionary<string, string> fileSOPs = new Dictionary<string, string>();
            SortedDictionary<double, string> fileZPos = new SortedDictionary<double, string>();

            // create hash table correlating filenames with image SOP UIDs
            foreach (FileInfo dcm in dcms)
            {
                DicomFile df = Util.LoadDICOMFile(dcm.FullName);
                openDicom.DataStructure.DataSet.DataSet ds = df.DataSet;
                openDicom.DataStructure.Tag tg = new openDicom.DataStructure.Tag("0008", "0018");
                openDicom.DataStructure.DataSet.DataElement de = ds[tg];
                openDicom.DataStructure.Value vv = de.Value;
                fileSOPs.Add(vv[0].ToString(), dcm.FullName);
                if (dialog != null)
                {
                    dialog.Status.Text = "Loading series " + series + ": " + dcm.Name;
                    Application.DoEvents();
                }
                tg = new openDicom.DataStructure.Tag("0020", "1041");
                de = ds[tg];
                vv = de.Value;
                double zpos = double.Parse(vv[0].ToString());
                fileZPos.Add(zpos, dcm.FullName);
            }

            // create series file
            TextWriter fout = new StreamWriter(root + "\\" + series + ".srs");
            foreach (double zpos in fileZPos.Keys)
            {
                fout.WriteLine(zpos.ToString() + "\t" + fileZPos[zpos]);
            }
            fout.Close();

            // create LIDCNodule objects
            foreach (FileInfo xml in xmls)
            {
                // open XML document
                XmlDocument doc = new XmlDocument();
                XmlTextReader reader = new XmlTextReader(xml.FullName);
                doc.Load(reader);
                reader.Close();

                XmlNodeList sessions = doc.FirstChild.NextSibling.ChildNodes;
                foreach (XmlNode session in sessions)
                {
                    // each physician has a separate session in the xml file
                    if (session.Name == "readingSession")
                    {
                        XmlNodeList nodes = session.ChildNodes;
                        foreach (XmlNode node in nodes)
                        {
                            // physicians sometimes mark multiple nodules
                            if (node.Name == "unblindedReadNodule")
                            {
                                // annotations and nodule ID are consistent across slices
                                Dictionary<string, int> Annotations = new Dictionary<string, int>();
                                string noduleID = "";
                                foreach (XmlNode info in node.ChildNodes)
                                {
                                    if (info.Name == "noduleID")
                                    {
                                        noduleID = info.FirstChild.Value.ToString();
                                        if (noduleID.StartsWith("Nodule "))
                                            noduleID = noduleID.Substring(7);
                                        if (dialog != null)
                                        {
                                            dialog.Status.Text = "Loading series " + series + ": Nodule " + noduleID;
                                            Application.DoEvents();
                                        }
                                    }
                                    else if (info.Name == "characteristics")
                                    {
                                        // store annotations in temporary structure
                                        foreach (XmlNode annotation in info.ChildNodes)
                                            Annotations.Add(annotation.Name, int.Parse(annotation.FirstChild.Value));
                                    }
                                    else if (info.Name == "roi")
                                    {
                                        // each slice = different LIDCNodule instance
                                        LIDCNodule nodule = new LIDCNodule();
                                        bool exclude = false;
                                        foreach (XmlNode attribute in info.ChildNodes)
                                        {
                                            if (attribute.Name == "imageSOP_UID")
                                            {
                                                // set SOP UID and retrieve the corresponding DICOM filename
                                                nodule.ImageSOP_UID = attribute.FirstChild.Value.ToString();
                                                if (fileSOPs.ContainsKey(nodule.ImageSOP_UID))
                                                    nodule.OriginalDICOMFilename = fileSOPs[nodule.ImageSOP_UID];
                                            }
                                            else if (attribute.Name == "inclusion")
                                            {
                                                string value = attribute.FirstChild.Value.ToString();
                                                if (value.ToLower() == "false")
                                                    exclude = true;
                                            }
                                            else if (attribute.Name == "edgeMap")
                                            {
                                                // add contour point
                                                int x = 0, y = 0;
                                                foreach (XmlNode coord in attribute.ChildNodes)
                                                {
                                                    if (coord.Name == "xCoord")
                                                        x = int.Parse(coord.FirstChild.Value);
                                                    else if (coord.Name == "yCoord")
                                                        y = int.Parse(coord.FirstChild.Value);
                                                }
                                                // set min/max boundaries
                                                if (x < nodule.MinX)
                                                    nodule.MinX = x;
                                                if (x > nodule.MaxX)
                                                    nodule.MaxX = x;
                                                if (y < nodule.MinY)
                                                    nodule.MinY = y;
                                                if (y > nodule.MaxY)
                                                    nodule.MaxY = y;
                                                nodule.Width = nodule.MaxX - nodule.MinX + 1;
                                                nodule.Height = nodule.MaxY - nodule.MinY + 1;
                                                nodule.Points.Add(new System.Drawing.Point(x, y));
                                            }
                                        }
                                        foreach (string key in Annotations.Keys)
                                        {
                                            // copy in stored annotations
                                            nodule.Annotations.Add(key, Annotations[key]);
                                        }
                                        nodule.NoduleID = noduleID;
                                        nodule.StudyInstanceUID = study;
                                        nodule.SeriesInstanceUID = series;

                                        // only add nodules with images with no dimension smaller than five
                                        if (!exclude && nodule.OriginalDICOMFilename != null &&
                                            nodule.Width >= 5 && nodule.Height >= 5)
                                        {
                                            nodule.GetActualSize();

                                            // calculate centroid and compare with other images
                                            nodule.CalculateCentroid();
                                            foreach (LIDCNodule n in nodules)
                                            {
                                                if (nodule.SeriesInstanceUID == n.SeriesInstanceUID && nodule.NoduleID == n.NoduleID)
                                                {
                                                    // same as another image
                                                    nodule.Nodule_no = n.Nodule_no;
                                                    break;
                                                }
                                                //double dist = Math.Sqrt(Math.Pow(nodule.CentroidX - n.CentroidX, 2) + Math.Pow(nodule.CentroidY - n.CentroidY, 2));
                                                //if (dist < CENTROID_THRESHOLD)
                                                if (Math.Abs(nodule.CentroidX - n.CentroidX) < CENTROID_THRESHOLD &&
                                                    Math.Abs(nodule.CentroidY - n.CentroidY) < CENTROID_THRESHOLD &&
                                                    nodule.SeriesInstanceUID == n.SeriesInstanceUID)
                                                {
                                                    // same as another image
                                                    nodule.Nodule_no = n.Nodule_no;
                                                    break;
                                                }
                                            }
                                            if (nodule.Nodule_no.Equals(""))
                                            {
                                                // new nodule
                                                nodule.Nodule_no = (++nodule_no).ToString();
                                            }

                                            // calculate features
#if CALCULATE_HARALICK
                                            haralick.ExtractFeatures(nodule);
#endif
#if CALCULATE_GABOR
                                            gabor.ExtractFeatures(nodule);
#endif
#if CALCULATE_MARKOV
                                            markov.ExtractFeatures(nodule);
#endif
                                            LocalHaralick.ExtractFeatures(nodule);

                                            // free up memory
                                            nodule.PurgePixelData();

                                            nodules.Add(nodule);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return nodules;
        }
    }
}