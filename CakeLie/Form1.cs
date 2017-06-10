namespace CakeLie
{
    using System.Windows.Forms;
    using Common.Log.Conf.CakeLie;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            LogExample.Start();
        }
    }
}
