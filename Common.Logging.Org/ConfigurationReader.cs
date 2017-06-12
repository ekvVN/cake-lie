namespace Common.Logging.Org
{
    using System.Xml;
    using Configuration;

    public class ConfigurationReader : IConfigurationReader
    {
        private readonly string _configFileName;

        public ConfigurationReader(string configFileName)
        {
            _configFileName = configFileName;
        }

        object IConfigurationReader.GetSection(string sectionName)
        {
            return GetSection(sectionName);
        }

        public LogSetting GetSection(string sectionName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(_configFileName);
            var settings = new ConfigurationSectionHandler().Create(null, null, xmlDoc.SelectSingleNode("configuration/" + sectionName));
            return settings;
        }
    }
}