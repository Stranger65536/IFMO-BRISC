#define INCLUDE_UNSAFE_CODE


using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using openDicom.DataStructure;
using openDicom.File;
using openDicom.Registry;

namespace BRISC.Core
{
    public static class Util
    {
        public delegate double[][,] FeatureMethod(int[,] data);


        public static string DATA_PATH = "../../../data/";


        public static string ORIGINAL_IMAGES_PATH = DATA_PATH + "original-images-dicom/";


        public static string NODULE_IMAGE_PATH = DATA_PATH + "nodules-images/";


        public static string PRIMARY_XML = "nodules-primary.xml";


        public static string DATAPATH_FILE = "datapath.txt";


        public static bool GLOBAL_DICT_SET;


        public static short BG_VALUE = -2000;


        public static bool WINDOWING = true;


        public static int WINDOW_LOWER = 0;


        public static int WINDOW_UPPER = 850;


        public static void SetDataPath(string path)
        {
            DATA_PATH = path;

            // make sure there's a slash at the end
            if (!DATA_PATH.EndsWith("\\"))
                DATA_PATH += '\\';

            // need to reset NODULE_IMAGE_PATH and ORIGINAL_IMAGES_PATH
            NODULE_IMAGE_PATH = DATA_PATH + "nodules-images/";
            ORIGINAL_IMAGES_PATH = DATA_PATH + "original-images-dicom/";
        }


        public static void LoadDataPath()
        {
            LoadDataPath(DATAPATH_FILE);
        }


        public static void LoadDataPath(string filename)
        {
            try
            {
                TextReader fin = new StreamReader(filename);
                var line = fin.ReadLine();
                if (line != null && new DirectoryInfo(line) != null)
                    SetDataPath(line);
                fin.Close();
            }
            catch (Exception)
            {
            }
        }


        public static void SaveDataPath()
        {
            SaveDataPath(DATAPATH_FILE);
        }


        public static void SaveDataPath(string filename)
        {
            try
            {
                TextWriter fout = new StreamWriter(filename);
                fout.WriteLine(DATA_PATH);
                fout.Close();
            }
            catch (Exception)
            {
            }
        }


        public static int[,] LoadDICOMPixelData(string filename)
        {
            // initialize global data element dictionary'
            if (!GLOBAL_DICT_SET)
            {
                DataElementDictionary.Global = new DataElementDictionary(DATA_PATH + "dicom-elements-2007.dic",
                    DictionaryFileFormat.BinaryFile);
                GLOBAL_DICT_SET = true;
            }

            if (!File.Exists(filename))
                return null;

            // load raw DICOM pixel data
            var df = new DicomFile(filename, false);
            var pxd = df.PixelData;

            if (pxd.ToArray()[0] is byte[])
            {
                var idata = (byte[]) pxd.ToArray()[0];
                var rows = pxd.Rows;
                var cols = pxd.Columns;

                // save to two-dimensional pixel data array
                var image = new int[rows, cols];
                for (int r = 0, i = 0; r < image.GetLength(0); r++)
                    for (var c = 0; c < image.GetLength(1); c++)
                        image[r, c] = idata[i++];

                return image;
            }
            else
            {
                var idata = (short[]) pxd.ToArray()[0];
                var rows = pxd.Rows;
                var cols = pxd.Columns;

                // save to two-dimensional pixel data array
                var image = new int[rows, cols];
                for (int r = 0, i = 0; r < image.GetLength(0); r++)
                    for (var c = 0; c < image.GetLength(1); c++)
                        image[r, c] = idata[i++];

                return image;
            }
        }


        public static DicomFile LoadDICOMFile(string filename)
        {
            // initialize global data element dictionary
            if (!GLOBAL_DICT_SET)
            {
                DataElementDictionary.Global = new DataElementDictionary(DATA_PATH + "dicom-elements-2007.dic",
                    DictionaryFileFormat.BinaryFile);
                GLOBAL_DICT_SET = true;
            }

            if (!File.Exists(filename))
                return null;

            // load raw DICOM pixel data
            var df = new DicomFile(filename, false);
            return df;
        }


        public static object[] LoadDICOMHeaderInfo(string filename, string header1, string header2)
        {
            object[] values = null;
            var df = LoadDICOMFile(filename);
            if (df != null)
            {
                var ds = df.DataSet;
                var tg = new Tag(header1, header2);
                var de = ds[tg];
                var vv = de.Value;
                values = vv.ToArray();
            }
            return values;
        }


        public static int[,] SegmentNodule(int[,] data, LIDCNodule nodule, int ybuff, int xbuff)
        {
            // pad the width and height of the segmented image
            var width = nodule.Width + 2*xbuff;
            var height = nodule.Height + 2*ybuff;

            // allocate memory
            var segNodule = new int[height, width];

            // copy values
            var yy = 0;
            for (var y = nodule.MinY - ybuff - 1; y < nodule.MaxY + ybuff; y++)
            {
                var xx = 0;
                for (var x = nodule.MinX - xbuff - 1; x < nodule.MaxX + xbuff; x++)
                {
                    segNodule[yy, xx] = data[y, x];
                    xx++;
                }
                yy++;
            }
            return segNodule;
        }


        public static double ComputeMean(double[,] data)
        {
            double sum = 0;
            foreach (var val in data)
            {
                sum += val;
            }
            return sum /= data.Length;
        }


        public static double ComputeStdDev(double[,] data)
        {
            var mean = ComputeMean(data);
            double sum = 0;
            foreach (var val in data)
            {
                sum += Math.Pow(val - mean, 2);
            }
            return Math.Sqrt(sum/(data.Length - 1));
        }


        public static double[] ComputeHistogram(double[,] data, int histBins, double histMinVal, double histMaxVal)
        {
            var hist = new double[histBins];
            for (var i = 0; i < histBins; i++)
            {
                hist[i] = 0;
            }
            double minval = 0;
            var binrange = (histMaxVal - minval)/histBins;
            int binnum;
            var count = 0;

            foreach (var val in data)
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

                    binnum = (int) Math.Floor((val - minval)/binrange);
                    if (binnum > histBins - 1)
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


        private static double[] normalizeHistogram(double[] hist, int n)
        {
            for (var i = 0; i < hist.Length; i++)
            {
                hist[i] /= n;
            }
            return hist;
        }


        public static double[] FindHistMeanStd(LIDCNoduleDB noduleDB, ProgressBar pbar, FeatureMethod featureMethod)
        {
            var ret = new double[3];
            var mean = findMeanHist(noduleDB, pbar, featureMethod);
            var std = findStdHist(noduleDB, mean, pbar, featureMethod);

            ret[0] = mean;
            ret[1] = std;
            return ret;
        }


        private static double findStdHist(LIDCNoduleDB noduleDB, double mean, ProgressBar pbar, FeatureMethod featureMethod)
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


            foreach (var nodule in noduleDB)
            {
                var origionalData = LoadDICOMPixelData(ORIGINAL_IMAGES_PATH + nodule.GetOriginalDICOMFilename());
                var segData = SegmentNodule(origionalData, nodule, 4, 4);


                // obtain the feature set
                var features = featureMethod(segData);
                // do std calculations
                for (var i = 0; i < features.GetLength(0); i++)
                {
                    foreach (var val in features[i])
                    {
                        sum += Math.Pow(val - mean, 2);
                        numvals++;
                    }
                }
                pbar.Value++;
                pbar.Refresh();
            }
            return Math.Sqrt(sum/numvals);
        }


        private static double findMeanHist(LIDCNoduleDB noduleDB, ProgressBar pbar, FeatureMethod featureMethod)
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

            foreach (var nodule in noduleDB)
            {
                var origionalData = LoadDICOMPixelData(ORIGINAL_IMAGES_PATH + nodule.GetOriginalDICOMFilename());
                var segData = SegmentNodule(origionalData, nodule, 4, 4);


                // obtain the feature set
                var features = featureMethod(segData);
                // do mean calculations
                for (var i = 0; i < features.GetLength(0); i++)
                {
                    foreach (var val in features[i])
                    {
                        sum += val;
                        numvals++;
                    }
                }
                pbar.Value++;
                pbar.Refresh();
            }
            return sum/numvals;
        }

#if INCLUDE_UNSAFE_CODE


        public static Bitmap LoadDICOMBitmap(string filename)
        {
            // initialize global data element dictionary
            if (!GLOBAL_DICT_SET)
            {
                DataElementDictionary.Global = new DataElementDictionary(DATA_PATH + "dicom-elements-2007.dic",
                    DictionaryFileFormat.BinaryFile);
                GLOBAL_DICT_SET = true;
            }

            if (!File.Exists(filename))
                return null;

            // load raw DICOM pixel data
            var df = new DicomFile(filename, false);
            var pxd = df.PixelData;
            var idata = (short[]) pxd.ToArray()[0];
            var rows = pxd.Rows;
            var cols = pxd.Columns;

            // find min and max pixel values
            var min = WINDOWING ? (short) WINDOW_LOWER : short.MaxValue;
            var max = WINDOWING ? (short) WINDOW_UPPER : short.MinValue;
            if (!WINDOWING)
            {
                for (var i = 0; i < idata.Length; i++)
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
            var ubmp = new UnsafeBitmap(cols, rows);
            ubmp.LockBitmap();
            UnsafeBitmap.PixelData pds;

            // copy pixel data into bitmap
            byte val;
            for (var j = 0; j < idata.Length; j++)
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
                        val = (byte) ((idata[j] - min)/(double) (max - min + 1)*256.0);
                }
                pds.red = pds.green = pds.blue = val;
                ubmp.SetPixel(j%cols, j/cols, pds);
            }

            ubmp.UnlockBitmap();
            return ubmp.Bitmap;
        }


        public static Bitmap ConvertPixelDataToBitmap(int[,] image)
        {
            // find min and max pixel values
            var rows = image.GetLength(0);
            var cols = image.GetLength(1);

            // find min and max pixel values
            var min = WINDOWING ? WINDOW_LOWER : int.MaxValue;
            var max = WINDOWING ? WINDOW_UPPER : int.MinValue;
            if (!WINDOWING)
            {
                for (var r = 0; r < image.GetLength(0); r++)
                {
                    for (var c = 0; c < image.GetLength(1); c++)
                    {
                        if (image[r, c] != BG_VALUE)
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
            var ubmp = new UnsafeBitmap(cols, rows);
            ubmp.LockBitmap();
            UnsafeBitmap.PixelData pds;

            // copy pixel data into bitmap
            byte val;
            for (var r = 0; r < image.GetLength(0); r++)
            {
                for (var c = 0; c < image.GetLength(1); c++)
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
                            val = (byte) ((image[r, c] - min)/(double) (max - min + 1)*256.0);
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
    }
}