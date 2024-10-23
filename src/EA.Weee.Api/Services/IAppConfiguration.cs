namespace EA.Weee.Api.Services
{
    using System.ComponentModel;

    public interface IAppConfiguration
    {
        string Environment { get; set; }
        string ConnectionString { get; set; }
        string SiteRoot { get; set; }
        string VerificationEmailBypassDomains { get; set; }
        string ApiSecret { get; set; }
        bool MaintenanceMode { get; set; }
        string GovUkPayBaseUrl { get; set; }

        string GovUkPayApiKey { get; set; }

        string GovUkPayMopUpJobSchedule { get; set; }

        bool ProxyEnabled { get; set; }

        bool ByPassProxyOnLocal { get; set; }

        string ProxyWebAddress { get; set; }

        bool ProxyUseDefaultCredentials { get; set; }

        int GovUkPayLastProcessedMinutes { get; set; }

        int GovUkPayWindowMinutes { get; set; }

        bool GovUkPayMopUpJobEnabled { get; set; }
    }
}