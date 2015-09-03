namespace EA.Weee.Core.Configuration
{
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;

    public class ConfigurationManagerWrapper : IConfigurationManagerWrapper
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        public string GetKeyValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public bool HasKey(string key)
        {
            return ConfigurationManager.AppSettings.AllKeys.Select((string x) => x.ToUpperInvariant()).Contains(key.ToUpperInvariant());
        }
    }
}
