namespace EA.Weee.Email
{
    using EA.Prsd.Email;
    using System.Configuration;

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
