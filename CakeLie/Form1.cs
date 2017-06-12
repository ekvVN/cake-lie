﻿namespace CakeLie
{
    using System.Windows.Forms;
    using Common.Log.Conf.CakeLie;
    using Common.Logging;

    public partial class Form1 : Form
    {
        private readonly ILog _log = LogManager.GetLogger("LogExample");

        static Form1()
        {
            // Инициализация в статическом конструкторе до вызова LogManager.GetLogger()
            LogAdapterConfiguration.Init();
        }

        public Form1()
        {
            InitializeComponent();

            _log.Info("Hello NLog from Common.Logging");
        }
    }
}
