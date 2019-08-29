namespace EA.Prsd.Email
{
    using System.Configuration;

    /// <summary>
    ///     This class reads the email configuration from the current app.config or web.config file.
    /// </summary>
    public class ConfigReader : IEmailConfiguration
    {
        private readonly string prefix;

        /// <summary>
        ///     Creates a new instance of <see cref="ConfigReader" />.
        /// </summary>
        /// <param name="prefix">
        ///     The prefix used for environment-specific app settings.
        ///     E.g. "Weee" or "Iws".
        /// </param>
        public ConfigReader(string prefix)
        {
            this.prefix = prefix;
        }

        public string SystemEmailAddress
        {
            get
            {
                var settingName = GetAppSettingName("MailFrom");
                return ConfigurationManager.AppSettings.Get(settingName);
            }
        }

        public bool TruncateEmailAfterPlus
        {
            get
            {
                var settingName = GetAppSettingName("TruncateEmailAfterPlus");
                return bool.Parse(ConfigurationManager.AppSettings.Get(settingName));
            }
        }

        public bool SendEmail
        {
            get
            {
                var settingName = GetAppSettingName("SendEmail");
                return bool.Parse(ConfigurationManager.AppSettings.Get(settingName));
            }
        }

        protected string GetAppSettingName(string name)
        {
            return string.Format("{0}.{1}", prefix, name);
        }
    }
}