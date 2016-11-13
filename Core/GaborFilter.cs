using System;

namespace BRISC.Core
{
    public class GaborFilter : FeatureExtractor
    {
        public static int histBins = 8;
        public static double histMaxVal = 600;
        public static double histMinVal = 0;

        public void ExtractFeatures(LIDCNodule nodule)
        {
            // grab the original lung image
            var originalData = nodule.OriginalPixelData;

            // segment out the nodule with padding
            var segData = Util.SegmentNodule(originalData, nodule, 4, 4);

            // allocate memory
            nodule.GaborHist = new double[4, 3][];
            nodule.GaborMean = new double[4, 3];
            nodule.GaborStdDev = new double[4, 3];

            var gaborResponse = ApplyFilter(segData); // apply filter to get back 12 response images   

            var responseIndex = 0;
            var orientationIndex = 0;
            // for all orientations
            for (double theta = 0; theta <= 3*pi()/4; theta += pi()/4)
            {
                var scaleIndex = 0;
                // and all frequencies
                for (var frequency = .3; frequency <= .5; frequency += .1)
                {
                    // compute the histogram/mean/std and store it in the LIDCNodule
                    nodule.GaborHist[orientationIndex, scaleIndex] = Util.ComputeHistogram(gaborResponse[responseIndex], histBins,
                        histMinVal, histMaxVal);
                    nodule.GaborMean[orientationIndex, scaleIndex] = Util.ComputeMean(gaborResponse[responseIndex]);
                    nodule.GaborStdDev[orientationIndex, scaleIndex] = Util.ComputeStdDev(gaborResponse[responseIndex]);
                    scaleIndex++;
                    responseIndex++;
                }
                orientationIndex++;
            }
        }

        public static double[][,] ApplyFilter(int[,] I)
        {
            var Iout = new double[12][,];
            var responseIndex = 0;

            // for all orientations
            for (double theta = 0; theta <= 3*pi()/4; theta += pi()/4)
            {
                // and all frequencies
                for (var frequency = .3; frequency <= .5; frequency += .1)
                {
                    // size of filter
                    var Sx = 4;
                    var Sy = 4;
                    // filter parameters
                    var gamma = .5;
                    var lamda = 1/frequency;
                    var sigma = .56*lamda;

                    // set up gabor filter
                    var G = buildFilter(Sx, Sy, sigma, gamma, lamda, theta);
                    // convolve image with gabor filter
                    Iout[responseIndex] = convolve(I, G);

                    // get the absolute value for all values in the response
                    for (var y = 0; y < Iout[responseIndex].GetLength(0); y++)
                        for (var x = 0; x < Iout[responseIndex].GetLength(1); x++)
                            Iout[responseIndex][y, x] = abs(Iout[responseIndex][y, x]);

                    responseIndex++;
                }
            }
            return Iout;
        }

        private static double[,] buildFilter(int Sx, int Sy, double sigma, double gamma, double lamda, double theta)
        {
            var G = new double[2*Sx + 1, 2*Sy + 1]; // Filter matrix

            for (var x = -Sx; x <= Sx; x++)
                for (var y = -Sy; y <= Sy; y++)
                {
                    var x_theta = x*cos(theta) + y*sin(theta);
                    var y_theta = -x*sin(theta) + y*cos(theta);

                    var gaussian = exp(-.5*(pow(x_theta, 2) + pow(gamma, 2)*pow(y_theta, 2))/pow(sigma, 2));
                    var harmonic = sin(2*pi()*x_theta/lamda); // odd componant only -- complex operations hard in c#

                    G[Sx + x, Sy + y] = gaussian*harmonic;
                }

            return G;
        }

        private static double[,] convolve(int[,] I, double[,] filter)
        {
            var pad = (int) Math.Floor((double) filter.GetLength(0)/2); // amount of padding needed around the image
            var Iout = new double[I.GetLength(0) - 2*pad, I.GetLength(1) - 2*pad];

            for (var y = 0; y < I.GetLength(0) - 2*pad; y++) // go over every pixel less the padding in the image
                for (var x = 0; x < I.GetLength(1) - 2*pad; x++)
                    for (var fy = -pad; fy <= pad; fy++) // apply every value in the filter
                        for (var fx = -pad; fx <= pad; fx++)
                            Iout[y, x] += I[y + pad + fy, x + pad + fx]*filter[fy + pad, fx + pad];

            return Iout;
        }

        private static double sin(double num)
        {
            return Math.Sin(num);
        }

        private static double cos(double num)
        {
            return Math.Cos(num);
        }

        private static double tan(double num)
        {
            return Math.Tan(num);
        }

        private static double exp(double num)
        {
            return Math.Exp(num);
        }

        private static double pow(double num, double power)
        {
            return Math.Pow(num, power);
        }

        private static double pi()
        {
            return Math.PI;
        }

        private static double sqrt(double num)
        {
            return Math.Sqrt(num);
        }

        private static int floor(double num)
        {
            return (int) Math.Floor(num);
        }

        private static double abs(double num)
        {
            return Math.Abs(num);
        }
    }
}