﻿namespace EA.Weee.Web.Services
{
    using System;
    using System.ComponentModel;

    public class AppConfiguration : IAppConfiguration
    {
        [DefaultValue("Development")]
        public string Environment { get; set; }

        public string GoogleAnalyticsAccountId { get; set; }

        public string SiteRoot { get; set; }

        [DefaultValue("true")]
        public string SendEmail { get; set; }

        public string MailFrom { get; set; }

        public string ApiUrl { get; set; }

        public string ApiSecret { get; set; }

        public string ApiClientId { get; set; }

        public string ApiClientCredentialSecret { get; set; }

        public string ApiClientCredentialId { get; set; }

        [DefaultValue(60)]
        public double ApiTimeoutInSeconds { get; set; }

        [DefaultValue("false")]
        public string TruncateEmailAfterPlus { get; set; }

        /// <summary>
        /// This value determines whether then "Test" area is accessible.
        /// The value should be false for production environments.
        /// </summary>
        [DefaultValue(false)]
        public bool EnableTestArea { get; set; }

        public string DonePageUrl { get; set; }

        [DefaultValue(false)]
        public bool EnableDataReturns { get; set; }

        [DefaultValue(false)]
        public bool EnableAATFReturns { get; set; }

        [DefaultValue(false)]
        public bool EnablePBSEvidenceNotes { get; set; }

        /// <summary>
        /// defines whether the radio-button option "Manage PCS evidence notes" will be available in menu or not
        /// </summary>
        [DefaultValue(false)]
        public bool EnablePCSEvidenceNotes { get; set; }

        [DefaultValue(false)]
        public bool EnableAATFEvidenceNotes { get; set; }

        /// <summary>
        /// This setting determines whether the admin area allows managing of obligations.
        /// </summary>
        [DefaultValue(false)]
        public bool EnablePCSObligations { get; set; }

        /// <summary>
        /// This setting determines whether the admin area allows managing of charges and invoicing.
        /// </summary>
        [DefaultValue(false)]
        public bool EnableInvoicing { get; set; }

        [DefaultValue(15.0)]
        public double OrganisationCacheDurationMins { get; set; }

        [DefaultValue(10)]
        public int MaximumOrganisationSearchResults { get; set; }

        [DefaultValue(10)]
        public int MaximumAatfOrganisationSearchResults { get; set; }

        [DefaultValue(10)]
        public int MaximumProducerOrganisationSearchResults { get; set; }

        [DefaultValue(false)]
        public bool MaintenanceMode { get; set; }

        [DefaultValue("01/01/2023")]
        public DateTime EvidenceNotesSiteSelectionDateFrom { get; set; }

        [DefaultValue("25")]
        public int DefaultInternalPagingPageSize { get; set; }

        [DefaultValue("10")]
        public int DefaultExternalPagingPageSize { get; set; }

        [DefaultValue(false)]
        public bool ProxyEnabled { get; set; }

        [DefaultValue(false)]
        public bool ByPassProxyOnLocal { get; set; }

        public string ProxyWebAddress { get; set; }

        [DefaultValue(false)]
        public bool ProxyUseDefaultCredentials { get; set; }

        public string CompaniesHouseReferencePath { get; set; }

        public string CompaniesHouseBaseUrl { get; set; }

        public string AddressLookupReferencePath { get; set; }

        public string AddressLookupBaseUrl { get; set; }

        public string GovUkPayBaseUrl { get; set; }

        public string GovUkPayApiKey { get; set; }

        public string GovUkPayReturnBaseUrl { get; set; }

        public string GovUkPayDescription { get; set; }

        [DefaultValue(3000)]
        public int GovUkPayAmountInPence { get; set; }

        public string GovUkPayTokenSecret { get; set; }

        public string GovUkPayTokenSalt { get; set; }

        public TimeSpan GovUkPayTokenLifeTime { get; set; }

        public string OAuthTokenEndpoint { get; set; }

        public string OAuthTokenClientId { get; set; }

        public string OAuthTokenClientSecret { get; set; }

        public string CompaniesHouseScope { get; set; }

        public string AddressLookupScope { get; set; }

        [DefaultValue("01/01/2025")]
        public DateTime SmallProducerFeatureEnabledFrom { get; set; }
    }
}