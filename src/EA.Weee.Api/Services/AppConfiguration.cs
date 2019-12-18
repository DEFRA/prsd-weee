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
    }
}