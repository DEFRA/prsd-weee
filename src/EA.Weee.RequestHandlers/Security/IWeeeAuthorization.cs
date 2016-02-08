namespace EA.Weee.RequestHandlers.Security
{
    using System;
    using Core.Security;

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
        /// Ensures that the user can access the internal area.
        /// </summary>
        void EnsureCanAccessInternalArea(bool requiresActiveUser);

        /// <summary>
        /// Checks that the user can access the internal area.
        /// </summary>
        bool CheckCanAccessInternalArea();

        /// <summary>
        /// Checks that the user can access the internal area.
        /// </summary>
        bool CheckCanAccessInternalArea(bool requiresActiveUser);

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

        /// <summary>
        /// Checks that the principal represents a user with
        /// the specified role.
        /// </summary>
        bool CheckUserInRole(Roles role);

        /// <summary>
        /// Ensures that the principal represents a user with
        /// the specified role.
        /// </summary>
        void EnsureUserInRole(Roles role);

        /// <summary>
        /// Ensures that the principal represents a user with a claim
        /// granting them access to the specified scheme.
        /// </summary>
        /// <param name="schemeId"></param>
        void EnsureInternalOrSchemeAccess(Guid schemeId);

        /// <summary>
        /// Checks that the principal represents a user with a claim
        /// granting them access to the specified scheme.
        /// </summary>
        bool CheckInternalOrSchemeAccess(Guid schemeId);
    }
}
