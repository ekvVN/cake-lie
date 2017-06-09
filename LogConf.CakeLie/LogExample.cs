namespace LogConf.CakeLie
{
    using Common.Logging;

    /// <summary>
    /// LogExample - пример записи в лог
    /// </summary>
    public static class LogExample
    {
        private static readonly ILog _log = LogManager.GetLogger("LogExample");

        public static void Start()
        {
            _log.Info("Hello NLog from Common.Logging");
        }
    }
}
