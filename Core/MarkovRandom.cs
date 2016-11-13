using System;
using System.Collections.Generic;
using System.Text;
using BRISC.Matrix;

namespace BRISC.Core
{
    /// <summary>
    /// Performs Markov random field operations on a LIDCNodule.
    /// </summary>
    /// <remarks>
    /// <div align="center">
    /// <img src="markov.png" />
    /// </div>
    /// </remarks>
    class MarkovRandom : FeatureExtractor
    {
        /// <summary>
        /// Minimum value for histogam binning. All values below this will be binned into the bottom bin.
        /// </summary>
        public static double histMinVal = 0;
        /// <summary>
        /// Maximum value for histogram binning. All values above this will be binned into the top bin.
        /// </summary>
        public static double histMaxVal = 0;
        /// <summary>
        /// Number of bins in the response histograms
        /// </summary>
        public static int binSize = 512;
        /// <summary>
        /// Padding needed around the image
        /// </summary>
        private static int pad = 4;
        /// <summary>
        /// Performs Markov analysis on the given nodule and saves the 
        /// results to the appropriate LIDCNodule fields.
        /// </summary>
        /// <param name="nodule">LIDCNodule to run Markov on</param>
        public void ExtractFeatures(LIDCNodule nodule)
        {
            int pad = 4;
            
            int mean = 63369;
            int std = 109236;
            MarkovRandom.histMinVal = 0;
            MarkovRandom.histMaxVal = mean+(3*std);

            // grab the original lung image
            int[,] originalData = nodule.OriginalPixelData;

            // segment out the nodule with padding
            int[,] segData = Util.SegmentNodule(originalData, nodule, pad, pad);
            
            double[][,] markovParams = FindParameters(segData);
            
            // allocate space to save histogram and mean/std in the nodule
            nodule.MarkovHist   = new double[5][];
            nodule.MarkovMean   = new double[5];
            nodule.MarkovStd    = new double[5];
            // compute the historam and mean/std for each parameter
            for (int i = 0; i < 5; i++)
            {
                nodule.MarkovHist[i] = Util.ComputeHistogram(markovParams[i], binSize, MarkovRandom.histMinVal, MarkovRandom.histMaxVal); 
                nodule.MarkovMean[i] = Util.ComputeMean(markovParams[i]);
                nodule.MarkovStd[i] = Util.ComputeStdDev(markovParams[i]);
            }
        }

        /// <summary>
        /// Normalizes the image with zero mean
        /// </summary>
        /// <param name="segData">Image to normalize</param>
        /// <remarks>Results are worse if we normalize. Not sure why this is, reguardless we are not using this function.
        /// It's here just in case.</remarks>
        private static void normalize(ref int[,] segData)
        {
            int mean = 0;
            int count = 0;
            foreach (int val in segData)
            {
                mean += val;
                count++;
            }
            mean /= count;

            for (int y = 0; y < segData.GetLength(0); y++)
            {
                for (int x = 0; x < segData.GetLength(1); x++)
                {
                    segData[y, x] -= mean;
                }
            }

        }
        /// <summary>
        /// Finds the MRF parameters
        /// </summary>
        /// <param name="segData">Image to run calculations on</param>
        /// <returns>Five MRF parameter matrices</returns>
        public static double[][,] FindParameters(int[,] segData)
        {
            // matrix to store the markov parameters
            double[][,] markovParams = new double[5][,];

            // allocate space for beta value
            for (int i = 0; i < 5; i++)
            {
                markovParams[i] = new double[segData.GetLength(0) - 2 * pad, segData.GetLength(1) - 2 * pad];
            }

            double sum_var = 0;
            // for every pixel in the image -- less the padding -- find the parameters
            for (int y = pad; y < segData.GetLength(0) - pad; y++)
            {
                for (int x = pad; x < segData.GetLength(1) - pad; x++)
                {
                    GeneralMatrix sum_corr = new GeneralMatrix(4, 4);
                    GeneralMatrix sum_vector = new GeneralMatrix(4, 1);
                    GeneralMatrix parameters = new GeneralMatrix(4, 1);

                    calcSumCorrVect(segData, x, y, ref sum_corr, ref sum_vector);

                    //find the parameters for the given pixel
                    parameters = (sum_corr.Inverse()).Multiply(sum_vector);


                    sum_var = calcSumVar(segData, x, y, ref parameters);
                    double variance = sum_var / 81;

                    // fill up the markov feature matrix with the beta parameters
                    for (int i = 0; i < 4; i++)
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

        /// <summary>
        /// Calculates new features from the beta parameters
        /// </summary>
        /// <param name="segData">Original image</param>
        /// <param name="x">Current column index</param>
        /// <param name="y">Current row index</param>
        /// <param name="pad">Padding around the image</param>
        /// <param name="markovParams">Passed in as beta values. Returned as new features</param>
        private static void deriveNewFeatures(int[,] segData, int x, int y, int pad, ref double[][,] markovParams)
        {           
            // temporary new feature vector
            double[] sum_nf = new double[4];    
            
            // for every value in the estimation window
            for (int row = y - 3; row <= y + 3; row++)
            {
                for (int col = x - 3; col <= x + 3; col++)
                {
                    double[] Q = { 
                            segData[row,col+1] + segData[row,col-1], 
                            segData[row+1,col] + segData[row-1,col],
                            segData[row+1,col-1] + segData[row-1,col+1],
                            segData[row+1,col+1] + segData[row-1,col-1]
                    };
                    // sum up the new feature
                    for (int pars = 0; pars < 4; pars++)
                    {
                        sum_nf[pars] += System.Math.Pow((segData[row, col] - markovParams[pars][y-pad, x-pad] * Q[pars]), 2);
                    }
                }
            }
            // store the feature back into the markovParams matrix
            for (int i = 0; i < sum_nf.GetLength(0); i++)
            {
                markovParams[i][y-pad,x-pad] = sum_nf[i] / 81;
            }            
        }
        /// <summary>
        /// Function calculates the sum of the correlation and vectors
        /// </summary>
        /// <param name="segData">Original image</param>
        /// <param name="x">Current column index</param>
        /// <param name="y">Current row index</param>
        /// <param name="sum_corr">Matrix to store the sum of the correlation</param>
        /// <param name="sum_vector">Matrix to store the sum of the vectors</param>
        private static void calcSumCorrVect(int[,] segData, int x, int y, ref GeneralMatrix sum_corr, ref GeneralMatrix sum_vector)
        {
            for (int row = y - 3; row <= y + 3; row++)
            {
                for (int col = x - 3; col <= x + 3; col++)
                {
                    double[][] Q = new double[][]
                    { 
                        new double[] {segData[row,col+1] + segData[row,col-1]}, 
                        new double[] {segData[row+1,col]+ segData[row-1,col]}, 
                        new double[] {segData[row+1,col-1] + segData[row-1,col+1]}, 
                        new double[] {segData[row+1,col+1] + segData[row-1,col-1]} 
                    };
                    GeneralMatrix QMatrix = new GeneralMatrix(Q);
                    
                    sum_corr += (QMatrix.Multiply(QMatrix.Transpose()));
                    sum_vector += (QMatrix.Multiply(segData[row, col]));                  
                }
            }
        }
        /// <summary>
        /// Sums up the variances and the first part of the beta parameter
        /// </summary>
        /// <param name="segData">Original image</param>
        /// <param name="x">Current column index</param>
        /// <param name="y">Current row index</param>
        /// <param name="parameters">Matrix to store beta parameters</param>
        /// <returns>Sum of the Variance</returns>
        private static double calcSumVar(int[,] segData, int x, int y, ref GeneralMatrix parameters)
        {
            double sum_var = 0;
            for (int row = y - 3; row <= y + 3; row++)
            {
                for (int col = x - 3; col <= x + 3; col++)
                {
                    double[][] Q = new double[][]
                    { 
                        new double[] {segData[row,col+1] + segData[row,col-1]}, 
                        new double[] {segData[row+1,col]+ segData[row-1,col]}, 
                        new double[] {segData[row+1,col-1] + segData[row-1,col+1]}, 
                        new double[] {segData[row+1,col+1] + segData[row-1,col-1]} 
                    };
                    GeneralMatrix QMatrix = new GeneralMatrix(Q);
                    GeneralMatrix I = new GeneralMatrix(1,1);
                    I.SetElement(0, 0, (double)segData[row, col]);
                    sum_var += System.Math.Pow((I.Subtract((parameters.Transpose()).Multiply(QMatrix))).GetElement(0,0),2);
                }
            }
            return sum_var;
        }

      
    }
}
