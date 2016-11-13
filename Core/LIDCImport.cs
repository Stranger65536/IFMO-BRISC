#define CALCULATE_HARALICK
#define CALCULATE_GABOR
#define CALCULATE_MARKOV


using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using BRISC.GUI;
using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;
using openDicom.File;


namespace BRISC.Core
{
    public class LIDCImport
    {
        public static double CENTROID_THRESHOLD = 10.0;


        private static int nodule_no;


        public static void ImportAllStudies(string folder)
        {
            // initialize dialog
            var dialog = new Splash();
            dialog.FirstCheck.Text = "Import LIDC data";
            dialog.SecondCheck.Visible = false;
            dialog.ThirdCheck.Visible = false;
            dialog.FourthCheck.Visible = false;

            // main nodule database
            var noduleDB = new LIDCNoduleDB();

            // get list of studies in folder
            var dir = new DirectoryInfo(folder);
            var subdirs = dir.GetDirectories();

            // update dialog
            dialog.Progress.Minimum = 0;
            dialog.Progress.Maximum = subdirs.Length;
            dialog.Progress.Value = 0;
            dialog.Show();

            //int seriesDone = 0;
            foreach (var subdir in subdirs)
            {
                foreach (var studydir in subdir.GetDirectories())
                {
                    foreach (var seriesdir in studydir.GetDirectories())
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
                        var nodules = ImportStudy(folder, folder + "\\" + subdir.Name, studydir.Name, seriesdir.Name, dialog);
                        foreach (var nodule in nodules)
                            noduleDB.AddNodule(nodule);
                        nodules.Clear();
                    }
                }

                // update dialog
                dialog.Progress.Value++;
            }
            foreach (var nodule in noduleDB)
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

        public static ArrayList getHistogram(LIDCNodule lungNodule, LIDCNoduleDB noduleList)
        {
            var numberOfFeatures = 11;
            var numBins = 96;
            var numDist = 4;
            var numDir = 4;
            var allHists = new Dictionary<int, ArrayList>();
            var orderedNodules = new Dictionary<int, double>();
            var min = noduleList.getlocalMinDictionary();
            var max = noduleList.getlocalMaxDictionary();
            var countFeatureTotal = 0;
            var countDistDirPixels = 0;

            double[,] tempHist = null;
            var locationPerFeature = new int[numberOfFeatures];
            for (var i = 0; i < numberOfFeatures; i++)
            {
                locationPerFeature[i] = 0;
            }
            //combine all of the feature information per feature for the entire nodule to have a histogram done on it
            foreach (ArrayList arrayOfHist in lungNodule.LocalHaralick)
            {
                if (tempHist == null)
                {
                    var maxSize = 0;
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
                    foreach (var s in i.Keys)
                    {
                        var singlePixelsingleFeature = i[s];
                        //check to see if the minimum is negative - skew the results to positive if true.
                        //normalize feature vectors;
                        countDistDirPixels = locationPerFeature[countFeatureTotal];
                        for (var p = 0; p < singlePixelsingleFeature.GetLength(0); p++)
                        {
                            for (var j = 0; j < singlePixelsingleFeature.GetLength(1); j++)
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
            var AllFeatureHist = new ArrayList();
            if (tempHist != null)
            {
                //allHists is a Dictionary - each contains an array that represents the histogram for a specific 
                //nodule (the key is the nodule ID).
                var hist = new double[numBins];
                var tempFeatureHistogram = new double[1, tempHist.GetLength(1)];
                for (var i = 0; i < numberOfFeatures; i++)
                {
                    for (var k = 0; k < tempHist.GetLength(1); k++)
                    {
                        tempFeatureHistogram[0, k] = tempHist[i, k];
                    }
                    hist = Util.ComputeHistogram(tempFeatureHistogram, numBins, 0, 1);
                    AllFeatureHist.Add(hist);
                }
            }
            return AllFeatureHist;
        }


        public static List<LIDCNodule> ImportStudy(string root, string folder, string study, string series)
        {
            return ImportStudy(root, folder, study, series, null);
        }


        public static List<LIDCNodule> ImportStudy(string root, string folder, string study, string series, Splash dialog)
        {
            // list of imported nodule objects
            var nodules = new List<LIDCNodule>();

            FeatureExtractor haralick = new GlobalCoOccurrence();
            FeatureExtractor gabor = new GaborFilter();
            FeatureExtractor markov = new MarkovRandom();
            FeatureExtractor LocalHaralick = new LocalCoOccurrence();

            // get lists of xml and dicom files
            var dir = new DirectoryInfo(folder + "\\" + study + "\\" + series);
            var xmls = dir.GetFiles("*.xml");
            var dcms = dir.GetFiles("*.dcm");
            var fileSOPs = new Dictionary<string, string>();
            var fileZPos = new SortedDictionary<double, string>();

            // create hash table correlating filenames with image SOP UIDs
            foreach (var dcm in dcms)
            {
                var df = Util.LoadDICOMFile(dcm.FullName);
                var ds = df.DataSet;
                var tg = new Tag("0008", "0018");
                var de = ds[tg];
                var vv = de.Value;
                fileSOPs.Add(vv[0].ToString(), dcm.FullName);
                if (dialog != null)
                {
                    dialog.Status.Text = "Loading series " + series + ": " + dcm.Name;
                    Application.DoEvents();
                }
                tg = new Tag("0020", "1041");
                de = ds[tg];
                vv = de.Value;
                var zpos = double.Parse(vv[0].ToString());
                fileZPos.Add(zpos, dcm.FullName);
            }

            // create series file
            TextWriter fout = new StreamWriter(root + "\\" + series + ".srs");
            foreach (var zpos in fileZPos.Keys)
            {
                fout.WriteLine(zpos + "\t" + fileZPos[zpos]);
            }
            fout.Close();

            // create LIDCNodule objects
            foreach (var xml in xmls)
            {
                // open XML document
                var doc = new XmlDocument();
                var reader = new XmlTextReader(xml.FullName);
                doc.Load(reader);
                reader.Close();

                var sessions = doc.FirstChild.NextSibling.ChildNodes;
                foreach (XmlNode session in sessions)
                {
                    // each physician has a separate session in the xml file
                    if (session.Name == "readingSession")
                    {
                        var nodes = session.ChildNodes;
                        foreach (XmlNode node in nodes)
                        {
                            // physicians sometimes mark multiple nodules
                            if (node.Name == "unblindedReadNodule")
                            {
                                // annotations and nodule ID are consistent across slices
                                var Annotations = new Dictionary<string, int>();
                                var noduleID = "";
                                foreach (XmlNode info in node.ChildNodes)
                                {
                                    if (info.Name == "noduleID")
                                    {
                                        noduleID = info.FirstChild.Value;
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
                                        var nodule = new LIDCNodule();
                                        var exclude = false;
                                        foreach (XmlNode attribute in info.ChildNodes)
                                        {
                                            if (attribute.Name == "imageSOP_UID")
                                            {
                                                // set SOP UID and retrieve the corresponding DICOM filename
                                                nodule.ImageSOP_UID = attribute.FirstChild.Value;
                                                if (fileSOPs.ContainsKey(nodule.ImageSOP_UID))
                                                    nodule.OriginalDICOMFilename = fileSOPs[nodule.ImageSOP_UID];
                                            }
                                            else if (attribute.Name == "inclusion")
                                            {
                                                var value = attribute.FirstChild.Value;
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
                                                nodule.Points.Add(new Point(x, y));
                                            }
                                        }
                                        foreach (var key in Annotations.Keys)
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
                                            foreach (var n in nodules)
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