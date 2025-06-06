﻿namespace EA.Weee.Web.Services
{
    using System;

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

        string ApiClientCredentialSecret { get; set; }

        string ApiClientCredentialId { get; set; }

        double ApiTimeoutInSeconds { get; set; }

        string TruncateEmailAfterPlus { get; set; }

        bool EnableTestArea { get; set; }

        string DonePageUrl { get; set; }

        bool EnableDataReturns { get; set; }

        bool EnableAATFReturns { get; set; }

        bool EnablePCSEvidenceNotes { get; set; }

        bool EnablePBSEvidenceNotes { get; set; }

        bool EnableAATFEvidenceNotes { get; set; }

        /// <summary>
        /// This setting determines whether the admin area allows managing of obligations.
        /// </summary>
        bool EnablePCSObligations { get; set; }

        /// <summary>
        /// This setting determines whether the admin area allows managing of charges and invoicing.
        /// </summary>
        bool EnableInvoicing { get; set; }

        double OrganisationCacheDurationMins { get; set; }

        int MaximumOrganisationSearchResults { get; set; }

        int MaximumAatfOrganisationSearchResults { get; set; }

        int MaximumProducerOrganisationSearchResults { get; set; }

        bool MaintenanceMode { get; set; }

        DateTime EvidenceNotesSiteSelectionDateFrom { get; set; }

        int DefaultInternalPagingPageSize { get; set; }

        int DefaultExternalPagingPageSize { get; set; }

        bool ProxyEnabled { get; set; }

        bool ByPassProxyOnLocal { get; set; }

        string ProxyWebAddress { get; set; }

        bool ProxyUseDefaultCredentials { get; set; }

        string CompaniesHouseReferencePath { get; set; }

        string CompaniesHouseBaseUrl { get; set; }

        string AddressLookupReferencePath { get; set; }

        string AddressLookupBaseUrl { get; set; }

        string GovUkPayBaseUrl { get; set; }

        string GovUkPayApiKey { get; set; }

        string GovUkPayReturnBaseUrl { get; set; }

        string GovUkPayDescription { get; set; }

        int GovUkPayAmountInPence { get; set; }

        string GovUkPayTokenSecret { get; set; }

        string GovUkPayTokenSalt { get; set; }

        TimeSpan GovUkPayTokenLifeTime { get; set; }

        string OAuthTokenEndpoint { get; set; }

        string OAuthTokenClientId { get; set; }

        string OAuthTokenClientSecret { get; set; }

        string CompaniesHouseScope { get; set; }

        string AddressLookupScope { get; set; }

        DateTime SmallProducerFeatureEnabledFrom { get; set; }
    }
}