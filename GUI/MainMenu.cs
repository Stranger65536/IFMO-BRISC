using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using BRISC.Core;

namespace BRISC.GUI
{
    /// <summary>
    /// Simple menu to choose between the nodule viewer and the series viewer
    /// </summary>
    public partial class MainMenu : Form
    {
        /// <summary>
        /// Initialize new menu form
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
        }

        private void noduleViewer_Click(object sender, EventArgs e)
        {
            this.Hide();
            NoduleViewer nv = new NoduleViewer();
            nv.Show();
        }

        private void seriesViewer_Click(object sender, EventArgs e)
        {
            this.Hide();
            //FolderBrowserDialog dlg = new FolderBrowserDialog();
            //if (dlg.ShowDialog(this) == DialogResult.OK)
            //{
            //    Util.ORIGINAL_IMAGES_PATH = Util.DATA_PATH;
                SeriesViewer sv = new SeriesViewer();
                sv.Show();
            //}
        }

        private void runTask_Click(object sender, EventArgs e)
        {
            noduleViewer.Enabled = false;
            comboPrimary.Enabled = false;
            seriesViewer.Enabled = false;
            runTask.Enabled = false;
            Refresh();


            //Program.doTask();


            
            refreshCombo();
            noduleViewer.Enabled = false;
            comboPrimary.Enabled = false;
            seriesViewer.Enabled = false;
            runTask.Enabled = false;
            foreach (string xmlfile in comboPrimary.Items)
            {
                Util.PRIMARY_XML = xmlfile;
                Program.TestPrecisionRecall("trials-nitems.txt", "nitems");
            }
            Program.ExtractAllXMLs();
            refreshCombo();
            foreach (string xmlfile in comboPrimary.Items)
            {
                Util.PRIMARY_XML = xmlfile;
                Program.TestPrecisionRecall("trials.txt", "agree");
            }
            combineResultsFiles();
            
            refreshCombo();
            
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            refreshCombo();
        }

        private void combineResultsFiles()
        {
            string data = "";
            foreach (FileInfo fi in (new DirectoryInfo(Util.DATA_PATH)).GetFiles("counts.txt"))
            {
                StreamReader fin = fi.OpenText();
                data += fin.ReadToEnd() + "\n\n";
                fin.Close();
            } 
            foreach (FileInfo fi in (new DirectoryInfo(Util.DATA_PATH)).GetFiles("results-*.txt"))
            {
                StreamReader fin = fi.OpenText();
                data += "----------------------------------------------------\n";
                data += fi.Name + "\n";
                data += fin.ReadToEnd() + "\n\n";
                fin.Close();
            }
            StreamWriter fout = new StreamWriter(new FileStream(Util.DATA_PATH + "results-combined.txt", FileMode.OpenOrCreate));
            fout.Write(data);
            fout.Close();
        }

        private void refreshCombo()
        {
            try
            {
                comboPrimary.Items.Clear();
                DirectoryInfo di = new DirectoryInfo(Util.DATA_PATH);
                FileInfo[] files = di.GetFiles("nodules-primary*.xml");
                foreach (FileInfo fi in files)
                    comboPrimary.Items.Add(fi.Name);
            }
            catch (Exception) { }
            comboPrimary.Enabled = true;
            if (comboPrimary.Items.Count > 0)
            {
                comboPrimary.Text = Util.PRIMARY_XML;
                noduleViewer.Enabled = true;
                seriesViewer.Enabled = true;
                runTask.Enabled = true;
            }
            else
            {
                comboPrimary.Text = "";
                Util.PRIMARY_XML = "";
                noduleViewer.Enabled = false;
                seriesViewer.Enabled = false;
                runTask.Enabled = false;
            }
        }

        private void MainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void comboPrimary_SelectedIndexChanged(object sender, EventArgs e)
        {
            Util.PRIMARY_XML = comboPrimary.Text;
            noduleViewer.Enabled = true;
            runTask.Enabled = true;
        }

        private void import_Click(object sender, EventArgs e)
        {
            import.Enabled = false;
            noduleViewer.Enabled = false;
            comboPrimary.Enabled = false;
            seriesViewer.Enabled = false;
            runTask.Enabled = false;
            Refresh();
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                if (!File.Exists(dlg.SelectedPath + "\\dicom-elements-2004.dic"))
                {
                    MessageBox.Show("This directory does not contain a DICOM dictionary. You can download one from:\r\n\r\nhttp://svn.sourceforge.net/viewvc/*checkout*/brisc/main/data/dicom-elements-2004.dic", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Util.SetDataPath(dlg.SelectedPath);
                    Util.SaveDataPath();
                    LIDCImport.ImportAllStudies(Util.DATA_PATH); // "C:\\LIDC");
                }
            }
            import.Enabled = true;
            comboPrimary.Enabled = true;
            refreshCombo();
        }
    }
}