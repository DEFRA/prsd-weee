namespace EA.Weee.Email
{
    using EA.Prsd.Email;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
