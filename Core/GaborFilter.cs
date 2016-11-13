using System;
using System.Collections.Generic;
using System.Text;

namespace BRISC.Core
{
    /// <summary>
    /// Performs Gabor filtering operations on a LIDCNodule. 
    /// </summary>
    /// <remarks>Gabor filtering is an operation in which an NxN matrix (the Gabor filter)
    /// is applied (convolved) to a given image. The filter is defined in BuildFilter. The
    /// filter has two parameters which vary -- orientation (angle) and scale (frequency).
    /// <div align="center">
    /// <img src="gabor.png" />
    /// </div>
    /// </remarks>
    public class GaborFilter : FeatureExtractor
    {
        /// <summary>
        /// Number of bins in the response histograms
        /// </summary>
        public static int histBins = 8;
        /// <summary>
        /// Maximum value for the response histogram. Everything above this will be binned into the top bin
        /// <remarks>Currently set to about three standard deviations above the mean for our dataset.</remarks>
        /// </summary>
        public static double histMaxVal = 600;
        /// <summary>
        /// Minimum value for the response histogram. Everything below this value will be binned into the bottom bin
        /// </summary>
        public static double histMinVal = 0;
        
        /// <summary>
        /// Performs a Gabor filter on the given nodule and saves the histogram of the response images (for each orientation and
        /// frequency) to the appropriate LIDCNodule fields.
        /// </summary>
        /// <param name="nodule">LIDCNodule to run the Gabor filter on</param>
        public void ExtractFeatures(LIDCNodule nodule)
        {

            // grab the original lung image
            int[,] originalData = nodule.OriginalPixelData;

            // segment out the nodule with padding
            int[,] segData = Util.SegmentNodule(originalData, nodule, 4, 4);

            // allocate memory
            nodule.GaborHist = new double[4, 3][];
            nodule.GaborMean = new double[4, 3];
            nodule.GaborStdDev = new double[4, 3];
            
            double[][,] gaborResponse = ApplyFilter(segData);   // apply filter to get back 12 response images   
            
            int responseIndex = 0;
            int orientationIndex = 0;
            // for all orientations
            for (double theta = 0; theta <= (3 * pi() / 4); theta += (pi() / 4))    
            {
                int scaleIndex = 0;
                // and all frequencies
                for (double frequency = .3; frequency <= .5; frequency += .1)           
                {                     
                    // compute the histogram/mean/std and store it in the LIDCNodule
                    nodule.GaborHist[orientationIndex, scaleIndex] = Util.ComputeHistogram(gaborResponse[responseIndex], GaborFilter.histBins, GaborFilter.histMinVal, GaborFilter.histMaxVal);    
                    nodule.GaborMean[orientationIndex, scaleIndex] = Util.ComputeMean(gaborResponse[responseIndex]);
                    nodule.GaborStdDev[orientationIndex, scaleIndex] = Util.ComputeStdDev(gaborResponse[responseIndex]);
                    scaleIndex++;
                    responseIndex++;
                }
                orientationIndex++;
            }
      

        }

        #region features
        /// <summary>
        /// Applies a bank of Gabor filters to the given image
        /// </summary>
        /// <param name="I">Input image</param>
        /// <returns>Twelve Gabor response images</returns>
        public static double[][,] ApplyFilter(int[,] I)
        {
            double[][,] Iout = new double[12][,];
            int responseIndex = 0;

            // for all orientations
            for (double theta = 0; theta <= (3 * pi() / 4); theta += (pi() / 4))    
            {
                // and all frequencies
                for (double frequency = .3; frequency <= .5; frequency += .1)
                {
                    // size of filter
                    int Sx = 4;
                    int Sy = 4;
                    // filter parameters
                    double gamma = .5;
                    double lamda = 1 / frequency;
                    double sigma = .56 * lamda;

                    // set up gabor filter
                    double[,] G = buildFilter(Sx, Sy, sigma, gamma, lamda, theta);
                    // convolve image with gabor filter
                    Iout[responseIndex] = convolve(I, G);

                    // get the absolute value for all values in the response
                    for (int y = 0; y < Iout[responseIndex].GetLength(0); y++)
                        for (int x = 0; x < Iout[responseIndex].GetLength(1); x++)
                            Iout[responseIndex][y, x] = abs(Iout[responseIndex][y, x]);
                    
                    responseIndex++;
                }
            }
            return Iout;
        }

        /// <summary>
        /// Builds a Gabor filter
        /// </summary>
        /// <param name="Sx">width</param>
        /// <param name="Sy">height</param>
        /// <param name="sigma">Width of Gaussian</param>
        /// <param name="gamma">Spatial aspect ratio. Should be .5</param>
        /// <param name="lamda">Wavelength</param>
        /// <param name="theta">Orientaiton of filter</param>
        /// <returns>Gabor filter in a 2D double matrix</returns>
        private static double[,] buildFilter(int Sx, int Sy, double sigma, double gamma, double lamda, double theta)
        {
            double[,] G = new double[2 * Sx + 1, 2 * Sy + 1];    // Filter matrix

            for (int x = -Sx; x <= Sx; x++)
                for (int y = -Sy; y <= Sy; y++)
                {

                    double x_theta = x * cos(theta) + y * sin(theta);
                    double y_theta = -x * sin(theta) + y * cos(theta);

                    double gaussian = exp((-.5) * (pow(x_theta, 2) + pow(gamma, 2) * pow(y_theta, 2)) / (pow(sigma, 2)));
                    double harmonic = sin((2 * pi() * x_theta) / lamda);  // odd componant only -- complex operations hard in c#

                    G[Sx + x, Sy + y] = gaussian * harmonic;
                }
            
            return G;
        }

        /// <summary>
        /// Convolves an image with the given filter
        /// </summary>
        /// <param name="I">Source image to convole</param>
        /// <param name="filter">Filter to apply to source</param>
        /// <returns>Convolved image</returns>
        private static double[,] convolve(int[,] I, double[,] filter)
        {

            int pad = (int)System.Math.Floor((double)filter.GetLength(0) / 2); // amount of padding needed around the image
            double[,] Iout = new double[I.GetLength(0) - 2 * pad, I.GetLength(1) - 2 * pad];

            for (int y = 0; y < (I.GetLength(0) - 2 * pad); y++)        // go over every pixel less the padding in the image
                for (int x = 0; x < (I.GetLength(1) - 2 * pad); x++)
                    for (int fy = -pad; fy <= pad; fy++)                // apply every value in the filter
                        for (int fx = -pad; fx <= pad; fx++)
                            Iout[y, x] += (double)I[(y + pad) + fy, (x + pad) + fx] * filter[fy + pad, fx + pad];

            return Iout;
        }
      
       
        #endregion

      

        #region system.math wrappers
        private static double sin(double num)
        {
            return System.Math.Sin(num);
        }
        private static double cos(double num)
        {
            return System.Math.Cos(num);
        }
        private static double tan(double num)
        {
            return System.Math.Tan(num);
        }
        private static double exp(double num)
        {
            return System.Math.Exp(num);
        }
        private static double pow(double num, double power)
        {
            return System.Math.Pow(num, power);
        }
        private static double pi()
        {
            return System.Math.PI;
        }
        private static double sqrt(double num)
        {
            return System.Math.Sqrt(num);
        }
        private static int floor(double num)
        {
            return (int)System.Math.Floor(num);
        }
        private static double abs(double num)
        {
            return System.Math.Abs(num);
        }
        #endregion


    }
}
