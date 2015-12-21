namespace EA.Weee.Email
{
    using System.Configuration;
    using EA.Prsd.Email;

    public class WeeeConfigReader : ConfigReader, IWeeeEmailConfiguration
    {
        public WeeeConfigReader()
            : base("Weee")
        {
        }

        public string ExternalUserLoginUrl
        {
            get
            {
                string settingName = GetAppSettingName("ExternalUserLoginUrl");
                return ConfigurationManager.AppSettings.Get(settingName);
            }
        }
    }
}
