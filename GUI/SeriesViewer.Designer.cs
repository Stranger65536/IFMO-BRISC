namespace BRISC.GUI
{
    partial class SeriesViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesViewer));
            this.label1 = new System.Windows.Forms.Label();
            this.comboSeries = new System.Windows.Forms.ComboBox();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.lblImagesLoaded = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.lblCurrentImage = new System.Windows.Forms.Label();
            this.lblWindow = new System.Windows.Forms.Label();
            this.lblHelp = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Bitstream Vera Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SeriesID:";
            // 
            // comboSeries
            // 
            this.comboSeries.BackColor = System.Drawing.Color.Black;
            this.comboSeries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSeries.Font = new System.Drawing.Font("Bitstream Vera Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboSeries.ForeColor = System.Drawing.Color.Green;
            this.comboSeries.FormattingEnabled = true;
            this.comboSeries.Location = new System.Drawing.Point(88, 6);
            this.comboSeries.Name = "comboSeries";
            this.comboSeries.Size = new System.Drawing.Size(240, 21);
            this.comboSeries.TabIndex = 1;
            this.comboSeries.SelectedIndexChanged += new System.EventHandler(this.comboSeries_SelectedIndexChanged);
            // 
            // picImage
            // 
            this.picImage.BackColor = System.Drawing.Color.Black;
            this.picImage.Location = new System.Drawing.Point(115, 152);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(141, 108);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picImage.TabIndex = 2;
            this.picImage.TabStop = false;
            this.picImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picImage_MouseDown);
            this.picImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picImage_MouseMove);
            this.picImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picImage_MouseUp);
            // 
            // lblImagesLoaded
            // 
            this.lblImagesLoaded.AutoSize = true;
            this.lblImagesLoaded.BackColor = System.Drawing.Color.Black;
            this.lblImagesLoaded.Font = new System.Drawing.Font("Bitstream Vera Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImagesLoaded.ForeColor = System.Drawing.Color.Green;
            this.lblImagesLoaded.Location = new System.Drawing.Point(85, 30);
            this.lblImagesLoaded.Name = "lblImagesLoaded";
            this.lblImagesLoaded.Size = new System.Drawing.Size(126, 13);
            this.lblImagesLoaded.TabIndex = 3;
            this.lblImagesLoaded.Text = "0 image(s) loaded";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 500;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // lblCurrentImage
            // 
            this.lblCurrentImage.AutoSize = true;
            this.lblCurrentImage.BackColor = System.Drawing.Color.Black;
            this.lblCurrentImage.Font = new System.Drawing.Font("Bitstream Vera Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentImage.ForeColor = System.Drawing.Color.Green;
            this.lblCurrentImage.Location = new System.Drawing.Point(12, 55);
            this.lblCurrentImage.Name = "lblCurrentImage";
            this.lblCurrentImage.Size = new System.Drawing.Size(105, 13);
            this.lblCurrentImage.TabIndex = 4;
            this.lblCurrentImage.Text = "Z-pos: 0 (0/0)";
            // 
            // lblWindow
            // 
            this.lblWindow.AutoSize = true;
            this.lblWindow.BackColor = System.Drawing.Color.Black;
            this.lblWindow.Font = new System.Drawing.Font("Bitstream Vera Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWindow.ForeColor = System.Drawing.Color.Green;
            this.lblWindow.Location = new System.Drawing.Point(12, 68);
            this.lblWindow.Name = "lblWindow";
            this.lblWindow.Size = new System.Drawing.Size(161, 13);
            this.lblWindow.TabIndex = 5;
            this.lblWindow.Text = "Intensities: [0 - 800]";
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.Font = new System.Drawing.Font("Bitstream Vera Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHelp.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblHelp.Location = new System.Drawing.Point(12, 92);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(35, 13);
            this.lblHelp.TabIndex = 6;
            this.lblHelp.TabStop = true;
            this.lblHelp.Text = "Help";
            this.lblHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHelp_LinkClicked);
            // 
            // SeriesViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(504, 503);
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.lblWindow);
            this.Controls.Add(this.lblCurrentImage);
            this.Controls.Add(this.lblImagesLoaded);
            this.Controls.Add(this.comboSeries);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picImage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SeriesViewer";
            this.Text = "BRISC Series Viewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SeriesViewer_FormClosed);
            this.Resize += new System.EventHandler(this.SeriesViewer_Resize);
            this.Load += new System.EventHandler(this.SeriesViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboSeries;
        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.Label lblImagesLoaded;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label lblCurrentImage;
        private System.Windows.Forms.Label lblWindow;
        private System.Windows.Forms.LinkLabel lblHelp;

    }
}