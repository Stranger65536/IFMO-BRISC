using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;
using openDicom.File;


namespace BRISC.Core
{
    public class LIDCNodule
    {
        private static int nextNUID;


        public LIDCNodule()
        {
            NUID = nextNUID++;
            Nodule_no = "";
            Annotations = new Dictionary<string, int>();
            Haralick = new Dictionary<string, double>();
            LocalHaralick = new ArrayList();
            localHist = new ArrayList();
            Points = new List<Point>();
            segmentedPixelData = null;
            originalPixelData = null;
            MinX = int.MaxValue;
            MaxX = int.MinValue;
            MinY = int.MaxValue;
            MaxY = int.MinValue;
            Width = 0;
            Height = 0;
        }


        public LIDCNodule(XmlElement el)
            : this()
        {
            FromXML(el);
        }


        public LIDCNodule(int[,] originalPixelData, int minX, int maxX, int minY, int maxY)
        {
            this.originalPixelData = originalPixelData;
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            Width = maxX - minX + 1;
            Height = maxY - minY + 1;
        }


        public void FromXML(XmlElement el)
        {
            // read primary data
            StudyInstanceUID = getXMLTextElement(el, "studyInstanceID");
            SeriesInstanceUID = getXMLTextElement(el, "seriesInstanceID");
            NoduleID = getXMLTextElement(el, "noduleID");
            Nodule_no = getXMLTextElement(el, "nodule_no");
            ImageSOP_UID = getXMLTextElement(el, "imageSOP_UID");
            OriginalDICOMFilename = getXMLTextElement(el, "filename");

            if (!getXMLTextElement(el, "minX").Equals(""))
                MinX = int.Parse(getXMLTextElement(el, "minX"));
            if (!getXMLTextElement(el, "maxX").Equals(""))
                MaxX = int.Parse(getXMLTextElement(el, "maxX"));
            if (!getXMLTextElement(el, "minY").Equals(""))
                MinY = int.Parse(getXMLTextElement(el, "minY"));
            if (!getXMLTextElement(el, "maxY").Equals(""))
                MaxY = int.Parse(getXMLTextElement(el, "maxY"));
            if (!getXMLTextElement(el, "actualWidth").Equals(""))
                ActualWidth = double.Parse(getXMLTextElement(el, "actualWidth"));
            if (!getXMLTextElement(el, "actualHeight").Equals(""))
                ActualHeight = double.Parse(getXMLTextElement(el, "actualHeight"));

            // calculate other members
            Slice = ImageSOP_UID.Substring(ImageSOP_UID.LastIndexOf('.') + 1);
            Width = MaxX - MinX + 1;
            Height = MaxY - MinY + 1;

            // read annotation data
            Annotations.Clear();
            if (el.SelectSingleNode("annotations") != null)
                foreach (XmlElement annot in el.SelectSingleNode("annotations").ChildNodes)
                    Annotations.Add(annot.Name, int.Parse(annot.FirstChild.Value));

            // read Haralick data
            Haralick.Clear();
            if (el.SelectSingleNode("haralick") != null)
                foreach (XmlElement hara in el.SelectSingleNode("haralick").ChildNodes)
                    Haralick.Add(hara.Name, double.Parse(hara.FirstChild.Value));

            if (el.SelectSingleNode("local") != null)
            {
                foreach (XmlElement loc in el.SelectSingleNode("local").ChildNodes)
                {
                    var hist = loc.FirstChild.Value.Split(' ');
                    var tempHist = new double[hist.Length];
                    var i = 0;
                    foreach (var s in hist)
                    {
                        tempHist[i] = double.Parse(s);
                        i++;
                    }
                    localHist.Add(tempHist);
                }
                /*
                foreach (XmlElement loc in el.SelectSingleNode("local").ChildNodes)
                {
                    ArrayList columnPixel = new ArrayList();
                    foreach (XmlElement columns in loc)
                    {
                        ArrayList rowPixel = new ArrayList();
                        foreach (XmlElement rows in columns)
                        {
                            Dictionary<string, double[,]> tempDic = new Dictionary<string, double[,]>();
                            foreach (XmlElement dicString in rows)
                            {
                                double[,] tempFeatures = new double[4,4];
                                foreach (XmlElement childNod in dicString.ChildNodes)
                                {
                                    string featureRow = childNod.GetAttribute("paramRow");
                                    int featRow = Convert.ToInt32(featureRow);
                                    string featureColumn = childNod.GetAttribute("paramColumn");
                                    int featColumn = Convert.ToInt32(featureColumn);
                                    string featureValue = childNod.FirstChild.ToString();
                                    double featValue = Convert.ToDouble(featureValue);
                                    tempFeatures[featRow,featColumn] = featValue;
                                }
                                tempDic.Add(dicString.Name, tempFeatures);
                            }
                            rowPixel.Add(tempDic);
                        }
                        columnPixel.Add(rowPixel);
                    }
                    LocalHaralick.Add(columnPixel);
                }*/
            }

            // read Gabor data
            if (el.SelectSingleNode("gabor") != null)
            {
                // allocate memory
                int numOrientation; // total number or orientations
                int numScale;

                var lastIndex = el.SelectSingleNode("gabor").ChildNodes.Count - 1; // index of the last element under <gabor>
                var last = (XmlElement) el.SelectSingleNode("gabor").ChildNodes.Item(lastIndex);
                // grab the last element so we can find out how big to make the arrays

                numOrientation = int.Parse(last.GetAttribute("orientation")) + 1;
                numScale = int.Parse(last.GetAttribute("scale")) + 1;

                GaborHist = new double[numOrientation, numScale][];
                GaborMean = new double[numOrientation, numScale];
                GaborStdDev = new double[numOrientation, numScale];

                foreach (XmlElement gaborFeat in el.SelectSingleNode("gabor").ChildNodes)
                {
                    var currOrientation = int.Parse(gaborFeat.GetAttribute("orientation"));
                    var currScale = int.Parse(gaborFeat.GetAttribute("scale"));

                    if (gaborFeat.Name == "gaborhist")
                    {
                        var hist = gaborFeat.FirstChild.Value.Split(' ');
                        GaborHist[currOrientation, currScale] = new double[hist.Length];
                        var i = 0;
                        foreach (var s in hist)
                            GaborHist[currOrientation, currScale][i++] = double.Parse(s);
                    }
                    else if (gaborFeat.Name == "gabormean")
                    {
                        GaborMean[currOrientation, currScale] = double.Parse(gaborFeat.FirstChild.Value);
                    }
                    else if (gaborFeat.Name == "gaborstddev")
                    {
                        GaborStdDev[currOrientation, currScale] = double.Parse(gaborFeat.FirstChild.Value);
                    }
                }
            }

            // read Markov data
            if (el.SelectSingleNode("markov") != null)
            {
                var lastIndex = el.SelectSingleNode("markov").ChildNodes.Count - 1;
                var last = (XmlElement) el.SelectSingleNode("markov").ChildNodes.Item(lastIndex);

                var paramNums = int.Parse(last.GetAttribute("param")) + 1;
                MarkovHist = new double[paramNums][];
                MarkovMean = new double[paramNums];
                MarkovStd = new double[paramNums];

                foreach (XmlElement markovFeat in el.SelectSingleNode("markov").ChildNodes)
                {
                    var paramIndex = int.Parse(markovFeat.GetAttribute("param"));

                    if (markovFeat.Name == "markovhist")
                    {
                        var hist = markovFeat.FirstChild.Value.Split(' ');
                        MarkovHist[paramIndex] = new double[hist.Length];

                        var i = 0;
                        foreach (var s in hist)
                            MarkovHist[paramIndex][i++] = double.Parse(s);
                    }
                    else if (markovFeat.Name == "markovmean")
                    {
                        MarkovMean[paramIndex] = double.Parse(markovFeat.FirstChild.Value);
                    }
                    else if (markovFeat.Name == "markovstddev")
                    {
                        MarkovStd[paramIndex] = double.Parse(markovFeat.FirstChild.Value);
                    }
                }
            }

            // read point data
            Points.Clear();
            if (el.SelectSingleNode("points") != null)
                foreach (XmlElement point in el.SelectSingleNode("points").ChildNodes)
                    Points.Add(new Point(int.Parse(point.GetAttribute("x")), int.Parse(point.GetAttribute("y"))));
        }


        public XmlElement ToXML(XmlDocument doc)
        {
            // create element
            var el = doc.CreateElement("nodule");

            // add primary data
            if (SAVE_XML_PRIMARY)
            {
                el.AppendChild(createXMLTextElement(doc, "studyInstanceID", StudyInstanceUID));
                el.AppendChild(createXMLTextElement(doc, "seriesInstanceID", SeriesInstanceUID));
                el.AppendChild(createXMLTextElement(doc, "noduleID", NoduleID));
                el.AppendChild(createXMLTextElement(doc, "nodule_no", Nodule_no));
                el.AppendChild(createXMLTextElement(doc, "imageSOP_UID", ImageSOP_UID));
                el.AppendChild(createXMLTextElement(doc, "filename", OriginalDICOMFilename));
            }

            // save min/max pixel data
            if (SAVE_XML_BOUNDS)
            {
                el.AppendChild(createXMLTextElement(doc, "minX", MinX.ToString()));
                el.AppendChild(createXMLTextElement(doc, "maxX", MaxX.ToString()));
                el.AppendChild(createXMLTextElement(doc, "minY", MinY.ToString()));
                el.AppendChild(createXMLTextElement(doc, "maxY", MaxY.ToString()));
                el.AppendChild(createXMLTextElement(doc, "actualWidth", ActualWidth.ToString()));
                el.AppendChild(createXMLTextElement(doc, "actualHeight", ActualHeight.ToString()));
            }

            // add annotation data
            if (SAVE_XML_ANNOTATIONS)
            {
                var xanns = doc.CreateElement("annotations");
                foreach (var s in Annotations.Keys)
                    xanns.AppendChild(createXMLTextElement(doc, s, Annotations[s].ToString()));
                el.AppendChild(xanns);
            }

            // add Haralick data
            if (SAVE_XML_HARALICK)
            {
                var xhars = doc.CreateElement("haralick");
                foreach (var s in Haralick.Keys)
                    xhars.AppendChild(createXMLTextElement(doc, s, Haralick[s].ToString()));
                el.AppendChild(xhars);
            }

            // add Gabor data
            if (SAVE_XML_GABOR && GaborHist != null)
            {
                var gabor = doc.CreateElement("gabor");
                for (var i = 0; i < GaborHist.GetLength(0); i++)
                {
                    for (var j = 0; j < GaborHist.GetLength(1); j++)
                    {
                        var xgaborHist = doc.CreateElement("gaborhist");
                        xgaborHist.SetAttribute("orientation", i.ToString());
                        xgaborHist.SetAttribute("scale", j.ToString());

                        var histLine = "";
                        for (var k = 0; k < GaborHist[i, j].Length; k++)
                        {
                            if (k == 0)
                                histLine = GaborHist[i, j][k].ToString();
                            else
                                histLine += " " + GaborHist[i, j][k];
                        }
                        xgaborHist.AppendChild(doc.CreateTextNode(histLine));
                        gabor.AppendChild(xgaborHist);

                        var xgaborMean = doc.CreateElement("gabormean");
                        xgaborMean.SetAttribute("orientation", i.ToString());
                        xgaborMean.SetAttribute("scale", j.ToString());
                        xgaborMean.AppendChild(doc.CreateTextNode(GaborMean[i, j].ToString()));
                        gabor.AppendChild(xgaborMean);

                        var xgaborStdDev = doc.CreateElement("gaborstddev");
                        xgaborStdDev.SetAttribute("orientation", i.ToString());
                        xgaborStdDev.SetAttribute("scale", j.ToString());
                        xgaborStdDev.AppendChild(doc.CreateTextNode(GaborStdDev[i, j].ToString()));
                        gabor.AppendChild(xgaborStdDev);
                    }
                }
                el.AppendChild(gabor);
            }

            // add Markov data
            if (SAVE_XML_MARKOV && MarkovHist != null)
            {
                var markov = doc.CreateElement("markov");
                for (var i = 0; i < MarkovHist.GetLength(0); i++)
                {
                    var xMarkovHist = doc.CreateElement("markovhist");
                    xMarkovHist.SetAttribute("param", i.ToString());

                    var histLine = "";
                    for (var k = 0; k < MarkovHist[i].GetLength(0); k++)
                    {
                        if (k == 0)
                            histLine = MarkovHist[i][k].ToString();
                        else
                            histLine += " " + MarkovHist[i][k];
                    }
                    xMarkovHist.AppendChild(doc.CreateTextNode(histLine));
                    markov.AppendChild(xMarkovHist);

                    var xMarkovMean = doc.CreateElement("markovmean");
                    xMarkovMean.SetAttribute("param", i.ToString());
                    xMarkovMean.AppendChild(doc.CreateTextNode(MarkovMean[i].ToString()));
                    markov.AppendChild(xMarkovMean);

                    var xMarkovStd = doc.CreateElement("markovstddev");
                    xMarkovStd.SetAttribute("param", i.ToString());
                    xMarkovStd.AppendChild(doc.CreateTextNode(MarkovStd[i].ToString()));
                    markov.AppendChild(xMarkovStd);
                }
                el.AppendChild(markov);
            }

            // add point data
            if (SAVE_XML_POINTS)
            {
                var xpoints = doc.CreateElement("points");
                foreach (var p in Points)
                {
                    var xpoint = doc.CreateElement("point");
                    xpoint.SetAttribute("x", p.X.ToString());
                    xpoint.SetAttribute("y", p.Y.ToString());
                    xpoints.AppendChild(xpoint);
                }
                el.AppendChild(xpoints);
            }
            if (SAVE_XML_LOCAL)
            {
                var xlocal = doc.CreateElement("local");
                var count = 0;
                foreach (double[] histogram in localHist)
                {
                    var xfeature = doc.CreateElement("localHist");
                    xfeature.SetAttribute("param", count.ToString());
                    var localHistogram = "";
                    localHistogram = histogram[0].ToString();
                    for (var i = 1; i < histogram.Length; i++)
                    {
                        localHistogram = localHistogram + " " + histogram[i];
                    }
                    xfeature.AppendChild(doc.CreateTextNode(localHistogram));
                    xlocal.AppendChild(xfeature);
                    count++;
                }
                el.AppendChild(xlocal);
            }

            return el;
        }


        public bool Equals(LIDCNodule n)
        {
            // nodules are "equal" if series, slide and ID are the same
            return SeriesInstanceUID.Equals(n.SeriesInstanceUID) &&
                   ImageSOP_UID.Equals(n.ImageSOP_UID) &&
                   NoduleID.Equals(n.NoduleID);
        }


        public void Merge(LIDCNodule n)
        {
            if (!Equals(n))
                return;

            // read annotation data
            foreach (var s in n.Annotations.Keys)
                if (!Annotations.ContainsKey(s))
                    Annotations.Add(s, n.Annotations[s]);

            // read Global Co-occurrence data
            foreach (var s in n.Haralick.Keys)
                if (!Haralick.ContainsKey(s))
                    Haralick.Add(s, n.Haralick[s]);
            //read Local Co-occurrence data
            if (n.localHist.Count != 0)
            {
                localHist = n.localHist;
            }
            // read point data
            foreach (var p in n.Points)
            {
                var found = false;
                foreach (var pp in Points)
                {
                    if (p.Equals(pp))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    Points.Add(p);
            }

            // read Gabor data
            if (n.GaborHist != null) // make sure Gabor data exists
            {
                GaborHist = n.GaborHist;
                GaborMean = n.GaborMean;
                GaborStdDev = n.GaborStdDev;
            }
            // read Markov data
            if (n.MarkovHist != null)
            {
                MarkovHist = n.MarkovHist;
                MarkovMean = n.MarkovMean;
                MarkovStd = n.MarkovStd;
            }
        }


        public string GetSegmentedDICOMFilename()
        {
            return SeriesInstanceUID + "-" + ImageSOP_UID + "-" + Nodule_no + "-" + NoduleID + ".dcm";
        }


        public string GetOriginalDICOMFilename()
        {
            return SeriesInstanceUID + "/" + ImageSOP_UID + ".dcm";
        }


        public PhysicalSize GetActualSize()
        {
            var ps = new PhysicalSize();
            var df = Util.LoadDICOMFile(OriginalDICOMFilename);
            if (df != null)
            {
                var mi = df.MetaInformation;
                var ds = df.DataSet;
                var tg = new Tag("0028", "0030");
                var de = ds[tg];
                var vv = de.Value;
                ps.Width = Width*double.Parse(vv[0].ToString());
                ps.Height = Height*double.Parse(vv[1].ToString());
                ActualWidth = ps.Width;
                ActualHeight = ps.Height;
            }
            return ps;
        }


        public void CalculateCentroid()
        {
            CentroidX = CentroidY = 0;
            foreach (var p in Points)
            {
                CentroidX += p.X;
                CentroidY += p.Y;
            }
            CentroidX /= Points.Count;
            CentroidY /= Points.Count;
        }


        public void PurgePixelData()
        {
            segmentedPixelData = null;
            originalPixelData = null;
        }


        private double[,][] getSmallGaborHist(int divBy)
        {
            if (divBy <= 1) // don't do any work if the histogram is being divided by 1  
                return gaborHist;

            if (divBy%2 != 0) // only works if dividing the histogram by a power of two
                return null;

            // if we already resized this, don't do it again
            if (gaborHistResized != null)
                return gaborHistResized;

            gaborHistResized = new double[gaborHist.GetLength(0), gaborHist.GetLength(1)][];

            for (var i = 0; i < gaborHist.GetLength(0); i++) // for each histogram in the nodule
            {
                for (var j = 0; j < gaborHist.GetLength(1); j++)
                {
                    gaborHistResized[i, j] = new double[gaborHist[i, j].Length/divBy];
                    for (var k = 0; k < gaborHistResized[i, j].Length; k++) // for each value in the new  histogram
                    {
                        for (var timeThrough = 0; timeThrough < divBy; timeThrough++) // sum the next <divBy> values from the old histogram
                        {
                            gaborHistResized[i, j][k] += gaborHist[i, j][divBy*k + timeThrough];
                        }
                    }
                }
            }
            return gaborHistResized;
        }


        private double[][] getSmallMarkovHist(int divBy)
        {
            if (divBy <= 1)
                return markovHist;

            if (markovHistResized != null)
                return markovHistResized;

            markovHistResized = new double[markovHist.GetLength(0)][];

            for (var i = 0; i < markovHist.GetLength(0); i++)
            {
                markovHistResized[i] = new double[markovHist[i].Length/divBy];
                for (var k = 0; k < markovHistResized[i].Length; k++)
                {
                    for (var timeThrough = 0; timeThrough < divBy; timeThrough++)
                    {
                        markovHistResized[i][k] += markovHist[i][divBy*k + timeThrough];
                    }
                }
            }
            return markovHistResized;
        }


        private string getXMLTextElement(XmlElement el, string name)
        {
            // read from a "<name>value</name>" element
            var node = el.SelectSingleNode(name);
            if (node != null)
                return node.FirstChild.Value;
            return "";
        }


        private XmlElement createXMLTextElement(XmlDocument doc, string name, string value)
        {
            // create a "<name>value</name>" element
            var el = doc.CreateElement(name);
            el.AppendChild(doc.CreateTextNode(value));
            return el;
        }


        public struct PhysicalSize
        {
            public double Width;


            public double Height;
        }

        public int NUID;


        public string MdbPK;


        public string StudyInstanceUID;


        public string SeriesInstanceUID;


        public string NoduleID;


        public string Nodule_no;


        public string ImageSOP_UID;


        public int MinX;


        public int MaxX;


        public int MinY;


        public int MaxY;


        public string Slice;


        public int CentroidX;


        public int CentroidY;


        public int Width;


        public int Height;


        public double ActualWidth = double.NaN;


        public double ActualHeight = double.NaN;


        public double Temp_dist;


        public double Temp_adist;


        public double pcaDist;


        public double annotateDist;

        public Dictionary<string, double> Haralick;


        public ArrayList LocalHaralick;


        public ArrayList localHist;


        public double[] PCAperNod = null;


        private double[,][] gaborHist;


        private double[,][] gaborHistResized;


        public double[,][] GaborHist
        {
            get { return getSmallGaborHist(GaborHistDivBy); }
            set { gaborHist = value; }
        }


        private int gaborHistDivBy;


        public int GaborHistDivBy
        {
            get { return gaborHistDivBy; }
            set
            {
                gaborHistDivBy = value;
                // whenever we change the size of the histogram remove the temporary histogram
                gaborHistResized = null;
            }
        }


        public double[,] GaborMean;


        public double[,] GaborStdDev;


        private double[][] markovHist;


        private double[][] markovHistResized;


        public double[][] MarkovHist
        {
            get { return getSmallMarkovHist(MarkovHistDivBy); }
            set { markovHist = value; }
        }


        private int markovHistDivBy;


        public int MarkovHistDivBy
        {
            get { return markovHistDivBy; }
            set
            {
                markovHistDivBy = value;
                // whenever we change the size of the histogram, remove the temporary histogram
                markovHistResized = null;
            }
        }


        public double[] MarkovMean;


        public double[] MarkovStd;


        public Dictionary<string, int> Annotations;


        public List<Point> Points;


        private int[,] segmentedPixelData;


        private int[,] originalPixelData;


        public string OriginalDICOMFilename;

        //5.84375 = 256 bins while there is 1496 different intensity values
        //2.921875 = 512
        //23.375 = 64 bins
        //11.6875 = 128 bins
        private readonly double binNumber = 23.375;


        public int[,] SegmentedPixelData
        {
            get
            {
                if (segmentedPixelData == null)
                {
                    originalPixelData = Util.LoadDICOMPixelData(OriginalDICOMFilename);
                    segmentedPixelData = Util.SegmentNodule(originalPixelData, this, 0, 0);
                    if (Points.Count > 0)
                    {
                        // set pixels outside contour to background value
                        var rows = Height;
                        var cols = Width;
                        var contour = new bool[rows, cols];
                        for (var r = 0; r < rows; r++)
                            for (var c = 0; c < cols; c++)
                                contour[r, c] = false;
                        foreach (var p in Points)
                        {
                            var row = p.Y - MinY;
                            var col = p.X - MinX;
                            try
                            {
                                contour[row, col] = true;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        for (var r = 0; r < rows; r++)
                            for (var c = 0; c < cols && !contour[r, c]; c++)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (var r = 0; r < rows; r++)
                            for (var c = cols - 1; c >= 0 && !contour[r, c]; c--)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (var c = 0; c < cols; c++)
                            for (var r = 0; r < rows && !contour[r, c]; r++)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (var c = 0; c < cols; c++)
                            for (var r = rows - 1; r >= 0 && !contour[r, c]; r--)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (var i = 0; i < segmentedPixelData.GetLength(0); i++)
                        {
                            for (var j = 0; j < segmentedPixelData.GetLength(1); j++)
                            {
                                if (segmentedPixelData[i, j] != -2000)
                                {
                                    segmentedPixelData[i, j] = (int) (segmentedPixelData[i, j]/binNumber);
                                }
                            }
                        }
                    }
                }
                return segmentedPixelData;
            }
        }


        public int[,] OriginalPixelData
        {
            get
            {
                if (originalPixelData == null)
                {
                    originalPixelData = Util.LoadDICOMPixelData(OriginalDICOMFilename);
                }
                return originalPixelData;
            }
        }

        public static bool SAVE_XML_PRIMARY = true;


        public static bool SAVE_XML_BOUNDS = false;


        public static bool SAVE_XML_ANNOTATIONS = false;


        public static bool SAVE_XML_HARALICK = false;


        public static bool SAVE_XML_GABOR = false;


        public static bool SAVE_XML_MARKOV = false;


        public static bool SAVE_XML_POINTS = false;


        public static bool SAVE_XML_LOCAL = false;
    }
}