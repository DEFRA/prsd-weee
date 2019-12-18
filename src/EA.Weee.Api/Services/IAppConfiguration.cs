namespace EA.Weee.Api.Services
{
    public interface IAppConfiguration
    {
        string Environment { get; set; }
        string ConnectionString { get; set; }
        string SiteRoot { get; set; }
        string VerificationEmailBypassDomains { get; set; }
        string ApiSecret { get; set; }
        bool MaintenanceMode { get; set; }
    }
}