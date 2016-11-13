// RUN NORMALLY (IF COMMENTED, RUN SCRIPTED TASK)
#define NORMAL_GUI_EXECUTION

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using BRISC.Core;

namespace BRISC.GUI
{
    /// <summary>
    /// Serves as a program entry point.
    /// </summary>
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Util.LoadDataPath();
            
#if NORMAL_GUI_EXECUTION

            // SHOW MAIN MENU
            MainMenu mm = new MainMenu();
            mm.Show();
            Application.Run();
#else
            // PERFORM SCRIPTED TASK
            doTask();
#endif

        }

        /// <summary>
        /// Perform a scripted task
        /// </summary>
        public static void doTask()
        {
            // which tast should we do?


            //calcDescriptors(2);

            //calcMarkovStats();
            //createMainXMLFile();
            //calcGaborStats();
            //testMarkovPrecisionRecall();

            //findNoduleSizeHist();

           
            //testCoOccurrence();

            //testGaborPrecisionRecall();
            //testHaralickPrecisionRecall();

            //extractAllXMLs();
            //TestPrecisionRecall();

            calcAllActualSizes();
            printDataToText();

            //calcAllActualSizes();

            //testCode();
        }
      
        #region Tasks

        /// <summary>
        /// SCRIPTED TASK: Perform precision/recall calculations. Uses trial descriptions in given filename.
        /// </summary>
        public static void TestPrecisionRecall(string trialsFilename, string resultsPrefix)
        {
            //string trialsFilename = "trials.txt";
            string resultsFilename = "results-" + resultsPrefix + "-" + Util.PRIMARY_XML.Replace(".xml","") + ".txt";
            double[] results = new double[2];
            string features;
            string similarity;
            int nItems;
            double thresh;

            // display splash screen
            Splash screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            // load data
            LIDCNoduleDB myDB = new LIDCNoduleDB(Util.DATA_PATH + Util.PRIMARY_XML, screen.Progress);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-haralick.xml", screen.Progress, false);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-gabor.xml", screen.Progress, false);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-markov.xml", screen.Progress, false);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml", screen.Progress, false);

            // constant parameter(s)
            thresh = double.PositiveInfinity;

            // find out which trial to start at
            StreamReader fin = new StreamReader(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.OpenOrCreate));
            int startTrialNum = 1; //1;
            string line;
            while ((line = fin.ReadLine()) != null)
                startTrialNum = int.Parse(line.Substring(0, line.IndexOf('\t'))) + 1;
            fin.Close();

            // get list of trials to run
            StreamReader fin2 = new StreamReader(new FileStream(Util.DATA_PATH + trialsFilename, FileMode.OpenOrCreate));
            int trial = 0;
            while ( (line = fin2.ReadLine()) != null)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                
                if (++trial >= startTrialNum)
                {
                    // set parameters
                    features = tokens[0];
                    similarity = tokens[1];
                    nItems = int.Parse(tokens[2]);

                    // update status
                    screen.Status.Text = trial.ToString() + ": " + similarity + "  [nitems=" + nItems.ToString() + "]  <" + features + ">";
                    screen.Refresh();

                    // do precision/recall calculation
                    results = myDB.CalcMeanPrecisionAndRecall(features, similarity, nItems, thresh, screen.Progress);

                    // output results
                    StreamWriter fout = new StreamWriter(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.Append));
                    fout.Write(trial.ToString() + "\t" + features + "\t" + similarity + "\t" + nItems.ToString() + "\t");
                    fout.WriteLine(thresh.ToString() + "\t" + results[0].ToString("0.0000") + "\t" + results[1].ToString("0.0000"));
                    fout.Close();
                }

            }
            fin2.Close();

            screen.Close();
        }

        /// <summary>
        /// SCRIPTED TASK: Build all XML subset files
        /// </summary>
        public static void ExtractAllXMLs()
        {
            // display splash screen
            Splash screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            screen.Progress.Minimum = 0;
            screen.Progress.Maximum = 12; //4;
            screen.Progress.Value = 0;

            LIDCNoduleDB myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            string counts = "total: " + myDB.TotalNoduleCount + "\r\n";

            for (int p = 2; p <= 4; p++)
            {
                counts += "texture-" + p + ": " + extractNodules("texture", p, 25, 16000).ToString() + "\r\n";
                screen.Progress.Value++;
            }
            for (int p = 2; p <= 4; p++)
            {
                counts += "malignancy-" + p + ": " + extractNodules("malignancy", p, 25, 16000).ToString() + "\r\n";
                screen.Progress.Value++;
            }

            /* OLD VALUES (for smaller data set)
            counts += "25-126: " + extractNodules(1, 25, 126).ToString() + "\r\n";
            screen.Progress.Value++;
            counts += "127-285: " + extractNodules(1, 127, 285).ToString() + "\r\n";
            screen.Progress.Value++;
            counts += "286-598: " + extractNodules(1, 286, 598).ToString() + "\r\n";
            screen.Progress.Value++;
            counts += "599-6400: " + extractNodules(1, 599, 6400).ToString() + "\r\n";
            screen.Progress.Value++;*/

            counts += "25-104: " + extractNodules("texture", 1, 25, 104).ToString() + "\r\n";
            screen.Progress.Value++;
            counts += "105-234: " + extractNodules("texture", 1, 105, 234).ToString() + "\r\n";
            screen.Progress.Value++;
            counts += "235-625: " + extractNodules("texture", 1, 235, 625).ToString() + "\r\n";
            screen.Progress.Value++;
            counts += "626-16000: " + extractNodules("texture", 1, 626, 16000).ToString() + "\r\n";
            screen.Progress.Value++;

            StreamWriter fout = new StreamWriter(new FileStream(Util.DATA_PATH + "counts.txt", FileMode.OpenOrCreate));
            fout.WriteLine(counts);
            fout.Close();
            //MessageBox.Show(counts);

            screen.Close();
        }

        /// <summary>
        /// SCRIPTED TASK: Builds a main XML input file with primary, annotation, Haralick and Gabor data
        /// </summary>
        /// <remarks>
        /// This task merges all of the XML data except for point data 
        /// into a single XML file. This makes it easier to load for other tasks.
        /// </remarks>
        private static void createMainXMLFile()
        {
            LIDCNodule.SAVE_XML_ANNOTATIONS     = true;
            LIDCNodule.SAVE_XML_GABOR           = true;
            LIDCNodule.SAVE_XML_HARALICK        = true;
            LIDCNodule.SAVE_XML_MARKOV          = true;
            LIDCNodule.SAVE_XML_PRIMARY         = true;
            LIDCNodule.SAVE_XML_BOUNDS          = true;
            LIDCNoduleDB myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-haralick.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-gabor.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-markov.xml");
            myDB.SaveToXML(Util.DATA_PATH + "nodules-main.xml");
        }

        /// <summary>
        /// SCRIPTED TASK: Run Haralick precision/recall calculations. 
        /// </summary>
        /// <remarks>
        /// Runs calculations for all 2047 different combinations of feature vectors, 
        /// all three similarity measures, and five distances (1, 2, 3, 5 and 10). 
        /// This will yield 30,705 unique trials and precision/recall pairs. The 
        /// procedure stores all parameter and result data in a text file 
        /// ("haralick-results.txt") 
        /// as they are calculated. Obviously, this takes an enormous amount of time, 
        /// but luckily it does not all need to run at once. Quitting the program with 
        /// end task will not corrupt the data file. If the program is interupted, 
        /// simply restart it and it will read the file to find out where it left off 
        /// the last time and resume at that point.
        /// </remarks>
        private static void testHaralickPrecisionRecall()
        {
            string resultsFilename = "haralick-results.txt";
            double[] results = new double[2];
            string features;
            string similarity;
            int nItems;
            double thresh;

            // display splash screen
            Splash screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            // load data
            LIDCNoduleDB myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-main.xml", screen.Progress);

            // constant parameter(s)
            //similarity = "Euclidean";
            //nItems = 5;
            thresh = double.PositiveInfinity;

            // find out which trial to start at
            StreamReader fin = new StreamReader(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.OpenOrCreate));
            int startTrialNum = 1; //1;
            string line;
            while ((line = fin.ReadLine()) != null)
                startTrialNum = int.Parse(line.Substring(0, line.IndexOf('\t'))) + 1;
            fin.Close();

            // get list of trials to run
            ArrayList allTrials = new ArrayList();
            StreamReader fin2 = new StreamReader(new FileStream(Util.DATA_PATH + "besttrials.txt", FileMode.OpenOrCreate));
            while ((line = fin2.ReadLine()) != null)
                allTrials.Add(int.Parse(line));
            fin2.Close();

            // different similarity measures
            int trial = 0;
            foreach (string s in new string[] { "Euclidean", "Manhattan", "Chebychev" })
            {
                // different image features
                //for (int ff = 1; ff < 0x800; ff++)      // all combinations of features
                /*foreach (string f in new string[] { 
                "correlation entropy", 
                "contrast correlation homogeneity energy", 
                "correlation clusterTendency", 
                "correlation entropy clusterTendency" })*/
                for (int t = 0; t < allTrials.Count; t++)
                {
                    // form feature string
                    int ff = (int)allTrials[t];
                    string f = "";
                    for (int j = 0; j < 12; j++)
                        if ((ff & (1 << j)) > 0)
                            f += GlobalCoOccurrence.featureNames[j] + " ";
                    f = f.Substring(0, f.Length - 1);

                    // different numbers of items
                    int[] itemNums = new int[] { 1, 2, 3, 5, 10 };
                    for (int n = 0; n < itemNums.Length; n++)
                    {
                        if (++trial >= startTrialNum)
                        {
                            //int minSize = 0;

                            // set parameters
                            features = "Haralick " + f;
                            similarity = s;
                            nItems = itemNums[n];

                            // update status
                            screen.Status.Text = trial.ToString() + ": " + similarity + "  [nitems=" + nItems.ToString() + "]  <" + features + ">";
                            screen.Refresh();

                            // do precision/recall calculation
                            //results = myDB.CalcMeanPrecisionAndRecall(features, similarity, nItems, thresh, minSize, screen.Progress);

                            // output results
                            StreamWriter fout = new StreamWriter(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.Append));
                            fout.Write(trial.ToString() + "\t" + ff + "\t"  + features + "\t" + similarity + "\t" + nItems.ToString() + "\t");
                            fout.WriteLine(thresh.ToString() + "\t" + results[0].ToString("0.0000") + "\t" + results[1].ToString("0.0000"));
                            fout.Close();
                        }
                    }
                }
            }
            
            screen.Close();
        }
        
        /// <summary>
        /// SCRIPTED TASK: Do some tests to make sure co-occurrence is working right
        /// </summary>
        private static void testCoOccurrence()
        {
            int[,] data = new int[5, 5] {
                {-2000, 0, 1, 2, 1},
                {0, 2, 2, 0, 2},
                {1, 1, 1, 2, 1},
                {0, 2, 0, 1, 0},
                {0, 1, 2, 2, -2000}
            };
            GlobalCoOccurrence g = new GlobalCoOccurrence();
            CombineCoOccurrence c = g.PerformCoOccurrence(data, -2000, 4);
            double[,] com = c.getCoOccurrence(1, 1);
            string msg = "";
            for (int rr = 0; rr < com.GetLength(0); rr++)
            {
                for (int cc = 0; cc < com.GetLength(1); cc++)
                {
                    msg += com[rr, cc] + "\t";
                }
                msg += "\r\n";
            }
            MessageBox.Show(msg);
        }

        /// <summary>
        /// SCRIPTED TASK: Extract info for nodules that meet the given criteria into a new XML file
        /// </summary>
        /// <param name="agreetype">Physician rating to agree on</param>
        /// <param name="numberOfPhysicians">Number of physicians that must agree on the rating of agreetype</param>
        /// <param name="minSize">Minimum nodule size (square pixels)</param>
        /// <param name="maxSize">Maximum nodule size (square pixels)</param>
        private static int extractNodules(string agreetype, int numberOfPhysicians, int minSize, int maxSize)
        {
            //string agreetype = "malignancy";
            LIDCNodule.SAVE_XML_PRIMARY = true;
            LIDCNodule.SAVE_XML_BOUNDS = true;
            LIDCNoduleDB myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml", false);
            LIDCNoduleDB newDB = new LIDCNoduleDB();

            foreach (LIDCNodule nodule in myDB)
            {
                int count = 1;
                foreach (LIDCNodule nodule2 in myDB)
                {
                    if (nodule.Slice == nodule2.Slice && 
                        nodule.Nodule_no == nodule2.Nodule_no &&
                        nodule.Annotations[agreetype] == nodule2.Annotations[agreetype] &&
                        nodule.NoduleID != nodule2.NoduleID)
                    {
                        count++;
                    }
                }
                // physician agreement
                if (count >= numberOfPhysicians)
                {
                    // size requirements
                    if (nodule.Width * nodule.Height >= minSize && 
                        nodule.Width * nodule.Height <= maxSize)
                    {
                        newDB.AddNodule(nodule);
                    }
                }
            }

            newDB.SaveToXML(Util.DATA_PATH + "nodules-primary-agree-" + agreetype + "-" + numberOfPhysicians + "-size-" + minSize + "-" + maxSize + ".xml");
            return newDB.TotalNoduleCount;
        }

        /// <summary>
        /// SCRIPTED TASK: Print some needed data to a text file for analysis in Excel.
        /// </summary>
        private static void printDataToText()
        {
            LIDCNoduleDB myDB = new LIDCNoduleDB(Util.DATA_PATH + Util.PRIMARY_XML);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-primary2.xml", false);
            StreamWriter fout = new StreamWriter(new FileStream(Util.DATA_PATH + "data.txt", FileMode.OpenOrCreate));
            foreach (LIDCNodule n in myDB)
                //fout.WriteLine(nod.Width + "\t" + nod.Height);
                //fout.WriteLine(n.SeriesInstanceUID + "\t" + n.Slice + "\t" + n.Nodule_no + "\t" + n.NoduleID + "\t" + n.Annotations["texture"]);
                fout.WriteLine(((int)(n.Width * n.Height)).ToString() + "\t" + ((double)(n.ActualWidth * n.ActualHeight)).ToString());
            fout.Close();
        }

        /// <summary>
        /// SCRIPTED TASK: Calculate the descriptors for all nodules
        /// </summary>
        /// <param name="descChoice">Determines which descriptor to use: (1) Haralick (2) Gabor (3) Markov</param>
        private static void calcDescriptors(int descChoice)
        {
            FeatureExtractor haralick = new GlobalCoOccurrence();
            FeatureExtractor gabor = new GaborFilter();
            FeatureExtractor markov = new MarkovRandom();

            // display splash screen
            Splash screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            // load nodules
            LIDCNoduleDB noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml", screen.Progress);
            screen.Progress.Maximum = noduleDB.TotalNoduleCount;
            screen.Progress.Value = 0;
            
            if (descChoice == 1)
            {
                // calculate Haralick
                screen.Status.Text = "Calculating Haralick data ...";
                screen.Refresh();
                foreach (LIDCNodule n in noduleDB)
                {
                    haralick.ExtractFeatures(n);
                    screen.Progress.Value++;
                }

                // save to XML
                LIDCNodule.SAVE_XML_PRIMARY = true;
                LIDCNodule.SAVE_XML_HARALICK = true;
                noduleDB.SaveToXML(Util.DATA_PATH + "nodules-newharalick.xml");

            }
            else if (descChoice == 2)
            {
                // calculate Gabor
                screen.Status.Text = "Calculating Gabor data ...";
                screen.Refresh();
                foreach (LIDCNodule n in noduleDB)
                {
                    gabor.ExtractFeatures(n);
                    screen.Progress.Value++;
                }

                // save to XML
                LIDCNodule.SAVE_XML_PRIMARY = true;
                LIDCNodule.SAVE_XML_GABOR = true;
                noduleDB.SaveToXML(Util.DATA_PATH + "nodules-newgabor.xml");

            }
            else if (descChoice == 3)
            {
                // calculate Markov
                screen.Status.Text = "Calculating Markov data ...";
                screen.Refresh();
                foreach (LIDCNodule n in noduleDB)
                {
                    markov.ExtractFeatures(n);
                    screen.Progress.Value++;
                }

                // save to XML
                LIDCNodule.SAVE_XML_PRIMARY = true;
                LIDCNodule.SAVE_XML_MARKOV = true;
                noduleDB.SaveToXML(Util.DATA_PATH + "nodules-newmarkov.xml");

            }
            screen.Close();

        }

        /// <summary>
        /// SCRIPTED TASK: Finds the mean/standard deviation of the Markov paramerter for all images
        /// </summary>
        private static void calcMarkovStats()
        {
            // display splash screen
            Splash screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            LIDCNoduleDB noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-main.xml", screen.Progress);
            screen.Status.Text = "Finding mean/std...";
            screen.Refresh();

            Util.FeatureMethod methodHandler = MarkovRandom.FindParameters;
            double[] meanStd = Util.FindHistMeanStd(noduleDB,screen.Progress,methodHandler);

            System.Windows.Forms.MessageBox.Show("Mean: " + meanStd[0] + "\nStd: " + meanStd[1]);
            
            screen.Close();
        }

        /// <summary>
        /// SCRIPTED TASK: Finds the mean/standard deviation of the Gabor response images for all images
        /// </summary>
        private static void calcGaborStats()
        {
            // display splash screen
            Splash screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            LIDCNoduleDB noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-main.xml", screen.Progress);
            screen.Status.Text = "Finding mean/std...";
            screen.Refresh();

            Util.FeatureMethod methodHandler = GaborFilter.ApplyFilter;
            double[] meanStd = Util.FindHistMeanStd(noduleDB, screen.Progress, methodHandler);

            System.Windows.Forms.MessageBox.Show("Mean: " + meanStd[0] + "\nStd: " + meanStd[1]);
            screen.Close();
        }

        /// <summary>
        /// SCRIPTED TASK: Generates all actual nodule sizes and saves to primary XML
        /// </summary>
        private static void calcAllActualSizes()
        {
            LIDCNodule.SAVE_XML_PRIMARY = true;
            LIDCNodule.SAVE_XML_BOUNDS = true;
            LIDCNoduleDB myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            foreach (LIDCNodule n in myDB)
                n.GetActualSize();
            myDB.SaveToXML(Util.DATA_PATH + "nodules-primary2.xml");
        }

        #endregion

        /// <summary>
        /// CODE FOR PAT - EXAMPLE OF EXTERNAL BRISC USAGE
        /// </summary>
        public static void testCode()
        {
            // this would typically be done at program startup
            // so that the database is already in memory and
            // ready to go when a query is executed
            Util.SetDataPath("C:\\LIDC\\");
            LIDCNoduleDB noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml");
            noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-gabor.xml");

            // number of images to return
            int k = 4;

            // dummy data
            int[,] rawdata = new int[,] {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
            };
            
            // create a new nodule object from the raw pixel data
            LIDCNodule nodule = new LIDCNodule(rawdata, 5, 10, 5, 10);

            // calculate features using Gabor filter
            (new GaborFilter()).ExtractFeatures(nodule);

            // run a query against the database
            LinkedList<LIDCNodule> results = noduleDB.RunQuery(nodule, k);

            // "results" now contains the k nearest nodules to the query nodule
            // sorted in ascending order of distance (first image is best match)
            foreach (LIDCNodule n in results)
                System.Windows.Forms.MessageBox.Show(n.ImageSOP_UID + "\r\n" + n.Temp_dist.ToString() + "\r\n" + n.OriginalDICOMFilename);
        }
    }
}
