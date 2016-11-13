using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using BRISC.Core;

namespace BRISC.GUI
{
    /// <summary>
    /// Simple DICOM series viewer for LIDC lung CT database
    /// </summary>
    /// <remarks>
    /// Instructions:
    /// <list type="unordered">
    /// <item>Hold LEFT mouse button and drag mouse to adjust intensity windowing</item>
    /// <item>Hold RIGHT mouse button and drag mouse to navigate within series</item>
    /// <item>Hold MIDDLE mouse button and drag mouse to pan image</item>
    /// <item>Scroll MOUSE WHEEL to zoom in/out</item>
    /// </list>
    /// </remarks>
    public partial class SeriesViewer : Form
    {
        /// <summary>
        /// Main image data -- array of matrices
        /// </summary>
        private int[][,] imageData;

        /// <summary>
        /// Stores the actual CT slice z-positions
        /// </summary>
        private double[] slicePositions;

        /// <summary>
        /// Currently viewed series
        /// </summary>
        private string currentSeries;

        /// <summary>
        /// Currently viewed slice
        /// </summary>
        private int currentImage;

        /// <summary>
        /// Images load in this background thread
        /// </summary>
        private Thread loadingThread;

        /// <summary>
        /// Number of images loaded so far (used for cross-thread communication)
        /// </summary>
        private int imagesLoaded;

        /// <summary>
        /// Mouse event flag (are any buttons currently pressed?)
        /// </summary>
        private MouseButtons currentMouseButton;

        /// <summary>
        /// Mouse event location (last known mouse location)
        /// </summary>
        private int oldX, oldY;

        /// <summary>
        /// Initialize new series viewer window
        /// </summary>
        public SeriesViewer()
        {
            InitializeComponent();
        }

        #region Form event handlers
        private void SeriesViewer_Load(object sender, EventArgs e)
        {
            // set up mousewheel handler (it's not in the form designer for some reason)
            picImage.MouseWheel += new MouseEventHandler(picImage_MouseWheel);

            // reset pan and zoom
            SeriesViewer_Resize(sender, e);

            // set default windowing options
            Util.WINDOWING = true;
            Util.WINDOW_LOWER = 0;
            Util.WINDOW_UPPER = 800;

            // load the list of series
            loadSeriesList();

            // initialize mouse event flag
            currentMouseButton = MouseButtons.None;
        }
        private void SeriesViewer_Resize(object sender, EventArgs e)
        {
            // reset pan and zoom (set image to fullscreen -- aspect ratio adjusted)
            int minDim = Math.Min(ClientSize.Width, ClientSize.Height);
            picImage.Size = new Size(minDim, minDim);
            picImage.Location = new Point((ClientSize.Width - minDim) / 2, (ClientSize.Height - minDim) / 2);
            refreshImage();
        }
        private void SeriesViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            // if we're still loading a series, abort
            if (loadingThread != null && loadingThread.ThreadState == ThreadState.Running)
                loadingThread.Abort();

            // just to make sure we really exit
            Application.Exit();
        }
        #endregion

        #region Image event handlers
        private void picImage_MouseDown(object sender, MouseEventArgs e)
        {
            currentMouseButton = e.Button;
            oldX = e.X;
            oldY = e.Y;
        }
        private void picImage_MouseUp(object sender, MouseEventArgs e)
        {
            currentMouseButton = MouseButtons.None;
        }
        private void picImage_MouseMove(object sender, MouseEventArgs e)
        {
            // mouse location change
            int dx = e.X - oldX;
            int dy = e.Y - oldY;

            if (currentMouseButton == MouseButtons.Left)
            {
                // left mouse button -> adjust intensity windowing
                Util.WINDOW_LOWER += dx * 20;
                Util.WINDOW_UPPER += -dy * 20;

                // enforce valid windowing options
                if (Util.WINDOW_LOWER < -2000)
                    Util.WINDOW_LOWER = -2000;  // no lower than -2000
                if (Util.WINDOW_UPPER > 2000)
                    Util.WINDOW_UPPER = 2000;   // no higher than -2000
                if (Util.WINDOW_LOWER > Util.WINDOW_UPPER)
                {
                    // if lower > higher, swap them
                    int temp = Util.WINDOW_LOWER;
                    Util.WINDOW_LOWER = Util.WINDOW_UPPER;
                    Util.WINDOW_UPPER = temp;
                }
                lblWindow.Text = "Intensities: [" + Util.WINDOW_LOWER.ToString() + " - " + Util.WINDOW_UPPER.ToString() + "]";
                refreshImage();
            }
            else if (currentMouseButton == MouseButtons.Right)
            {
                // right mouse button -> move within series
                if (dx < 0 && currentImage > 0)
                    currentImage--;
                else if (dx > 0 && currentImage < imagesLoaded-1)
                    currentImage++;
                lblCurrentImage.Text = "Z-pos: " + slicePositions[currentImage].ToString() + 
                    " (" + (currentImage+1).ToString() + "/" + imagesLoaded.ToString() + ")";
                refreshImage();
            }
            else if (currentMouseButton == MouseButtons.Middle)
            {
                // middle mouse button -> pan image
                if (dx != 0 || dy != 0)
                    picImage.Location = new Point(picImage.Left + dx, picImage.Top + dy);
            }
            oldX = e.X;
            oldY = e.Y;
        }
        private void picImage_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                // zoom in
                int oldWidth = picImage.Width;
                int oldHeight = picImage.Height;
                picImage.Size = new Size(oldWidth * 2, oldHeight * 2);
                picImage.Location = new Point(picImage.Left - oldWidth / 2, picImage.Top - oldHeight / 2);

                /* CENTER ZOOM ON MOUSE CURSOR -- HARDER THAN IT SEEMS???
                int oldMouseX = e.X - picImage.Left;
                int oldMouseY = e.Y - picImage.Top;
                int newMouseX = oldMouseX * 2;
                int newMouseY = oldMouseY * 2;
                int newLeft = newMouseX
                picImage.Location = new Point(ClientSize.Width / 2 - picImage.Width / 2 + (oldWidth - e.X),
                    ClientSize.Height / 2 - picImage.Height / 2 - e.Y);*/
            }
            else if (e.Delta < 0)
            {
                // zoom out
                int oldWidth = picImage.Width;
                int oldHeight = picImage.Height;
                picImage.Size = new Size(oldWidth / 2, oldHeight / 2);
                picImage.Location = new Point(picImage.Left + picImage.Width / 2, picImage.Top + picImage.Height / 2);
            }
        }
        #endregion

        #region Misc event handlers
        private void comboSeries_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if we're still loading a series, abort
            if (loadingThread != null && loadingThread.ThreadState == ThreadState.Running)
            {
                loadingThread.Abort();
                imageData = null;
                slicePositions = null;
            }

            // start loading the new series data in a new background thread
            currentSeries = comboSeries.Text;
            loadingThread = new Thread(new ThreadStart(loadSeriesData));
            loadingThread.Priority = ThreadPriority.Lowest;
            loadingThread.Start();
            currentImage = 0;

            // reset view (pan and zoom)
            SeriesViewer_Resize(sender, e);

            // set focus to image (so mousewheel doesn't scroll combo box)
            picImage.Focus();
        }
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            // update info labels
            lblWindow.Text = "Intensities: [" + Util.WINDOW_LOWER.ToString() + " - " + Util.WINDOW_UPPER.ToString() + "]";
            lblCurrentImage.Text = "Z-pos: " + ((slicePositions != null) ? slicePositions[currentImage].ToString() : "0") +
                " (" + ((slicePositions != null) ? (currentImage+1).ToString() : "0") + "/" + imagesLoaded.ToString() + ")";;
            lblImagesLoaded.Text = imagesLoaded.ToString() + " image(s) loaded";
        }
        private void lblHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // show instructions in a message box
            string msg = "";
            msg += "LIDC Series Viewer\r\n";
            msg += "Copyright (C) 2006 DePaul University\r\n\r\n";
            msg += "This program comes with ABSOLUTELY NO WARRANTY. This is free\r\n";
            msg += "software, and you are welcome to redistribute it under certain\r\n";
            msg += "conditions; see GPL.txt that came with this program for details.\r\n\r\n";
            msg += "Select a series from the drop-down menu.\r\n";
            msg += "  - Hold LEFT mouse button and drag mouse to adjust intensity windowing\r\n";
            msg += "  - Hold RIGHT mouse button and drag mouse to navigate within series\r\n";
            msg += "  - Hold MIDDLE mouse button and drag mouse to pan image\r\n";
            msg += "  - Scroll MOUSE WHEEL to zoom in/out";
            MessageBox.Show(msg, "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        /// <summary>
        /// Gets a list of CT series, based on the folder names, and adds them to the combo box
        /// </summary>
        private void loadSeriesList()
        {
            if (!Directory.Exists(Util.DATA_PATH))
            {
                MessageBox.Show("Folder does not exist: " + Util.DATA_PATH, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // get a list of series, sorted by last number
            DirectoryInfo mainDir = new DirectoryInfo(Util.DATA_PATH);
            FileInfo[] seriesFiles = mainDir.GetFiles("*.srs");
            SortedList<int, string> sortedDirs = new SortedList<int, string>();
            char[] splitChars = {'.'};
            try
            {
                foreach (FileInfo series in seriesFiles)
                {
                    string[] fnp = series.Name.Split(splitChars);
                    sortedDirs.Add(int.Parse(fnp[fnp.Length - 2]), series.Name.Substring(0, series.Name.LastIndexOf(".")));
                }
            }
            catch (Exception) { }

            // add series to combobox
            comboSeries.Items.Clear();
            comboSeries.Items.Add("(none)");
            foreach (string series in sortedDirs.Values)
                    comboSeries.Items.Add(series);

            // don't load a series to begin with
            loadingThread = null;
            currentImage = 0;
            comboSeries.SelectedIndex = 0;
            currentSeries = "(none)";
            refreshImage();
            picImage.Focus();
        }

        /// <summary>
        /// Loads all image data for the current series (this is called in a background thread)
        /// </summary>
        private void loadSeriesData()
        {
            // reset # of images
            imagesLoaded = 0;

            // don't load the "none" series
            if (currentSeries.Equals("(none)"))
                return;

            // get a list of images and sort by slice position
            TextReader fin = new StreamReader(Util.DATA_PATH + "\\" + currentSeries + ".srs");
            SortedList<double, string> sortedFiles = new SortedList<double, string>();
            string line;
            char[] splitChars = {'\t'};
            while ((line = fin.ReadLine()) != null)
            {
                string[] tokens = line.Split(splitChars);
                sortedFiles.Add(double.Parse(tokens[0]), tokens[1]);
            }
            fin.Close();

            // allocate memory for image data array and slice numbers
            imageData = new int[sortedFiles.Count][,];
            slicePositions = new double[sortedFiles.Count];

            // load each image
            foreach (double slice in sortedFiles.Keys)
            {
                slicePositions[imagesLoaded] = slice;
                imageData[imagesLoaded++] = Util.LoadDICOMPixelData(sortedFiles[slice]);
                if (imagesLoaded == 2)
                    refreshImage();
            }
        }

        /// <summary>
        /// Redraw the current image from stored CT data
        /// </summary>
        private void refreshImage()
        {
            // make sure we're supposed to actually draw an image
            if (imageData != null && currentImage < imagesLoaded && imageData[currentImage] != null)
                picImage.Image = Util.ConvertPixelDataToBitmap(imageData[currentImage]);
            else
                picImage.Image = null;
        }
    }
}