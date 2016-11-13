#define INCLUDE_UNSAFE_CODE

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;
using openDicom.Encoding;
using openDicom.File;
using openDicom.Image;
using openDicom.Registry;

namespace BRISC.Core
{
    /// <summary>
    /// Contains miscellaneous static methods and constants, primarily dealing with DICOM file interaction
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Relative path to data files
        /// </summary>
        public static string DATA_PATH = "../../../data/";
        /// <summary>
        /// Relative path to original dicom images
        /// </summary>
        public static string ORIGINAL_IMAGES_PATH = Util.DATA_PATH + "original-images-dicom/";
        /// <summary>
        /// Relative path to segmented nodule images
        /// </summary>
        public static string NODULE_IMAGE_PATH = Util.DATA_PATH + "nodules-images/";
        /// <summary>
        /// Filename of primary XML data file
        /// </summary>
        public static string PRIMARY_XML = "nodules-primary.xml";
        /// <summary>
        /// Standard data path filename
        /// </summary>
        public static string DATAPATH_FILE = "datapath.txt";

        /// <summary>
        /// Has the global DICOM dictionary been created yet?
        /// </summary>
        public static bool GLOBAL_DICT_SET = false;

        /// <summary>
        /// Signal value that represents the background intensity in a DICOM image
        /// </summary>
        public static Int16 BG_VALUE = -2000;

        /// <summary>
        /// Turns on/off custom linear window leveling
        /// </summary>
        public static bool WINDOWING = true;

        /// <summary>
        /// Specifies lower bound for custom window leveling
        /// </summary>
        public static int WINDOW_LOWER = 0;

        /// <summary>
        /// Specifies upper bound for custom window leveling
        /// </summary>
        public static int WINDOW_UPPER = 850;

        /// <summary>
        /// Sets the current data path
        /// </summary>
        /// <param name="path">Data folder</param>
        public static void SetDataPath(string path)
        {
            DATA_PATH = path;

            // make sure there's a slash at the end
            if (!DATA_PATH.EndsWith("\\"))
                DATA_PATH += '\\';

            // need to reset NODULE_IMAGE_PATH and ORIGINAL_IMAGES_PATH
            Util.NODULE_IMAGE_PATH = Util.DATA_PATH + "nodules-images/";
            Util.ORIGINAL_IMAGES_PATH = Util.DATA_PATH + "original-images-dicom/";
        }

        /// <summary>
        /// Load the data path from the standard file
        /// </summary>
        public static void LoadDataPath()
        {
            LoadDataPath(DATAPATH_FILE);
        }

        /// <summary>
        /// Loads the data path from a given file
        /// </summary>
        /// <param name="filename">Name of file (first line should contain path to data directory)</param>
        public static void LoadDataPath(string filename)
        {
            try
            {
                TextReader fin = new StreamReader(filename);
                string line = fin.ReadLine();
                if (line != null && new DirectoryInfo(line) != null)
                    SetDataPath(line);
                fin.Close();
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Save the data path to the standard file
        /// </summary>
        public static void SaveDataPath()
        {
            SaveDataPath(DATAPATH_FILE);
        }

        /// <summary>
        /// Save the data path to a given file
        /// </summary>
        /// <param name="filename">Name of file</param>
        public static void SaveDataPath(string filename)
        {
            try
            {
                TextWriter fout = new StreamWriter(filename);
                fout.WriteLine(Util.DATA_PATH);
                fout.Close();
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Loads pixel data from a DICOM file into a 2-dimensional integer array
        /// </summary>
        /// <param name="filename">Filename of .dcm data file</param>
        /// <returns>2-dimensional matrix of signed integer pixel data</returns>
        public static int[,] LoadDICOMPixelData(string filename)
        {
            // initialize global data element dictionary'
            if (!GLOBAL_DICT_SET)
            {
                DataElementDictionary.Global = new DataElementDictionary(Util.DATA_PATH + "dicom-elements-2004.dic", DictionaryFileFormat.BinaryFile);
                GLOBAL_DICT_SET = true;
            }

            if (!File.Exists(filename))
                return null;

            // load raw DICOM pixel data
            DicomFile df = new DicomFile(filename, false);
            PixelData pxd = df.PixelData;
            Int16[] idata = (Int16[])pxd.ToArray()[0];
            int rows = pxd.Rows;
            int cols = pxd.Columns;

            // save to two-dimensional pixel data array
            int[,] image = new int[rows, cols];
            for (int r = 0, i = 0; r < image.GetLength(0); r++)
                for (int c = 0; c < image.GetLength(1); c++)
                    image[r, c] = (int)idata[i++];

            return image;
        }

        /// <summary>
        /// Loads a DicomFile object from a .dcm file.
        /// </summary>
        /// <param name="filename">DICOM file</param>
        /// <returns>DicomFile object (null if filename is invalid)</returns>
        public static DicomFile LoadDICOMFile(string filename)
        {
            // initialize global data element dictionary
            if (!GLOBAL_DICT_SET)
            {
                DataElementDictionary.Global = new DataElementDictionary(Util.DATA_PATH + "dicom-elements-2007.dic", DictionaryFileFormat.BinaryFile);
                GLOBAL_DICT_SET = true;
            }

            if (!File.Exists(filename))
                return null;

            // load raw DICOM pixel data
            DicomFile df = new DicomFile(filename, false);
            return df;
        }

        /// <summary>
        /// Loads value from a DICOM file header
        /// </summary>
        /// <param name="filename">DICOM filename</param>
        /// <param name="header1">First header ID</param>
        /// <param name="header2">Second header ID</param>
        /// <returns></returns>
        public static object[] LoadDICOMHeaderInfo(string filename, string header1, string header2)
        {
            object[] values = null;
            openDicom.File.DicomFile df = Util.LoadDICOMFile(filename);
            if (df != null)
            {
                openDicom.DataStructure.DataSet.DataSet ds = df.DataSet;
                openDicom.DataStructure.Tag tg = new openDicom.DataStructure.Tag(header1, header2);
                openDicom.DataStructure.DataSet.DataElement de = ds[tg];
                openDicom.DataStructure.Value vv = de.Value;
                values = vv.ToArray();
            }
            return values;
        }

#if INCLUDE_UNSAFE_CODE

        /// <summary>
        /// Loads pixel data from a DICOM file and converts it to a 
        /// Bitmap object, windowing according to WINDOWING, 
        /// WINDOW_LOWER and WINDOW_UPPER.
        /// <remarks>
        /// This function is equivalent 
        /// to calling <c>ConvertPixelDataToBitmap(LoadDICOMPixelData(filename))</c>, 
        /// but is theoretically a bit faster because it converts 
        /// directly between the 1-dimensional DICOM data and the 1-dimensional 
        /// pixel data; ie. it doesn't convert to a 2-dimensional structure 
        /// in between, as LoadDICOMPixelData() does.
        /// </remarks>
        /// </summary>
        /// <param name="filename">Filename of .dcm data file</param>
        /// <returns>24-bit bitmap object</returns>
        public static Bitmap LoadDICOMBitmap(string filename)
        {
            // initialize global data element dictionary
            if (!GLOBAL_DICT_SET)
            {
                DataElementDictionary.Global = new DataElementDictionary(Util.DATA_PATH + "dicom-elements-2004.dic", DictionaryFileFormat.BinaryFile);
                GLOBAL_DICT_SET = true;
            }

            if (!File.Exists(filename))
                return null;

            // load raw DICOM pixel data
            DicomFile df = new DicomFile(filename, false);
            PixelData pxd = df.PixelData;
            Int16[] idata = (Int16[])pxd.ToArray()[0];
            int rows = pxd.Rows;
            int cols = pxd.Columns;

            // find min and max pixel values
            Int16 min = (WINDOWING ? (Int16)WINDOW_LOWER : Int16.MaxValue);
            Int16 max = (WINDOWING ? (Int16)WINDOW_UPPER : Int16.MinValue);
            if (!WINDOWING)
            {
                for (int i = 0; i < idata.Length; i++)
                {
                    if (idata[i] != BG_VALUE)
                    {
                        if (idata[i] < min)
                            min = idata[i];
                        if (idata[i] > max)
                            max = idata[i];
                    }
                }
            }

            // create new unsafe bitmap and lock pixel data
            UnsafeBitmap ubmp = new UnsafeBitmap(cols, rows);
            ubmp.LockBitmap();
            UnsafeBitmap.PixelData pds;

            // copy pixel data into bitmap
            byte val;
            for (int j = 0; j < idata.Length; j++)
            {
                if (idata[j] == BG_VALUE)
                    val = 0;
                else
                {
                    if (idata[j] >= max)
                        val = 255;
                    else if (idata[j] <= min)
                        val = 0;
                    else
                        val = (byte)(((double)(idata[j] - min)) / ((double)(max - min + 1)) * 256.0);
                }
                pds.red = pds.green = pds.blue = val;
                ubmp.SetPixel(j % cols, j / cols, pds);
            }

            ubmp.UnlockBitmap();
            return ubmp.Bitmap;
        }

        /// <summary>
        /// Converts raw pixel data to a Bitmap object, windowing according to WINDOWING, WINDOW_LOWER and WINDOW_UPPER
        /// </summary>
        /// <param name="image">2-dimensional matrix of signed integer pixel data</param>
        /// <returns>24-bit bitmap object with the same size as the input image matrix</returns>
        public static Bitmap ConvertPixelDataToBitmap(int[,] image)
        {
            // find min and max pixel values
            int rows = image.GetLength(0);
            int cols = image.GetLength(1);

            // find min and max pixel values
            int min = (WINDOWING ? (int)WINDOW_LOWER : int.MaxValue);
            int max = (WINDOWING ? (int)WINDOW_UPPER : int.MinValue);
            if (!WINDOWING)
            {
                for (int r = 0; r < image.GetLength(0); r++)
                {
                    for (int c = 0; c < image.GetLength(1); c++)
                    {
                        if (image[r, c] != (int)BG_VALUE)
                        {
                            if (image[r, c] < min)
                                min = image[r, c];
                            if (image[r, c] > max)
                                max = image[r, c];
                        }
                    }
                }
            }

            // create new unsafe bitmap and lock pixel data
            UnsafeBitmap ubmp = new UnsafeBitmap(cols, rows);
            ubmp.LockBitmap();
            UnsafeBitmap.PixelData pds;

            // copy pixel data into bitmap
            byte val;
            for (int r = 0; r < image.GetLength(0); r++)
            {
                for (int c = 0; c < image.GetLength(1); c++)
                { 
                    if (image[r, c] == BG_VALUE)
                        val = 0;
                    else
                    {
                        if (image[r, c] >= max)
                            val = 255;
                        else if (image[r, c] <= min)
                            val = 0;
                        else
                            val = (byte)(((double)(image[r, c] - min)) / ((double)(max - min + 1)) * 256.0);
                    }
                    pds.red = pds.green = pds.blue = val;
                    ubmp.SetPixel(c, r, pds);
                }
            }

            // release pixel data and return bitmap
            ubmp.UnlockBitmap();
            return ubmp.Bitmap;
        }

#endif

        /// <summary>
        /// Segments a nodule from original data with padding
        /// </summary>
        /// <param name="data">Original data</param>
        /// <param name="nodule">Nodule to segment out</param>
        /// <param name="ybuff">row padding</param>
        /// <param name="xbuff">column padding</param>
        /// <returns>Segmented nodule data</returns>
        public static int[,] SegmentNodule(int[,] data, LIDCNodule nodule, int ybuff, int xbuff)
        {
            // pad the width and height of the segmented image
            int width = nodule.Width + (2 * xbuff);
            int height = nodule.Height + (2 * ybuff);

            // allocate memory
            int[,] segNodule = new int[height, width];

            // copy values
            int yy = 0;
            for (int y = (nodule.MinY - ybuff) - 1; y < (nodule.MaxY + ybuff); y++)
            {
                int xx = 0;
                for (int x = (nodule.MinX - xbuff) - 1; x < (nodule.MaxX + xbuff); x++)
                {
                    segNodule[yy, xx] = data[y, x];
                    xx++;
                }
                yy++;
            }
            return segNodule;
        }

        /// <summary>
        /// Computes the mean value from the NxN matrix
        /// </summary>
        /// <param name="data">Input matrix</param>
        /// <returns>Mean</returns>
        public static double ComputeMean(double[,] data)
        {
            double sum = 0;
            foreach (double val in data)
            {
                sum += val;
            }
            return sum /= data.Length;
        }

        /// <summary>
        /// Computes the standard deviation from the NxN matrix
        /// </summary>
        /// <param name="data">Input matrix</param>
        /// <returns>Standard deviation</returns>
        public static double ComputeStdDev(double[,] data)
        {
            double mean = ComputeMean(data);
            double sum = 0;
            foreach (double val in data)
            {
                sum += System.Math.Pow((val - mean), 2);
            }
            return System.Math.Sqrt(sum / (data.Length - 1));
        }

        /// <summary>
        /// Computes a histogram of the NxN matrix
        /// </summary>
        /// <param name="data">Matrix to create the histogram from</param>
        /// <param name="histBins">Number of bins to use</param>
        /// <param name="histMinVal">Minimum cutoff value for the histogram</param>
        /// <param name="histMaxVal">Maximum cutoff value for the histogram</param>
        /// <returns>Histogram of the intensity values</returns>
        public static double[] ComputeHistogram(double[,] data, int histBins, double histMinVal, double histMaxVal)
        {
            double[] hist = new double[histBins];
            for (int i = 0; i < histBins; i++)
            {
                hist[i] = 0;
            }
            double minval = 0;
            double binrange = (histMaxVal - minval) / histBins;
            int binnum;
            int count = 0;

            foreach (double val in data)
            {
                if (!double.IsNaN(val))
                {
                    count++;
                    // anything above the maximum value is put into the top bin
                    if (val >= histMaxVal)
                    {
                        hist[histBins - 1]++;
                        continue;
                    }
                    // anything below the minimum value is put into the bottom bin
                    if (val <= minval)
                    {
                        hist[0]++;
                        continue;
                    }

                    binnum = (int)System.Math.Floor((val - minval) / binrange);
                    if (binnum > (histBins - 1))
                    {
                        binnum = binnum - 1;
                    }
                    hist[binnum]++;
                }
                else
                {
                    break;
                }
            }
            return normalizeHistogram(hist, count);
        }

        /// <summary>
        /// Normalizes the histogram so that each bin has values between 0 and 1
        /// </summary>
        /// <param name="hist">The histogram to normalize</param>
        /// <param name="n">Total number of intensity values in the origional image</param>
        /// <returns>Normalized histogram</returns>
        private static double[] normalizeHistogram(double[] hist, int n)
        {
            for (int i = 0; i < hist.Length; i++)
            {
                hist[i] /= n;
            }
            return hist;
        }

        /// <summary>
        /// Delegate for use with FindHistCutoff. Will point to the function which performs feature extraction. 
        /// Only works for feature methods which return arrays of feature matrices.
        /// </summary>
        /// <param name="data">Array to extract feature from</param>
        /// <returns>Array of feature matrices</returns>
        public delegate double[][,] FeatureMethod(int[,] data);
       
        /// <summary>
        /// Function to find a good max/min value for a histogram. Runs a feature extraction method on every nodule in the DB
        /// and finds the mean and standard deviation from all the responses.
        /// </summary>
        /// <param name="noduleDB">The nodule database</param>
        /// <param name="pbar">Progress bar</param>
        /// <param name="featureMethod">Feature extraction method to use</param>
        /// <returns>mean: ret[0] standard deviation: ret[1]</returns>
        public static double[] FindHistMeanStd(LIDCNoduleDB noduleDB, System.Windows.Forms.ProgressBar pbar, FeatureMethod featureMethod)
        {
            double[] ret = new double[3];
            double mean = findMeanHist(noduleDB, pbar, featureMethod);
            double std = findStdHist(noduleDB, mean, pbar, featureMethod);
            
            ret[0] = mean;
            ret[1] = std;
            return ret;
        }

        /// <summary>
        /// Finds the standard deviation of all the features from every nodule in the database. Requires mean to be already 
        /// computed
        /// </summary>
        /// <param name="noduleDB">The nodule database</param>
        /// <param name="mean">Mean</param>
        /// <param name="pbar">Progress Bar</param>
        /// <param name="featureMethod">feature extraction method to use</param>
        /// <returns>Standard deviation</returns>
        private static double findStdHist(LIDCNoduleDB noduleDB, double mean, System.Windows.Forms.ProgressBar pbar, FeatureMethod featureMethod )
        {
            if (pbar != null)
            {
                // initialize progress bar
                pbar.Minimum = 0;
                pbar.Maximum = noduleDB.Nodules.Count;
                pbar.Value = 0;
                pbar.Refresh();
            }

            double sum = 0;
            double numvals = 0;


            foreach (LIDCNodule nodule in noduleDB)
            {
                int[,] origionalData = Util.LoadDICOMPixelData(Util.ORIGINAL_IMAGES_PATH + nodule.GetOriginalDICOMFilename());
                int[,] segData = Util.SegmentNodule(origionalData, nodule, 4, 4);


                // obtain the feature set
                double[][,] features = featureMethod(segData);
                // do std calculations
                for (int i = 0; i < features.GetLength(0); i++)
                {
                    foreach (double val in features[i])
                    {
                        sum += System.Math.Pow((val - mean), 2);
                        numvals++;
                    }
                }
                pbar.Value++;
                pbar.Refresh();
            }
            return System.Math.Sqrt(sum / numvals);
        }

        /// <summary>
        /// Finds the mean of all the features from every nodule in the database.
        /// </summary>
        /// <param name="noduleDB">The nodule database</param>
        /// <param name="pbar">Progress bar</param>
        /// <param name="featureMethod">Feature extraction method to use</param>
        /// <returns>Mean</returns>
        private static double findMeanHist(LIDCNoduleDB noduleDB, System.Windows.Forms.ProgressBar pbar, FeatureMethod featureMethod)
        {
            if (pbar != null)
            {
                // initialize progress bar
                pbar.Minimum = 0;
                pbar.Maximum = noduleDB.Nodules.Count;
                pbar.Value = 0;
                pbar.Refresh();
            }

            double sum = 0;
            double numvals = 0;

            foreach (LIDCNodule nodule in noduleDB)
            {
                int[,] origionalData = Util.LoadDICOMPixelData(Util.ORIGINAL_IMAGES_PATH + nodule.GetOriginalDICOMFilename());
                int[,] segData = Util.SegmentNodule(origionalData, nodule, 4, 4);


                // obtain the feature set
                double[][,] features = featureMethod(segData);
                // do mean calculations
                for (int i = 0; i < features.GetLength(0); i++)
                {
                    foreach (double val in features[i])
                    {
                        sum += val;
                        numvals++;
                    }
                }
                pbar.Value++;
                pbar.Refresh();
            }
            return sum / numvals;
        }

    }
}
