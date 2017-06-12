namespace CakeLie
{
    using System.Windows.Forms;
    using Common.Logging;
    using Common.Logging.Conf.ExampleLog4Net;

    public partial class Form1 : Form
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Form1));

        static Form1()
        {
            // Инициализация в статическом конструкторе до вызова LogManager.GetLogger()
            LogAdapterConfiguration.Init();
        }

        public Form1()
        {
            InitializeComponent();

            _log.Info("Hello log4net from Common.Logging");
        }
    }
}
