namespace BRISC.GUI
{
    partial class FeatureVectorDialog
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Contrast");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Correlation");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Energy");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Homogeneity");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Entropy");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Third Order Moment");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Inverse Variance");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Sum Average");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Variance");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Cluster Tendency");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Maximum Probability");
            this.FeatureView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // featureView
            // 
            this.FeatureView.BackColor = System.Drawing.Color.Black;
            this.FeatureView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FeatureView.CheckBoxes = true;
            this.FeatureView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FeatureView.Font = new System.Drawing.Font("Bitstream Vera Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FeatureView.ForeColor = System.Drawing.Color.Silver;
            listViewItem1.StateImageIndex = 0;
            listViewItem1.Tag = "contrast";
            listViewItem2.StateImageIndex = 0;
            listViewItem2.Tag = "correlation";
            listViewItem3.StateImageIndex = 0;
            listViewItem3.Tag = "energy";
            listViewItem4.StateImageIndex = 0;
            listViewItem4.Tag = "homogeneity";
            listViewItem5.StateImageIndex = 0;
            listViewItem5.Tag = "entropy";
            listViewItem6.StateImageIndex = 0;
            listViewItem6.Tag = "thirdOrderMoment";
            listViewItem7.StateImageIndex = 0;
            listViewItem7.Tag = "inverseVariance";
            listViewItem8.StateImageIndex = 0;
            listViewItem8.Tag = "sumAverage";
            listViewItem9.StateImageIndex = 0;
            listViewItem9.Tag = "variance";
            listViewItem10.StateImageIndex = 0;
            listViewItem10.Tag = "clusterTendency";
            listViewItem11.StateImageIndex = 0;
            listViewItem11.Tag = "maximumProbability";
            this.FeatureView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11});
            this.FeatureView.Location = new System.Drawing.Point(0, 0);
            this.FeatureView.MultiSelect = false;
            this.FeatureView.Name = "FeatureView";
            this.FeatureView.Size = new System.Drawing.Size(159, 193);
            this.FeatureView.TabIndex = 0;
            this.FeatureView.UseCompatibleStateImageBehavior = false;
            this.FeatureView.View = System.Windows.Forms.View.SmallIcon;
            // 
            // FeatureVectorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(159, 193);
            this.Controls.Add(this.FeatureView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeatureVectorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Feature Vector";
            this.ResumeLayout(false);

        }

        
    }
}