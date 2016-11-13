namespace BRISC.GUI
{
    partial class MainMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.noduleViewer = new System.Windows.Forms.Button();
            this.seriesViewer = new System.Windows.Forms.Button();
            this.runTask = new System.Windows.Forms.Button();
            this.comboPrimary = new System.Windows.Forms.ComboBox();
            this.import = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bitstream Vera Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(62, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 19);
            this.label2.TabIndex = 8;
            this.label2.Text = "BRISC Really IS Cool";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bitstream Vera Sans", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(59, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 31);
            this.label1.TabIndex = 7;
            this.label1.Text = "BRISC";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(15, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(41, 39);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 333);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "NSF REU MedIX \'06";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 349);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "DePaul University, Chicago, IL";
            // 
            // noduleViewer
            // 
            this.noduleViewer.Font = new System.Drawing.Font("Bitstream Vera Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noduleViewer.Location = new System.Drawing.Point(55, 175);
            this.noduleViewer.Name = "noduleViewer";
            this.noduleViewer.Size = new System.Drawing.Size(165, 38);
            this.noduleViewer.TabIndex = 0;
            this.noduleViewer.Text = "Nodule Viewer";
            this.noduleViewer.UseVisualStyleBackColor = true;
            this.noduleViewer.Click += new System.EventHandler(this.noduleViewer_Click);
            // 
            // seriesViewer
            // 
            this.seriesViewer.Font = new System.Drawing.Font("Bitstream Vera Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seriesViewer.Location = new System.Drawing.Point(55, 224);
            this.seriesViewer.Name = "seriesViewer";
            this.seriesViewer.Size = new System.Drawing.Size(165, 38);
            this.seriesViewer.TabIndex = 1;
            this.seriesViewer.Text = "Series Viewer";
            this.seriesViewer.UseVisualStyleBackColor = true;
            this.seriesViewer.Click += new System.EventHandler(this.seriesViewer_Click);
            // 
            // runTask
            // 
            this.runTask.Font = new System.Drawing.Font("Bitstream Vera Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runTask.Location = new System.Drawing.Point(55, 275);
            this.runTask.Name = "runTask";
            this.runTask.Size = new System.Drawing.Size(165, 38);
            this.runTask.TabIndex = 11;
            this.runTask.Text = "Run Analysis";
            this.runTask.UseVisualStyleBackColor = true;
            this.runTask.Click += new System.EventHandler(this.runTask_Click);
            // 
            // comboPrimary
            // 
            this.comboPrimary.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboPrimary.FormattingEnabled = true;
            this.comboPrimary.Location = new System.Drawing.Point(20, 146);
            this.comboPrimary.Name = "comboPrimary";
            this.comboPrimary.Size = new System.Drawing.Size(237, 21);
            this.comboPrimary.TabIndex = 12;
            this.comboPrimary.SelectedIndexChanged += new System.EventHandler(this.comboPrimary_SelectedIndexChanged);
            // 
            // import
            // 
            this.import.Font = new System.Drawing.Font("Bitstream Vera Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.import.Location = new System.Drawing.Point(55, 93);
            this.import.Name = "import";
            this.import.Size = new System.Drawing.Size(165, 38);
            this.import.TabIndex = 13;
            this.import.Text = "Import LIDC Data";
            this.import.UseVisualStyleBackColor = true;
            this.import.Click += new System.EventHandler(this.import_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 372);
            this.Controls.Add(this.import);
            this.Controls.Add(this.comboPrimary);
            this.Controls.Add(this.runTask);
            this.Controls.Add(this.seriesViewer);
            this.Controls.Add(this.noduleViewer);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BRISC";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainMenu_FormClosed);
            this.Load += new System.EventHandler(this.MainMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button noduleViewer;
        private System.Windows.Forms.Button seriesViewer;
        private System.Windows.Forms.Button runTask;
        private System.Windows.Forms.ComboBox comboPrimary;
        private System.Windows.Forms.Button import;
    }
}