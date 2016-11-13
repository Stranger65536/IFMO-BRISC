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
    /// A "pretty" splash screen for displaying the status of a lenthy process
    /// </summary>
    public partial class Splash : Form
    {
        /// <summary>
        /// Initialize new splash screen
        /// </summary>
        public Splash()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Progress bar
        /// </summary>
        public ProgressBar Progress
        {
            get
            {
                Application.DoEvents();
                return progressBar;
            }
        }
        /// <summary>
        /// Status line
        /// </summary>
        public Label Status
        {
            get
            {
                return status;
            }
        }
        /// <summary>
        /// First checkbox (upper-left)
        /// </summary>
        public CheckBox FirstCheck
        {
            get
            {
                return checkNodule;
            }
        }
        /// <summary>
        /// Second checkbox (lower-left)
        /// </summary>
        public CheckBox SecondCheck
        {
            get
            {
                return checkDistance;
            }
        }
        /// <summary>
        /// Third checkbox (upper-right)
        /// </summary>
        public CheckBox ThirdCheck
        {
            get
            {
                return checkScore;
            }
        }
        /// <summary>
        /// Fourth checkbox (lower-right)
        /// </summary>
        public CheckBox FourthCheck
        {
            get
            {
                return checkBounds;
            }
        }
    }
}