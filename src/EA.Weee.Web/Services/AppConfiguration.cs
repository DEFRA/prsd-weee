namespace EA.Weee.Web.Services
{
    using System.ComponentModel;

    public class AppConfiguration
    {
        [DefaultValue("Development")]
        public string Environment { get; set; }

        public string GoogleAnalyticsAccountId { get; set; }

        public string SiteRoot { get; set; }

        public string ApiUrl { get; set; }

        public string ApiSecret { get; set; }
    }
}