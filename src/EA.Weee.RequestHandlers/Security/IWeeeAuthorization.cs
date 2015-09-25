﻿namespace EA.Weee.RequestHandlers.Security
{
    using System;

    /// <summary>
    /// Provides evaluation of claims-based authorisation for WEEE resources.
    /// </summary>
    public interface IWeeeAuthorization
    {
        /// <summary>
        /// Ensures that the user can access the internal area.
        /// </summary>
        void EnsureCanAccessInternalArea();

        /// <summary>
        /// Checks that the user can access the internal area.
        /// </summary>
        bool CheckCanAccessInternalArea();

        /// <summary>
        /// Ensures that the user can access the external area.
        /// </summary>
        void EnsureCanAccessExternalArea();

        /// <summary>
        /// Checks that the user can access the external area.
        /// </summary>
        bool CheckCanAccessExternalArea();

        /// <summary>
        /// Ensures that the principal represents a user with a claim
        /// granting them access to the specified organisation.
        /// </summary>
        void EnsureOrganisationAccess(Guid organisationId);

        /// <summary>
        /// Checks that the principal represents a user with a claim
        /// granting them access to the specified organisation.
        /// </summary>
        bool CheckOrganisationAccess(Guid organisationId);

        /// <summary>
        /// Ensures that the principal represents a user with a claim
        /// granting them access to the specified organisation.
        /// </summary>
        void EnsureInternalOrOrganisationAccess(Guid organisationId);

        /// <summary>
        /// Checks that the principal represents a user with a claim
        /// granting them access to the specified organisation.
        /// </summary>
        bool CheckInternalOrOrganisationAccess(Guid organisationId);

        /// <summary>
        /// Ensures that the principal represents a user with a claim
        /// granting them access to the specified scheme.
        /// </summary>
        void EnsureSchemeAccess(Guid schemeId);

        /// <summary>
        /// Checks that the principal represents a user with a claim
        /// granting them access to the specified scheme.
        /// </summary>
        bool CheckSchemeAccess(Guid schemeId);
    }
}
