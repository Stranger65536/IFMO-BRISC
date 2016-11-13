using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BRISC.Core;

namespace BRISC.GUI
{
    /// <summary>
    /// Feature vector selection dialog
    /// </summary>
    public partial class FeatureVectorDialog : Form
    {
        /// <summary>
        /// Initialize new dialog
        /// </summary>
        public FeatureVectorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Feature listview
        /// </summary>
        public ListView FeatureView
        {
            get
            {
                return featureView;
            }
        }
    }
}