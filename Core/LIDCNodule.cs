using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BRISC.Core
{
    /// <summary>
    /// Represents a LIDC database nodule
    /// </summary>
    public class LIDCNodule
    {
        /// <summary>
        /// Used to assign each nodule a unique number
        /// </summary>
        private static int nextNUID = 0;

        #region Primary data
        /// <summary>
        /// Unique nodule ID (assigned in constructor -- will NEVER be the same for two different LIDCNodule objects!)
        /// </summary>
        public int NUID;
        /// <summary>
        /// Primary key value from Ekarin's Access database file (not used - DEPRECATED)
        /// </summary>
        public string MdbPK;
        /// <summary>
        /// Study ID number from DICOM files
        /// </summary>
        public string StudyInstanceUID;
        /// <summary>
        /// Series ID number from DICOM files
        /// </summary>
        public string SeriesInstanceUID;
        /// <summary>
        /// Nodule ID number from XML annotation (unique nodule, physician, series)
        /// </summary>
        public string NoduleID;
        /// <summary>
        /// Nodule number -- identifies the same nodule from different slices and/or physicians (computed by Ekarin using centroid comparison)
        /// </summary>
        public string Nodule_no;
        /// <summary>
        /// Image ID number from DICOM files
        /// </summary>
        public string ImageSOP_UID;
        /// <summary>
        /// Left boundary of nodule with respect to the original DICOM file dimensions
        /// </summary>
        public int MinX;
        /// <summary>
        /// Right boundary of nodule with respect to the original DICOM file dimensions
        /// </summary>
        public int MaxX;
        /// <summary>
        /// Top boundary of nodule with respect to the original DICOM file dimensions
        /// </summary>
        public int MinY;
        /// <summary>
        /// Bottom boundary of nodule with respect to the original DICOM file dimensions
        /// </summary>
        public int MaxY;
        #endregion

        #region Computed data
        /// <summary>
        /// CT slice number (last number in imageSOP_UID)
        /// </summary>
        public string Slice;
        /// <summary>
        /// X-coordinate of nodule centroid
        /// </summary>
        public int CentroidX;
        /// <summary>
        /// Y-coordinate of nodule centroid
        /// </summary>
        public int CentroidY;
        /// <summary>
        /// Nodule width (maxX - minX) in pixels
        /// </summary>
        public int Width;
        /// <summary>
        /// Nodule height (maxY - minY) in pixels
        /// </summary>
        public int Height;
        /// <summary>
        /// Physical width in millimeters
        /// </summary>
        public double ActualWidth = double.NaN;
        /// <summary>
        /// Physical height in millimeters
        /// </summary>
        public double ActualHeight = double.NaN;
        /// <summary>
        /// Temporary variable used for storing distance from query images while running a query
        /// </summary>
        public double Temp_dist;
        /// <summary>
        /// Temporary variable used for storing annotation distance from query images while running a query
        /// </summary>
        public double Temp_adist;
        /// <summary>
        /// Temporary variable used for storing distance from query images while running a query
        /// </summary>
        public double pcaDist;
        /// <summary>
        /// Temporary variable used for storing distance from query images while running a query
        /// </summary>
        public double annotateDist;
        #endregion
        
        #region External data
        /// <summary>
        /// Haralick data as dictionary with feature name-value pairs
        /// </summary>
        public Dictionary<string, double> Haralick;
        /// <summary>
        /// Haralick data as dictionary with feature name-value pairs for local co-occurrence
        /// each feature pair contains all the values for every pixel for that feature.
        /// </summary>
        public ArrayList LocalHaralick;

        /// <summary>
        /// The histogram used in Local Co-occurrence.
        /// </summary>
        public ArrayList localHist;


        /// <summary>
        /// The array that stores the PCA information
        /// </summary>
        public double[] PCAperNod = null;
        /// <summary>
        /// Two dimensional array of Gabor response histograms (orientation x scale)
        /// </summary>
        private double[,][] gaborHist = null;
        /// <summary>
        /// Resized histogram. It is here for optimization. Since we will probably access
        /// the histogram multiple times before resizing it, there is no need to recompute it each time --
        /// just store it in here.
        /// </summary>
        private double[,][] gaborHistResized = null;
        /// <summary>
        /// Accessor for gaborHist. Can re-bin histogram based on public field GaborHistDivBy
        /// </summary>
        public double[,][] GaborHist
        {
            get
            {
                return getSmallGaborHist(GaborHistDivBy);
            }
            set
            {
                gaborHist = value;
            }
        }
        /// <summary>
        /// How much to divide the histogram by
        /// </summary>
        private int gaborHistDivBy = 0;
        /// <summary>
        /// Accessor for gaborHistDivBy. Verifies that the value is a power of two.
        /// </summary>
        public int GaborHistDivBy
        {
            get
            {
                return gaborHistDivBy;
            }
            set
            {
                gaborHistDivBy = value;
                // whenever we change the size of the histogram remove the temporary histogram
                gaborHistResized = null;
            }
        }
        /// <summary>
        /// Mean Gabor response
        /// </summary>
        public double[,] GaborMean = null;
        /// <summary>
        /// Standard deviation of Gabor responses
        /// </summary>
        public double[,] GaborStdDev = null;
        /// <summary>
        /// Markov data
        /// </summary>
        private double[][] markovHist = null;
        /// <summary>
        /// Resized histogram. It is here for optimization. Since we will probably access
        /// the histogram multiple times before resizing it, there is no need to recompute it each time --
        /// just store it in here.
        /// </summary>
        private double[][] markovHistResized = null;
        /// <summary>
        /// Accessor for the Markov histogram. Used to resize the histogram.
        /// </summary>
        public double[][] MarkovHist
        {
            get
            {
                return getSmallMarkovHist(MarkovHistDivBy);
            }
            set
            {
                markovHist = value;
            }
        }
        /// <summary>
        /// How much to divide the histogram by
        /// </summary>
        private int markovHistDivBy = 0;
        /// <summary>
        /// Accessor for the Markov histogram divisor
        /// </summary>
        public int MarkovHistDivBy
        {
            get
            {
                return markovHistDivBy;
            }
            set
            {
                markovHistDivBy = value;
                // whenever we change the size of the histogram, remove the temporary histogram
                markovHistResized = null;
            }
        }
        /// <summary>
        /// Mean of the Markov data
        /// </summary>
        public double[] MarkovMean = null;
        /// <summary>
        /// Standard deviation of the Markov data
        /// </summary>
        public double[] MarkovStd = null;
        /// <summary>
        /// Annotation data as dictionary with annotation name-value pairs
        /// </summary>
        public Dictionary<string, int> Annotations;
        /// <summary>
        /// Nodule contour data as list of points
        /// </summary>
        public List<Point> Points;
        /// <summary>
        /// Raw DICOM pixel data (segmented - pixels outside the boundary contain the value Util.BG_VALUE)
        /// </summary>
        private int[,] segmentedPixelData;
        /// <summary>
        /// Raw DICOM pixel data (full image)
        /// </summary>
        private int[,] originalPixelData;
        /// <summary>
        /// Filename (relative to Util.DATA_PATH) for original DICOM image
        /// </summary>
        public string OriginalDICOMFilename;

        //5.84375 = 256 bins while there is 1496 different intensity values
        //2.921875 = 512
        //23.375 = 64 bins
        //11.6875 = 128 bins
        private double binNumber = 23.375;
        /// <summary>
        /// Used to determine how many values are placed in each bin when the pixel data is binned. 
        ///</summary>

        /// <summary>
        /// Returns the extracted DICOM pixel data (pixels outside the boundary contain the value Util.BG_VALUE)
        /// </summary>
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
                        int rows = Height;
                        int cols = Width;
                        bool[,] contour = new bool[rows, cols];
                        for (int r = 0; r < rows; r++)
                            for (int c = 0; c < cols; c++)
                                contour[r, c] = false;
                        foreach (Point p in Points)
                        {
                            int row = p.Y - MinY;
                            int col = p.X - MinX;
                            try
                            {
                                contour[row, col] = true;
                            }
                            catch (Exception) { }
                        }
                        for (int r = 0; r < rows; r++)
                            for (int c = 0; c < cols && !contour[r, c]; c++)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (int r = 0; r < rows; r++)
                            for (int c = cols - 1; c >= 0 && !contour[r, c]; c--)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (int c = 0; c < cols; c++)
                            for (int r = 0; r < rows && !contour[r, c]; r++)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (int c = 0; c < cols; c++)
                            for (int r = rows - 1; r >= 0 && !contour[r, c]; r--)
                                segmentedPixelData[r, c] = Util.BG_VALUE;
                        for (int i = 0; i < segmentedPixelData.GetLength(0); i++)
                        {
                            for (int j = 0; j < segmentedPixelData.GetLength(1); j++)
                            {
                                if (segmentedPixelData[i, j] != -2000)
                                {
                                    segmentedPixelData[i, j] = (int)((double)segmentedPixelData[i, j] / binNumber);
                                }
                            }
                        }
                    }
                }
                return segmentedPixelData;
            }
        }

        /// <summary>
        /// Returns the original DICOM pixel data
        /// </summary>
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

        #endregion

        #region XML export options
        /// <summary>
        /// Whether to save primary data (series, noduleID, etc.) when exporting to XML
        /// </summary>
        public static bool SAVE_XML_PRIMARY = true;
        /// <summary>
        /// Whether to save min/max pixel boundary data when exporting to XML
        /// </summary>
        public static bool SAVE_XML_BOUNDS = false;
        /// <summary>
        /// Whether to save annotation data when exporting to XML
        /// </summary>
        public static bool SAVE_XML_ANNOTATIONS = false;
        /// <summary>
        /// Whether to save haralick data when exporting to XML
        /// </summary>
        public static bool SAVE_XML_HARALICK = false;
        /// <summary>
        /// Whether to save Gabor data when exporting to XML
        /// </summary>
        public static bool SAVE_XML_GABOR = false;
        /// <summary>
        /// Whether to save Markov data when exporting to XML
        /// </summary>
        public static bool SAVE_XML_MARKOV = false;
        /// <summary>
        /// Whether to save point data when exporting to XML
        /// </summary>
        public static bool SAVE_XML_POINTS = false;
        /// <summary>
        /// Whether to save Local Co-occurrence data when exporting to XML
        /// </summary>
        public static bool SAVE_XML_LOCAL = false;
        #endregion

        /// <summary>
        /// Create a new, blank nodule structure
        /// </summary>
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

        /// <summary>
        /// Create a nodule structure from an XML element
        /// </summary>
        /// <param name="el">Element to parse</param>
        public LIDCNodule(XmlElement el)
            : this()
        {
            FromXML(el);
        }

        /// <summary>
        /// Create a nodule structure from raw original pixel data
        /// </summary>
        /// <param name="originalPixelData">Two-dimensional integer pixel data</param>
        /// <param name="minX">Left boundary</param>
        /// <param name="maxX">Right boundary</param>
        /// <param name="minY">Top boundary</param>
        /// <param name="maxY">Bottom boundary</param>
        public LIDCNodule(int[,] originalPixelData, int minX, int maxX, int minY, int maxY)
        {
            this.originalPixelData = originalPixelData;
            this.MinX = minX;
            this.MaxX = maxX;
            this.MinY = minY;
            this.MaxY = maxY;
            this.Width = maxX - minX + 1;
            this.Height = maxY - minY + 1;
        }

        /// <summary>
        /// Read nodule data from an XML element
        /// </summary>
        /// <param name="el">Element to parse</param>
        public void FromXML(XmlElement el)
        {
            // read primary data
            StudyInstanceUID = getXMLTextElement(el, "studyInstanceID");
            SeriesInstanceUID = getXMLTextElement(el, "seriesInstanceID");
            NoduleID = getXMLTextElement(el, "noduleID");
            Nodule_no = getXMLTextElement(el, "nodule_no");
            ImageSOP_UID = getXMLTextElement(el, "imageSOP_UID");
            OriginalDICOMFilename = getXMLTextElement(el, "filename");

            if (!(getXMLTextElement(el, "minX").Equals("")))
                MinX = int.Parse(getXMLTextElement(el, "minX"));
            if (!(getXMLTextElement(el, "maxX").Equals("")))
                MaxX = int.Parse(getXMLTextElement(el, "maxX"));
            if (!(getXMLTextElement(el, "minY").Equals("")))
                MinY = int.Parse(getXMLTextElement(el, "minY"));
            if (!(getXMLTextElement(el, "maxY").Equals("")))
                MaxY = int.Parse(getXMLTextElement(el, "maxY"));
            if (!(getXMLTextElement(el, "actualWidth").Equals("")))
                ActualWidth = double.Parse(getXMLTextElement(el, "actualWidth"));
            if (!(getXMLTextElement(el, "actualHeight").Equals("")))
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
                    string[] hist = (loc.FirstChild.Value.Split(' '));
                    double[] tempHist = new double[hist.Length];
                    int i = 0;
                    foreach (string s in hist)
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
                int numOrientation;     // total number or orientations
                int numScale;
                
                int lastIndex = el.SelectSingleNode("gabor").ChildNodes.Count - 1;      // index of the last element under <gabor>
                XmlElement last = (XmlElement)el.SelectSingleNode("gabor").ChildNodes.Item(lastIndex); // grab the last element so we can find out how big to make the arrays
                
                numOrientation  = int.Parse(last.GetAttribute("orientation")) + 1;
                numScale        = int.Parse(last.GetAttribute("scale")) + 1;

                GaborHist       = new double[numOrientation, numScale][];
                GaborMean       = new double[numOrientation, numScale];
                GaborStdDev     = new double[numOrientation, numScale];

                foreach (XmlElement gaborFeat in el.SelectSingleNode("gabor").ChildNodes)
                {
                    
                    int currOrientation = int.Parse(gaborFeat.GetAttribute("orientation"));
                    int currScale = int.Parse(gaborFeat.GetAttribute("scale"));

                    if (gaborFeat.Name == "gaborhist")
                    {
                        string[] hist = (gaborFeat.FirstChild.Value.Split(' '));
                        GaborHist[currOrientation, currScale] = new double[hist.Length];
                        int i = 0;
                        foreach (string s in hist)
                            GaborHist[currOrientation,currScale][i++] = double.Parse(s);
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
                int lastIndex = el.SelectSingleNode("markov").ChildNodes.Count - 1;
                XmlElement last = (XmlElement)el.SelectSingleNode("markov").ChildNodes.Item(lastIndex);

                int paramNums = int.Parse(last.GetAttribute("param")) + 1;
                MarkovHist = new double[paramNums][];
                MarkovMean = new double[paramNums];
                MarkovStd = new double[paramNums];

                foreach (XmlElement markovFeat in el.SelectSingleNode("markov").ChildNodes)
                {
                    int paramIndex = int.Parse(markovFeat.GetAttribute("param"));

                    if (markovFeat.Name == "markovhist")
                    {
                        string[] hist = markovFeat.FirstChild.Value.Split(' ');
                        MarkovHist[paramIndex] = new double[hist.Length];

                        int i = 0; 
                        foreach(string s in hist)
                            MarkovHist[paramIndex][i++] = double.Parse(s);
                    }
                    else if(markovFeat.Name == "markovmean")
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

        /// <summary>
        /// Save nodule data to an XML element
        /// </summary>
        /// <param name="doc">Target XML document</param>
        /// <returns>XML element representation of the nodule data</returns>
        public XmlElement ToXML(XmlDocument doc)
        {
            // create element
            XmlElement el = doc.CreateElement("nodule");

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
                XmlElement xanns = doc.CreateElement("annotations");
                foreach (string s in Annotations.Keys)
                    xanns.AppendChild(createXMLTextElement(doc, s, Annotations[s].ToString()));
                el.AppendChild(xanns);
            }

            // add Haralick data
            if (SAVE_XML_HARALICK)
            {
                XmlElement xhars = doc.CreateElement("haralick");
                foreach (string s in Haralick.Keys)
                    xhars.AppendChild(createXMLTextElement(doc, s, Haralick[s].ToString()));
                el.AppendChild(xhars);
            }
            
            // add Gabor data
            if (SAVE_XML_GABOR && GaborHist != null)
            {
                XmlElement gabor = doc.CreateElement("gabor");
                for (int i = 0; i < GaborHist.GetLength(0); i++)
                {
                    for (int j = 0; j < GaborHist.GetLength(1); j++)
                    {
                        XmlElement xgaborHist = doc.CreateElement("gaborhist");
                        xgaborHist.SetAttribute("orientation", i.ToString());
                        xgaborHist.SetAttribute("scale", j.ToString());

                        string histLine = "";
                        for (int k = 0; k < GaborHist[i,j].Length; k++) 
                        {
                            if (k == 0)
                                histLine = GaborHist[i, j][k].ToString();
                            else
                                histLine += " " + GaborHist[i, j][k].ToString();
                        }
                        xgaborHist.AppendChild(doc.CreateTextNode(histLine));
                        gabor.AppendChild(xgaborHist);
                        
                        XmlElement xgaborMean = doc.CreateElement("gabormean");
                        xgaborMean.SetAttribute("orientation", i.ToString());
                        xgaborMean.SetAttribute("scale", j.ToString());
                        xgaborMean.AppendChild(doc.CreateTextNode(GaborMean[i, j].ToString()));
                        gabor.AppendChild(xgaborMean);

                        XmlElement xgaborStdDev = doc.CreateElement("gaborstddev");
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
                XmlElement markov = doc.CreateElement("markov");
                for (int i = 0; i < MarkovHist.GetLength(0); i++)
                {
                    XmlElement xMarkovHist = doc.CreateElement("markovhist");
                    xMarkovHist.SetAttribute("param", i.ToString());

                    string histLine = "";
                    for (int k = 0; k < MarkovHist[i].GetLength(0); k++)
                    {
                        if (k == 0)
                            histLine = MarkovHist[i][k].ToString();
                        else
                            histLine += " " + MarkovHist[i][k].ToString();
                    }
                    xMarkovHist.AppendChild(doc.CreateTextNode(histLine));
                    markov.AppendChild(xMarkovHist);

                    XmlElement xMarkovMean = doc.CreateElement("markovmean");
                    xMarkovMean.SetAttribute("param", i.ToString());
                    xMarkovMean.AppendChild(doc.CreateTextNode(MarkovMean[i].ToString()));
                    markov.AppendChild(xMarkovMean);

                    XmlElement xMarkovStd = doc.CreateElement("markovstddev");
                    xMarkovStd.SetAttribute("param",i.ToString());
                    xMarkovStd.AppendChild(doc.CreateTextNode(MarkovStd[i].ToString()));
                    markov.AppendChild(xMarkovStd);
                }
                el.AppendChild(markov);
            }
            
            // add point data
            if (SAVE_XML_POINTS)
            {
                XmlElement xpoints = doc.CreateElement("points");
                foreach (Point p in Points)
                {
                    XmlElement xpoint = doc.CreateElement("point");
                    xpoint.SetAttribute("x", p.X.ToString());
                    xpoint.SetAttribute("y", p.Y.ToString());
                    xpoints.AppendChild(xpoint);
                }
                el.AppendChild(xpoints);
            }
            if (SAVE_XML_LOCAL)
            {
                XmlElement xlocal = doc.CreateElement("local");
                int count = 0;
                foreach (double[] histogram in localHist)
                {
                    XmlElement xfeature = doc.CreateElement("localHist");
                    xfeature.SetAttribute("param", count.ToString());
                    string localHistogram = "";
                    localHistogram = histogram[0].ToString();
                    for (int i = 1; i < histogram.Length; i++)
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

        /// <summary>
        /// Compares two LIDCNodule objects
        /// </summary>
        /// <param name="n">Comparison nodule</param>
        /// <returns>True if the series, slice and node IDs match, false otherwise</returns>
        public bool Equals(LIDCNodule n)
        {
            // nodules are "equal" if series, slide and ID are the same
            return (this.SeriesInstanceUID.Equals(n.SeriesInstanceUID) &&
                this.ImageSOP_UID.Equals(n.ImageSOP_UID) && 
                this.NoduleID.Equals(n.NoduleID));
        }

        /// <summary>
        /// Merges in secondary data from another node that doesn't exist in this node
        /// </summary>
        /// <param name="n">Node with additional data</param>
        public void Merge(LIDCNodule n)
        {
            if (!(Equals(n)))
                return;

            // read annotation data
            foreach (string s in n.Annotations.Keys)
                if (!(Annotations.ContainsKey(s)))
                    Annotations.Add(s, n.Annotations[s]);

            // read Global Co-occurrence data
            foreach (string s in n.Haralick.Keys)
                if (!(Haralick.ContainsKey(s)))
                    Haralick.Add(s, n.Haralick[s]);
            //read Local Co-occurrence data
            if (n.localHist.Count != 0)
            {
                localHist = n.localHist;
            }
            // read point data
            foreach (Point p in n.Points)
            {
                bool found = false;
                foreach (Point pp in Points)
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
            if (n.GaborHist != null)    // make sure Gabor data exists
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

        /// <summary>
        /// Builds the filename of the corresponding DICOM segmented nodule image
        /// </summary>
        /// <returns>Filename (.dcm extension, no path included)</returns>
        public string GetSegmentedDICOMFilename()
        {
            return SeriesInstanceUID + "-" + ImageSOP_UID + "-" + Nodule_no + "-" + NoduleID + ".dcm";
        }

        /// <summary>
        /// Builds the filename of the corresponding original DICOM image
        /// </summary>
        /// <returns>Filename (.dcm extension, no path included)</returns>
        public string GetOriginalDICOMFilename()
        {
            return SeriesInstanceUID + "/" + ImageSOP_UID + ".dcm";
        }

        /// <summary>
        /// Structure for representing the physical size of a nodule.
        /// </summary>
        /// <remarks>
        /// This class is necessary because the standard System.Drawing.Size 
        /// class uses integers, but floating-point variables are needed to 
        /// store small sizes in millimeters.
        /// </remarks>
        public struct PhysicalSize
        {
            /// <summary>
            /// Floating-point width
            /// </summary>
            public double Width;

            /// <summary>
            /// Floating-point height
            /// </summary>
            public double Height;
        }

        /// <summary>
        /// Reads the DICOM pixel spacing property and calculates the nodule's actual height and width in millimeters.
        /// </summary>
        /// <returns>Physical size of nodule in mm</returns>
        public PhysicalSize GetActualSize()
        {
            PhysicalSize ps = new PhysicalSize();
            openDicom.File.DicomFile df = Util.LoadDICOMFile(OriginalDICOMFilename);
            if (df != null)
            {
                openDicom.File.FileMetaInformation mi = df.MetaInformation;
                openDicom.DataStructure.DataSet.DataSet ds = df.DataSet;
                openDicom.DataStructure.Tag tg = new openDicom.DataStructure.Tag("0028", "0030");
                openDicom.DataStructure.DataSet.DataElement de = ds[tg];
                openDicom.DataStructure.Value vv = de.Value;
                ps.Width = (double)Width * double.Parse(vv[0].ToString());
                ps.Height = (double)Height * double.Parse(vv[1].ToString());
                ActualWidth = ps.Width;
                ActualHeight = ps.Height;
            }
            return ps;
        }

        /// <summary>
        /// Calculate the nodule centroid (point data must be loaded)
        /// </summary>
        public void CalculateCentroid()
        {
            CentroidX = CentroidY = 0;
            foreach (Point p in Points)
            {
                CentroidX += p.X;
                CentroidY += p.Y;
            }
            CentroidX /= Points.Count;
            CentroidY /= Points.Count;
        }

        /// <summary>
        /// De-allocate pixel data from memory
        /// </summary>
        public void PurgePixelData()
        {
            segmentedPixelData = null;
            originalPixelData = null;
        }

        /// <summary>
        /// Bins the Gabor histogram in to a smaller histogram
        /// </summary>
        /// <param name="divBy">Number to divide the origional histogram by. Must be a power of two, anything else returns null</param>
        /// <returns>Smaller histogram</returns>
        private double[,][] getSmallGaborHist(int divBy)
        {
            if (divBy <= 1)          // don't do any work if the histogram is being divided by 1  
                return this.gaborHist;

            if ((divBy % 2) != 0)   // only works if dividing the histogram by a power of two
                return null;

            // if we already resized this, don't do it again
            if (this.gaborHistResized != null)
                return this.gaborHistResized;

            this.gaborHistResized = new double[this.gaborHist.GetLength(0), this.gaborHist.GetLength(1)][];

            for (int i = 0; i < this.gaborHist.GetLength(0); i++)    // for each histogram in the nodule
            {
                for (int j = 0; j < this.gaborHist.GetLength(1); j++)
                {
                    this.gaborHistResized[i, j] = new double[(this.gaborHist[i, j].Length / divBy)];
                    for (int k = 0; k < this.gaborHistResized[i, j].Length; k++)                 // for each value in the new  histogram
                    {
                        for (int timeThrough = 0; timeThrough < divBy; timeThrough++)    // sum the next <divBy> values from the old histogram
                        {
                            this.gaborHistResized[i, j][k] += this.gaborHist[i, j][divBy * k + timeThrough];
                        }
                    }
                }
            }
            return this.gaborHistResized;
        }

        /// <summary>
        /// Bins the Markov histogram in to a smaller histogram
        /// </summary>
        /// <param name="divBy">Number to divide the origional histogram by. Must be a power of two, anything else returns null</param>
        /// <returns>Smaller histogram</returns>
        private double[][] getSmallMarkovHist(int divBy)
        {
            if(divBy <= 1)
                return this.markovHist;

            if(this.markovHistResized != null)
                return this.markovHistResized;

            this.markovHistResized = new double[this.markovHist.GetLength(0)][];

            for (int i = 0; i < this.markovHist.GetLength(0); i++)
            {
                this.markovHistResized[i] = new double[this.markovHist[i].Length / divBy];
                for (int k = 0; k < this.markovHistResized[i].Length; k++)
                {
                    for(int timeThrough = 0; timeThrough < divBy; timeThrough++)
                    {
                        this.markovHistResized[i][k] += this.markovHist[i][divBy * k + timeThrough];
                    }
                }
            }
            return this.markovHistResized;
        }

        /// <summary>
        /// Reads a value from an XML element
        /// </summary>
        /// <param name="el">Parent XML element to read</param>
        /// <param name="name">Name of desired element</param>
        /// <returns>Value of the desired element</returns>
        private string getXMLTextElement(XmlElement el, string name)
        {
            // read from a "<name>value</name>" element
            XmlNode node = el.SelectSingleNode(name);
            if (node != null)
                return node.FirstChild.Value;
            else
                return "";
        }

        /// <summary>
        /// Generates an XML element object with the specified subelement
        /// </summary>
        /// <param name="doc">Target XML document</param>
        /// <param name="name">Desired element name</param>
        /// <param name="value">Desired element value</param>
        /// <returns>Created XML object</returns>
        private XmlElement createXMLTextElement(XmlDocument doc, string name, string value)
        {
            // create a "<name>value</name>" element
            XmlElement el = doc.CreateElement(name);
            el.AppendChild(doc.CreateTextNode(value));
            return el;
        }
    }
}
