namespace EA.Weee.Api.Services
{
    using System.ComponentModel;

    public class AppConfiguration : IAppConfiguration
    {
        [DefaultValue("Development")]
        public string Environment { get; set; }

        [DisplayName("DefaultConnection")]
        public string ConnectionString { get; set; }

        public string SiteRoot { get; set; }

        public string VerificationEmailBypassDomains { get; set; }

        public string ApiSecret { get; set; }

        public bool MaintenanceMode { get; set; }

        public string GovUkPayBaseUrl { get; set; }

        public string GovUkPayApiKey { get; set; }

        public string GovUkPayMopUpJobSchedule { get; set; }

        [DefaultValue(false)]
        public bool ProxyEnabled { get; set; }

        [DefaultValue(false)]
        public bool ByPassProxyOnLocal { get; set; }

        public string ProxyWebAddress { get; set; }

        [DefaultValue(false)]
        public bool ProxyUseDefaultCredentials { get; set; }

        [DefaultValue(30)]
        public int GovUkPayLastProcessedMinutes { get; set; }

        [DefaultValue(180)]
        public int GovUkPayWindowMinutes { get; set; }

        [DefaultValue(true)]
        public bool GovUkPayMopUpJobEnabled { get; set; }
    }
}