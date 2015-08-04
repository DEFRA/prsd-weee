namespace EA.Weee.Core.Configuration
{
    using System.Collections.Specialized;
    using System.Configuration;
    using EmailRules;

    public class ConfigurationManagerWrapper : IConfigurationManagerWrapper
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        public RulesSection InternalEmailRules
        {
            get { return (RulesSection)ConfigurationManager.GetSection("internalEmailRules"); }
        }
    }
}
