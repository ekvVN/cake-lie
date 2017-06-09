namespace CakeLie
{
    using System.Windows.Forms;
    using LogConf.CakeLie;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            LogExample.Start();
        }
    }
}
