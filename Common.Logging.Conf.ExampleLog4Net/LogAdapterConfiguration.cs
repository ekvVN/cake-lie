namespace Common.Logging.Conf.ExampleLog4Net
{
    using log4net.Config;
    using Org;

    public static class LogAdapterConfiguration
    {
        /// <summary>
        /// Имя файла конфигурации адаптера
        /// </summary>
        private const string ConfigFileName = "LogAdapter.Log4Net.config";

        public static void Init()
        {
            LogManager.Reset(new ConfigurationReader(ConfigFileName));
            XmlConfigurator.Configure();
        }
    }
}
