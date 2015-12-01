namespace EA.Weee.Web.Services
{
    public interface IAppConfiguration
    {
        string Environment { get; set; }

        string GoogleAnalyticsAccountId { get; set; }

        string SiteRoot { get; set; }

        string SendEmail { get; set; }

        string MailFrom { get; set; }

        string ApiUrl { get; set; }

        string ApiSecret { get; set; }

        string ApiClientId { get; set; }

        double ApiTimeoutInSeconds { get; set; }

        string TruncateEmailAfterPlus { get; set; }

        bool EnableTestArea { get; set; }

        string DonePageUrl { get; set; }

        bool EnableDataReturns { get; set; }

        double OrganisationNamesCacheDurationMins { get; set; }
    }
}