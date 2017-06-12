namespace Common.Logging.Conf.ExampleNLog
{
    using Logging;
    using Org;

    public static class LogAdapterConfiguration
    {
        /// <summary>
        /// Имя файла конфигурации адаптера
        /// </summary>
        private const string ConfigFileName = "LogAdapter.NLog41.config";

        public static void Init()
        {
            LogManager.Reset(new ConfigurationReader(ConfigFileName));
        }
    }
}