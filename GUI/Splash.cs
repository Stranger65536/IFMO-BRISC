using System.Windows.Forms;


namespace BRISC.GUI
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }


        public ProgressBar Progress
        {
            get
            {
                Application.DoEvents();
                return progressBar;
            }
        }


        public Label Status { get; private set; }


        public CheckBox FirstCheck { get; private set; }


        public CheckBox SecondCheck { get; private set; }


        public CheckBox ThirdCheck { get; private set; }


        public CheckBox FourthCheck { get; private set; }
    }
}