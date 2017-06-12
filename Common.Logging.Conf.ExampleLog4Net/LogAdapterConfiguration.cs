namespace Common.Logging.Conf.ExampleLog4Net
{
    using System.IO;
    using log4net.Config;
    using Org;

    public static class LogAdapterConfiguration
    {
        /// <summary>
        /// Имя файла конфигурации адаптера
        /// </summary>
        private const string AdapterConfigFileName = "LogAdapter.Log4Net.config";

        public static void Init()
        {
            LogManager.Reset(new ConfigurationReader(AdapterConfigFileName));
            XmlConfigurator.Configure(new FileInfo("Log4Net.config"));
        }
    }
}
