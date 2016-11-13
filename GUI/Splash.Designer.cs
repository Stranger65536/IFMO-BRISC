namespace BRISC.GUI
{
    partial class Splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.panel1 = new System.Windows.Forms.Panel();
            this.license = new System.Windows.Forms.TextBox();
            this.checkBounds = new System.Windows.Forms.CheckBox();
            this.checkScore = new System.Windows.Forms.CheckBox();
            this.checkDistance = new System.Windows.Forms.CheckBox();
            this.checkNodule = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.status = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.license);
            this.panel1.Controls.Add(this.checkBounds);
            this.panel1.Controls.Add(this.checkScore);
            this.panel1.Controls.Add(this.checkDistance);
            this.panel1.Controls.Add(this.checkNodule);
            this.panel1.Controls.Add(this.progressBar);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.status);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(424, 251);
            this.panel1.TabIndex = 3;
            // 
            // license
            // 
            this.license.BackColor = System.Drawing.SystemColors.Control;
            this.license.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.license.Font = new System.Drawing.Font("Bitstream Vera Sans", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.license.Location = new System.Drawing.Point(48, 201);
            this.license.Multiline = true;
            this.license.Name = "license";
            this.license.Size = new System.Drawing.Size(262, 36);
            this.license.TabIndex = 15;
            this.license.TabStop = false;
            this.license.Text = "Copyright (C) 2006 DePaul University\r\nThis program is distributed under the terms" +
                " of the GNU General Public License; see GPL.txt for details.";
            // 
            // checkBounds
            // 
            this.checkBounds.AutoSize = true;
            this.checkBounds.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBounds.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBounds.Location = new System.Drawing.Point(241, 114);
            this.checkBounds.Name = "checkBounds";
            this.checkBounds.Size = new System.Drawing.Size(111, 17);
            this.checkBounds.TabIndex = 13;
            this.checkBounds.Text = "Thumbnail data";
            this.checkBounds.UseVisualStyleBackColor = true;
            this.checkBounds.Visible = false;
            // 
            // checkScore
            // 
            this.checkScore.AutoSize = true;
            this.checkScore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkScore.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkScore.Location = new System.Drawing.Point(76, 114);
            this.checkScore.Name = "checkScore";
            this.checkScore.Size = new System.Drawing.Size(112, 17);
            this.checkScore.TabIndex = 12;
            this.checkScore.Text = "Annotation data";
            this.checkScore.UseVisualStyleBackColor = true;
            // 
            // checkDistance
            // 
            this.checkDistance.AutoSize = true;
            this.checkDistance.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkDistance.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkDistance.Location = new System.Drawing.Point(241, 91);
            this.checkDistance.Name = "checkDistance";
            this.checkDistance.Size = new System.Drawing.Size(96, 17);
            this.checkDistance.TabIndex = 11;
            this.checkDistance.Text = "Feature Data";
            this.checkDistance.UseVisualStyleBackColor = true;
            // 
            // checkNodule
            // 
            this.checkNodule.AutoSize = true;
            this.checkNodule.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkNodule.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkNodule.Location = new System.Drawing.Point(76, 91);
            this.checkNodule.Name = "checkNodule";
            this.checkNodule.Size = new System.Drawing.Size(91, 17);
            this.checkNodule.TabIndex = 10;
            this.checkNodule.Text = "Nodule data";
            this.checkNodule.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(23, 163);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(377, 18);
            this.progressBar.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bitstream Vera Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(72, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "LIDC Nodule Viewer";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bitstream Vera Sans", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(68, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "BRISC";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(23, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(41, 39);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.Location = new System.Drawing.Point(20, 149);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(66, 13);
            this.status.TabIndex = 7;
            this.status.Text = "Loading ...";
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 251);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BRISC";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox checkDistance;
        private System.Windows.Forms.CheckBox checkNodule;
        private System.Windows.Forms.CheckBox checkScore;
        private System.Windows.Forms.CheckBox checkBounds;
        private System.Windows.Forms.TextBox license;
    }
}