using System;
using BRISC.Matrix;


namespace BRISC.Core
{
    internal class MarkovRandom : FeatureExtractor
    {
        public static double histMinVal;


        public static double histMaxVal;


        public static int binSize = 512;


        private static readonly int pad = 4;


        public void ExtractFeatures(LIDCNodule nodule)
        {
            var pad = 4;

            var mean = 63369;
            var std = 109236;
            histMinVal = 0;
            histMaxVal = mean + 3*std;

            // grab the original lung image
            var originalData = nodule.OriginalPixelData;

            // segment out the nodule with padding
            var segData = Util.SegmentNodule(originalData, nodule, pad, pad);

            var markovParams = FindParameters(segData);

            // allocate space to save histogram and mean/std in the nodule
            nodule.MarkovHist = new double[5][];
            nodule.MarkovMean = new double[5];
            nodule.MarkovStd = new double[5];
            // compute the historam and mean/std for each parameter
            for (var i = 0; i < 5; i++)
            {
                nodule.MarkovHist[i] = Util.ComputeHistogram(markovParams[i], binSize, histMinVal, histMaxVal);
                nodule.MarkovMean[i] = Util.ComputeMean(markovParams[i]);
                nodule.MarkovStd[i] = Util.ComputeStdDev(markovParams[i]);
            }
        }


        private static void normalize(ref int[,] segData)
        {
            var mean = 0;
            var count = 0;
            foreach (var val in segData)
            {
                mean += val;
                count++;
            }
            mean /= count;

            for (var y = 0; y < segData.GetLength(0); y++)
            {
                for (var x = 0; x < segData.GetLength(1); x++)
                {
                    segData[y, x] -= mean;
                }
            }
        }


        public static double[][,] FindParameters(int[,] segData)
        {
            // matrix to store the markov parameters
            var markovParams = new double[5][,];

            // allocate space for beta value
            for (var i = 0; i < 5; i++)
            {
                markovParams[i] = new double[segData.GetLength(0) - 2*pad, segData.GetLength(1) - 2*pad];
            }

            double sum_var = 0;
            // for every pixel in the image -- less the padding -- find the parameters
            for (var y = pad; y < segData.GetLength(0) - pad; y++)
            {
                for (var x = pad; x < segData.GetLength(1) - pad; x++)
                {
                    var sum_corr = new GeneralMatrix(4, 4);
                    var sum_vector = new GeneralMatrix(4, 1);
                    var parameters = new GeneralMatrix(4, 1);

                    calcSumCorrVect(segData, x, y, ref sum_corr, ref sum_vector);

                    //find the parameters for the given pixel
                    parameters = sum_corr.Inverse().Multiply(sum_vector);


                    sum_var = calcSumVar(segData, x, y, ref parameters);
                    var variance = sum_var/81;

                    // fill up the markov feature matrix with the beta parameters
                    for (var i = 0; i < 4; i++)
                    {
                        markovParams[i][y - pad, x - pad] = parameters.GetElement(i, 0);
                    }
                    markovParams[4][y - pad, x - pad] = variance;

                    // find the new features from the beta values
                    deriveNewFeatures(segData, x, y, pad, ref markovParams);
                }
            }
            return markovParams;
        }


        private static void deriveNewFeatures(int[,] segData, int x, int y, int pad, ref double[][,] markovParams)
        {
            // temporary new feature vector
            var sum_nf = new double[4];

            // for every value in the estimation window
            for (var row = y - 3; row <= y + 3; row++)
            {
                for (var col = x - 3; col <= x + 3; col++)
                {
                    double[] Q =
                    {
                        segData[row, col + 1] + segData[row, col - 1],
                        segData[row + 1, col] + segData[row - 1, col],
                        segData[row + 1, col - 1] + segData[row - 1, col + 1],
                        segData[row + 1, col + 1] + segData[row - 1, col - 1]
                    };
                    // sum up the new feature
                    for (var pars = 0; pars < 4; pars++)
                    {
                        sum_nf[pars] += Math.Pow(segData[row, col] - markovParams[pars][y - pad, x - pad]*Q[pars], 2);
                    }
                }
            }
            // store the feature back into the markovParams matrix
            for (var i = 0; i < sum_nf.GetLength(0); i++)
            {
                markovParams[i][y - pad, x - pad] = sum_nf[i]/81;
            }
        }


        private static void calcSumCorrVect(int[,] segData, int x, int y, ref GeneralMatrix sum_corr, ref GeneralMatrix sum_vector)
        {
            for (var row = y - 3; row <= y + 3; row++)
            {
                for (var col = x - 3; col <= x + 3; col++)
                {
                    double[][] Q =
                    {
                        new double[] {segData[row, col + 1] + segData[row, col - 1]},
                        new double[] {segData[row + 1, col] + segData[row - 1, col]},
                        new double[] {segData[row + 1, col - 1] + segData[row - 1, col + 1]},
                        new double[] {segData[row + 1, col + 1] + segData[row - 1, col - 1]}
                    };
                    var QMatrix = new GeneralMatrix(Q);

                    sum_corr += QMatrix.Multiply(QMatrix.Transpose());
                    sum_vector += QMatrix.Multiply(segData[row, col]);
                }
            }
        }


        private static double calcSumVar(int[,] segData, int x, int y, ref GeneralMatrix parameters)
        {
            double sum_var = 0;
            for (var row = y - 3; row <= y + 3; row++)
            {
                for (var col = x - 3; col <= x + 3; col++)
                {
                    double[][] Q =
                    {
                        new double[] {segData[row, col + 1] + segData[row, col - 1]},
                        new double[] {segData[row + 1, col] + segData[row - 1, col]},
                        new double[] {segData[row + 1, col - 1] + segData[row - 1, col + 1]},
                        new double[] {segData[row + 1, col + 1] + segData[row - 1, col - 1]}
                    };
                    var QMatrix = new GeneralMatrix(Q);
                    var I = new GeneralMatrix(1, 1);
                    I.SetElement(0, 0, segData[row, col]);
                    sum_var += Math.Pow(I.Subtract(parameters.Transpose().Multiply(QMatrix)).GetElement(0, 0), 2);
                }
            }
            return sum_var;
        }
    }
}