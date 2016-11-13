using System;
using System.Collections;
using System.Collections.Generic;


namespace BRISC.Core
{
    public class LocalCoOccurrence : FeatureExtractor
    {
        public static string[] featureNames = new string[11]
        {
            "contrast", "correlation", "energy", "homogeneity", "entropy",
            "thirdOrderMoment", "inverseVariance", "sumAverage", "variance", "clusterTendency", "maximumProbability"
        };


        private static readonly int numDirs = 4;


        private static readonly int numDists = 4;


        private static readonly int numFeats = featureNames.Length;


        public int windowSize = 5;


        public void ExtractFeatures(LIDCNodule nodule)
        {
            // calculate co-occurrence matrices
            var g = new LocalCoOccurrence();
            var FullData = nodule.SegmentedPixelData;
            //check to see if the window size is too big for the pixel data, if it is make it 
            //the size of the nodule image
            if (FullData.GetLength(0) < windowSize)
            {
                windowSize = FullData.GetLength(0);
            }
            if (FullData.GetLength(1) < windowSize)
            {
                windowSize = FullData.GetLength(1);
            }
            var buff = windowSize;
            for (var row = 0; row <= FullData.GetLength(0) - windowSize; row++)
            {
                var temp = new ArrayList();
                for (var column = 0; column <= FullData.GetLength(1) - windowSize; column++)
                {
                    var data = new int[windowSize, windowSize];
                    for (var t = 0; t < buff; t++)
                    {
                        for (var q = 0; q < buff; q++)
                        {
                            data[q, t] = FullData[row + q, column + t];
                        }
                    }
                    //check to see if the pixel with the features is usable (i.e. not -2000)
                    if (data[(int) Math.Floor((double) windowSize/2), (int) Math.Floor((double) windowSize/2)] != -2000)
                    {
                        var c = g.PerformCoOccurrence(data, -2000, numDists);

                        // calculate [ distance x direction x feature ] matrix
                        var haralick = new double[numDists, numDirs, numFeats];
                        for (var dist = 0; dist < numDists; dist++)
                        {
                            for (var dir = 0; dir < numDirs; dir++)
                            {
                                // get matrix
                                var com = c.getCoOccurrence(dist + 1, dir + 1);

                                // calculate row and column means
                                double imean = 0.0, jmean = 0.0, ivar = 0.0, jvar = 0.0;
                                for (var i = 0; i < com.GetLength(0); i++)
                                {
                                    for (var j = 0; j < com.GetLength(1); j++)
                                    {
                                        imean += i*com[i, j];
                                        jmean += j*com[i, j];
                                    }
                                }

                                // calculate row and column variances
                                for (var i = 0; i < com.GetLength(0); i++)
                                {
                                    for (var j = 0; j < com.GetLength(1); j++)
                                    {
                                        ivar += Math.Pow(i - imean, 2.0)*com[i, j];
                                        jvar += Math.Pow(j - jmean, 2.0)*com[i, j];
                                    }
                                }

                                // calculate features
                                for (var i = 0; i < com.GetLength(0); i++)
                                {
                                    for (var j = 0; j < com.GetLength(1); j++)
                                    {
                                        // contrast
                                        haralick[dist, dir, 0] += Math.Pow(i - j, 2.0)*com[i, j];
                                        if (ivar != 0.0 && jvar != 0.0)
                                        {
                                            // correlation
                                            haralick[dist, dir, 1] += (i - imean)*(j - jmean)*com[i, j]/Math.Sqrt(ivar*jvar);
                                        }
                                        // energy
                                        haralick[dist, dir, 2] += Math.Pow(com[i, j], 2.0);
                                        if (1 + Math.Abs(i - j) != 0.0)
                                        {
                                            // homogeneity
                                            haralick[dist, dir, 3] += com[i, j]/(1.0 + Math.Abs(i - j));
                                        }
                                        if (com[i, j] != 0.0)
                                        {
                                            // entropy
                                            haralick[dist, dir, 4] += -(com[i, j]*Math.Log(com[i, j]));
                                        }
                                        // third order moment
                                        haralick[dist, dir, 5] += com[i, j]*Math.Pow(i - j, 3.0);
                                        if (i - j != 0.0)
                                        {
                                            // inverse variance
                                            haralick[dist, dir, 6] += com[i, j]/Math.Pow(i - j, 2.0);
                                        }
                                        // sum average
                                        haralick[dist, dir, 7] += 0.5*(i*com[i, j] + j*com[i, j]);
                                        // variance
                                        haralick[dist, dir, 8] += 0.5*
                                                                  (com[i, j]*Math.Pow(i - imean, 2.0) + com[i, j]*Math.Pow(j - jmean, 2.0));
                                        // cluster tendency
                                        haralick[dist, dir, 9] += com[i, j]*Math.Pow(i - imean + j - jmean, 2.0);
                                        if (i == 0 && j == 0 || com[i, j] > haralick[dist, dir, 10])
                                        {
                                            // maximum probability
                                            haralick[dist, dir, 10] = com[i, j];
                                        }
                                    }
                                }
                            }
                        }

                        var haralickDic = new Dictionary<string, double[,]>();
                        for (var f = 0; f < numFeats; f++)
                        {
                            var tempharalick = new double[numDists, numDirs];
                            for (var dir = 0; dir < numDirs; dir++)
                            {
                                for (var dist = 0; dist < numDists; dist++)
                                {
                                    tempharalick[dist, dir] = haralick[dist, dir, f];
                                }
                            }
                            if (haralickDic.ContainsKey(featureNames[f]))
                                haralickDic[featureNames[f]] = tempharalick;
                            else
                                haralickDic.Add(featureNames[f], tempharalick);
                        }
                        temp.Add(haralickDic);
                    }
                }
                nodule.LocalHaralick.Add(temp);
            }
        }


        public LocalCombineCoOccurrence PerformCoOccurrence(int[,] mOriginal, int nNoValueIndicate, int numDistance)
        {
            var nDirectionNum = 4; // 1..4

            // get list of co-occurrence row/column values
            var arrHeader = getCoOccurrenceHeader(mOriginal, nNoValueIndicate);
            var mHeader = new int[arrHeader.Count];
            for (var i = 0; i < mHeader.GetLength(0); i++)
                mHeader[i] = (int) arrHeader[i];


            // generate all co-occurrence matrices for one pixel and store them in an ArrayList
            var arrPixCoOccurrence = new ArrayList();
            for (var i = 0; i < numDistance; i++)
            {
                var arrCoOccurrence = new ArrayList();
                for (var j = 0; j < nDirectionNum; j++)
                    arrCoOccurrence.Add(getCoOccurrence(mOriginal, arrHeader, j + 1, i + 1));
                arrPixCoOccurrence.Add(arrCoOccurrence);
            }

            var combineCo = new LocalCombineCoOccurrence(mHeader, mHeader, arrPixCoOccurrence);

            return combineCo;
        }


        private double[,] getCoOccurrence(int[,] mOriginal, ArrayList arrHeader, int direction, int distance)
        {
            //  4   3   2
            //   \  |  /
            //    \ | /
            //      X ----- 1
            var mCoOccurrence = new double[arrHeader.Count, arrHeader.Count];
            int iElement, jElement, iPos, jPos, nTotalPair;

            var rBegin = -1;
            var rIncrement = -1;
            var rEnd = -1;
            var cBegin = -1;
            var cIncrement = -1;
            var cEnd = -1;

            switch (direction)
            {
                case 1:
                    rBegin = 0;
                    cBegin = 0;
                    rEnd = mOriginal.GetLength(0); // row
                    cEnd = mOriginal.GetLength(1) - distance; //column
                    rIncrement = 0;
                    cIncrement = distance;
                    break;
                case 2:
                    rBegin = distance;
                    cBegin = 0;
                    rEnd = mOriginal.GetLength(0); // row
                    cEnd = mOriginal.GetLength(1) - distance; //column
                    rIncrement = -distance;
                    cIncrement = distance;
                    break;
                case 3:
                    rBegin = distance;
                    cBegin = 0;
                    rEnd = mOriginal.GetLength(0); // row
                    cEnd = mOriginal.GetLength(1); //column
                    rIncrement = -distance;
                    cIncrement = 0;
                    break;
                case 4:
                    rBegin = distance;
                    cBegin = distance;
                    rEnd = mOriginal.GetLength(0); // row
                    cEnd = mOriginal.GetLength(1); //column
                    rIncrement = -distance;
                    cIncrement = -distance;
                    break;
            }

            // Initialize Matrix
            for (var r = 0; r < mCoOccurrence.GetLength(0); r++)
                for (var c = 0; c < mCoOccurrence.GetLength(1); c++)
                    mCoOccurrence[r, c] = 0;

            // Check Pair
            nTotalPair = 0;
            for (var r = rBegin; r < rEnd; r++)
            {
                for (var c = cBegin; c < cEnd; c++)
                {
                    iElement = mOriginal[r, c];
                    jElement = mOriginal[r + rIncrement, c + cIncrement];
                    iPos = arrHeader.IndexOf(iElement);
                    jPos = arrHeader.IndexOf(jElement);
                    if ((iPos >= 0) && (jPos >= 0))
                    {
                        mCoOccurrence[iPos, jPos] = mCoOccurrence[iPos, jPos] + 1;
                        nTotalPair++;
                    }
                }
            }


            if (nTotalPair == 0)
            {
                nTotalPair = 1;
            }
            // Normalize CoOccurrence
            for (var r = 0; r < mCoOccurrence.GetLength(0); r++)
                for (var c = 0; c < mCoOccurrence.GetLength(1); c++)
                    mCoOccurrence[r, c] = mCoOccurrence[r, c]/nTotalPair;

            return mCoOccurrence;
        }


        public ArrayList getCoOccurrenceHeader(int[,] mOriginal, int nNoValueIndicate)
        {
            var m = new int[mOriginal.GetLength(0)*mOriginal.GetLength(1)];
            var idx = 0;
            var arrDistinct = new ArrayList();
            var nTemp = nNoValueIndicate;

            // ------------------------
            // Get to 1 Dimension Array
            // For sorting purpose
            for (var i = 0; i < mOriginal.GetLength(0); i++)
            {
                for (var j = 0; j < mOriginal.GetLength(1); j++)
                {
                    m[idx] = mOriginal[i, j];
                    idx++;
                }
            }
            Array.Sort(m);

            // -------------------------------
            // Get Distinct value
            // For Co-Occurrence matrix header
            for (var i = 0; i < m.GetLength(0); i++)
            {
                if (m[i] != nTemp)
                {
                    nTemp = m[i];
                    arrDistinct.Add(nTemp);
                }
            }

            return arrDistinct;
        }
    }


    public class LocalCombineCoOccurrence
    {
        private readonly ArrayList x_AllCoOccurrence;


        public LocalCombineCoOccurrence(int[] iHeader, int[] jHeader, ArrayList arrAllCoOccurrence)
        {
            I = iHeader;
            J = jHeader;
            x_AllCoOccurrence = arrAllCoOccurrence;
        }


        public int[] I { get; }


        public int[] J { get; }


        public ArrayList getCoOccurrence(int distance)
        {
            //  4   3   2
            //   \  |  /
            //    \ | /
            //      X ----- 1
            if ((distance > 0) && (distance <= x_AllCoOccurrence.Count))
                return (ArrayList) x_AllCoOccurrence[distance - 1];
            return null;
        }


        public double[,] getCoOccurrence(int distance, int direction)
        {
            //  4   3   2
            //   \  |  /
            //    \ | /
            //      X ----- 1
            if ((distance > 0) && (distance <= x_AllCoOccurrence.Count))
            {
                var arr = (ArrayList) x_AllCoOccurrence[distance - 1];
                if ((direction > 0) && (direction <= arr.Count))
                    return (double[,]) arr[direction - 1];
                return null;
            }
            return null;
        }
    }
}