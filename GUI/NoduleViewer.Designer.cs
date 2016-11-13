namespace BRISC.GUI
{
    partial class NoduleViewer
    {
        
        
        
        private System.ComponentModel.IContainer components = null;

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoduleViewer));
            this.noduleImage = new System.Windows.Forms.PictureBox();
            this.noduleInfo = new System.Windows.Forms.TextBox();
            this.noduleThumb = new System.Windows.Forms.PictureBox();
            this.noduleThumbs = new System.Windows.Forms.ImageList(this.components);
            this.noduleView2 = new System.Windows.Forms.ListView();
            this.Filename = new System.Windows.Forms.ColumnHeader();
            this.Slice = new System.Windows.Forms.ColumnHeader();
            this.NodeID = new System.Windows.Forms.ColumnHeader();
            this.SessionID = new System.Windows.Forms.ColumnHeader();
            this.Dist = new System.Windows.Forms.ColumnHeader();
            this.Score = new System.Windows.Forms.ColumnHeader();
            this.noduleThumb2 = new System.Windows.Forms.PictureBox();
            this.noduleInfo2 = new System.Windows.Forms.TextBox();
            this.noduleImage2 = new System.Windows.Forms.PictureBox();
            this.rankByDistance = new System.Windows.Forms.Button();
            this.noduleView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.threshold = new System.Windows.Forms.TrackBar();
            this.scoreBox = new System.Windows.Forms.ComboBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.fullImage = new System.Windows.Forms.PictureBox();
            this.fullImage2 = new System.Windows.Forms.PictureBox();
            this.distance = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chooseFeatures = new System.Windows.Forms.Button();
            this.updTopItems = new System.Windows.Forms.NumericUpDown();
            this.labelRecall = new System.Windows.Forms.Label();
            this.precision = new System.Windows.Forms.TextBox();
            this.recall = new System.Windows.Forms.TextBox();
            this.checkThreshold = new System.Windows.Forms.CheckBox();
            this.checkTopItems = new System.Windows.Forms.CheckBox();
            this.analyze = new System.Windows.Forms.Button();
            this.comboMarkov = new System.Windows.Forms.ComboBox();
            this.comboGabor = new System.Windows.Forms.ComboBox();
            this.comboGlobal = new System.Windows.Forms.ComboBox();
            this.checkWindow = new System.Windows.Forms.CheckBox();
            this.updLower = new System.Windows.Forms.NumericUpDown();
            this.updUpper = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.barLower = new System.Windows.Forms.TrackBar();
            this.barUpper = new System.Windows.Forms.TrackBar();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelControls = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboLocal = new System.Windows.Forms.ComboBox();
            this.comboFeature = new System.Windows.Forms.ComboBox();
            this.progressAnalyze = new System.Windows.Forms.ProgressBar();
            this.labelPrecision = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelFill = new System.Windows.Forms.TableLayoutPanel();
            this.panelCenterControls = new System.Windows.Forms.TableLayoutPanel();
            this.splitLeftForm = new System.Windows.Forms.Splitter();
            this.splitRightForm = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.noduleImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.noduleThumb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.noduleThumb2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.noduleImage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullImage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updTopItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updLower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updUpper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barLower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barUpper)).BeginInit();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelFill.SuspendLayout();
            this.panelCenterControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // noduleImage
            // 
            this.noduleImage.BackColor = System.Drawing.Color.Black;
            this.noduleImage.Location = new System.Drawing.Point(0, 0);
            this.noduleImage.Name = "noduleImage";
            this.noduleImage.Size = new System.Drawing.Size(150, 108);
            this.noduleImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.noduleImage.TabIndex = 1;
            this.noduleImage.TabStop = false;
            this.noduleImage.Visible = false;
            this.noduleImage.MouseLeave += new System.EventHandler(this.noduleImage_MouseLeave);
            // 
            // noduleInfo
            // 
            this.noduleInfo.AcceptsReturn = true;
            this.noduleInfo.AcceptsTab = true;
            this.noduleInfo.BackColor = System.Drawing.Color.Black;
            this.noduleInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.noduleInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.noduleInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noduleInfo.ForeColor = System.Drawing.Color.Silver;
            this.noduleInfo.Location = new System.Drawing.Point(0, 284);
            this.noduleInfo.Multiline = true;
            this.noduleInfo.Name = "noduleInfo";
            this.noduleInfo.ReadOnly = true;
            this.noduleInfo.Size = new System.Drawing.Size(249, 219);
            this.noduleInfo.TabIndex = 2;
            // 
            // noduleThumb
            // 
            this.noduleThumb.Dock = System.Windows.Forms.DockStyle.Top;
            this.noduleThumb.Location = new System.Drawing.Point(0, 0);
            this.noduleThumb.Name = "noduleThumb";
            this.noduleThumb.Size = new System.Drawing.Size(249, 42);
            this.noduleThumb.TabIndex = 3;
            this.noduleThumb.TabStop = false;
            this.noduleThumb.MouseEnter += new System.EventHandler(this.noduleThumb_MouseEnter);
            // 
            // noduleThumbs
            // 
            this.noduleThumbs.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.noduleThumbs.ImageSize = new System.Drawing.Size(16, 16);
            this.noduleThumbs.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // noduleView2
            // 
            this.noduleView2.BackColor = System.Drawing.Color.Black;
            this.noduleView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.noduleView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Filename,
            this.Slice,
            this.NodeID,
            this.SessionID,
            this.Dist,
            this.Score});
            this.noduleView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noduleView2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noduleView2.ForeColor = System.Drawing.Color.Silver;
            this.noduleView2.FullRowSelect = true;
            this.noduleView2.HideSelection = false;
            this.noduleView2.LargeImageList = this.noduleThumbs;
            this.noduleView2.Location = new System.Drawing.Point(3, 557);
            this.noduleView2.MultiSelect = false;
            this.noduleView2.Name = "noduleView2";
            this.noduleView2.Size = new System.Drawing.Size(592, 162);
            this.noduleView2.SmallImageList = this.noduleThumbs;
            this.noduleView2.TabIndex = 5;
            this.noduleView2.TileSize = new System.Drawing.Size(380, 26);
            this.noduleView2.UseCompatibleStateImageBehavior = false;
            this.noduleView2.View = System.Windows.Forms.View.Details;
            this.noduleView2.DoubleClick += new System.EventHandler(this.noduleView2_DoubleClick);
            this.noduleView2.SelectedIndexChanged += new System.EventHandler(this.noduleView2_SelectedIndexChanged);
            this.noduleView2.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.noduleView2_ColumnClick);
            // 
            // Filename
            // 
            this.Filename.Tag = "filename";
            this.Filename.Text = "SeriesID";
            this.Filename.Width = 187;
            // 
            // Slice
            // 
            this.Slice.Tag = "slice";
            this.Slice.Text = "Slice";
            this.Slice.Width = 40;
            // 
            // NodeID
            // 
            this.NodeID.Tag = "node";
            this.NodeID.Text = "No";
            this.NodeID.Width = 35;
            // 
            // SessionID
            // 
            this.SessionID.Tag = "session";
            this.SessionID.Text = "NoduleID";
            this.SessionID.Width = 66;
            // 
            // Dist
            // 
            this.Dist.Tag = "dist";
            this.Dist.Text = "Distance";
            this.Dist.Width = 66;
            // 
            // Score
            // 
            this.Score.Tag = "score";
            this.Score.Text = "Score";
            this.Score.Width = 48;
            // 
            // noduleThumb2
            // 
            this.noduleThumb2.Dock = System.Windows.Forms.DockStyle.Top;
            this.noduleThumb2.Location = new System.Drawing.Point(0, 0);
            this.noduleThumb2.Name = "noduleThumb2";
            this.noduleThumb2.Size = new System.Drawing.Size(255, 42);
            this.noduleThumb2.TabIndex = 8;
            this.noduleThumb2.TabStop = false;
            this.noduleThumb2.MouseEnter += new System.EventHandler(this.noduleThumb2_MouseEnter);
            // 
            // noduleInfo2
            // 
            this.noduleInfo2.AcceptsReturn = true;
            this.noduleInfo2.AcceptsTab = true;
            this.noduleInfo2.BackColor = System.Drawing.Color.Black;
            this.noduleInfo2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.noduleInfo2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.noduleInfo2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noduleInfo2.ForeColor = System.Drawing.Color.Silver;
            this.noduleInfo2.Location = new System.Drawing.Point(0, 284);
            this.noduleInfo2.Multiline = true;
            this.noduleInfo2.Name = "noduleInfo2";
            this.noduleInfo2.ReadOnly = true;
            this.noduleInfo2.Size = new System.Drawing.Size(255, 219);
            this.noduleInfo2.TabIndex = 7;
            // 
            // noduleImage2
            // 
            this.noduleImage2.BackColor = System.Drawing.Color.Black;
            this.noduleImage2.Location = new System.Drawing.Point(0, 0);
            this.noduleImage2.Name = "noduleImage2";
            this.noduleImage2.Size = new System.Drawing.Size(161, 110);
            this.noduleImage2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.noduleImage2.TabIndex = 6;
            this.noduleImage2.TabStop = false;
            this.noduleImage2.Visible = false;
            this.noduleImage2.MouseLeave += new System.EventHandler(this.noduleImage2_MouseLeave);
            // 
            // rankByDistance
            // 
            this.rankByDistance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rankByDistance.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rankByDistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rankByDistance.ForeColor = System.Drawing.Color.Silver;
            this.rankByDistance.Location = new System.Drawing.Point(319, 4);
            this.rankByDistance.Name = "rankByDistance";
            this.rankByDistance.Size = new System.Drawing.Size(121, 32);
            this.rankByDistance.TabIndex = 0;
            this.rankByDistance.Text = "Run &Query";
            this.rankByDistance.UseVisualStyleBackColor = false;
            this.rankByDistance.Click += new System.EventHandler(this.rankByDistance_Click);
            // 
            // noduleView
            // 
            this.noduleView.BackColor = System.Drawing.Color.Black;
            this.noduleView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.noduleView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.noduleView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noduleView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noduleView.ForeColor = System.Drawing.Color.Silver;
            this.noduleView.FullRowSelect = true;
            this.noduleView.HideSelection = false;
            this.noduleView.LargeImageList = this.noduleThumbs;
            this.noduleView.Location = new System.Drawing.Point(3, 3);
            this.noduleView.MultiSelect = false;
            this.noduleView.Name = "noduleView";
            this.noduleView.Size = new System.Drawing.Size(592, 388);
            this.noduleView.SmallImageList = this.noduleThumbs;
            this.noduleView.TabIndex = 1;
            this.noduleView.TileSize = new System.Drawing.Size(380, 26);
            this.noduleView.UseCompatibleStateImageBehavior = false;
            this.noduleView.View = System.Windows.Forms.View.Details;
            this.noduleView.DoubleClick += new System.EventHandler(this.noduleView_DoubleClick);
            this.noduleView.SelectedIndexChanged += new System.EventHandler(this.noduleView_SelectedIndexChanged);
            this.noduleView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.noduleView_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "filename";
            this.columnHeader1.Text = "SeriesID";
            this.columnHeader1.Width = 187;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "slice";
            this.columnHeader2.Text = "Slice";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Tag = "node";
            this.columnHeader3.Text = "No";
            this.columnHeader3.Width = 34;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Tag = "session";
            this.columnHeader4.Text = "NoduleID";
            this.columnHeader4.Width = 62;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Width";
            this.columnHeader5.Width = 42;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Height";
            this.columnHeader6.Width = 48;
            // 
            // threshold
            // 
            this.threshold.LargeChange = 2;
            this.threshold.Location = new System.Drawing.Point(204, 56);
            this.threshold.Maximum = 20;
            this.threshold.Name = "threshold";
            this.threshold.Size = new System.Drawing.Size(92, 45);
            this.threshold.TabIndex = 16;
            this.threshold.TickFrequency = 4;
            this.threshold.TickStyle = System.Windows.Forms.TickStyle.None;
            this.threshold.Value = 5;
            this.threshold.Visible = false;
            this.threshold.Scroll += new System.EventHandler(this.threshold_Scroll);
            // 
            // scoreBox
            // 
            this.scoreBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scoreBox.FormattingEnabled = true;
            this.scoreBox.Items.AddRange(new object[] {
            "(all)",
            "Calcification",
            "Internal Structure",
            "Lobulation",
            "Malignancy",
            "Margin",
            "Sphericity",
            "Spiculation",
            "Subtlety",
            "Texture"});
            this.scoreBox.Location = new System.Drawing.Point(149, 155);
            this.scoreBox.Name = "scoreBox";
            this.scoreBox.Size = new System.Drawing.Size(117, 21);
            this.scoreBox.TabIndex = 19;
            this.scoreBox.Visible = false;
            // 
            // lblResults
            // 
            this.lblResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResults.ForeColor = System.Drawing.Color.Silver;
            this.lblResults.Location = new System.Drawing.Point(316, 45);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(124, 13);
            this.lblResults.TabIndex = 21;
            this.lblResults.Text = "0 result(s)";
            this.lblResults.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fullImage
            // 
            this.fullImage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fullImage.Location = new System.Drawing.Point(0, 503);
            this.fullImage.Name = "fullImage";
            this.fullImage.Size = new System.Drawing.Size(249, 240);
            this.fullImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.fullImage.TabIndex = 22;
            this.fullImage.TabStop = false;
            this.fullImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fullImage_MouseDown);
            this.fullImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.fullImage_MouseMove);
            this.fullImage.Paint += new System.Windows.Forms.PaintEventHandler(this.fullImage_Paint);
            this.fullImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fullImage_MouseUp);
            // 
            // fullImage2
            // 
            this.fullImage2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fullImage2.Location = new System.Drawing.Point(0, 503);
            this.fullImage2.Name = "fullImage2";
            this.fullImage2.Size = new System.Drawing.Size(255, 240);
            this.fullImage2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.fullImage2.TabIndex = 23;
            this.fullImage2.TabStop = false;
            this.fullImage2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fullImage2_MouseDown);
            this.fullImage2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.fullImage2_MouseMove);
            this.fullImage2.Paint += new System.Windows.Forms.PaintEventHandler(this.fullImage2_Paint);
            this.fullImage2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fullImage2_MouseUp);
            // 
            // distance
            // 
            this.distance.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.distance.Location = new System.Drawing.Point(255, 143);
            this.distance.Name = "distance";
            this.distance.ReadOnly = true;
            this.distance.Size = new System.Drawing.Size(65, 20);
            this.distance.TabIndex = 10;
            this.distance.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(189, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Distance:";
            this.label1.Visible = false;
            // 
            // chooseFeatures
            // 
            this.chooseFeatures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chooseFeatures.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chooseFeatures.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chooseFeatures.ForeColor = System.Drawing.Color.Silver;
            this.chooseFeatures.Location = new System.Drawing.Point(3, 114);
            this.chooseFeatures.Name = "chooseFeatures";
            this.chooseFeatures.Size = new System.Drawing.Size(125, 23);
            this.chooseFeatures.TabIndex = 5;
            this.chooseFeatures.Text = "Choose &Vector ...";
            this.chooseFeatures.UseVisualStyleBackColor = false;
            this.chooseFeatures.Click += new System.EventHandler(this.chooseFeatures_Click);
            // 
            // updTopItems
            // 
            this.updTopItems.BackColor = System.Drawing.Color.Black;
            this.updTopItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.updTopItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updTopItems.ForeColor = System.Drawing.Color.Silver;
            this.updTopItems.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.updTopItems.Location = new System.Drawing.Point(199, 13);
            this.updTopItems.Maximum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            this.updTopItems.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updTopItems.Name = "updTopItems";
            this.updTopItems.Size = new System.Drawing.Size(55, 16);
            this.updTopItems.TabIndex = 26;
            this.updTopItems.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updTopItems.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // labelRecall
            // 
            this.labelRecall.AutoSize = true;
            this.labelRecall.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRecall.ForeColor = System.Drawing.Color.Silver;
            this.labelRecall.Location = new System.Drawing.Point(316, 76);
            this.labelRecall.Name = "labelRecall";
            this.labelRecall.Size = new System.Drawing.Size(40, 13);
            this.labelRecall.TabIndex = 31;
            this.labelRecall.Text = "Recall:";
            // 
            // precision
            // 
            this.precision.BackColor = System.Drawing.Color.Black;
            this.precision.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.precision.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.precision.ForeColor = System.Drawing.Color.Silver;
            this.precision.Location = new System.Drawing.Point(375, 58);
            this.precision.Name = "precision";
            this.precision.ReadOnly = true;
            this.precision.Size = new System.Drawing.Size(63, 20);
            this.precision.TabIndex = 32;
            this.precision.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // recall
            // 
            this.recall.BackColor = System.Drawing.Color.Black;
            this.recall.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.recall.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recall.ForeColor = System.Drawing.Color.Silver;
            this.recall.Location = new System.Drawing.Point(375, 76);
            this.recall.Name = "recall";
            this.recall.ReadOnly = true;
            this.recall.Size = new System.Drawing.Size(65, 20);
            this.recall.TabIndex = 33;
            this.recall.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkThreshold
            // 
            this.checkThreshold.AutoSize = true;
            this.checkThreshold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkThreshold.ForeColor = System.Drawing.Color.Silver;
            this.checkThreshold.Location = new System.Drawing.Point(149, 41);
            this.checkThreshold.Name = "checkThreshold";
            this.checkThreshold.Size = new System.Drawing.Size(85, 17);
            this.checkThreshold.TabIndex = 34;
            this.checkThreshold.Text = "Threshold: 5";
            this.checkThreshold.UseVisualStyleBackColor = true;
            this.checkThreshold.Visible = false;
            // 
            // checkTopItems
            // 
            this.checkTopItems.AutoSize = true;
            this.checkTopItems.Checked = true;
            this.checkTopItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTopItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkTopItems.ForeColor = System.Drawing.Color.Silver;
            this.checkTopItems.Location = new System.Drawing.Point(149, 13);
            this.checkTopItems.Name = "checkTopItems";
            this.checkTopItems.Size = new System.Drawing.Size(147, 17);
            this.checkTopItems.TabIndex = 35;
            this.checkTopItems.Text = "Top                          items";
            this.checkTopItems.UseVisualStyleBackColor = true;
            // 
            // analyze
            // 
            this.analyze.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.analyze.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.analyze.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.analyze.ForeColor = System.Drawing.Color.Silver;
            this.analyze.Location = new System.Drawing.Point(322, 113);
            this.analyze.Name = "analyze";
            this.analyze.Size = new System.Drawing.Size(92, 25);
            this.analyze.TabIndex = 37;
            this.analyze.Text = "&Analyze";
            this.analyze.UseVisualStyleBackColor = false;
            this.analyze.Click += new System.EventHandler(this.analyze_Click);
            // 
            // comboMarkov
            // 
            this.comboMarkov.BackColor = System.Drawing.Color.Black;
            this.comboMarkov.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMarkov.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboMarkov.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboMarkov.ForeColor = System.Drawing.Color.Silver;
            this.comboMarkov.FormattingEnabled = true;
            this.comboMarkov.Items.AddRange(new object[] {
            "Chi-Square (H)",
            "Jeffrey Diverg. (H)",
            "Euclidean (M-S)"});
            this.comboMarkov.Location = new System.Drawing.Point(3, 85);
            this.comboMarkov.Name = "comboMarkov";
            this.comboMarkov.Size = new System.Drawing.Size(125, 21);
            this.comboMarkov.TabIndex = 10;
            this.comboMarkov.SelectedIndexChanged += new System.EventHandler(this.comboMarkov_SelectedIndexChanged);
            // 
            // comboGabor
            // 
            this.comboGabor.BackColor = System.Drawing.Color.Black;
            this.comboGabor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGabor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboGabor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboGabor.ForeColor = System.Drawing.Color.Silver;
            this.comboGabor.FormattingEnabled = true;
            this.comboGabor.Items.AddRange(new object[] {
            "Chi-Square (H)",
            "Jeffrey Diverg. (H)",
            "Euclidean (M-S)"});
            this.comboGabor.Location = new System.Drawing.Point(3, 85);
            this.comboGabor.Name = "comboGabor";
            this.comboGabor.Size = new System.Drawing.Size(125, 21);
            this.comboGabor.TabIndex = 8;
            this.comboGabor.SelectedIndexChanged += new System.EventHandler(this.comboGabor_SelectedIndexChanged);
            // 
            // comboGlobal
            // 
            this.comboGlobal.BackColor = System.Drawing.Color.Black;
            this.comboGlobal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGlobal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboGlobal.ForeColor = System.Drawing.Color.Silver;
            this.comboGlobal.FormattingEnabled = true;
            this.comboGlobal.Items.AddRange(new object[] {
            "Euclidean",
            "Manhattan",
            "Chebychev"});
            this.comboGlobal.Location = new System.Drawing.Point(3, 86);
            this.comboGlobal.Name = "comboGlobal";
            this.comboGlobal.Size = new System.Drawing.Size(125, 21);
            this.comboGlobal.TabIndex = 6;
            this.comboGlobal.SelectedIndexChanged += new System.EventHandler(this.comboGlobal_SelectedIndexChanged);
            // 
            // checkWindow
            // 
            this.checkWindow.AutoSize = true;
            this.checkWindow.Checked = true;
            this.checkWindow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkWindow.ForeColor = System.Drawing.Color.Silver;
            this.checkWindow.Location = new System.Drawing.Point(149, 66);
            this.checkWindow.Name = "checkWindow";
            this.checkWindow.Size = new System.Drawing.Size(124, 17);
            this.checkWindow.TabIndex = 39;
            this.checkWindow.Text = "Intensity Windowing:";
            this.checkWindow.UseVisualStyleBackColor = true;
            this.checkWindow.CheckedChanged += new System.EventHandler(this.checkWindow_CheckedChanged);
            // 
            // updLower
            // 
            this.updLower.BackColor = System.Drawing.Color.Black;
            this.updLower.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.updLower.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updLower.ForeColor = System.Drawing.Color.Silver;
            this.updLower.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.updLower.Location = new System.Drawing.Point(159, 89);
            this.updLower.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.updLower.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.updLower.Name = "updLower";
            this.updLower.Size = new System.Drawing.Size(52, 16);
            this.updLower.TabIndex = 41;
            this.updLower.ValueChanged += new System.EventHandler(this.updLower_ValueChanged);
            // 
            // updUpper
            // 
            this.updUpper.BackColor = System.Drawing.Color.Black;
            this.updUpper.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.updUpper.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updUpper.ForeColor = System.Drawing.Color.Silver;
            this.updUpper.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.updUpper.Location = new System.Drawing.Point(255, 89);
            this.updUpper.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.updUpper.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.updUpper.Name = "updUpper";
            this.updUpper.Size = new System.Drawing.Size(52, 16);
            this.updUpper.TabIndex = 42;
            this.updUpper.Value = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.updUpper.ValueChanged += new System.EventHandler(this.updUpper_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(225, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "to";
            // 
            // barLower
            // 
            this.barLower.LargeChange = 200;
            this.barLower.Location = new System.Drawing.Point(147, 110);
            this.barLower.Maximum = 2000;
            this.barLower.Minimum = -2000;
            this.barLower.Name = "barLower";
            this.barLower.Size = new System.Drawing.Size(76, 45);
            this.barLower.SmallChange = 100;
            this.barLower.TabIndex = 44;
            this.barLower.TickFrequency = 200;
            this.barLower.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barLower.Scroll += new System.EventHandler(this.barLower_Scroll);
            // 
            // barUpper
            // 
            this.barUpper.LargeChange = 200;
            this.barUpper.Location = new System.Drawing.Point(244, 110);
            this.barUpper.Maximum = 2000;
            this.barUpper.Minimum = -2000;
            this.barUpper.Name = "barUpper";
            this.barUpper.Size = new System.Drawing.Size(76, 45);
            this.barUpper.SmallChange = 100;
            this.barUpper.TabIndex = 45;
            this.barUpper.TickFrequency = 200;
            this.barUpper.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barUpper.Value = 900;
            this.barUpper.Scroll += new System.EventHandler(this.barUpper_Scroll);
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.noduleInfo);
            this.panelLeft.Controls.Add(this.noduleImage);
            this.panelLeft.Controls.Add(this.noduleThumb);
            this.panelLeft.Controls.Add(this.fullImage);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(249, 743);
            this.panelLeft.TabIndex = 46;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.noduleInfo2);
            this.panelRight.Controls.Add(this.noduleImage2);
            this.panelRight.Controls.Add(this.noduleThumb2);
            this.panelRight.Controls.Add(this.fullImage2);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(847, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(255, 743);
            this.panelRight.TabIndex = 47;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.checkBox1);
            this.panelControls.Controls.Add(this.comboLocal);
            this.panelControls.Controls.Add(this.comboFeature);
            this.panelControls.Controls.Add(this.chooseFeatures);
            this.panelControls.Controls.Add(this.comboGlobal);
            this.panelControls.Controls.Add(this.progressAnalyze);
            this.panelControls.Controls.Add(this.labelPrecision);
            this.panelControls.Controls.Add(this.updTopItems);
            this.panelControls.Controls.Add(this.checkTopItems);
            this.panelControls.Controls.Add(this.barUpper);
            this.panelControls.Controls.Add(this.checkThreshold);
            this.panelControls.Controls.Add(this.analyze);
            this.panelControls.Controls.Add(this.barLower);
            this.panelControls.Controls.Add(this.label2);
            this.panelControls.Controls.Add(this.checkWindow);
            this.panelControls.Controls.Add(this.updUpper);
            this.panelControls.Controls.Add(this.updLower);
            this.panelControls.Controls.Add(this.label1);
            this.panelControls.Controls.Add(this.distance);
            this.panelControls.Controls.Add(this.labelRecall);
            this.panelControls.Controls.Add(this.precision);
            this.panelControls.Controls.Add(this.scoreBox);
            this.panelControls.Controls.Add(this.recall);
            this.panelControls.Controls.Add(this.lblResults);
            this.panelControls.Controls.Add(this.rankByDistance);
            this.panelControls.Controls.Add(this.threshold);
            this.panelControls.Controls.Add(this.comboGabor);
            this.panelControls.Controls.Add(this.comboMarkov);
            this.panelControls.Controls.Add(this.label3);
            this.panelControls.Controls.Add(this.label4);
            this.panelControls.Location = new System.Drawing.Point(75, 3);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(442, 164);
            this.panelControls.TabIndex = 46;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ForeColor = System.Drawing.Color.Silver;
            this.checkBox1.Location = new System.Drawing.Point(322, 92);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(104, 17);
            this.checkBox1.TabIndex = 52;
            this.checkBox1.Text = "Annotation Eval.";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // comboLocal
            // 
            this.comboLocal.BackColor = System.Drawing.Color.Black;
            this.comboLocal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLocal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboLocal.ForeColor = System.Drawing.Color.Silver;
            this.comboLocal.FormattingEnabled = true;
            this.comboLocal.Items.AddRange(new object[] {
            "Chi-Square (H)",
            "Jeffrey Diverg. (H)"});
            this.comboLocal.Location = new System.Drawing.Point(3, 86);
            this.comboLocal.Name = "comboLocal";
            this.comboLocal.Size = new System.Drawing.Size(125, 21);
            this.comboLocal.TabIndex = 51;
            // 
            // comboFeature
            // 
            this.comboFeature.BackColor = System.Drawing.Color.Black;
            this.comboFeature.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFeature.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboFeature.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboFeature.ForeColor = System.Drawing.Color.Silver;
            this.comboFeature.FormattingEnabled = true;
            this.comboFeature.Items.AddRange(new object[] {
            "Global",
            "Local",
            "Gabor",
            "Markov",
            "All-Features",
            "Annotations"});
            this.comboFeature.Location = new System.Drawing.Point(3, 18);
            this.comboFeature.Name = "comboFeature";
            this.comboFeature.Size = new System.Drawing.Size(125, 21);
            this.comboFeature.TabIndex = 48;
            this.comboFeature.SelectedIndexChanged += new System.EventHandler(this.comboFeature_SelectedIndexChanged);
            // 
            // progressAnalyze
            // 
            this.progressAnalyze.Location = new System.Drawing.Point(322, 140);
            this.progressAnalyze.Name = "progressAnalyze";
            this.progressAnalyze.Size = new System.Drawing.Size(92, 11);
            this.progressAnalyze.TabIndex = 47;
            this.progressAnalyze.Visible = false;
            // 
            // labelPrecision
            // 
            this.labelPrecision.AutoSize = true;
            this.labelPrecision.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrecision.ForeColor = System.Drawing.Color.Silver;
            this.labelPrecision.Location = new System.Drawing.Point(316, 58);
            this.labelPrecision.Name = "labelPrecision";
            this.labelPrecision.Size = new System.Drawing.Size(53, 13);
            this.labelPrecision.TabIndex = 46;
            this.labelPrecision.Text = "Precision:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Silver;
            this.label3.Location = new System.Drawing.Point(0, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 49;
            this.label3.Text = "Image Feature:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Silver;
            this.label4.Location = new System.Drawing.Point(0, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 50;
            this.label4.Text = "Similarity Measure:";
            // 
            // panelFill
            // 
            this.panelFill.ColumnCount = 1;
            this.panelFill.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelFill.Controls.Add(this.noduleView, 0, 0);
            this.panelFill.Controls.Add(this.noduleView2, 0, 2);
            this.panelFill.Controls.Add(this.panelCenterControls, 0, 1);
            this.panelFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFill.Location = new System.Drawing.Point(249, 0);
            this.panelFill.Name = "panelFill";
            this.panelFill.RowCount = 4;
            this.panelFill.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.panelFill.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.panelFill.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.panelFill.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelFill.Size = new System.Drawing.Size(598, 743);
            this.panelFill.TabIndex = 49;
            // 
            // panelCenterControls
            // 
            this.panelCenterControls.ColumnCount = 3;
            this.panelCenterControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelCenterControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelCenterControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelCenterControls.Controls.Add(this.panelControls, 1, 0);
            this.panelCenterControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCenterControls.Location = new System.Drawing.Point(3, 397);
            this.panelCenterControls.Name = "panelCenterControls";
            this.panelCenterControls.RowCount = 1;
            this.panelCenterControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelCenterControls.Size = new System.Drawing.Size(592, 154);
            this.panelCenterControls.TabIndex = 6;
            // 
            // splitLeftForm
            // 
            this.splitLeftForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitLeftForm.Location = new System.Drawing.Point(249, 0);
            this.splitLeftForm.Name = "splitLeftForm";
            this.splitLeftForm.Size = new System.Drawing.Size(3, 743);
            this.splitLeftForm.TabIndex = 50;
            this.splitLeftForm.TabStop = false;
            this.splitLeftForm.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitLeftForm_SplitterMoved);
            // 
            // splitRightForm
            // 
            this.splitRightForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitRightForm.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitRightForm.Location = new System.Drawing.Point(844, 0);
            this.splitRightForm.Name = "splitRightForm";
            this.splitRightForm.Size = new System.Drawing.Size(3, 743);
            this.splitRightForm.TabIndex = 51;
            this.splitRightForm.TabStop = false;
            this.splitRightForm.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitRightForm_SplitterMoved);
            // 
            // NoduleViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1102, 743);
            this.Controls.Add(this.splitRightForm);
            this.Controls.Add(this.splitLeftForm);
            this.Controls.Add(this.panelFill);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "NoduleViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BRISC Nodule Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NoduleViewer_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NoduleViewer_KeyDown);
            this.Load += new System.EventHandler(this.NoduleViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.noduleImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.noduleThumb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.noduleThumb2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.noduleImage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullImage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updTopItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updLower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updUpper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barLower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barUpper)).EndInit();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelFill.ResumeLayout(false);
            this.panelCenterControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        

        private System.Windows.Forms.PictureBox noduleImage;
        private System.Windows.Forms.TextBox noduleInfo;
        private System.Windows.Forms.PictureBox noduleThumb;
        private System.Windows.Forms.ImageList noduleThumbs;
        private System.Windows.Forms.ListView noduleView2;
        private System.Windows.Forms.PictureBox noduleThumb2;
        private System.Windows.Forms.TextBox noduleInfo2;
        private System.Windows.Forms.PictureBox noduleImage2;
        private System.Windows.Forms.Button rankByDistance;
        private System.Windows.Forms.ColumnHeader Filename;
        private System.Windows.Forms.ColumnHeader NodeID;
        private System.Windows.Forms.ColumnHeader SessionID;
        private System.Windows.Forms.ColumnHeader Slice;
        private System.Windows.Forms.ListView noduleView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TrackBar threshold;
        private System.Windows.Forms.ColumnHeader Score;
        private System.Windows.Forms.ComboBox scoreBox;
        private System.Windows.Forms.ColumnHeader Dist;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.PictureBox fullImage;
        private System.Windows.Forms.PictureBox fullImage2;
        private System.Windows.Forms.TextBox distance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button chooseFeatures;
        private System.Windows.Forms.NumericUpDown updTopItems;
        private System.Windows.Forms.Label labelRecall;
        private System.Windows.Forms.TextBox precision;
        private System.Windows.Forms.TextBox recall;
        private System.Windows.Forms.CheckBox checkThreshold;
        private System.Windows.Forms.CheckBox checkTopItems;
        private System.Windows.Forms.Button analyze;
        private System.Windows.Forms.ComboBox comboMarkov;
        private System.Windows.Forms.ComboBox comboGabor;
        private System.Windows.Forms.ComboBox comboGlobal;
        private System.Windows.Forms.CheckBox checkWindow;
        private System.Windows.Forms.NumericUpDown updLower;
        private System.Windows.Forms.NumericUpDown updUpper;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar barLower;
        private System.Windows.Forms.TrackBar barUpper;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.TableLayoutPanel panelFill;
        private System.Windows.Forms.TableLayoutPanel panelCenterControls;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Label labelPrecision;
        private System.Windows.Forms.ProgressBar progressAnalyze;
        private System.Windows.Forms.Splitter splitLeftForm;
        private System.Windows.Forms.Splitter splitRightForm;
        private System.Windows.Forms.ComboBox comboFeature;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboLocal;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

