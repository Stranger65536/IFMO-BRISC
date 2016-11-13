using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;


namespace BRISC.Core
{
    public static class Similarity
    {
        public static double CalcGaborDistance(LIDCNodule a, LIDCNodule b, string similarityMeasure)
        {
            var dist = 0.0;
            if (similarityMeasure == "Chi-Square (H)")
            {
                if (a.GaborHist == null || b.GaborHist == null)
                    return double.NaN;

                for (var i = 0; i < a.GaborHist.GetLength(0); i++)
                    for (var j = 0; j < a.GaborHist.GetLength(1); j++)
                        dist += distChiSquare(a.GaborHist[i, j], b.GaborHist[i, j]);
            }
            else if (similarityMeasure == "Jeffrey Diverg. (H)")
            {
                if (a.GaborHist == null || b.GaborHist == null)
                    return double.NaN;

                for (var i = 0; i < a.GaborHist.GetLength(0); i++)
                    for (var j = 0; j < a.GaborHist.GetLength(1); j++)
                        dist += distJeffrey(a.GaborHist[i, j], b.GaborHist[i, j]);
            }
            else if (similarityMeasure == "Euclidean (M-S)")
            {
                if (a.GaborMean == null || b.GaborMean == null)
                    return double.NaN;

                for (var i = 0; i < a.GaborMean.GetLength(0); i++)
                {
                    for (var j = 0; j < a.GaborMean.GetLength(1); j++)
                    {
                        double[] distanceMSA = {a.GaborMean[i, j], a.GaborStdDev[i, j]};
                        double[] distanceMSB = {b.GaborMean[i, j], b.GaborStdDev[i, j]};
                        dist += distEuclidean(distanceMSA, distanceMSB);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a distance measure...");
            }
            return dist;
        }

        public static double CalcLocalHaralickDistance(LIDCNodule a, LIDCNodule b, LinkedList<string> currentFeatureVector,
            LIDCNoduleDB noduleDB, string similarityMeasure)
        {
            var numBins = 96;
            var aHist = a.localHist;
            var bHist = b.localHist;


            var staticOne = new double[aHist.Count, numBins];
            var featureNumber = 0;
            foreach (double[] work in aHist)
            {
                for (var i = 0; i < work.Length; i++)
                {
                    staticOne[featureNumber, i] = work[i];
                }
                featureNumber++;
            }
            var dist = new double[aHist.Count];
            featureNumber = 0;
            for (var i = 0; i < featureNumber; i++)
            {
                dist[i] = 0;
            }
            foreach (double[] work2 in bHist)
            {
                var temp = new double[work2.Length];
                for (var j = 0; j < work2.Length; j++)
                {
                    temp[j] = staticOne[featureNumber, j];
                }
                if (similarityMeasure == "Chi-Square (H)")
                {
                    dist[featureNumber] = dist[featureNumber] + distJeffrey(temp, work2);
                }
                else if (similarityMeasure == "Jeffrey Diverg. (H)")
                {
                    dist[featureNumber] = dist[featureNumber] + distJeffrey(temp, work2);
                }
                featureNumber++;
            }
            double averageDistance = 0;
            for (var m = 0; m < dist.Length; m++)
            {
                averageDistance = averageDistance + dist[m];
            }
            averageDistance = averageDistance/dist.Length;
            return averageDistance;
        }


        public static double CalcHaralickDistance(LIDCNodule a, LIDCNodule b, LinkedList<string> currentFeatureVector, LIDCNoduleDB noduleDB,
            string similarityMeasure)
        {
            var dist = 0.0;
            var features = new string[currentFeatureVector.Count];
            var node = currentFeatureVector.First;
            var i = 0;
            while (node != null)
            {
                features[i++] = node.Value;
                node = node.Next;
            }
            var v1 = noduleDB.GetNormalizedFeatureVector(a, features);
            var v2 = noduleDB.GetNormalizedFeatureVector(b, features);
            if (similarityMeasure == "Euclidean")
            {
                dist = distEuclidean(v1, v2);
            }
            else if (similarityMeasure == "Manhattan")
            {
                dist = distManhattan(v1, v2);
            }
            else if (similarityMeasure == "Chebychev")
            {
                dist = distChebychev(v1, v2);
            }
            return dist;
        }


        public static double[] normalizeFeatureVectors(double[] larger, double[] smaller, double wRatio, double hRatio)
        {
            var test = new double[21];
            return test;
        }


        public static double CalcMarkovDistance(LIDCNodule a, LIDCNodule b, string similarityMeasure)
        {
            var dist = 0.0;
            if (similarityMeasure == "Chi-Square (H)")
            {
                if (a.MarkovHist == null || b.MarkovHist == null)
                    return double.NaN;

                for (var i = 0; i < a.MarkovHist.GetLength(0); i++)
                {
                    dist += distChiSquare(a.MarkovHist[i], b.MarkovHist[i]);
                }
            }
            else if (similarityMeasure == "Jeffrey Diverg. (H)")
            {
                if (a.MarkovHist == null || b.MarkovHist == null)
                    return double.NaN;

                for (var i = 0; i < a.MarkovHist.GetLength(0); i++)
                {
                    dist += distJeffrey(a.MarkovHist[i], b.MarkovHist[i]);
                }
            }
            else if (similarityMeasure == "Euclidean (M-S)")
            {
                if (a.MarkovMean == null || b.MarkovMean == null)
                    return double.NaN;

                for (var i = 0; i < a.MarkovMean.GetLength(0); i++)
                {
                    double[] distanceMSA = {a.MarkovMean[i], a.MarkovStd[i]};
                    double[] distanceMSB = {b.MarkovMean[i], b.MarkovStd[i]};

                    dist += distEuclidean(distanceMSA, distanceMSB);
                }
            }
            return dist;
        }

        private static double[] meanHistogram(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                return null;

            var mean = new double[a.Length];

            for (var i = 0; i < a.Length; i++)
            {
                mean[i] = (a[i] + b[i])/2;
            }
            return mean;
        }


        public static double distChiSquare(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                return -1;

            var dist = 0.0;
            var mean = meanHistogram(a, b);
            for (var i = 0; i < a.Length; i++)
            {
                if (mean[i] == 0) // don't want to divide by zero
                    continue;

                dist += Math.Pow(a[i] - mean[i], 2)/mean[i];
            }
            return dist;
        }


        public static double distJeffrey(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                return -1;

            var dist = 0.0;
            var mean = meanHistogram(a, b); // find the mean histogram of a and b
            for (var i = 0; i < a.Length; i++)
            {
                if (mean[i] == 0 || a[i] == 0 || b[i] == 0) //dont' want to devide by zero
                    continue;

                dist += a[i]*Math.Log(a[i]/mean[i]) + b[i]*Math.Log(b[i]/mean[i]);
            }
            return dist;
        }

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


        private static double distChebychev(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0.0;
            var dist = Math.Abs(a[0] - b[0]);
            for (var i = 1; i < a.Length; i++)
            {
                dist = Math.Max(dist, Math.Abs(a[i] - b[i]));
            }
            return dist;
        }
    }
}