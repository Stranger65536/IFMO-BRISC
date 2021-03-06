using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using BRISC.Core;

namespace BRISC.GUI
{
    public partial class NoduleViewer : Form
    {
        public NoduleViewer()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                calcUsingAnnotations = true;
            }
            else
            {
                calcUsingAnnotations = false;
            }
        }

        // main nodule data structure
        private LIDCNoduleDB noduleDB;

        // main feature vector description
        private LinkedList<string> currentFeatureVector;

        // currently selected nodules
        private LIDCNodule selectedNodule; // left view
        private LIDCNodule selectedNodule2; // right view

        // DICOM working data storage (for faster windowing)
        private int[,] nImage;
        private int[,] nFullImage;
        private int[,] nImage2;
        private int[,] nFullImage2;

        // sorts list views by columns
        private ListViewColumnSorter lvwColumnSorter;
        private ListViewColumnSorter lvwColumnSorter2;

        // thumbnail load flag (slower, but prettier)
        private bool loadThumbnails;

        // zoom image hover flags
        private bool needRefresh = true;
        private bool needRefresh2 = true;


        private MouseButtons currentMouseButton;


        private int oldX, oldY;

        private static readonly bool LOAD_XML_ANNOTATIONS = true;
        private static readonly bool LOAD_XML_HARALICK = true;
        private static readonly bool LOAD_XML_GABOR = true;
        private static readonly bool LOAD_XML_MARKOV = true;
        private static readonly bool LOAD_XML_POINTS = true;
        private static readonly bool LOAD_XML_LOCAL = true;
        private static bool calcUsingAnnotations;

        private void NoduleViewer_Load(object sender, EventArgs e)
        {
            var loadScreen = new Splash();
            loadScreen.Status.Text = "Initializing feature vector ...";
            loadScreen.Show();
            loadScreen.Refresh();

            // initialize feature vector
            currentFeatureVector = new LinkedList<string>();
            currentFeatureVector.AddLast("contrast");
            currentFeatureVector.AddLast("homogeneity");
            currentFeatureVector.AddLast("entropy");
            currentFeatureVector.AddLast("sumAverage");
            currentFeatureVector.AddLast("variance");
            currentFeatureVector.AddLast("maximumProbability");

            // prompt for thumbnail flag
            var r =
                MessageBox.Show("Do you want to load image thumbnails (this takes longer, but results in a prettier interface)?",
                    "LIDCViewer", MessageBoxButtons.YesNo);
            if (r.ToString().Equals("Yes"))
            {
                loadThumbnails = true;
                loadScreen.FourthCheck.Visible = true;
            }

            // load all nodule data
            loadData(loadScreen);

            // make sure the nodule database loaded
            if (noduleDB == null)
                return;

            // add nodule data to the main listview
            addAllNodulesToListView(noduleView);

            // UNCOMMENT TO ADD ALL NODULES TO SECOND LISTVIEW INITIALLY
            //addAllNodulesToListView(noduleView2);

            // create listview column sorter instances for listview controls
            lvwColumnSorter = new ListViewColumnSorter();
            lvwColumnSorter2 = new ListViewColumnSorter();
            noduleView.ListViewItemSorter = lvwColumnSorter;
            noduleView2.ListViewItemSorter = lvwColumnSorter2;

            // update threshold label
            threshold_Scroll(sender, e);

            // set "top N items" scroller maximum
            updTopItems.Maximum = noduleDB.TotalNoduleCount;

            // combo box defaults (make sure SOMETHING is selected)
            scoreBox.SelectedIndex = 0;
            comboGlobal.SelectedIndex = 0;
            comboGabor.SelectedIndex = 0;
            comboMarkov.SelectedIndex = 0;
            comboFeature.SelectedIndex = 0;

            // set initial window position/size options
            WindowState = FormWindowState.Maximized;
            panelLeft.Width = panelRight.Width = (ClientSize.Width - 475)/2;
            fullImage.Height = fullImage2.Height = fullImage.Width;
            Show();
            Refresh();
            noduleView.Focus();

            // get rid of splash screen
            loadScreen.Hide();
            loadScreen.Dispose();
        }

        private void loadData(Splash loadScreen)
        {
            if (!File.Exists(Util.DATA_PATH + "nodules-primary.xml"))
            {
                MessageBox.Show("Cannot find nodules-primary.xml in " + Util.DATA_PATH, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            // load primary nodule data
            loadScreen.FirstCheck.ForeColor = SystemColors.HighlightText;
            loadScreen.Status.Text = "Loading primary nodule data ...";
            loadScreen.Refresh();
            noduleDB = new LIDCNoduleDB(Util.DATA_PATH + Util.PRIMARY_XML, loadScreen.Progress);
            loadScreen.FirstCheck.Checked = true;
            loadScreen.FirstCheck.ForeColor = SystemColors.ControlText;
            loadScreen.Refresh();

            // load annotation data
            loadScreen.ThirdCheck.ForeColor = SystemColors.HighlightText;
            loadScreen.Status.Text = "Loading annotation data ...";
            loadScreen.Refresh();
            if (LOAD_XML_ANNOTATIONS)
                noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-annotations.xml", loadScreen.Progress, false);
            loadScreen.ThirdCheck.Checked = true;
            loadScreen.ThirdCheck.ForeColor = SystemColors.ControlText;
            loadScreen.Refresh();

            // load Global Co-occurrence data
            loadScreen.SecondCheck.ForeColor = SystemColors.HighlightText;
            loadScreen.Status.Text = "Loading Global Co-occurrence data ...";
            loadScreen.Refresh();
            if (LOAD_XML_HARALICK)
                noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-haralick.xml", loadScreen.Progress, false);
            loadScreen.Refresh();

            // load Local Co-occurrence data
            loadScreen.SecondCheck.ForeColor = SystemColors.HighlightText;
            loadScreen.Status.Text = "Loading Local Co-occurrence data ...";
            loadScreen.Refresh();
            if (LOAD_XML_LOCAL)
            {
                noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-local.xml", loadScreen.Progress, false);
            }
            loadScreen.Refresh();

            // load Gabor data
            loadScreen.Status.Text = "Loading Gabor data ...";
            loadScreen.Refresh();
            if (LOAD_XML_GABOR)
                noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-gabor.xml", loadScreen.Progress, false);

            // load Markov data
            loadScreen.Status.Text = "Loading Markov data ...";
            loadScreen.Refresh();
            if (LOAD_XML_MARKOV)
                noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-markov.xml", loadScreen.Progress, false);
            loadScreen.SecondCheck.Checked = true;
            loadScreen.SecondCheck.ForeColor = SystemColors.ControlText;

            // load point data
            if (LOAD_XML_POINTS)
            {
                loadScreen.Status.Text = "Loading Point data ...";
                loadScreen.Refresh();
                noduleDB.LoadFromXML(Util.DATA_PATH + "nodules-points.xml", loadScreen.Progress, false);
            }
            //Compute PCA for all features
            //loadScreen.Status.Text = "Computing PCA data ...";
            //loadScreen.Refresh();
            //noduleDB.doPCA(loadScreen.Progress);
            // load thumbnail data
            if (loadThumbnails)
            {
                var oldWindowing = Util.WINDOWING;
                Util.WINDOWING = false;
                loadScreen.FourthCheck.ForeColor = SystemColors.HighlightText;
                loadScreen.Status.Text = "Loading nodule thumbnails ...";
                loadScreen.Progress.Minimum = 0;
                loadScreen.Progress.Maximum = noduleDB.Nodules.Count;
                loadScreen.Progress.Value = 0;
                loadScreen.Refresh();
                foreach (var n in noduleDB.Nodules)
                {
                    try
                    {
                        //UnsafeBitmap ubmp = new UnsafeBitmap(Util.LoadDICOMBitmap(Util.NODULE_IMAGE_PATH + n.GetSegmentedDICOMFilename()), 16, 16);
                        var ubmp = new UnsafeBitmap(Util.ConvertPixelDataToBitmap(n.SegmentedPixelData));
                        noduleThumbs.Images.Add(n.NUID.ToString(), ubmp.Bitmap);

                        int malignancy = n.Annotations["malignancy"];
                        string outputFile = Directory.GetParent(n.OriginalDICOMFilename).FullName + @"\" + malignancy + " " + Guid.NewGuid() + @".bmp";
                        
                        // Copy into bitmap
                        Bitmap bitmap = Util.ConvertPixelDataToBitmap(n.SegmentedPixelData);
                        bitmap.Save(outputFile, ImageFormat.Bmp);

                        loadScreen.Progress.Value++;
                        loadScreen.Refresh();
                        Application.DoEvents();
                    }
                    catch (Exception)
                    {
                        /* ignore missing thumbnails */
                    }
                }
                loadScreen.FourthCheck.Checked = true;
                loadScreen.FourthCheck.ForeColor = SystemColors.ControlText;
                loadScreen.Refresh();
                Util.WINDOWING = oldWindowing;
            }
            Text += "    [" + noduleDB.TotalNoduleCount + " image(s) - " + noduleDB.UniqueNodulecount + " nodule(s)]";
        }

        private void addAllNodulesToListView(ListView lv)
        {
            foreach (var n in noduleDB)
            {
                var newlvi = lv.Items.Add(n.SeriesInstanceUID);
                newlvi.Tag = n.NUID;
                newlvi.ImageKey = n.NUID.ToString();
                newlvi.SubItems.Add(n.Slice);
                newlvi.SubItems.Add(n.Nodule_no);
                newlvi.SubItems.Add(n.NoduleID);
                //newlvi.SubItems.Add(n.ActualWidth.ToString());
                newlvi.SubItems.Add(n.Width.ToString());
                //newlvi.SubItems.Add(n.ActualHeight.ToString());
                newlvi.SubItems.Add(n.Height.ToString());
            }
        }

        private void noduleView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // find the nodule item in the main data structure
            selectedNodule = selectNodule(noduleView);

            // load image data
            if (selectedNodule != null)
            {
                //nImage = Util.LoadDICOMPixelData(Util.NODULE_IMAGE_PATH + selectedNodule.GetSegmentedDICOMFilename());
                nImage = selectedNodule.SegmentedPixelData;
                //nFullImage = Util.LoadDICOMPixelData(Util.ORIGINAL_IMAGES_PATH + selectedNodule.GetOriginalDICOMFilename());
                nFullImage = selectedNodule.OriginalPixelData;
            }

            // refresh all the information displays
            refreshImage();
            refreshInfo();
            refreshDistance();
        }

        private void noduleView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // find the nodule item in the main data structure
            selectedNodule2 = selectNodule(noduleView2);

            // load image data
            if (selectedNodule2 != null)
            {
                //nImage2 = Util.LoadDICOMPixelData(Util.NODULE_IMAGE_PATH + selectedNodule2.GetSegmentedDICOMFilename());
                nImage2 = selectedNodule2.SegmentedPixelData;
                //nFullImage2 = Util.LoadDICOMPixelData(Util.ORIGINAL_IMAGES_PATH + selectedNodule2.GetOriginalDICOMFilename());
                nFullImage2 = selectedNodule2.OriginalPixelData;
            }

            // refresh all the information displays
            refreshImage();
            refreshInfo();
            refreshDistance();
        }

        private void noduleView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            noduleView.Sorting = SortOrder.None;
            noduleView.ListViewItemSorter = lvwColumnSorter;

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // first column is string data, others are numeric
            if (e.Column > 0)
                lvwColumnSorter.SortType = ListViewColumnSorter.SortStyle.Integer;
            else
                lvwColumnSorter.SortType = ListViewColumnSorter.SortStyle.String;

            // Perform the sort with these new sort options.
            noduleView.Sort();
        }

        private void noduleView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            noduleView2.Sorting = SortOrder.None;
            noduleView2.ListViewItemSorter = lvwColumnSorter2;

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter2.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter2.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter2.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter2.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter2.SortColumn = e.Column;
                lvwColumnSorter2.Order = SortOrder.Ascending;
            }

            // fourth column is floating-point, first column is string, others are integer
            if (e.Column == 4)
                lvwColumnSorter2.SortType = ListViewColumnSorter.SortStyle.Float;
            else if (e.Column > 0)
                lvwColumnSorter2.SortType = ListViewColumnSorter.SortStyle.Integer;
            else
                lvwColumnSorter2.SortType = ListViewColumnSorter.SortStyle.String;

            // Perform the sort with these new sort options.
            noduleView2.Sort();
        }

        private void noduleView_DoubleClick(object sender, EventArgs e)
        {
            runQuery();
        }

        private void noduleView2_DoubleClick(object sender, EventArgs e)
        {
            // find double-clicked nodule in the first list box and run a query on it
            if (noduleView2.SelectedItems.Count == 1)
            {
                foreach (ListViewItem i in noduleView.Items)
                {
                    i.Selected = false;
                    if (i.Tag.ToString().Equals(noduleView2.SelectedItems[0].Tag.ToString()))
                    {
                        i.Selected = true;
                        noduleView.EnsureVisible(i.Index);
                        noduleView.Select();
                    }
                }
            }
            runQuery();
        }

        private void comboGlobal_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboGlobal.Text = "Global Co-occurrence";
            refreshDistance();
        }

        private void comboGabor_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboFeature.Text = "Gabor";
            refreshDistance();
        }

        private void comboMarkov_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboFeature.Text = "Markov";
            refreshDistance();
        }

        private void comboLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboFeature.Text = "Local Co-occurrence";
            refreshDistance();
        }

        private void comboFeature_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshDistance();
        }

        private void rankByDistance_Click(object sender, EventArgs e)
        {
            runQuery();
        }

        private void chooseFeatures_Click(object sender, EventArgs e)
        {
            // initialize dialog to match feature vector
            var vdlg = new FeatureVectorDialog();
            var n = currentFeatureVector.First;
            while (n != null)
            {
                foreach (ListViewItem i in vdlg.FeatureView.Items)
                {
                    if (n.Value.Equals(i.Tag))
                    {
                        i.Checked = true;
                        break;
                    }
                }
                n = n.Next;
            }

            // show the window modally (wait for it to close before continuing)
            vdlg.ShowDialog(this);

            // reset feature vector to match dialog contents
            currentFeatureVector.Clear();
            foreach (ListViewItem i in vdlg.FeatureView.Items)
            {
                if (i.Checked)
                {
                    currentFeatureVector.AddLast(i.Tag.ToString());
                }
            }
            refreshInfo();
            refreshDistance();
        }

        private void analyze_Click(object sender, EventArgs e)
        {
            calcMeanPrecisionRecall();
        }

        private void fullImage_MouseDown(object sender, MouseEventArgs e)
        {
            currentMouseButton = e.Button;
            oldX = e.X;
            oldY = e.Y;
        }

        private void fullImage_MouseUp(object sender, MouseEventArgs e)
        {
            currentMouseButton = MouseButtons.None;
        }

        private void fullImage_MouseMove(object sender, MouseEventArgs e)
        {
            // mouse location change
            var dx = e.X - oldX;
            var dy = e.Y - oldY;

            if (currentMouseButton == MouseButtons.Left)
            {
                // left mouse button -> adjust intensity windowing
                var newLower = Util.WINDOW_LOWER + dx*20;
                var newUpper = Util.WINDOW_UPPER + -dy*20;

                // enforce valid windowing options
                if (newLower > newUpper)
                {
                    // if lower > higher, swap them
                    var temp = newLower;
                    newLower = newUpper;
                    newUpper = temp;
                }
                newLower = Math.Min(newLower, 2000); // no lower than -2000
                newUpper = Math.Min(newUpper, 2000);
                newLower = Math.Max(newLower, -2000); // no higher than -2000
                newUpper = Math.Max(newUpper, -2000);
                updLower.Value = newLower;
                updUpper.Value = newUpper;
            }

            oldX = e.X;
            oldY = e.Y;
        }

        private void fullImage2_MouseDown(object sender, MouseEventArgs e)
        {
            // windowing options are shared by both sides of the screen, so
            // there's no need for duplicate windowing code
            fullImage_MouseDown(sender, e);
        }

        private void fullImage2_MouseUp(object sender, MouseEventArgs e)
        {
            fullImage_MouseUp(sender, e);
        }

        private void fullImage2_MouseMove(object sender, MouseEventArgs e)
        {
            fullImage_MouseMove(sender, e);
        }

        private void splitLeftPanel_SplitterMoved(object sender, SplitterEventArgs e)
        {
            refreshImage();
        }

        private void splitRightPanel_SplitterMoved(object sender, SplitterEventArgs e)
        {
            refreshImage();
        }

        private void threshold_Scroll(object sender, EventArgs e)
        {
            if (threshold.Value == threshold.Maximum)
                checkThreshold.Text = "Threshold: (all)";
            else
                checkThreshold.Text = "Threshold: " + (Math.Pow(threshold.Value, 2.0)/300).ToString("0.###");
        }

        private void noduleThumb_MouseEnter(object sender, EventArgs e)
        {
            // show full image on thumb mouseover
            if (selectedNodule != null)
            {
                noduleImage.Visible = true;
                if (needRefresh)
                {
                    noduleImage.Refresh();
                    needRefresh = false;
                }
            }
        }

        private void noduleImage_MouseLeave(object sender, EventArgs e)
        {
            // hide full image (back to thumb)
            noduleImage.Visible = false;
            needRefresh = true;
        }

        private void noduleThumb2_MouseEnter(object sender, EventArgs e)
        {
            // show full image on thumb mouseover
            if (selectedNodule2 != null)
            {
                noduleImage2.Visible = true;
                if (needRefresh2)
                {
                    noduleImage2.Refresh();
                    needRefresh2 = false;
                }
            }
        }

        private void noduleImage2_MouseLeave(object sender, EventArgs e)
        {
            // hide full image (back to thumb)
            noduleImage2.Visible = false;
            needRefresh2 = true;
        }

        private void checkWindow_CheckedChanged(object sender, EventArgs e)
        {
            // toggle windowing
            Util.WINDOWING = checkWindow.Checked;
            barLower.Enabled = checkWindow.Checked;
            barUpper.Enabled = checkWindow.Checked;
            Util.WINDOW_LOWER = (int) updLower.Value;
            Util.WINDOW_UPPER = (int) updUpper.Value;
            refreshImage();
        }

        private void updLower_ValueChanged(object sender, EventArgs e)
        {
            if (updUpper.Value < updLower.Value)
                updUpper.Value = updLower.Value + 50;
            if (checkWindow.Checked)
            {
                Util.WINDOW_LOWER = (int) updLower.Value;
                Util.WINDOW_UPPER = (int) updUpper.Value;
                refreshImage();
            }
        }

        private void updUpper_ValueChanged(object sender, EventArgs e)
        {
            if (updLower.Value > updUpper.Value)
                updLower.Value = updUpper.Value;
            if (checkWindow.Checked)
            {
                Util.WINDOW_LOWER = (int) updLower.Value;
                Util.WINDOW_UPPER = (int) updUpper.Value;
                refreshImage();
            }
        }

        private void barLower_Scroll(object sender, EventArgs e)
        {
            updLower.Value = barLower.Value;
            barUpper.Minimum = barLower.Value;
        }

        private void barUpper_Scroll(object sender, EventArgs e)
        {
            updUpper.Value = barUpper.Value;
            barLower.Maximum = barUpper.Value;
        }

        private void splitLeftForm_SplitterMoved(object sender, SplitterEventArgs e)
        {
            fullImage.Height = panelLeft.Width;
            refreshImage();
        }

        private void splitRightForm_SplitterMoved(object sender, SplitterEventArgs e)
        {
            fullImage2.Height = panelRight.Width;
            refreshImage();
        }

        private void NoduleViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void NoduleViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private LIDCNodule selectNodule(ListView lv)
        {
            // find the nodule item in the main data structure
            LIDCNodule node = null;
            foreach (var n in noduleDB.Nodules)
            {
                foreach (ListViewItem i in lv.SelectedItems)
                {
                    if (n.NUID.Equals(i.Tag))
                    {
                        node = n;
                        break;
                    }
                }
            }
            return node;
        }

        private void refreshImage()
        {
            if (selectedNodule != null)
            {
                // zoomed-in image
                try
                {
                    noduleImage.Image = Util.ConvertPixelDataToBitmap(nImage);
                    noduleThumb.Visible = true;
                }
                catch (Exception)
                {
                    noduleThumb.Visible = false;
                }

                // thumbnail
                if (noduleImage.Image != null)
                {
                    noduleThumb.Image = noduleImage.Image;
                    noduleThumb.Size = noduleImage.Image.Size;
                    resizePictureBox(noduleImage);
                }

                // full image
                try
                {
                    fullImage.Image = Util.ConvertPixelDataToBitmap(nFullImage);
                    fullImage.Visible = true;
                }
                catch (Exception)
                {
                    fullImage.Visible = false;
                }
            }
            if (selectedNodule2 != null)
            {
                // zoomed-in image
                try
                {
                    noduleImage2.Image = Util.ConvertPixelDataToBitmap(nImage2);
                    noduleThumb2.Visible = true;
                }
                catch (Exception)
                {
                    noduleThumb2.Visible = false;
                }

                // thumbnail
                if (noduleImage2.Image != null)
                {
                    noduleThumb2.Image = noduleImage2.Image;
                    noduleThumb2.Size = noduleImage2.Image.Size;
                    resizePictureBox(noduleImage2);
                }

                // full image
                try
                {
                    fullImage2.Image = Util.ConvertPixelDataToBitmap(nFullImage2);
                    fullImage2.Visible = true;
                }
                catch (Exception)
                {
                    fullImage2.Visible = false;
                }
            }
        }

        private void resizePictureBox(PictureBox box)
        {
            // scale image, preserving size ratio
            var height = box.Image.Height;
            var width = box.Image.Width;
            var maxSize = Math.Min(Math.Min(panelLeft.Width, panelLeft.Height - fullImage.Height - noduleInfo.Height),
                Math.Min(panelRight.Width, panelRight.Height - fullImage2.Height - noduleInfo2.Height));
            if (height > width)
            {
                width = width*(maxSize/height);
                height = maxSize;
            }
            else
            {
                height = height*(maxSize/width);
                width = maxSize;
            }
            var ubmp = new UnsafeBitmap((Bitmap) box.Image, width, height);
            box.Image = ubmp.Bitmap;
            box.Size = new Size(box.Image.Width, box.Image.Height);
        }

        private void fullImage_Paint(object sender, PaintEventArgs e)
        {
            if (selectedNodule != null)
            {
                // find coordinates
                var x = (int) (selectedNodule.MinX/512.0*fullImage.Width);
                var y = (int) (selectedNodule.MinY/512.0*fullImage.Height);
                var width = (int) ((selectedNodule.MaxX - selectedNodule.MinX)/512.0*fullImage.Width);
                var height = (int) ((selectedNodule.MaxY - selectedNodule.MinY)/512.0*fullImage.Height);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(200, 0, 255, 0), (float) 1.0),
                    new Rectangle(x - 1, y - 1, width + 2, height + 2));
            }
        }

        private void fullImage2_Paint(object sender, PaintEventArgs e)
        {
            if (selectedNodule2 != null)
            {
                // find coordinates
                var x = (int) (selectedNodule2.MinX/512.0*fullImage2.Width);
                var y = (int) (selectedNodule2.MinY/512.0*fullImage2.Height);
                var width = (int) ((selectedNodule2.MaxX - selectedNodule2.MinX)/512.0*fullImage2.Width);
                var height = (int) ((selectedNodule2.MaxY - selectedNodule2.MinY)/512.0*fullImage2.Height);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(200, 0, 255, 0), (float) 1.0),
                    new Rectangle(x - 1, y - 1, width + 2, height + 2));
            }
        }

        private void refreshInfo()
        {
            if (selectedNodule != null)
            {
                noduleInfo.Text = buildInfoString(selectedNodule);
            }
            if (selectedNodule2 != null)
            {
                noduleInfo2.Text = buildInfoString(selectedNodule2);
            }
        }

        private string buildInfoString(LIDCNodule currentNodule)
        {
            var info = "";

            info += "    " + currentNodule.SeriesInstanceUID + "\r\n";
            info += formatInfo("Slice #", currentNodule.Slice);
            info += formatInfo("Nodule #", currentNodule.Nodule_no);
            info += formatInfo("NoduleID", currentNodule.NoduleID);

            /*
            string stdfmt = "0.####";
            string lrgfmt = "0.000e0";

            info += "\r\n\r\n        -- HARALICK DESCRIPTORS --";
             descriptors
            if (currentNodule.Haralick.Count > 0)
            {
                info += formatFeature("contrast", "Contrast", currentNodule.Haralick["contrast"], lrgfmt);
                info += formatFeature("correlation", "Correlation", currentNodule.Haralick["correlation"], stdfmt);
                info += formatFeature("energy", "Energy", currentNodule.Haralick["energy"], stdfmt);
                info += formatFeature("homogeneity", "Homogeneity", currentNodule.Haralick["homogeneity"], stdfmt);
                info += formatFeature("entropy", "Entropy", currentNodule.Haralick["entropy"], stdfmt);
                info += formatFeature("thirdOrderMoment", "3rd Order Moment", currentNodule.Haralick["thirdOrderMoment"], lrgfmt);
                info += formatFeature("inverseVariance", "Inverse Variance", currentNodule.Haralick["inverseVariance"], stdfmt);
                info += formatFeature("sumAverage", "Sum Average", currentNodule.Haralick["sumAverage"], stdfmt);
                info += formatFeature("variance", "Variance", currentNodule.Haralick["variance"], lrgfmt);
                info += formatFeature("clusterTendency", "Cluster Tendency", currentNodule.Haralick["clusterTendency"], lrgfmt);
                info += formatFeature("maximumProbability", "Maximum Probability", currentNodule.Haralick["maximumProbability"], stdfmt);
            }
            info += formatInfo("Has Gabor features", (currentNodule.GaborHist == null ? "no" : "yes"));
            
            */

            info += "\r\n\r\n        -- PHYSICIAN ANNOTATIONS --";
            info += formatAnnotation("Calcification", currentNodule.Annotations["calcification"]);
            info += formatAnnotation("Internal Structure", currentNodule.Annotations["internalStructure"]);
            info += formatAnnotation("Lobulation", currentNodule.Annotations["lobulation"]);
            info += formatAnnotation("Malignancy", currentNodule.Annotations["malignancy"]);
            info += formatAnnotation("Margin", currentNodule.Annotations["margin"]);
            info += formatAnnotation("Sphericity", currentNodule.Annotations["sphericity"]);
            info += formatAnnotation("Spiculation", currentNodule.Annotations["spiculation"]);
            info += formatAnnotation("Subtlety", currentNodule.Annotations["subtlety"]);
            info += formatAnnotation("Texture", currentNodule.Annotations["texture"]);


            return info;
        }

        private string formatFeature(string feature, string label, double data, string format)
        {
            var padlength = 24;
            return "\r\n    " + ((currentFeatureVector.Contains(feature) ? "[*] " : "") + label + ":").PadRight(padlength) +
                   data.ToString(format);
        }

        private string formatAnnotation(string label, int data)
        {
            var padlength = 24;
            var bar = '◊'; // '#';
            return "\r\n    " + (label + ":").PadRight(padlength) + new string(bar, data);
        }

        private string formatInfo(string label, string data)
        {
            var padlength = 24;
            return "\r\n    " + (label + ":").PadRight(padlength) + data;
        }

        private void refreshDistance()
        {
            // enable/disable "choose feature vector" button based on whether Haralick is selected
            chooseFeatures.Visible = comboFeature.Text == "Global";
            comboGlobal.Visible = comboFeature.Text == "Global";
            comboGabor.Visible = comboFeature.Text == "Gabor";
            comboMarkov.Visible = comboFeature.Text == "Markov";
            comboLocal.Visible = comboFeature.Text == "Local";
        }

        private string getFeatureString()
        {
            var features = "";
            if (comboFeature.Text == "Global")
            {
                features = "Global";
                foreach (var s in currentFeatureVector)
                    features += s + " ";
            }
            else if (comboFeature.Text == "Annotations")
                features = "Annotations";
            else if (comboFeature.Text == "All-Features-w/-PCA")
            {
                features = "All-Features-w/-PCA";
                foreach (var s in currentFeatureVector)
                    features += s + " ";
            }
            else if (comboFeature.Text == "Gabor")
                features = "Gabor ";
            else if (comboFeature.Text == "Markov")
                features = "Markov ";
            else if (comboFeature.Text == "Local")
            {
                features = "Local";
                foreach (var s in currentFeatureVector)
                    features += s + " ";
            }
            else if (comboFeature.Text == "All-Features")
            {
                features = "All-Features";
                foreach (var s in currentFeatureVector)
                    features += s + " ";
            }
            return features;
        }

        private string getSimilarityString()
        {
            var similarity = "";
            if (comboFeature.Text == "Global")
                similarity = comboGlobal.Text;
            else if (comboFeature.Text == "Gabor")
                similarity = comboGabor.Text;
            else if (comboFeature.Text == "Markov")
                similarity = comboMarkov.Text;
            else if (comboFeature.Text == "Local")
                similarity = comboLocal.Text;
            else if (comboFeature.Text == "All-Features")
                similarity = "";
            return similarity;
        }

        private int getNumItems()
        {
            var numItems = (int) updTopItems.Value;
            if (!checkTopItems.Checked)
                numItems = noduleDB.TotalNoduleCount;
            return numItems;
        }

        private double getThreshold()
        {
            var threshVal = Math.Pow(threshold.Value, 2.0)/100.0;
            if (!checkThreshold.Checked)
                threshVal = threshVal = double.PositiveInfinity;
            return threshVal;
        }

        private void runQuery()
        {
            rankByDistance.Enabled = false;
            rankByDistance.Text = "Querying ...";
            noduleView2.Items.Clear();
            noduleView2.Sorting = SortOrder.None;
            noduleView2.ListViewItemSorter = null;
            precision.Text = "";
            recall.Text = "";
            distance.Text = "";
            lblResults.Text = "";
            selectedNodule2 = null;
            noduleInfo2.Text = "";
            noduleImage2.Image = null;
            noduleThumb2.Image = null;
            fullImage2.Image = null;
            distance.Text = "";
            refreshImage();
            Refresh();

            if (selectedNodule != null)
            {
                // determine query parameters
                var features = getFeatureString();
                var similarity = getSimilarityString();
                var nItems = getNumItems();
                var thresh = getThreshold();

                LinkedList<LIDCNodule> rnodes = null;
                try
                {
                    // run query
                    rnodes = noduleDB.RunQuery(selectedNodule, features, similarity, nItems, thresh);
                    if (rnodes != null)
                    {
                        // add nodes to list view from the linked list
                        foreach (var nn in rnodes)
                        {
                            var nlvi = noduleView2.Items.Add(nn.SeriesInstanceUID);
                            nlvi.ImageKey = nn.NUID.ToString();
                            nlvi.Tag = nn.NUID;
                            nlvi.SubItems.Add(nn.Slice);
                            nlvi.SubItems.Add(nn.Nodule_no);
                            nlvi.SubItems.Add(nn.NoduleID);
                            nlvi.SubItems.Add(nn.Temp_dist.ToString("0.0000"));
                            nlvi.SubItems.Add(nn.Temp_adist.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                lblResults.Text = noduleView2.Items.Count + " result(s)";

                if (rnodes != null)
                {
                    var stats = noduleDB.CalcPrecisionAndRecall(selectedNodule, rnodes);
                    precision.Text = stats[0].ToString();
                    recall.Text = stats[1].ToString("0.0000");
                }
            }
            rankByDistance.Enabled = true;
            rankByDistance.Text = "Run &Query";
            noduleView.Focus();
            Refresh();
        }

        private void calcMeanPrecisionRecall()
        {
            var meanPR = new double[2];
            meanPR[0] = double.NaN;
            meanPR[1] = double.NaN;

            var meanPR1 = new double[2];
            meanPR1[0] = double.NaN;
            meanPR1[1] = double.NaN;
            var meanPR2 = new double[2];
            meanPR2[0] = double.NaN;
            meanPR2[1] = double.NaN;


            var previousCaption = analyze.Text;
            analyze.Enabled = false;
            rankByDistance.Enabled = false;
            progressAnalyze.Visible = true;
            precision.Text = "";
            recall.Text = "";

            // determine query parameters
            var features = getFeatureString();
            var similarity = getSimilarityString();
            var nItems = getNumItems();
            var thresh = getThreshold();

            try
            {
                /*if (features.StartsWith("All-Features-w/-PCA"))
                {
                    noduleDB.pcaNum = Convert.ToInt32(textBox1.Text);
                }*/
                // run mean precision/recall analyzation
                if (calcUsingAnnotations)
                {
                    meanPR = noduleDB.CalcMeanPrecisionUsingAnnotations(features, similarity, nItems, thresh, progressAnalyze);
                }
                else
                {
                    meanPR = noduleDB.CalcMeanPrecisionAndRecall(features, similarity, nItems, thresh, progressAnalyze);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            analyze.Text = previousCaption;
            analyze.Enabled = true;
            rankByDistance.Enabled = true;
            progressAnalyze.Visible = false;
            precision.Text = meanPR[0].ToString("0.0000");
            recall.Text = meanPR[1].ToString("0.0000");
            selectedNodule = null;
            refreshImage();
        }
    }
}