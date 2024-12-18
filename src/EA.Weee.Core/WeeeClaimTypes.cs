﻿namespace EA.Weee.Core
{
    /// <summary>
    /// Defines the list of domain-specific resources/properties which can be represented
    /// by claims within WEEE.
    /// 
    /// Note: If a new claim type is added here, consider adding it to EA.Weee.Api.IdSrv.Scopes
    /// </summary>
    public static class WeeeClaimTypes
    {
        /// <summary>
        /// Represents the right to access resources associated with a specific organisation.
        /// The value of this claim is a string representation of the organisation's GUID.
        /// </summary>
        public const string OrganisationAccess = "http://www.environment-agency.gov.uk/WEEE/identity/claims/organisation-access";

        /// <summary>
        /// Represents the right to access resources associated with a specific scheme.
        /// The value of this claim is a string representation of the scheme's GUID.
        /// </summary>
        public const string SchemeAccess = "http://www.environment-agency.gov.uk/WEEE/identity/claims/scheme-access";

        /// <summary>
        /// Represents the right to access resources associated with a specific direct registrant.
        /// The value of this claim is a string representation of the direct registrant's GUID.
        /// </summary>
        public const string DirectRegistrantAccess = "http://www.environment-agency.gov.uk/WEEE/identity/claims/direct-registrant-access";
    }
}
