using System.Windows.Forms;


namespace BRISC.GUI
{
    public partial class FeatureVectorDialog : Form
    {
        public FeatureVectorDialog()
        {
            InitializeComponent();
        }


        public ListView FeatureView { get; private set; }
    }
}