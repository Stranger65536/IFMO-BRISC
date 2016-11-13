using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.IO;
//using CenterSpace.NMath.Kernel;
//using CenterSpace.NMath.Stats;
//using CenterSpace.NMath.StatsKernel;
using System.Windows.Forms;
namespace BRISC.Core
{
    /// <summary>
    /// A collection of LIDCNodule objects
    /// </summary>
    /// <remarks>
    /// This class contains methods for loading, maintaining and saving 
    /// LIDCNodule databases using XML files for serialization. It also 
    /// provides the core system for CBIR, with normalization, query 
    /// and performance (precision/recall) procedures.
    /// </remarks>
    public class LIDCNoduleDB
    {
        private List<LIDCNodule> nodules;

        /// <summary>
        /// Main nodule collection
        /// </summary>
        public List<LIDCNodule> Nodules
        {
            get
            {
                return nodules;
            }
        }

        /// <summary>
        /// Default image feature for queries
        /// </summary>
        public string DEFAULT_FEATURE = "Gabor";
        /// <summary>
        /// Default similarity measure for queries
        /// </summary>
        public string DEFAULT_SIMILARITY = "Chi-Square (H)";

        #region Nodule statistics

        private Dictionary<string, int> noduleCount;
        private double meanWidth = double.NaN;
        private double meanHeight = double.NaN;

        // min/max Haralick values (for normalization)
        private Dictionary<string, double> minHaralick;
        private Dictionary<string, double> maxHaralick;

        private Dictionary<string, double> LocalminHaralick;
        private Dictionary<string, double> LocalmaxHaralick;
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, string> sortDic = new Dictionary<int, string>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> tempMeanValue = new Dictionary<string, double>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> meanValue = new Dictionary<string, double>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> sValueTemp = new Dictionary<string, double>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> tempsVal = new Dictionary<string, double>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> sValue = new Dictionary<string, double>();
        //PrincipalComponentAnalysis pca = null;
        //PrincipalComponentAnalysis pca2 = null;
        //PrincipalComponentAnalysis pca3 = null;
        //PrincipalComponentAnalysis allPCA = null;
        //double[,] PCAScores = null;
        //double[,] PCAScores1 = null;
        //double[,] PCAScores2 = null;
        //double[,] PCAScores3 = null;
        //int countForPCA = 0;
        //public int pcaNum = 0;
        /// <summary>
        /// Creates a new, blank database
        /// </summary>
        public LIDCNoduleDB()
        {
            nodules = new List<LIDCNodule>();
            noduleCount = new Dictionary<string, int>();
            minHaralick = new Dictionary<string, double>();
            maxHaralick = new Dictionary<string, double>();
            LocalminHaralick = new Dictionary<string, double>();
            LocalmaxHaralick = new Dictionary<string, double>();
        }
        /// <summary>
        /// Returns the minimum feature for the haralick
        /// </summary>
        /// <returns>minimum feature</returns>
        public Dictionary<string, double> getlocalMinDictionary()
        {
            return LocalminHaralick;
        }

        /// <summary>
        /// Returns the maximum feature for the haralick
        /// </summary>
        /// <returns>maximum feature</returns>
        public Dictionary<string, double> getlocalMaxDictionary()
        {
            return LocalmaxHaralick;
        }
        /// <summary>
        /// Creates a database from an XML file
        /// </summary>
        /// <param name="xmlFileName">Filename of XML document</param>
        public LIDCNoduleDB(string xmlFileName)
            : this()
        {
            LoadFromXML(xmlFileName, null);
        }

        /// <summary>
        /// Creates a database from an XML file and displays process status in a progress bar
        /// </summary>
        /// <param name="xmlFileName">Filename of XML document</param>
        /// <param name="pbar">ProgressBar object to update</param>
        public LIDCNoduleDB(string xmlFileName, System.Windows.Forms.ProgressBar pbar)
            : this()
        {
            LoadFromXML(xmlFileName, pbar);
        }


        /// <summary>
        /// Adds a nodule to the database (no duplicate checking!) and updates general statistics
        /// </summary>
        /// <param name="nodule">Nodule to be added</param>
        public void AddNodule(LIDCNodule nodule)
        {
            nodules.Add(nodule);

            // increment nodule counters
            if (noduleCount.ContainsKey(nodule.Nodule_no))
                noduleCount[nodule.Nodule_no]++;
            else
                noduleCount.Add(nodule.Nodule_no, 1);

            updateHaralickStatistics(nodule);
            LocalUpdateHaralickStatistics(nodule);

            // update mean size statistics
            int sumWidth = 0;
            int sumHeight = 0;
            foreach (LIDCNodule n in nodules)
            {
                sumWidth += n.Width;
                sumHeight += n.Height;
            }
            meanWidth = (double)sumWidth / (double)nodules.Count;
            meanHeight = (double)sumHeight / (double)nodules.Count;
        }

        /// <summary>
        /// Compares minimum/maximum Local statistics with a particular nodule's values and replaces with them if appropriate
        /// </summary>
        /// <param name="nodule">Nodule to compare</param>
        private void LocalUpdateHaralickStatistics(LIDCNodule nodule)
        {
            // update min/max Haralick statistics
            foreach (ArrayList firstHara in nodule.LocalHaralick)
            {
                foreach (Dictionary<string, double[,]> nod in firstHara)
                {
                    foreach (string s in nod.Keys)
                    {
                        if (LocalminHaralick.ContainsKey(s))
                        {
                            double[,] tempFeature = nod[s];
                            for (int i = 0; i < tempFeature.GetLength(0); i++)
                            {
                                for (int k = 0; k < tempFeature.GetLength(1); k++)
                                {
                                    if (LocalminHaralick[s] > tempFeature[i, k])
                                        LocalminHaralick[s] = tempFeature[i, k];
                                    else if (LocalmaxHaralick[s] < tempFeature[i, k])
                                        LocalmaxHaralick[s] = tempFeature[i, k];
                                }
                            }
                        }
                        else
                        {
                            double[,] tempFeature = nod[s];
                            double tempMin = double.MaxValue;
                            double tempMax = double.MinValue;
                            for (int i = 0; i < tempFeature.GetLength(0); i++)
                            {
                                for (int k = 0; k < tempFeature.GetLength(1); k++)
                                {
                                    if (tempMin > tempFeature[i, k])
                                        tempMin = tempFeature[i, k];
                                    else if (tempMax < tempFeature[i, k])
                                        tempMax = tempFeature[i, k];
                                }
                            }
                            LocalminHaralick.Add(s, tempMin);
                            LocalmaxHaralick.Add(s, tempMax);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Compares minimum/maximum Global statistics with a particular nodule's values and replaces with them if appropriate
        /// </summary>
        /// <param name="nodule">Nodule to compare</param>
        private void updateHaralickStatistics(LIDCNodule nodule)
        {
            // update min/max Haralick statistics
            foreach (string s in nodule.Haralick.Keys)
            {
                if (minHaralick.ContainsKey(s))
                {
                    if (minHaralick[s] > nodule.Haralick[s])
                        minHaralick[s] = nodule.Haralick[s];
                    else if (maxHaralick[s] < nodule.Haralick[s])
                        maxHaralick[s] = nodule.Haralick[s];
                }
                else
                {
                    minHaralick.Add(s, nodule.Haralick[s]);
                    maxHaralick.Add(s, nodule.Haralick[s]);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the nodules in the database
        /// </summary>
        /// <remarks>
        /// This allows code like the following:
        /// <code>
        /// foreach (LIDCNodule n in myNoduleDB)
        /// {
        ///     MessageBox.Show(n.seriesInstanceUID);
        /// }
        /// </code>
        /// </remarks>
        /// <returns>Enumerator from the actual collection</returns>
        public List<LIDCNodule>.Enumerator GetEnumerator()
        {
            return nodules.GetEnumerator();
        }
        

        /// <summary>
        /// Loads nodules from an XML file
        /// </summary>
        /// <param name="xmlFileName">Filename of XML document</param>
        public void LoadFromXML(string xmlFileName)
        {
            LoadFromXML(xmlFileName, null, true);
        }

        /// <summary>
        /// Loads nodules from an XML file (optionally only merging and not adding new nodules)
        /// </summary>
        /// <param name="xmlFileName">Filename of XML document</param>
        /// <param name="addNewNodules">Whether to add new nodules or just merge in new data for existing nodules</param>
        public void LoadFromXML(string xmlFileName, bool addNewNodules)
        {
            LoadFromXML(xmlFileName, null, addNewNodules);
        }

        /// <summary>
        /// Loads nodules from an XML file and displays process status in a progress bar
        /// </summary>
        /// <param name="xmlFileName">Filename of XML document</param>
        /// <param name="pbar">ProgressBar object to update</param>
        public void LoadFromXML(string xmlFileName, System.Windows.Forms.ProgressBar pbar)
        {
            LoadFromXML(xmlFileName, pbar, true);
        }

        /// <summary>
        /// Loads nodules from an XML file and displays process status in a progress bar 
        /// (optionally only merging and not adding new nodules)
        /// </summary>
        /// <param name="xmlFileName">Filename of XML document</param>
        /// <param name="pbar">ProgressBar object to update</param>
        /// <param name="addNewNodules">Whether to add new nodules or just merge in new data for existing nodules</param>
        public void LoadFromXML(string xmlFileName, System.Windows.Forms.ProgressBar pbar, bool addNewNodules)
        {
            // open XML document
            XmlDocument doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(xmlFileName);
            doc.Load(reader);
            reader.Close();

            // get a list of XML nodes representing all the nodules from the file
            XmlNodeList nodes = doc.SelectNodes("nodules/nodule");

            if (pbar != null)
            {
                // initialize progress bar
                pbar.Minimum = 0;
                pbar.Maximum = nodes.Count;
                pbar.Value = 0;
                pbar.Refresh();
            }

            // read each element
            foreach (XmlElement n in nodes)
            {
                LIDCNodule nod = new LIDCNodule(n);
                
                foreach (LIDCNodule nn in nodules)
                {
                    // if an equivalent nodule has already been added,
                    // just merge in the new data
                    if (nn.Equals(nod))//nn.NUID == nod.NUID)//nn.Equals(nod))
                    {
                        nn.Merge(nod);
                        LocalUpdateHaralickStatistics(nod);
                        updateHaralickStatistics(nod);
                    }
                }
                if (addNewNodules)
                {
                    // a new nodule -- add it to the collection
                    AddNodule(new LIDCNodule(n));
                }
                if (pbar != null)
                {
                    // update progress bar
                    pbar.Value++;
                    pbar.Refresh();
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        /// <summary>
        /// Saves all nodule data to an XML file
        /// </summary>
        /// <param name="xmlFileName">Filename of XML document</param>
        public void SaveToXML(string xmlFileName)
        {
            // create XML file
            //if (xmlFileName != Util.DATA_PATH + "nodules-local.xml")
            //{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" ?>\n<nodules>\n</nodules>\n");
            XmlElement root = doc.DocumentElement;

            // add element for each nodule
            foreach (LIDCNodule n in nodules)
            {
                root.AppendChild(n.ToXML(doc));
            }

            // save to file
            doc.Save(xmlFileName);
        }

        
        /// <summary>
        /// Returns the total number of nodules in the database
        /// </summary>
        public int TotalNoduleCount
        {
            get
            {
                //return totalNoduleCount;
                return nodules.Count;
            }
        }

        /// <summary>
        /// Returns the number of unique nodules in the database (not counting multiple instances of the same nodule)
        /// </summary>
        public int UniqueNodulecount
        {
            get
            {
                return noduleCount.Count;
            }
        }

        /// <summary>
        /// Counts appearances of a unique nodule (a nodule may appear on multiple slices and/or annotated by multiple physicians
        /// </summary>
        /// <param name="noduleNumber">Nodule number</param>
        /// <returns>Number of times that nodule appears in the database</returns>
        public int GetNoduleCount(string noduleNumber)
        {
            if (noduleCount.ContainsKey(noduleNumber))
            {
                return noduleCount[noduleNumber];
            }
            else { return 0; }
        }
        
        /// <summary>
        /// Returns the average nodule width
        /// </summary>
        public double MeanNoduleWidth
        {
            get
            {
                return meanWidth;
            }
        }

        /// <summary>
        /// Returns the average nodule height
        /// </summary>
        public double MeanNoduleHeight
        {
            get
            {
                return meanHeight;
            }
        }


        /// <summary>
        /// Builds a feature vector
        /// </summary>
        /// <param name="nodule">Nodule of interest</param>
        /// <param name="features">Named descriptors for feature vector</param>
        /// <returns>Array of features</returns>
        public double[] GetFeatureVector(LIDCNodule nodule, string[] features)
        {
            double[] vector = new double[features.Length];
            for (int i = 0; i < features.Length; i++)
            {
                if (!(nodule.Haralick.ContainsKey(features[i])))
                {
                    throw new Exception("Nodule contains no information for feature \"" + features[i] + "\".");
                }
                else
                {
                    vector[i] = nodule.Haralick[features[i]];
                }
            }
            return vector;
        }

        /// <summary>
        /// Builds a normalized feature vector (all values are scaled to [0..1])
        /// </summary>
        /// <param name="nodule">Nodule of interest</param>
        /// <param name="features">Named descriptors for feature vector</param>
        /// <returns>Array of features</returns>
        public double[] GetNormalizedFeatureVector(LIDCNodule nodule, string[] features)
        {
            double[] vector = new double[features.Length];
            for (int i = 0; i < features.Length; i++)
            {
                if (!(nodule.Haralick.ContainsKey(features[i])))
                {
                    throw new Exception("Nodule contains no information for feature \"" + features[i] + "\".");
                }
                else
                {
                    vector[i] = (nodule.Haralick[features[i]] - minHaralick[features[i]]) / (maxHaralick[features[i]] - minHaralick[features[i]]);
                }
            }
            return vector;
        }

        /// <summary>
        /// Returns the value of a single Haralick feature, normalized on a scale of [0..1]
        /// </summary>
        /// <param name="nodule">Nodule of interest</param>
        /// <param name="feature">Named feature for extraction</param>
        /// <returns>Normalized feature value</returns>
        public double GetNormalizedFeature(LIDCNodule nodule, string feature)
        {
            if (!(nodule.Haralick.ContainsKey(feature)))
            {
                throw new Exception("Nodule contains no information for feature \"" + feature + "\".");
            }
            else
            {
                return (nodule.Haralick[feature] - minHaralick[feature]) / (maxHaralick[feature] - minHaralick[feature]);
            }
        }

        /// <summary>
        /// Runs a query on the nodule database (default options)
        /// </summary>
        /// <param name="queryImage">Nodule to query for</param>
        /// <param name="nItems">Number of items to return</param>
        /// <returns>List of query results</returns>
        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, int nItems)
        {
            return RunQuery(queryImage, DEFAULT_FEATURE, DEFAULT_SIMILARITY, nItems);
        }

        /// <summary>
        /// Runs a query on the nodule database (no distance threshold)
        /// </summary>
        /// <param name="queryImage">Nodule to query for</param>
        /// <param name="feature">Image features to use</param>
        /// <param name="similarity">Similarity measure to use</param>
        /// <param name="nItems">Number of items to return</param>
        /// <returns>List of query results</returns>
        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, string feature, string similarity, int nItems)
        {
            return RunQuery(queryImage, feature, similarity, nItems, double.PositiveInfinity);
        }

        /// <summary>
        /// Runs a query on the nodule database (no limit to # of items returned)
        /// </summary>
        /// <param name="queryImage">Nodule to query for</param>
        /// <param name="feature">Image features to use</param>
        /// <param name="similarity">Similarity measure to use</param>
        /// <param name="threshold">Highest distance to return</param>
        /// <returns>List of query results</returns>
        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, string feature, string similarity, double threshold)
        {
            return RunQuery(queryImage, feature, similarity, nodules.Count, threshold);
        }

        /// <summary>
        /// Runs a query on the nodule database
        /// </summary>
        /// <param name="queryImage">Nodule to query for</param>
        /// <param name="feature">Image features to use</param>
        /// <param name="similarity">Similarity measure to use</param>
        /// <param name="nItems">Number of items to return</param>
        /// <param name="threshold">Highest distance to return</param>
        /// <returns>List of query results</returns>
        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, string feature, string similarity, int nItems, double threshold)
        {
            LinkedList<LIDCNodule> rnodes = new LinkedList<LIDCNodule>();
            /*if (feature.StartsWith("All-Features-w/-PCA"))
            {
                rnodes = PCAdistance(queryImage);
                countForPCA++;
            }*/
           // else
           // {
                if (feature.StartsWith("Annotations"))
                {
                    AnnotateVariables();
                    rnodes = annoteSort(queryImage);
                }
                else
                {
                    if (feature.StartsWith("All-Features"))
                    {
                        rnodes = AllFeatures(queryImage, feature, similarity, nItems, threshold);
                    }
                    else
                    {
                        LinkedList<string> currentFeatureVector = new LinkedList<string>();
                        if (feature.StartsWith("Global") || feature.StartsWith("Local"))
                        {
                            string[] hfeatures = feature.Split(new char[] { ' ' });
                            for (int i = 1; i < hfeatures.Length; i++)
                                if (hfeatures[i] != "")
                                    currentFeatureVector.AddLast(hfeatures[i]);
                        }

                        // linked list to hold all nodules, sorted by distance from selected nodule (ascending order)

                        // for each nodule
                        foreach (LIDCNodule temp in nodules)
                        {
                            // don't add the selected image
                            if (!(temp.Equals(queryImage)))
                            {
                                // calculate feature distances
                                double ddist = 0.0;
                                if (feature.StartsWith("Local"))
                                {
                                    if (queryImage.LocalHaralick == null)
                                        throw new Exception("This nodule does not have Local descriptors.");
                                    else
                                    {
                                        if (temp.LocalHaralick != null)
                                            ddist = Similarity.CalcLocalHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                                        else
                                            ddist = double.NaN;

                                    }
                                }
                                if (feature.StartsWith("Global"))
                                {
                                    if (queryImage.Haralick == null)
                                        throw new Exception("This nodule does not have Global descriptors.");
                                    else
                                    {
                                        if (temp.Haralick != null)
                                            ddist = Similarity.CalcHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                                        else
                                            ddist = double.NaN;
                                    }
                                }
                                else if (feature.StartsWith("Gabor"))
                                {
                                    if (queryImage.GaborHist == null)
                                        throw new Exception("This nodule does not have Gabor descriptors.");
                                    else
                                    {
                                        if (temp.GaborHist != null)
                                            ddist = Similarity.CalcGaborDistance(queryImage, temp, similarity);
                                        else
                                            ddist = double.NaN;
                                    }
                                }
                                else if (feature.StartsWith("Markov"))
                                {
                                    if (queryImage.MarkovHist == null)
                                        throw new Exception("This nodule does not have Markov descriptors.");
                                    else
                                    {
                                        if (temp.MarkovHist != null)
                                            ddist = Similarity.CalcMarkovDistance(queryImage, temp, similarity);
                                        else
                                            ddist = double.NaN;
                                    }
                                }
                                temp.Temp_dist = ddist;

                                // only add images with real distances
                                if (!(double.IsNaN(ddist)))
                                {
                                    // limit by threshold
                                    if (double.IsPositiveInfinity(threshold) || ddist < threshold)
                                    {
                                        if (queryImage.Annotations != null && temp.Annotations != null &&
                                            queryImage.Annotations.ContainsKey("malignancy") && temp.Annotations.ContainsKey("malignancy"))
                                        {
                                            temp.Temp_adist = Math.Abs((double)queryImage.Annotations["malignancy"] - (double)temp.Annotations["malignancy"]);
                                        }

                                        // insert the new node at the appropriate place in the linked list
                                        LinkedListNode<LIDCNodule> n = rnodes.First;
                                        while (n != null && n.Value.Temp_dist <= temp.Temp_dist)
                                            n = n.Next;
                                        if (n == null)
                                            rnodes.AddLast(new LinkedListNode<LIDCNodule>(temp));
                                        else
                                            rnodes.AddBefore(n, new LinkedListNode<LIDCNodule>(temp));
                                    }
                                }
                            }
                        }
                    }
                }
            //}

            // linked list to hold the closest nodules to the selected nodule
            LinkedList<LIDCNodule> cnodes = new LinkedList<LIDCNodule>();

            int ii = 0;
            foreach (LIDCNodule nod in rnodes)
                if (ii++ < nItems)
                    cnodes.AddLast(nod);

            return cnodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryImage"></param>
        /// <param name="feature"></param>
        /// <param name="similarity"></param>
        /// <param name="nItems"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public LinkedList<LIDCNodule> AllFeatures(LIDCNodule queryImage,string feature, string similarity, int nItems, double threshold)
        {
            LinkedList<LIDCNodule> rnodes = new LinkedList<LIDCNodule>();
            LinkedList<string> currentFeatureVector = new LinkedList<string>();
            string[] hfeatures = feature.Split(new char[] { ' ' });
            for (int i = 1; i < hfeatures.Length; i++)
                if (hfeatures[i] != "")
                    currentFeatureVector.AddLast(hfeatures[i]);

            // linked list to hold all nodules, sorted by distance from selected nodule (ascending order)

            // for each nodule
            foreach (LIDCNodule temp in nodules)
            {
                // don't add the selected image
                if (!(temp.Equals(queryImage)))
                {
                    // calculate feature distances
                    double ddist = 0.0;
                    if (queryImage.LocalHaralick == null)
                        throw new Exception("This nodule does not have Local descriptors.");
                    else
                    {
                        similarity = "Jeffrey Diverg. (H)";
                        if (temp.LocalHaralick != null)
                            ddist = ddist + Similarity.CalcLocalHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                        else
                            ddist = double.NaN;

                    }
                    if (queryImage.GaborHist == null)
                        throw new Exception("This nodule does not have Gabor descriptors.");
                    else
                    {
                        similarity = "Chi-Square (H)";
                        if (temp.GaborHist != null)
                            ddist = ddist + Similarity.CalcGaborDistance(queryImage, temp, similarity);
                        else
                            ddist = double.NaN;
                    }
                    if (queryImage.MarkovHist == null)
                        throw new Exception("This nodule does not have Markov descriptors.");
                    else
                    {
                        similarity = "Chi-Square (H)";
                        if (temp.MarkovHist != null)
                            ddist = ddist + Similarity.CalcMarkovDistance(queryImage, temp, similarity);
                        else
                            ddist = double.NaN;
                    }
                    if (queryImage.Haralick == null)
                        throw new Exception("This nodule does not have Global descriptors.");
                    else
                    {
                        similarity = "Euclidean";
                        if (temp.Haralick != null)
                            ddist = ddist + Similarity.CalcHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                        else
                            ddist = double.NaN;
                    }
                    temp.Temp_dist = ddist / 4;

                    // only add images with real distances
                    if (!(double.IsNaN(ddist)))
                    {
                        // limit by threshold
                        if (double.IsPositiveInfinity(threshold) || ddist < threshold)
                        {
                            // insert the new node at the appropriate place in the linked list
                            LinkedListNode<LIDCNodule> n = rnodes.First;
                            while (n != null && n.Value.Temp_dist <= temp.Temp_dist)
                                n = n.Next;
                            if (n == null)
                                rnodes.AddLast(new LinkedListNode<LIDCNodule>(temp));
                            else
                                rnodes.AddBefore(n, new LinkedListNode<LIDCNodule>(temp));
                        }
                    }
                }
            }
            return rnodes;
        }

        /// <summary>
        /// Calculate precision and recall for query results
        /// </summary>
        /// <param name="queryImage">Nodule queried for</param>
        /// <param name="queryResults">List of query results</param>
        /// <returns>Two element array: [precision, recall]</returns>
        public double[] CalcPrecisionAndRecall(LIDCNodule queryImage, LinkedList<LIDCNodule> queryResults)
        {
            // calculate precision and recall
            int totalRetrieved = queryResults.Count;
            int numAppearances = GetNoduleCount(queryImage.Nodule_no) - 1;

            int numRetrieved = 0;
            foreach (LIDCNodule nod in queryResults)
                if (nod.Nodule_no.Equals(queryImage.Nodule_no))
                    numRetrieved++;

            double[] rp = new double[2];
            rp[0] = (double)numRetrieved / (double)totalRetrieved;
            rp[1] = (double)numRetrieved / (double)numAppearances;
            return rp;
        }

        /// <summary>
        /// Run a query for each image in the database with given query parameters and calculate mean precision and recall
        /// </summary>
        /// <param name="feature">Image features to use</param>
        /// <param name="similarity">Similarity measure to use</param>
        /// <param name="nItems">Number of items to return</param>
        /// <param name="threshold">Highest distance to return</param>
        /// <returns>Two element array: [precision, recall]</returns>
        public double[] CalcMeanPrecisionAndRecall(string feature, string similarity, int nItems, double threshold)
        {
            return CalcMeanPrecisionAndRecall(feature, similarity, nItems, threshold, null);
        }

        /// <summary>
        /// Run a query for each image in the database with given query parameters and calculate mean precision based off of annotations
        /// and displays process status in a progress bar.
        /// </summary>
        /// <param name="feature">Image features to use</param>
        /// <param name="similarity">Similarity measure to use</param>
        /// <param name="nItems">Number of items to return</param>
        /// <param name="threshold">Highest distance to return</param>
        /// <param name="pbar">ProgressBar object to update</param>
        /// <returns>Two element array: [precision, recall]</returns>
        public double[] CalcMeanPrecisionUsingAnnotations(string feature, string similarity, int nItems, double threshold, System.Windows.Forms.ProgressBar pbar)
        {
            double[] mpr = new double[2];
            mpr[0] = double.NaN;
            mpr[1] = double.NaN;

            if (pbar != null)
            {
                pbar.Minimum = 0;
                pbar.Maximum = nodules.Count;
                pbar.Value = 0;
            }

            int i = 0;
            int total = 0;
            AnnotateVariables();

            foreach (LIDCNodule inod in nodules)
            {
                //sorted 
                AnnotateVariables();
                LinkedList<LIDCNodule> rnodes = annoteSort(inod);

                if (pbar != null)
                {
                    pbar.Value = i++;
                    pbar.Refresh();
                    System.Windows.Forms.Application.DoEvents();
                }

                // run query
                LinkedList<LIDCNodule> cnodes = RunQuery(inod, feature, similarity, nodules.Count, threshold);

                // calculate precision and recall
                double[] rp = new double[2];
                rp[0] = compareAnnotateToOtherMethod(rnodes, cnodes, nItems);
                rp[1] = 0;
                // update mean precision/recall stats
                if (!double.IsNaN(rp[1]))
                {
                    if (double.IsNaN(mpr[0]))
                    {
                        mpr[0] = rp[0];
                        mpr[1] = rp[1];
                    }
                    else
                    {
                        mpr[0] += rp[0];       
                        mpr[1] += rp[1];
                    }
                }
                total++;
            }
            mpr[0] /= (double)total;
            mpr[1] /= (double)total;
            return mpr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inod"></param>
        /// <returns></returns>
        public LinkedList<LIDCNodule> annoteSort(LIDCNodule inod)
        {
            LinkedList<LIDCNodule> rnodes = new LinkedList<LIDCNodule>();
            foreach (LIDCNodule annotateSort in nodules)
            {
                annotateSort.annotateDist = normalizedJaccardMCDDistance(inod.Annotations, annotateSort.Annotations, sValue, meanValue);
                if (!(double.IsNaN(annotateSort.annotateDist)))
                {
                    // limit by threshold
                    if (annotateSort.NUID != inod.NUID && annotateSort.annotateDist < double.PositiveInfinity)
                    {
                        // insert the new node at the appropriate place in the linked list
                        LinkedListNode<LIDCNodule> n = rnodes.First;
                        while (n != null && n.Value.annotateDist <= annotateSort.annotateDist)
                            n = n.Next;
                        if (n == null)
                            rnodes.AddLast(new LinkedListNode<LIDCNodule>(annotateSort));
                        else
                            rnodes.AddBefore(n, new LinkedListNode<LIDCNodule>(annotateSort));
                    }
                }
            }
            return rnodes;
        }
        /// <summary>
        /// 
        /// </summary>
        public void AnnotateVariables()
        {

            if (sortDic.Count < nodules.Count)
            {
                //calculate all the dictionaries used in annotation sort.
                foreach (LIDCNodule sort in nodules)
                {
                    //sortDic has the unique nodule ID as the key and the nodule number as the value.
                    sortDic.Add(sort.NUID, sort.Nodule_no);
                }
                int countMean = 0;
                foreach (LIDCNodule meanIt in nodules)
                {
                    tempMeanValue = findMean(tempMeanValue, meanIt.Annotations);
                    countMean++;
                }
                foreach (string keyIt in tempMeanValue.Keys)
                {
                    meanValue.Add(keyIt, tempMeanValue[keyIt] / countMean);
                }
                foreach (string a in tempMeanValue.Keys)
                {
                    sValueTemp.Add(a, 0);
                }
                int countsValueTemp = 0;
                foreach (LIDCNodule sIt in nodules)
                {
                    tempsVal = subtractMeanFromIndividual(sIt.Annotations, meanValue);
                    sValueTemp = addDictionaries(sValueTemp, tempsVal);
                    countsValueTemp++;
                }
                foreach (string key in sValueTemp.Keys)
                {
                    sValue.Add(key, sValueTemp[key] / countsValueTemp);
                }
            }
        }

        /// <summary>
        /// Run a query for each image in the database with given query parameters and calculate mean precision and recall
        /// and displays process status in a progress bar.
        /// </summary>
        /// <param name="feature">Image features to use</param>
        /// <param name="similarity">Similarity measure to use</param>
        /// <param name="nItems">Number of items to return</param>
        /// <param name="threshold">Highest distance to return</param>
        /// <param name="pbar">ProgressBar object to update</param>
        /// <returns>Two element array: [precision, recall]</returns>
        public double[] CalcMeanPrecisionAndRecall(string feature, string similarity, int nItems, double threshold, System.Windows.Forms.ProgressBar pbar)
        {
            double[] mpr = new double[2];
            mpr[0] = double.NaN;
            mpr[1] = double.NaN;

            if (pbar != null)
            {
                pbar.Minimum = 0;
                pbar.Maximum = nodules.Count;
                pbar.Value = 0;
            }

            int i = 0;
            int total = 0;
            foreach (LIDCNodule inod in nodules)
            {
                if (pbar != null)
                {
                    pbar.Value = i++;
                    pbar.Refresh();
                    System.Windows.Forms.Application.DoEvents();
                }

                // run query
                LinkedList<LIDCNodule> cnodes = RunQuery(inod, feature, similarity, nItems, threshold);
                // calculate precision and recall
                double[] rp = CalcPrecisionAndRecall(inod, cnodes);
                // update mean precision/recall stats
                if (!double.IsNaN(rp[1]))
                {
                    if (double.IsNaN(mpr[0]))
                    {
                        mpr[0] = rp[0];
                        mpr[1] = rp[1];
                    }
                    else
                    {
                        mpr[0] += rp[0];
                        mpr[1] += rp[1];
                    }
                }
                total++;
            }
            mpr[0] /= (double)total;
            mpr[1] /= (double)total;
            return mpr;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotate"></param>
        /// <param name="comparedWith"></param>
        /// <param name="nItems"></param>
        /// <returns></returns>
        public double compareAnnotateToOtherMethod(LinkedList<LIDCNodule> annotate, LinkedList<LIDCNodule> comparedWith, int nItems)
        {
            int numItemsUsed = nItems;//10
            int n = annotate.Count;
            int nCompare = 0;
            int count = 0;
            double annotateCompareTotal = 0;
            LinkedListNode<LIDCNodule> compAnnotate = annotate.First;
            LinkedListNode<LIDCNodule> compAnnotate2 = comparedWith.First;
            LinkedList<LIDCNodule> FirstTenSeparateNods = new LinkedList<LIDCNodule>();
            LinkedList<LIDCNodule> FirstTenSeparateNods2 = new LinkedList<LIDCNodule>();
            int tenSeparate = 0;
            Dictionary<string, int> uniqueNoduleNo = new Dictionary<string, int>();
            while (tenSeparate < numItemsUsed)
            {
                if (!uniqueNoduleNo.ContainsKey(compAnnotate.Value.Nodule_no))
                {
                    tenSeparate++;
                    FirstTenSeparateNods.AddLast(new LinkedListNode<LIDCNodule>(compAnnotate.Value));
                    uniqueNoduleNo.Add(compAnnotate.Value.Nodule_no, 0);
                }
                compAnnotate = compAnnotate.Next;
            }
            uniqueNoduleNo.Clear();
            tenSeparate = 0;
            while (tenSeparate < numItemsUsed)
            {
                if (!uniqueNoduleNo.ContainsKey(compAnnotate2.Value.Nodule_no))
                {
                    tenSeparate++;
                    FirstTenSeparateNods2.AddLast(new LinkedListNode<LIDCNodule>(compAnnotate2.Value));
                    uniqueNoduleNo.Add(compAnnotate2.Value.Nodule_no, 0);
                }
                compAnnotate2 = compAnnotate2.Next;
            }
            compAnnotate = FirstTenSeparateNods.First;
            for (int q = 0; q < numItemsUsed; q++)//LIDCNodule compAnnotate in annotate)
            {
                compAnnotate2 = FirstTenSeparateNods2.First;
                n = numItemsUsed - q;//annotate.Count - count;
                nCompare = numItemsUsed;//comparedWith.Count;
                Dictionary<string, int> noRepeats = new Dictionary<string, int>();
                while (compAnnotate2 != null && compAnnotate2.Value.Nodule_no != compAnnotate.Value.Nodule_no && nCompare > 0)
                {
                    noRepeats.Add(compAnnotate2.Value.Nodule_no, 0);
                    nCompare--;
                    compAnnotate2 = compAnnotate2.Next;
                }
                annotateCompareTotal = annotateCompareTotal + (nCompare * n);
                count++;
                compAnnotate = compAnnotate.Next;
            }
            return annotateCompareTotal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addVal1"></param>
        /// <param name="addVal2"></param>
        /// <returns></returns>
        public Dictionary<string, double> addDictionaries(Dictionary<string, double> addVal1, Dictionary<string, double> addVal2)
        {
            Dictionary<string, double> summedValue = new Dictionary<string, double>();
            foreach (string key in addVal2.Keys)
            {
                summedValue.Add(key, (addVal1[key] + addVal2[key]));
            }
            return summedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sIt"></param>
        /// <param name="meanValue"></param>
        /// <returns></returns>
        public Dictionary<string, double> subtractMeanFromIndividual(Dictionary<string, int> sIt, Dictionary<string, double> meanValue)
        {
            Dictionary<string, double> temps = new Dictionary<string, double>();
            foreach (string key in sIt.Keys)
            {
                temps.Add(key, Math.Abs((double)sIt[key] - meanValue[key]));
            }
            return temps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentMean"></param>
        /// <param name="addValues"></param>
        /// <returns></returns>
        public Dictionary<string, double> findMean(Dictionary<string, double> currentMean, Dictionary<string, int> addValues)
        {
            foreach (string key in addValues.Keys)
            {
                if (!currentMean.ContainsKey(key))
                {
                    currentMean.Add(key, (double)addValues[key]);
                }
                else
                {
                    currentMean[key] = currentMean[key] + (double)addValues[key];
                }
            }
            return currentMean;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare1"></param>
        /// <param name="compare2"></param>
        /// <param name="sValue"></param>
        /// <param name="meanValue"></param>
        /// <returns></returns>
        public double normalizedJaccardMCDDistance(Dictionary<string, int> compare1, Dictionary<string, int> compare2, Dictionary<string, double> sValue, Dictionary<string, double> meanValue)
        {
            double Jdistance = 0;
            int count = 0;
            double[] tempForEuclidean1 = new double[compare1.Keys.Count - 2];
            double[] tempForEuclidean2 = new double[compare1.Keys.Count - 2];
            foreach (string annotations in compare1.Keys)
            {
                if (annotations == "calcification")
                {
                    if (compare1[annotations] != compare2[annotations])
                    {
                        Jdistance = Jdistance + 1.2;
                    }
                }
                else
                {
                    if (annotations == "internalStructure")
                    {
                        if (compare1[annotations] != compare2[annotations])
                        {
                            Jdistance = Jdistance + .8;
                        }
                    }
                    else
                    {
                        if (sValue[annotations] != 0)
                        {
                            tempForEuclidean1[count] = (((double)compare1[annotations] - meanValue[annotations]) / sValue[annotations]);
                            tempForEuclidean2[count] = (((double)compare2[annotations] - meanValue[annotations]) / sValue[annotations]);
                            count++;
                        }
                    }
                }
            }
            Jdistance = Jdistance + distEuclidean(tempForEuclidean1, tempForEuclidean2);
            Jdistance = Jdistance / compare1.Keys.Count;
            return Jdistance;
        }

       /* public LinkedList<LIDCNodule> PCAdistance(LIDCNodule inod)
        {
            int howManyToIncludeInPCA =pcaNum;

            foreach (LIDCNodule nod in nodules)
            {
                nod.PCAperNod = new double[howManyToIncludeInPCA];
            }
            int counter = 0;
            foreach (LIDCNodule tempNod in nodules)
            {
                for (int i = 0; i < howManyToIncludeInPCA; i++)
                {
                    tempNod.PCAperNod[i] = PCAScores[counter, i];
                }
                counter++;
            }
            LinkedList<LIDCNodule> rnodes = new LinkedList<LIDCNodule>();
            foreach (LIDCNodule PCAsort in nodules)
            {
                PCAsort.pcaDist = distManhattan(PCAsort.PCAperNod, inod.PCAperNod);
                if (!(double.IsNaN(PCAsort.pcaDist)))
                {
                    // limit by threshold
                    if (PCAsort.NUID != inod.NUID && PCAsort.pcaDist < double.PositiveInfinity)
                    {
                        // insert the new node at the appropriate place in the linked list
                        LinkedListNode<LIDCNodule> n = rnodes.First;
                        while (n != null && n.Value.pcaDist <= PCAsort.pcaDist)
                            n = n.Next;
                        if (n == null)
                            rnodes.AddLast(new LinkedListNode<LIDCNodule>(PCAsort));
                        else
                            rnodes.AddBefore(n, new LinkedListNode<LIDCNodule>(PCAsort));
                    }
                }
            }
            return rnodes;
        }
        public void doPCA(System.Windows.Forms.ProgressBar pbar)
        {
            if (pbar != null)
            {
                // initialize progress bar
                pbar.Minimum = 0;
                pbar.Maximum = nodules.Count;
                pbar.Value = 0;
                pbar.Refresh();
            }

            int whichNod = 0;
            LIDCNodule PCNod1 = nodules[0];
            double[] lengthFinder = (double[])PCNod1.localHist[0];
            double[,] allFeatures = new double[nodules.Count, PCNod1.GaborHist.Length * PCNod1.GaborHist[0, 0].Length + PCNod1.localHist.Count * lengthFinder.Length + PCNod1.MarkovHist.Length * PCNod1.MarkovHist[0].Length];
            foreach (LIDCNodule PCNod in nodules)
            {
                if (PCNod.LocalHaralick == null)
                    throw new Exception("This nodule does not have Local descriptors.");
                else
                {
                    if (PCNod.GaborHist == null)
                        throw new Exception("This nodule does not have Gabor descriptors.");
                    else
                    {
                        if (PCNod.MarkovHist == null)
                            throw new Exception("This nodule does not have Markov descriptors.");
                        else
                        {
                            int currentLoc = 0;
                            for (int i = 0; i < PCNod.localHist.Count; i++)
                            {
                                double[] temp = (double[])PCNod.localHist[i];
                                for (int k = 0; k < temp.Length; k++)
                                {
                                    allFeatures[whichNod, currentLoc] = temp[k];
                                    currentLoc++;
                                }
                            }

                            for (int i = 0; i < PCNod.GaborHist.GetLength(0); i++)
                            {
                                for (int k = 0; k < PCNod.GaborHist.GetLength(1); k++)
                                {
                                    for (int j = 0; j < PCNod.GaborHist[i, k].Length; j++)
                                    {
                                        allFeatures[whichNod, currentLoc] = PCNod.GaborHist[i, k][j];
                                        currentLoc++;
                                    }
                                }
                            }
                            for (int i = 0; i < PCNod.MarkovHist.Length; i++)
                            {
                                for (int k = 0; k < PCNod.MarkovHist[i].Length; k++)
                                {
                                    allFeatures[whichNod, currentLoc] = PCNod.MarkovHist[i][k];
                                    currentLoc++;
                                }
                            }
                        }
                    }
                }
                if (pbar != null)
                {
                    // update progress bar
                    pbar.Value++;
                    pbar.Refresh();
                    System.Windows.Forms.Application.DoEvents();
                }
                whichNod++;
            }

            CenterSpace.NMath.Core.DoubleMatrix data = new CenterSpace.NMath.Core.DoubleMatrix(allFeatures);
            // Class PrincipalComponentAnalysis performs a principal component
            // analysis on a given data set. The data may optionally be centered and
            // scaled before analysis takes place. By default, variables are centered
            // but not scaled.
            pca = new PrincipalComponentAnalysis(data, false, false); //pca = new PrincipalComponentAnalysis(data);
            MessageBox.Show(pca.Threshold(.99).ToString());
            PCAScores = pca.Scores.ToArray();
         

            /*
            if (pbar != null)
            {
                // initialize progress bar
                pbar.Minimum = 0;
                pbar.Maximum = nodules.Count;
                pbar.Value = 0;
                pbar.Refresh();
            }

            int whichNod = 0;
            LIDCNodule PCNod1 = nodules[0];
            double[] lengthFinder = (double[])PCNod1.localHist[0];
            double[,] gaborPCA = new double[nodules.Count, PCNod1.GaborHist.Length * PCNod1.GaborHist[0, 0].Length];
            double[,] localPCA = new double[nodules.Count, PCNod1.localHist.Count * lengthFinder.Length];
            double[,] markovPCA = new double[nodules.Count, PCNod1.MarkovHist.Length * PCNod1.MarkovHist[0].Length];
            foreach (LIDCNodule PCNod in nodules)
            {
                if (PCNod.LocalHaralick == null)
                    throw new Exception("This nodule does not have Local descriptors.");
                else
                {
                    if (PCNod.GaborHist == null)
                        throw new Exception("This nodule does not have Gabor descriptors.");
                    else
                    {
                        if (PCNod.MarkovHist == null)
                            throw new Exception("This nodule does not have Markov descriptors.");
                        else
                        {
                            int currentLoc = 0;
                            for (int i = 0; i < PCNod.localHist.Count; i++)
                            {
                                double[] temp = (double[])PCNod.localHist[i];
                                for (int k = 0; k < temp.Length; k++)
                                {
                                    localPCA[whichNod, currentLoc] = temp[k];
                                    currentLoc++;
                                }
                            }
                            currentLoc = 0;
                            for (int i = 0; i < PCNod.GaborHist.GetLength(0); i++)
                            {
                                for (int k = 0; k < PCNod.GaborHist.GetLength(1); k++)
                                {
                                    for (int j = 0; j < PCNod.GaborHist[i,k].Length; j++)
                                    {
                                        gaborPCA[whichNod, currentLoc] = PCNod.GaborHist[i, k][j];
                                        currentLoc++;
                                    }
                                }
                            }
                            currentLoc = 0;
                            for (int i = 0; i < PCNod.MarkovHist.Length; i++)
                            {
                                for (int k = 0; k < PCNod.MarkovHist[i].Length; k++)
                                {
                                    markovPCA[whichNod, currentLoc] = PCNod.MarkovHist[i][k];
                                    currentLoc++;
                                }
                            }
                             
                        }
                    }
                }
                if (pbar != null)
                {
                    // update progress bar
                    pbar.Value++;
                    pbar.Refresh();
                    System.Windows.Forms.Application.DoEvents();
                }
                whichNod++;
            }

            CenterSpace.NMath.Core.DoubleMatrix data = new CenterSpace.NMath.Core.DoubleMatrix(localPCA);
            CenterSpace.NMath.Core.DoubleMatrix data2 = new CenterSpace.NMath.Core.DoubleMatrix(gaborPCA);
            CenterSpace.NMath.Core.DoubleMatrix data3 = new CenterSpace.NMath.Core.DoubleMatrix(markovPCA);
            // Class PrincipalComponentAnalysis performs a principal component
            // analysis on a given data set. The data may optionally be centered and
            // scaled before analysis takes place. By default, variables are centered
            // but not scaled.
            pca = new PrincipalComponentAnalysis(data); //pca = new PrincipalComponentAnalysis(data);
            pca2 = new PrincipalComponentAnalysis(data2); //pca = new PrincipalComponentAnalysis(data);
            pca3 = new PrincipalComponentAnalysis(data3); //pca = new PrincipalComponentAnalysis(data);

            int numberOfComponentsForLocal = pca.Threshold(.99);
            int numberOfComponentsForGabor = pca2.Threshold(.99);
            int numberOfComponentsForMarkov = pca3.Threshold(.99);
            double[,] allFeatures = new double[nodules.Count, numberOfComponentsForGabor + numberOfComponentsForLocal + numberOfComponentsForMarkov];
            //PCAScores2 = pca.Loadings.ToArray();
            //eigenvalues = pca.Eigenvalues.ToArray();
            PCAScores1 = pca.Scores.ToArray();
            PCAScores2 = pca2.Scores.ToArray();
            PCAScores3 = pca3.Scores.ToArray();
            for (int i = 0; i < PCAScores1.GetLength(0); i++)
            {
                for (int k = 0; k < numberOfComponentsForGabor; k++)
                {
                    allFeatures[i, k] = PCAScores2[i, k];
                }
            }
            for (int i = 0; i < PCAScores2.GetLength(0); i++)
            {
                for (int k = 0; k < numberOfComponentsForLocal; k++)
                {
                    allFeatures[i, numberOfComponentsForGabor + k] = PCAScores1[i, k];
                }
            }

            for (int i = 0; i < PCAScores3.GetLength(0); i++)
            {
                for (int k = 0; k < numberOfComponentsForMarkov; k++)
                {
                    allFeatures[i, numberOfComponentsForGabor + numberOfComponentsForLocal + k] = PCAScores3[i, k];
                }
            }
            CenterSpace.NMath.Core.DoubleMatrix allData = new CenterSpace.NMath.Core.DoubleMatrix(allFeatures);
            allPCA = new PrincipalComponentAnalysis(allData);
            MessageBox.Show(allPCA.Threshold(.99).ToString());
            PCAScores = allPCA.Scores.ToArray();
        }*/
        //public double[] eigenvalues = null;

        /// <summary>
        /// Calculates the Euclidean distance between two floating-point vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Floating-point distance between input vectors</returns>
        private static double distEuclidean(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0.0;
            double dist = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                dist += Math.Pow(a[i] - b[i], 2);
            }
            dist = Math.Sqrt(dist);
            return dist;
        }
        private static double distManhattan(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0.0;
            double dist = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                dist += Math.Abs(a[i] - b[i]);
            }
            return dist;
        }
    }
}
