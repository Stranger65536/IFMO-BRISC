#define NORMAL_GUI_EXECUTION

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using BRISC.Core;

namespace BRISC.GUI
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Util.LoadDataPath();

#if NORMAL_GUI_EXECUTION

            // SHOW MAIN MENU
            var mm = new MainMenu();
            mm.Show();
            Application.Run();
#else
    // PERFORM SCRIPTED TASK
            doTask();
#endif
        }


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


        public static void testCode()
        {
            // this would typically be done at program startup
            // so that the database is already in memory and
            // ready to go when a query is executed
            Util.SetDataPath("C:\\LIDC\\");
            var noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml");
            noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-gabor.xml");

            // number of images to return
            var k = 4;

            // dummy data
            int[,] rawdata =
            {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            };

            // create a new nodule object from the raw pixel data
            var nodule = new LIDCNodule(rawdata, 5, 10, 5, 10);

            // calculate features using Gabor filter
            new GaborFilter().ExtractFeatures(nodule);

            // run a query against the database
            var results = noduleDB.RunQuery(nodule, k);

            // "results" now contains the k nearest nodules to the query nodule
            // sorted in ascending order of distance (first image is best match)
            foreach (var n in results)
                MessageBox.Show(n.ImageSOP_UID + "\r\n" + n.Temp_dist + "\r\n" + n.OriginalDICOMFilename);
        }

        public static void TestPrecisionRecall(string trialsFilename, string resultsPrefix)
        {
            //string trialsFilename = "trials.txt";
            var resultsFilename = "results-" + resultsPrefix + "-" + Util.PRIMARY_XML.Replace(".xml", "") + ".txt";
            var results = new double[2];
            string features;
            string similarity;
            int nItems;
            double thresh;

            // display splash screen
            var screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            // load data
            var myDB = new LIDCNoduleDB(Util.DATA_PATH + Util.PRIMARY_XML, screen.Progress);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-haralick.xml", screen.Progress, false);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-gabor.xml", screen.Progress, false);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-markov.xml", screen.Progress, false);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml", screen.Progress, false);

            // constant parameter(s)
            thresh = double.PositiveInfinity;

            // find out which trial to start at
            var fin = new StreamReader(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.OpenOrCreate));
            var startTrialNum = 1; //1;
            string line;
            while ((line = fin.ReadLine()) != null)
                startTrialNum = int.Parse(line.Substring(0, line.IndexOf('\t'))) + 1;
            fin.Close();

            // get list of trials to run
            var fin2 = new StreamReader(new FileStream(Util.DATA_PATH + trialsFilename, FileMode.OpenOrCreate));
            var trial = 0;
            while ((line = fin2.ReadLine()) != null)
            {
                var tokens = line.Split('\t');

                if (++trial >= startTrialNum)
                {
                    // set parameters
                    features = tokens[0];
                    similarity = tokens[1];
                    nItems = int.Parse(tokens[2]);

                    // update status
                    screen.Status.Text = trial + ": " + similarity + "  [nitems=" + nItems + "]  <" + features + ">";
                    screen.Refresh();

                    // do precision/recall calculation
                    results = myDB.CalcMeanPrecisionAndRecall(features, similarity, nItems, thresh, screen.Progress);

                    // output results
                    var fout = new StreamWriter(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.Append));
                    fout.Write(trial + "\t" + features + "\t" + similarity + "\t" + nItems + "\t");
                    fout.WriteLine(thresh + "\t" + results[0].ToString("0.0000") + "\t" + results[1].ToString("0.0000"));
                    fout.Close();
                }
            }
            fin2.Close();

            screen.Close();
        }


        public static void ExtractAllXMLs()
        {
            // display splash screen
            var screen = new Splash();
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

            var myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            var counts = "total: " + myDB.TotalNoduleCount + "\r\n";

            for (var p = 2; p <= 4; p++)
            {
                counts += "texture-" + p + ": " + extractNodules("texture", p, 25, 16000) + "\r\n";
                screen.Progress.Value++;
            }
            for (var p = 2; p <= 4; p++)
            {
                counts += "malignancy-" + p + ": " + extractNodules("malignancy", p, 25, 16000) + "\r\n";
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

            counts += "25-104: " + extractNodules("texture", 1, 25, 104) + "\r\n";
            screen.Progress.Value++;
            counts += "105-234: " + extractNodules("texture", 1, 105, 234) + "\r\n";
            screen.Progress.Value++;
            counts += "235-625: " + extractNodules("texture", 1, 235, 625) + "\r\n";
            screen.Progress.Value++;
            counts += "626-16000: " + extractNodules("texture", 1, 626, 16000) + "\r\n";
            screen.Progress.Value++;

            var fout = new StreamWriter(new FileStream(Util.DATA_PATH + "counts.txt", FileMode.OpenOrCreate));
            fout.WriteLine(counts);
            fout.Close();
            //MessageBox.Show(counts);

            screen.Close();
        }


        private static void createMainXMLFile()
        {
            LIDCNodule.SAVE_XML_ANNOTATIONS = true;
            LIDCNodule.SAVE_XML_GABOR = true;
            LIDCNodule.SAVE_XML_HARALICK = true;
            LIDCNodule.SAVE_XML_MARKOV = true;
            LIDCNodule.SAVE_XML_PRIMARY = true;
            LIDCNodule.SAVE_XML_BOUNDS = true;
            var myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-haralick.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-gabor.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-markov.xml");
            myDB.SaveToXML(Util.DATA_PATH + "nodules-main.xml");
        }


        private static void testHaralickPrecisionRecall()
        {
            var resultsFilename = "haralick-results.txt";
            var results = new double[2];
            string features;
            string similarity;
            int nItems;
            double thresh;

            // display splash screen
            var screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            // load data
            var myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-main.xml", screen.Progress);

            // constant parameter(s)
            //similarity = "Euclidean";
            //nItems = 5;
            thresh = double.PositiveInfinity;

            // find out which trial to start at
            var fin = new StreamReader(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.OpenOrCreate));
            var startTrialNum = 1; //1;
            string line;
            while ((line = fin.ReadLine()) != null)
                startTrialNum = int.Parse(line.Substring(0, line.IndexOf('\t'))) + 1;
            fin.Close();

            // get list of trials to run
            var allTrials = new ArrayList();
            var fin2 = new StreamReader(new FileStream(Util.DATA_PATH + "besttrials.txt", FileMode.OpenOrCreate));
            while ((line = fin2.ReadLine()) != null)
                allTrials.Add(int.Parse(line));
            fin2.Close();

            // different similarity measures
            var trial = 0;
            foreach (var s in new[] {"Euclidean", "Manhattan", "Chebychev"})
            {
                // different image features
                //for (int ff = 1; ff < 0x800; ff++)      // all combinations of features
                /*foreach (string f in new string[] { 
                "correlation entropy", 
                "contrast correlation homogeneity energy", 
                "correlation clusterTendency", 
                "correlation entropy clusterTendency" })*/
                for (var t = 0; t < allTrials.Count; t++)
                {
                    // form feature string
                    var ff = (int) allTrials[t];
                    var f = "";
                    for (var j = 0; j < 12; j++)
                        if ((ff & (1 << j)) > 0)
                            f += GlobalCoOccurrence.featureNames[j] + " ";
                    f = f.Substring(0, f.Length - 1);

                    // different numbers of items
                    int[] itemNums = {1, 2, 3, 5, 10};
                    for (var n = 0; n < itemNums.Length; n++)
                    {
                        if (++trial >= startTrialNum)
                        {
                            //int minSize = 0;

                            // set parameters
                            features = "Haralick " + f;
                            similarity = s;
                            nItems = itemNums[n];

                            // update status
                            screen.Status.Text = trial + ": " + similarity + "  [nitems=" + nItems + "]  <" + features + ">";
                            screen.Refresh();

                            // do precision/recall calculation
                            //results = myDB.CalcMeanPrecisionAndRecall(features, similarity, nItems, thresh, minSize, screen.Progress);

                            // output results
                            var fout = new StreamWriter(new FileStream(Util.DATA_PATH + resultsFilename, FileMode.Append));
                            fout.Write(trial + "\t" + ff + "\t" + features + "\t" + similarity + "\t" + nItems + "\t");
                            fout.WriteLine(thresh + "\t" + results[0].ToString("0.0000") + "\t" + results[1].ToString("0.0000"));
                            fout.Close();
                        }
                    }
                }
            }

            screen.Close();
        }


        private static void testCoOccurrence()
        {
            var data = new int[5, 5]
            {
                {-2000, 0, 1, 2, 1},
                {0, 2, 2, 0, 2},
                {1, 1, 1, 2, 1},
                {0, 2, 0, 1, 0},
                {0, 1, 2, 2, -2000}
            };
            var g = new GlobalCoOccurrence();
            var c = g.PerformCoOccurrence(data, -2000, 4);
            var com = c.getCoOccurrence(1, 1);
            var msg = "";
            for (var rr = 0; rr < com.GetLength(0); rr++)
            {
                for (var cc = 0; cc < com.GetLength(1); cc++)
                {
                    msg += com[rr, cc] + "\t";
                }
                msg += "\r\n";
            }
            MessageBox.Show(msg);
        }


        private static int extractNodules(string agreetype, int numberOfPhysicians, int minSize, int maxSize)
        {
            //string agreetype = "malignancy";
            LIDCNodule.SAVE_XML_PRIMARY = true;
            LIDCNodule.SAVE_XML_BOUNDS = true;
            var myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml", false);
            var newDB = new LIDCNoduleDB();

            foreach (var nodule in myDB)
            {
                var count = 1;
                foreach (var nodule2 in myDB)
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
                    if (nodule.Width*nodule.Height >= minSize &&
                        nodule.Width*nodule.Height <= maxSize)
                    {
                        newDB.AddNodule(nodule);
                    }
                }
            }

            newDB.SaveToXML(Util.DATA_PATH + "nodules-primary-agree-" + agreetype + "-" + numberOfPhysicians + "-size-" + minSize + "-" +
                            maxSize + ".xml");
            return newDB.TotalNoduleCount;
        }


        private static void printDataToText()
        {
            var myDB = new LIDCNoduleDB(Util.DATA_PATH + Util.PRIMARY_XML);
            myDB.LoadFromXML(Util.DATA_PATH + "nodules-primary2.xml", false);
            var fout = new StreamWriter(new FileStream(Util.DATA_PATH + "data.txt", FileMode.OpenOrCreate));
            foreach (var n in myDB)
                //fout.WriteLine(nod.Width + "\t" + nod.Height);
                //fout.WriteLine(n.SeriesInstanceUID + "\t" + n.Slice + "\t" + n.Nodule_no + "\t" + n.NoduleID + "\t" + n.Annotations["texture"]);
                fout.WriteLine(n.Width*n.Height + "\t" + n.ActualWidth*n.ActualHeight);
            fout.Close();
        }


        private static void calcDescriptors(int descChoice)
        {
            FeatureExtractor haralick = new GlobalCoOccurrence();
            FeatureExtractor gabor = new GaborFilter();
            FeatureExtractor markov = new MarkovRandom();

            // display splash screen
            var screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            // load nodules
            var noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml", screen.Progress);
            screen.Progress.Maximum = noduleDB.TotalNoduleCount;
            screen.Progress.Value = 0;

            if (descChoice == 1)
            {
                // calculate Haralick
                screen.Status.Text = "Calculating Haralick data ...";
                screen.Refresh();
                foreach (var n in noduleDB)
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
                foreach (var n in noduleDB)
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
                foreach (var n in noduleDB)
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


        private static void calcMarkovStats()
        {
            // display splash screen
            var screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            var noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-main.xml", screen.Progress);
            screen.Status.Text = "Finding mean/std...";
            screen.Refresh();

            Util.FeatureMethod methodHandler = MarkovRandom.FindParameters;
            var meanStd = Util.FindHistMeanStd(noduleDB, screen.Progress, methodHandler);

            MessageBox.Show("Mean: " + meanStd[0] + "\nStd: " + meanStd[1]);

            screen.Close();
        }


        private static void calcGaborStats()
        {
            // display splash screen
            var screen = new Splash();
            screen.Status.Text = "Loading nodule data ...";
            screen.FirstCheck.Visible = false;
            screen.SecondCheck.Visible = false;
            screen.ThirdCheck.Visible = false;
            screen.FourthCheck.Visible = false;
            screen.Show();
            screen.Refresh();

            var noduleDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-main.xml", screen.Progress);
            screen.Status.Text = "Finding mean/std...";
            screen.Refresh();

            Util.FeatureMethod methodHandler = GaborFilter.ApplyFilter;
            var meanStd = Util.FindHistMeanStd(noduleDB, screen.Progress, methodHandler);

            MessageBox.Show("Mean: " + meanStd[0] + "\nStd: " + meanStd[1]);
            screen.Close();
        }


        private static void calcAllActualSizes()
        {
            LIDCNodule.SAVE_XML_PRIMARY = true;
            LIDCNodule.SAVE_XML_BOUNDS = true;
            var myDB = new LIDCNoduleDB(Util.DATA_PATH + "nodules-primary.xml");
            foreach (var n in myDB)
                n.GetActualSize();
            myDB.SaveToXML(Util.DATA_PATH + "nodules-primary2.xml");
        }
    }
}