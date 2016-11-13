using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;


namespace BRISC.Core
{
    public class LIDCNoduleDB
    {
        public string DEFAULT_FEATURE = "Gabor";


        public string DEFAULT_SIMILARITY = "Chi-Square (H)";
        public Dictionary<string, double> meanValue = new Dictionary<string, double>();

        public Dictionary<int, string> sortDic = new Dictionary<int, string>();
        public Dictionary<string, double> sValue = new Dictionary<string, double>();
        public Dictionary<string, double> sValueTemp = new Dictionary<string, double>();
        public Dictionary<string, double> tempMeanValue = new Dictionary<string, double>();
        public Dictionary<string, double> tempsVal = new Dictionary<string, double>();

        public LIDCNoduleDB()
        {
            Nodules = new List<LIDCNodule>();
            noduleCount = new Dictionary<string, int>();
            minHaralick = new Dictionary<string, double>();
            maxHaralick = new Dictionary<string, double>();
            LocalminHaralick = new Dictionary<string, double>();
            LocalmaxHaralick = new Dictionary<string, double>();
        }


        public LIDCNoduleDB(string xmlFileName)
            : this()
        {
            LoadFromXML(xmlFileName, null);
        }


        public LIDCNoduleDB(string xmlFileName, ProgressBar pbar)
            : this()
        {
            LoadFromXML(xmlFileName, pbar);
        }


        public List<LIDCNodule> Nodules { get; }


        public int TotalNoduleCount
        {
            get
            {
                return Nodules.Count;
            }
        }


        public int UniqueNodulecount
        {
            get { return noduleCount.Count; }
        }


        public double MeanNoduleWidth { get; private set; } = double.NaN;


        public double MeanNoduleHeight { get; private set; } = double.NaN;


        public Dictionary<string, double> getlocalMinDictionary()
        {
            return LocalminHaralick;
        }


        public Dictionary<string, double> getlocalMaxDictionary()
        {
            return LocalmaxHaralick;
        }


        public void AddNodule(LIDCNodule nodule)
        {
            Nodules.Add(nodule);

            // increment nodule counters
            if (noduleCount.ContainsKey(nodule.Nodule_no))
                noduleCount[nodule.Nodule_no]++;
            else
                noduleCount.Add(nodule.Nodule_no, 1);

            updateHaralickStatistics(nodule);
            LocalUpdateHaralickStatistics(nodule);

            // update mean size statistics
            var sumWidth = 0;
            var sumHeight = 0;
            foreach (var n in Nodules)
            {
                sumWidth += n.Width;
                sumHeight += n.Height;
            }
            MeanNoduleWidth = sumWidth/(double) Nodules.Count;
            MeanNoduleHeight = sumHeight/(double) Nodules.Count;
        }


        private void LocalUpdateHaralickStatistics(LIDCNodule nodule)
        {
            // update min/max Haralick statistics
            foreach (ArrayList firstHara in nodule.LocalHaralick)
            {
                foreach (Dictionary<string, double[,]> nod in firstHara)
                {
                    foreach (var s in nod.Keys)
                    {
                        if (LocalminHaralick.ContainsKey(s))
                        {
                            var tempFeature = nod[s];
                            for (var i = 0; i < tempFeature.GetLength(0); i++)
                            {
                                for (var k = 0; k < tempFeature.GetLength(1); k++)
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
                            var tempFeature = nod[s];
                            var tempMin = double.MaxValue;
                            var tempMax = double.MinValue;
                            for (var i = 0; i < tempFeature.GetLength(0); i++)
                            {
                                for (var k = 0; k < tempFeature.GetLength(1); k++)
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


        private void updateHaralickStatistics(LIDCNodule nodule)
        {
            // update min/max Haralick statistics
            foreach (var s in nodule.Haralick.Keys)
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


        public List<LIDCNodule>.Enumerator GetEnumerator()
        {
            return Nodules.GetEnumerator();
        }


        public void LoadFromXML(string xmlFileName)
        {
            LoadFromXML(xmlFileName, null, true);
        }


        public void LoadFromXML(string xmlFileName, bool addNewNodules)
        {
            LoadFromXML(xmlFileName, null, addNewNodules);
        }


        public void LoadFromXML(string xmlFileName, ProgressBar pbar)
        {
            LoadFromXML(xmlFileName, pbar, true);
        }


        public void LoadFromXML(string xmlFileName, ProgressBar pbar, bool addNewNodules)
        {
            // open XML document
            var doc = new XmlDocument();
            var reader = new XmlTextReader(xmlFileName);
            doc.Load(reader);
            reader.Close();

            // get a list of XML nodes representing all the nodules from the file
            var nodes = doc.SelectNodes("nodules/nodule");

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
                var nod = new LIDCNodule(n);

                foreach (var nn in Nodules)
                {
                    // if an equivalent nodule has already been added,
                    // just merge in the new data
                    if (nn.Equals(nod)) //nn.NUID == nod.NUID)//nn.Equals(nod))
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
                    Application.DoEvents();
                }
            }
        }


        public void SaveToXML(string xmlFileName)
        {
            // create XML file
            //if (xmlFileName != Util.DATA_PATH + "nodules-local.xml")
            //{
            var doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" ?>\n<nodules>\n</nodules>\n");
            var root = doc.DocumentElement;

            // add element for each nodule
            foreach (var n in Nodules)
            {
                root.AppendChild(n.ToXML(doc));
            }

            // save to file
            doc.Save(xmlFileName);
        }


        public int GetNoduleCount(string noduleNumber)
        {
            if (noduleCount.ContainsKey(noduleNumber))
            {
                return noduleCount[noduleNumber];
            }
            return 0;
        }


        public double[] GetFeatureVector(LIDCNodule nodule, string[] features)
        {
            var vector = new double[features.Length];
            for (var i = 0; i < features.Length; i++)
            {
                if (!nodule.Haralick.ContainsKey(features[i]))
                {
                    throw new Exception("Nodule contains no information for feature \"" + features[i] + "\".");
                }
                vector[i] = nodule.Haralick[features[i]];
            }
            return vector;
        }


        public double[] GetNormalizedFeatureVector(LIDCNodule nodule, string[] features)
        {
            var vector = new double[features.Length];
            for (var i = 0; i < features.Length; i++)
            {
                if (!nodule.Haralick.ContainsKey(features[i]))
                {
                    throw new Exception("Nodule contains no information for feature \"" + features[i] + "\".");
                }
                vector[i] = (nodule.Haralick[features[i]] - minHaralick[features[i]])/(maxHaralick[features[i]] - minHaralick[features[i]]);
            }
            return vector;
        }


        public double GetNormalizedFeature(LIDCNodule nodule, string feature)
        {
            if (!nodule.Haralick.ContainsKey(feature))
            {
                throw new Exception("Nodule contains no information for feature \"" + feature + "\".");
            }
            return (nodule.Haralick[feature] - minHaralick[feature])/(maxHaralick[feature] - minHaralick[feature]);
        }


        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, int nItems)
        {
            return RunQuery(queryImage, DEFAULT_FEATURE, DEFAULT_SIMILARITY, nItems);
        }


        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, string feature, string similarity, int nItems)
        {
            return RunQuery(queryImage, feature, similarity, nItems, double.PositiveInfinity);
        }


        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, string feature, string similarity, double threshold)
        {
            return RunQuery(queryImage, feature, similarity, Nodules.Count, threshold);
        }


        public LinkedList<LIDCNodule> RunQuery(LIDCNodule queryImage, string feature, string similarity, int nItems, double threshold)
        {
            var rnodes = new LinkedList<LIDCNodule>();
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
                    var currentFeatureVector = new LinkedList<string>();
                    if (feature.StartsWith("Global") || feature.StartsWith("Local"))
                    {
                        var hfeatures = feature.Split(' ');
                        for (var i = 1; i < hfeatures.Length; i++)
                            if (hfeatures[i] != "")
                                currentFeatureVector.AddLast(hfeatures[i]);
                    }

                    // linked list to hold all nodules, sorted by distance from selected nodule (ascending order)

                    // for each nodule
                    foreach (var temp in Nodules)
                    {
                        // don't add the selected image
                        if (!temp.Equals(queryImage))
                        {
                            // calculate feature distances
                            var ddist = 0.0;
                            if (feature.StartsWith("Local"))
                            {
                                if (queryImage.LocalHaralick == null)
                                    throw new Exception("This nodule does not have Local descriptors.");
                                if (temp.LocalHaralick != null)
                                    ddist = Similarity.CalcLocalHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                                else
                                    ddist = double.NaN;
                            }
                            if (feature.StartsWith("Global"))
                            {
                                if (queryImage.Haralick == null)
                                    throw new Exception("This nodule does not have Global descriptors.");
                                if (temp.Haralick != null)
                                    ddist = Similarity.CalcHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                                else
                                    ddist = double.NaN;
                            }
                            else if (feature.StartsWith("Gabor"))
                            {
                                if (queryImage.GaborHist == null)
                                    throw new Exception("This nodule does not have Gabor descriptors.");
                                if (temp.GaborHist != null)
                                    ddist = Similarity.CalcGaborDistance(queryImage, temp, similarity);
                                else
                                    ddist = double.NaN;
                            }
                            else if (feature.StartsWith("Markov"))
                            {
                                if (queryImage.MarkovHist == null)
                                    throw new Exception("This nodule does not have Markov descriptors.");
                                if (temp.MarkovHist != null)
                                    ddist = Similarity.CalcMarkovDistance(queryImage, temp, similarity);
                                else
                                    ddist = double.NaN;
                            }
                            temp.Temp_dist = ddist;

                            // only add images with real distances
                            if (!double.IsNaN(ddist))
                            {
                                // limit by threshold
                                if (double.IsPositiveInfinity(threshold) || ddist < threshold)
                                {
                                    if (queryImage.Annotations != null && temp.Annotations != null &&
                                        queryImage.Annotations.ContainsKey("malignancy") && temp.Annotations.ContainsKey("malignancy"))
                                    {
                                        temp.Temp_adist =
                                            Math.Abs(queryImage.Annotations["malignancy"] - (double) temp.Annotations["malignancy"]);
                                    }

                                    // insert the new node at the appropriate place in the linked list
                                    var n = rnodes.First;
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
            var cnodes = new LinkedList<LIDCNodule>();

            var ii = 0;
            foreach (var nod in rnodes)
                if (ii++ < nItems)
                    cnodes.AddLast(nod);

            return cnodes;
        }

        public LinkedList<LIDCNodule> AllFeatures(LIDCNodule queryImage, string feature, string similarity, int nItems, double threshold)
        {
            var rnodes = new LinkedList<LIDCNodule>();
            var currentFeatureVector = new LinkedList<string>();
            var hfeatures = feature.Split(' ');
            for (var i = 1; i < hfeatures.Length; i++)
                if (hfeatures[i] != "")
                    currentFeatureVector.AddLast(hfeatures[i]);

            // linked list to hold all nodules, sorted by distance from selected nodule (ascending order)

            // for each nodule
            foreach (var temp in Nodules)
            {
                // don't add the selected image
                if (!temp.Equals(queryImage))
                {
                    // calculate feature distances
                    var ddist = 0.0;
                    if (queryImage.LocalHaralick == null)
                        throw new Exception("This nodule does not have Local descriptors.");
                    similarity = "Jeffrey Diverg. (H)";
                    if (temp.LocalHaralick != null)
                        ddist = ddist + Similarity.CalcLocalHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                    else
                        ddist = double.NaN;
                    if (queryImage.GaborHist == null)
                        throw new Exception("This nodule does not have Gabor descriptors.");
                    similarity = "Chi-Square (H)";
                    if (temp.GaborHist != null)
                        ddist = ddist + Similarity.CalcGaborDistance(queryImage, temp, similarity);
                    else
                        ddist = double.NaN;
                    if (queryImage.MarkovHist == null)
                        throw new Exception("This nodule does not have Markov descriptors.");
                    similarity = "Chi-Square (H)";
                    if (temp.MarkovHist != null)
                        ddist = ddist + Similarity.CalcMarkovDistance(queryImage, temp, similarity);
                    else
                        ddist = double.NaN;
                    if (queryImage.Haralick == null)
                        throw new Exception("This nodule does not have Global descriptors.");
                    similarity = "Euclidean";
                    if (temp.Haralick != null)
                        ddist = ddist + Similarity.CalcHaralickDistance(queryImage, temp, currentFeatureVector, this, similarity);
                    else
                        ddist = double.NaN;
                    temp.Temp_dist = ddist/4;

                    // only add images with real distances
                    if (!double.IsNaN(ddist))
                    {
                        // limit by threshold
                        if (double.IsPositiveInfinity(threshold) || ddist < threshold)
                        {
                            // insert the new node at the appropriate place in the linked list
                            var n = rnodes.First;
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


        public double[] CalcPrecisionAndRecall(LIDCNodule queryImage, LinkedList<LIDCNodule> queryResults)
        {
            // calculate precision and recall
            var totalRetrieved = queryResults.Count;
            var numAppearances = GetNoduleCount(queryImage.Nodule_no) - 1;

            var numRetrieved = 0;
            foreach (var nod in queryResults)
                if (nod.Nodule_no.Equals(queryImage.Nodule_no))
                    numRetrieved++;

            var rp = new double[2];
            rp[0] = numRetrieved/(double) totalRetrieved;
            rp[1] = numRetrieved/(double) numAppearances;
            return rp;
        }


        public double[] CalcMeanPrecisionAndRecall(string feature, string similarity, int nItems, double threshold)
        {
            return CalcMeanPrecisionAndRecall(feature, similarity, nItems, threshold, null);
        }


        public double[] CalcMeanPrecisionUsingAnnotations(string feature, string similarity, int nItems, double threshold, ProgressBar pbar)
        {
            var mpr = new double[2];
            mpr[0] = double.NaN;
            mpr[1] = double.NaN;

            if (pbar != null)
            {
                pbar.Minimum = 0;
                pbar.Maximum = Nodules.Count;
                pbar.Value = 0;
            }

            var i = 0;
            var total = 0;
            AnnotateVariables();

            foreach (var inod in Nodules)
            {
                //sorted 
                AnnotateVariables();
                var rnodes = annoteSort(inod);

                if (pbar != null)
                {
                    pbar.Value = i++;
                    pbar.Refresh();
                    Application.DoEvents();
                }

                // run query
                var cnodes = RunQuery(inod, feature, similarity, Nodules.Count, threshold);

                // calculate precision and recall
                var rp = new double[2];
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
            mpr[0] /= total;
            mpr[1] /= total;
            return mpr;
        }

        public LinkedList<LIDCNodule> annoteSort(LIDCNodule inod)
        {
            var rnodes = new LinkedList<LIDCNodule>();
            foreach (var annotateSort in Nodules)
            {
                annotateSort.annotateDist = normalizedJaccardMCDDistance(inod.Annotations, annotateSort.Annotations, sValue, meanValue);
                if (!double.IsNaN(annotateSort.annotateDist))
                {
                    // limit by threshold
                    if (annotateSort.NUID != inod.NUID && annotateSort.annotateDist < double.PositiveInfinity)
                    {
                        // insert the new node at the appropriate place in the linked list
                        var n = rnodes.First;
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

        public void AnnotateVariables()
        {
            if (sortDic.Count < Nodules.Count)
            {
                //calculate all the dictionaries used in annotation sort.
                foreach (var sort in Nodules)
                {
                    //sortDic has the unique nodule ID as the key and the nodule number as the value.
                    sortDic.Add(sort.NUID, sort.Nodule_no);
                }
                var countMean = 0;
                foreach (var meanIt in Nodules)
                {
                    tempMeanValue = findMean(tempMeanValue, meanIt.Annotations);
                    countMean++;
                }
                foreach (var keyIt in tempMeanValue.Keys)
                {
                    meanValue.Add(keyIt, tempMeanValue[keyIt]/countMean);
                }
                foreach (var a in tempMeanValue.Keys)
                {
                    sValueTemp.Add(a, 0);
                }
                var countsValueTemp = 0;
                foreach (var sIt in Nodules)
                {
                    tempsVal = subtractMeanFromIndividual(sIt.Annotations, meanValue);
                    sValueTemp = addDictionaries(sValueTemp, tempsVal);
                    countsValueTemp++;
                }
                foreach (var key in sValueTemp.Keys)
                {
                    sValue.Add(key, sValueTemp[key]/countsValueTemp);
                }
            }
        }


        public double[] CalcMeanPrecisionAndRecall(string feature, string similarity, int nItems, double threshold, ProgressBar pbar)
        {
            var mpr = new double[2];
            mpr[0] = double.NaN;
            mpr[1] = double.NaN;

            if (pbar != null)
            {
                pbar.Minimum = 0;
                pbar.Maximum = Nodules.Count;
                pbar.Value = 0;
            }

            var i = 0;
            var total = 0;
            foreach (var inod in Nodules)
            {
                if (pbar != null)
                {
                    pbar.Value = i++;
                    pbar.Refresh();
                    Application.DoEvents();
                }

                // run query
                var cnodes = RunQuery(inod, feature, similarity, nItems, threshold);
                // calculate precision and recall
                var rp = CalcPrecisionAndRecall(inod, cnodes);
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
            mpr[0] /= total;
            mpr[1] /= total;
            return mpr;
        }


        public double compareAnnotateToOtherMethod(LinkedList<LIDCNodule> annotate, LinkedList<LIDCNodule> comparedWith, int nItems)
        {
            var numItemsUsed = nItems; //10
            var n = annotate.Count;
            var nCompare = 0;
            var count = 0;
            double annotateCompareTotal = 0;
            var compAnnotate = annotate.First;
            var compAnnotate2 = comparedWith.First;
            var FirstTenSeparateNods = new LinkedList<LIDCNodule>();
            var FirstTenSeparateNods2 = new LinkedList<LIDCNodule>();
            var tenSeparate = 0;
            var uniqueNoduleNo = new Dictionary<string, int>();
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
            for (var q = 0; q < numItemsUsed; q++) //LIDCNodule compAnnotate in annotate)
            {
                compAnnotate2 = FirstTenSeparateNods2.First;
                n = numItemsUsed - q; //annotate.Count - count;
                nCompare = numItemsUsed; //comparedWith.Count;
                var noRepeats = new Dictionary<string, int>();
                while (compAnnotate2 != null && compAnnotate2.Value.Nodule_no != compAnnotate.Value.Nodule_no && nCompare > 0)
                {
                    noRepeats.Add(compAnnotate2.Value.Nodule_no, 0);
                    nCompare--;
                    compAnnotate2 = compAnnotate2.Next;
                }
                annotateCompareTotal = annotateCompareTotal + nCompare*n;
                count++;
                compAnnotate = compAnnotate.Next;
            }
            return annotateCompareTotal;
        }

        public Dictionary<string, double> addDictionaries(Dictionary<string, double> addVal1, Dictionary<string, double> addVal2)
        {
            var summedValue = new Dictionary<string, double>();
            foreach (var key in addVal2.Keys)
            {
                summedValue.Add(key, addVal1[key] + addVal2[key]);
            }
            return summedValue;
        }

        public Dictionary<string, double> subtractMeanFromIndividual(Dictionary<string, int> sIt, Dictionary<string, double> meanValue)
        {
            var temps = new Dictionary<string, double>();
            foreach (var key in sIt.Keys)
            {
                temps.Add(key, Math.Abs(sIt[key] - meanValue[key]));
            }
            return temps;
        }

        public Dictionary<string, double> findMean(Dictionary<string, double> currentMean, Dictionary<string, int> addValues)
        {
            foreach (var key in addValues.Keys)
            {
                if (!currentMean.ContainsKey(key))
                {
                    currentMean.Add(key, addValues[key]);
                }
                else
                {
                    currentMean[key] = currentMean[key] + addValues[key];
                }
            }
            return currentMean;
        }

        public double normalizedJaccardMCDDistance(Dictionary<string, int> compare1, Dictionary<string, int> compare2,
            Dictionary<string, double> sValue, Dictionary<string, double> meanValue)
        {
            double Jdistance = 0;
            var count = 0;
            var tempForEuclidean1 = new double[compare1.Keys.Count - 2];
            var tempForEuclidean2 = new double[compare1.Keys.Count - 2];
            foreach (var annotations in compare1.Keys)
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
                            tempForEuclidean1[count] = (compare1[annotations] - meanValue[annotations])/sValue[annotations];
                            tempForEuclidean2[count] = (compare2[annotations] - meanValue[annotations])/sValue[annotations];
                            count++;
                        }
                    }
                }
            }
            Jdistance = Jdistance + distEuclidean(tempForEuclidean1, tempForEuclidean2);
            Jdistance = Jdistance/compare1.Keys.Count;
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


        private static double distEuclidean(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0.0;
            var dist = 0.0;
            for (var i = 0; i < a.Length; i++)
            {
                dist += Math.Pow(a[i] - b[i], 2);
            }
            dist = Math.Sqrt(dist);
            return dist;
        }

        private static double distManhattan(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0.0;
            var dist = 0.0;
            for (var i = 0; i < a.Length; i++)
            {
                dist += Math.Abs(a[i] - b[i]);
            }
            return dist;
        }

        private readonly Dictionary<string, int> noduleCount;

        // min/max Haralick values (for normalization)
        private readonly Dictionary<string, double> minHaralick;
        private readonly Dictionary<string, double> maxHaralick;

        private readonly Dictionary<string, double> LocalminHaralick;
        private readonly Dictionary<string, double> LocalmaxHaralick;
    }
}