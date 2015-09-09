namespace EA.Prsd.Email
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class reads the email configuration from the current app.config or web.config file.
    /// </summary>
    public class ConfigReader : IEmailConfiguration
    {
        private readonly string prefix;

        /// <summary>
        /// Creates a new instance of <see cref="ConfigReader"/>.
        /// </summary>
        /// <param name="prefix">The prefix used for environment-specific app settings.
        /// E.g. "Weee" or "Iws".</param>
        public ConfigReader(string prefix)
        {
            this.prefix = prefix;
        }

        protected string GetAppSettingName(string name)
        {
            return string.Format("{0}.{1}", prefix, name);
        }

        public string SystemEmailAddress
        {
            get
            {
                string settingName = GetAppSettingName("MailFrom");
                return ConfigurationManager.AppSettings.Get(settingName);
            }
        }

        public bool TruncateEmailAfterPlus
        {
            get
            {
                string settingName = GetAppSettingName("TruncateEmailAfterPlus");
                return bool.Parse(ConfigurationManager.AppSettings.Get(settingName));
            }
        }

        public bool SendEmail
        {
            get
            {
                string settingName = GetAppSettingName("SendEmail");
                return bool.Parse(ConfigurationManager.AppSettings.Get(settingName));
            }
        }
    }
}
