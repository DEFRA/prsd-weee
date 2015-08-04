namespace EA.Weee.Core.Configuration
{
    using System.Collections.Specialized;
    using System.Configuration;
    using Configuration.InternalConfiguration;

    public class ConfigurationManagerWrapper : IConfigurationManagerWrapper
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        public InternalConfigurationSection InternalConfiguration
        {
            get { return (InternalConfigurationSection)ConfigurationManager.GetSection("internalConfiguration"); }
        }
    }
}
