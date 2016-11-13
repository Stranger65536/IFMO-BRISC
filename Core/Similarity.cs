using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace BRISC.Core
{
    /// <summary>
    /// Contains static similarity measures for LIDC nodule comparison
    /// </summary>
    public static class Similarity
    {
        /// <summary>
        /// Calculates the similarity of two nodules based on their Gabor descriptors
        /// </summary>
        /// <param name="a">Nodule A</param>
        /// <param name="b">Nodule B</param>
        /// <param name="similarityMeasure">Similarity measure to use</param>
        /// <returns>Similarity</returns>
        public static double CalcGaborDistance(LIDCNodule a, LIDCNodule b, string similarityMeasure)
        {
            double dist = 0.0;
            if (similarityMeasure == "Chi-Square (H)")
            {
                if (a.GaborHist == null || b.GaborHist == null)
                    return double.NaN;

                for (int i = 0; i < a.GaborHist.GetLength(0); i++)
                    for (int j = 0; j < a.GaborHist.GetLength(1); j++)
                        dist += Similarity.distChiSquare(a.GaborHist[i, j], b.GaborHist[i, j]);
            }
            else if (similarityMeasure == "Jeffrey Diverg. (H)")
            {
                if (a.GaborHist == null || b.GaborHist == null)
                    return double.NaN;

                for (int i = 0; i < a.GaborHist.GetLength(0); i++)
                    for (int j = 0; j < a.GaborHist.GetLength(1); j++)
                        dist += Similarity.distJeffrey(a.GaborHist[i, j], b.GaborHist[i, j]);
            }
            else if (similarityMeasure == "Euclidean (M-S)")
            {
                if (a.GaborMean == null || b.GaborMean == null)
                    return double.NaN;

                for (int i = 0; i < a.GaborMean.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GaborMean.GetLength(1); j++)
                    {
                        double[] distanceMSA = { a.GaborMean[i, j], a.GaborStdDev[i, j] };
                        double[] distanceMSB = { b.GaborMean[i, j], b.GaborStdDev[i, j] };
                        dist += Similarity.distEuclidean(distanceMSA, distanceMSB);
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select a distance measure...");
            }
            return dist;
        }
        
        /// <summary>
        /// Calculates the similarity of two nodules based on their Local Haralick descriptors
        /// </summary>
        /// <param name="a">Nodule A</param>
        /// <param name="b">Nodule B</param>
        /// <param name="currentFeatureVector">List of features to extract</param>
        /// <param name="noduleDB">Nodule database (needed to calculate normalized feature vectors)</param>
        /// <param name="similarityMeasure">Similarity measure to use</param>
        /// <returns>Similarity</returns>
        public static double CalcLocalHaralickDistance(LIDCNodule a, LIDCNodule b, LinkedList<string> currentFeatureVector, LIDCNoduleDB noduleDB, string similarityMeasure)
        {
            int numBins = 96;
            ArrayList aHist = a.localHist;
            ArrayList bHist = b.localHist;


            double[,] staticOne = new double[aHist.Count, numBins];
            int featureNumber = 0;
            foreach (double[] work in aHist)
            {
                for (int i = 0; i < work.Length; i++)
                {
                    staticOne[featureNumber, i] = work[i];
                }
                featureNumber++;
            }
            double[] dist = new double[aHist.Count];
            featureNumber = 0;
            for (int i = 0; i < featureNumber; i++)
            {
                dist[i] = 0;
            }
            foreach (double[] work2 in bHist)
            {
                double[] temp = new double[work2.Length];
                for (int j = 0; j < work2.Length; j++)
                {
                    temp[j] = staticOne[featureNumber, j];
                }
                if (similarityMeasure == "Chi-Square (H)")
                {
                    dist[featureNumber] = dist[featureNumber] + Similarity.distJeffrey(temp, work2);
                }
                else if (similarityMeasure == "Jeffrey Diverg. (H)")
                {
                    dist[featureNumber] = dist[featureNumber] + Similarity.distJeffrey(temp, work2);
                }
                featureNumber++;
            }
            double averageDistance = 0;
            for (int m = 0; m < dist.Length; m++)
            {
                averageDistance = averageDistance + dist[m];
            }
            averageDistance = averageDistance / dist.Length;
            return averageDistance;
        }
        /// <summary>
        /// Calculates the similarity of two nodules based on their Global Haralick descriptors
        /// </summary>
        /// <param name="a">Nodule A</param>
        /// <param name="b">Nodule B</param>
        /// <param name="currentFeatureVector">List of features to extract</param>
        /// <param name="noduleDB">Nodule database (needed to calculate normalized feature vectors)</param>
        /// <param name="similarityMeasure">Similarity measure to use</param>
        /// <returns>Similarity</returns>
        public static double CalcHaralickDistance(LIDCNodule a, LIDCNodule b, LinkedList<string> currentFeatureVector, LIDCNoduleDB noduleDB, string similarityMeasure)
        {
            double dist = 0.0;
            string[] features = new string[currentFeatureVector.Count];
            LinkedListNode<string> node = currentFeatureVector.First;
            int i = 0;
            while (node != null)
            {
                features[i++] = node.Value;
                node = node.Next;
            }
            double[] v1 = noduleDB.GetNormalizedFeatureVector(a, features);
            double[] v2 = noduleDB.GetNormalizedFeatureVector(b, features);
            if (similarityMeasure == "Euclidean")
            {
                dist = Similarity.distEuclidean(v1, v2);
            }
            else if (similarityMeasure == "Manhattan")
            {
                dist = Similarity.distManhattan(v1, v2);
            }
            else if (similarityMeasure == "Chebychev")
            {
                dist = Similarity.distChebychev(v1, v2);
            }
            return dist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="larger"></param>
        /// <param name="smaller"></param>
        /// <param name="wRatio"></param>
        /// <param name="hRatio"></param>
        /// <returns></returns>
        public static double[] normalizeFeatureVectors(double[] larger, double[] smaller, double wRatio, double hRatio)
        {
            double[] test = new double[21];
            return test;
        }


        /// <summary>
        /// Calculates the similarity of two nodules based on their Markov descriptors
        /// </summary>
        /// <param name="a">Nodule A</param>
        /// <param name="b">Nodule B</param>
        /// <param name="similarityMeasure">Which similarity to use</param>
        /// <returns>Similarity</returns>
        public static double CalcMarkovDistance(LIDCNodule a, LIDCNodule b, string similarityMeasure)
        {
            double dist = 0.0;
            if (similarityMeasure == "Chi-Square (H)")
            {
                if (a.MarkovHist == null || b.MarkovHist == null)
                    return double.NaN;

                for (int i = 0; i < a.MarkovHist.GetLength(0); i++)
                {
                    dist += Similarity.distChiSquare(a.MarkovHist[i], b.MarkovHist[i]);
                }
            }
            else if (similarityMeasure == "Jeffrey Diverg. (H)")
            {
                if (a.MarkovHist == null || b.MarkovHist == null)
                    return double.NaN;

                for (int i = 0; i < a.MarkovHist.GetLength(0); i++)
                {
                    dist += Similarity.distJeffrey(a.MarkovHist[i], b.MarkovHist[i]);
                }
            }
            else if (similarityMeasure == "Euclidean (M-S)")
            {
                if (a.MarkovMean == null || b.MarkovMean == null)
                    return double.NaN;

                for (int i = 0; i < a.MarkovMean.GetLength(0); i++)
                {
                    double[] distanceMSA = { a.MarkovMean[i], a.MarkovStd[i] };
                    double[] distanceMSB = { b.MarkovMean[i], b.MarkovStd[i] };

                    dist += Similarity.distEuclidean(distanceMSA, distanceMSB);
                }
            }
            return dist;
        }
        #region Histogram-based comparisons
        /// <summary>
        /// Finds the mean histogram of two histograms
        /// </summary>
        /// <param name="a">First histogram</param>
        /// <param name="b">Second histogram</param>
        /// <returns>Mean histogram</returns>
        private static double[] meanHistogram(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                return null;

            double[] mean = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                mean[i] = (a[i] + b[i]) / 2;
            }
            return mean;
        }
        /// <summary>
        /// Finds the Chi-square similarity between two histograms
        /// </summary>
        /// <param name="a">First histogram</param>
        /// <param name="b">Second histogram</param>
        /// <returns>Similarity measure</returns>
        public static double distChiSquare(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                return -1;

            double dist = 0.0;
            double[] mean = meanHistogram(a, b);
            for (int i = 0; i < a.Length; i++)
            {
                if (mean[i] == 0)  // don't want to divide by zero
                    continue;

                dist += System.Math.Pow((a[i] - mean[i]), 2) / mean[i];
            }
            return dist;
        }
        /// <summary>
        /// Finds the Jeffrey divergence between two histograms
        /// </summary>
        /// <param name="a">First histogram</param>
        /// <param name="b">Second histogram</param>
        /// <returns>Similarity measure</returns>
        public static double distJeffrey(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                return -1;

            double dist = 0.0;
            double[] mean = meanHistogram(a, b);     // find the mean histogram of a and b
            for (int i = 0; i < a.Length; i++)
            {
                if (mean[i] == 0 || a[i] == 0 || b[i] == 0)   //dont' want to devide by zero
                    continue;

                dist += (a[i] * System.Math.Log(a[i] / mean[i])) + (b[i] * System.Math.Log(b[i] / mean[i]));
            }
            return dist;
        }
        #endregion

        #region Point-based comparisons
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
        /// <summary>
        /// Calculates the Manhattan distance between two floating-point vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Floating-point distance between input vectors</returns>
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
        /// <summary>
        /// Calculates the Chebychev distance between two floating-point vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Floating-point distance between input vectors</returns>
        private static double distChebychev(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0.0;
            double dist = Math.Abs(a[0] - b[0]);
            for (int i = 1; i < a.Length; i++)
            {
                dist = Math.Max(dist, Math.Abs(a[i] - b[i]));
            }
            return dist;
        }
        #endregion
    }
}
