namespace EA.Weee.RequestHandlers.Security
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core;
    using System;
    using System.Linq;
    using System.Security;
    using System.Security.Claims;
    using DataAccess;
    using Domain;

    /// <summary>
    /// Provides evaluation of claims-based authorisation for WEEE resources.
    /// </summary>
    public class WeeeAuthorization : IWeeeAuthorization
    {
        private readonly WeeeContext context;

        private readonly IUserContext userContext;

        public WeeeAuthorization(WeeeContext context, IUserContext userContext)
        {
            Guard.ArgumentNotNull(() => context, context);
            Guard.ArgumentNotNull(() => userContext, userContext);

            this.context = context;
            this.userContext = userContext;
        }

        /// <summary>
        /// Ensures that the user can access the internal area.
        /// </summary>
        public void EnsureCanAccessInternalArea()
        {
            EnsureCanAccessInternalArea(true);
        }

        /// <summary>
        /// Ensures that the user can access the internal area.
        /// </summary>
        public void EnsureCanAccessInternalArea(bool requiresActiveUser)
        {
            bool canAccessInternalArea = CheckCanAccessInternalArea(requiresActiveUser);

            if (!canAccessInternalArea)
            {
                string message = "The user is not authorised to access the internal area.";
                throw new SecurityException(message);
            }
        }

        /// <summary>
        /// Checks that the user can access the internal area.
        /// </summary>
        public bool CheckCanAccessInternalArea()
        {
            return CheckCanAccessInternalArea(true);
        }

        /// <summary>
        /// Checks that the user can access the internal area.
        /// </summary>
        public bool CheckCanAccessInternalArea(bool requiresActiveUser)
        {
            Claim claim = new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea);

            var userId = userContext.UserId.ToString();

            return HasClaim(claim) &&
                   (!requiresActiveUser || context.CompetentAuthorityUsers.Single(u => u.UserId == userId).UserStatus == UserStatus.Active);
        }

        /// <summary>
        /// Ensures that the user can access the external area.
        /// </summary>
        public void EnsureCanAccessExternalArea()
        {
            bool canAccessExternalArea = CheckCanAccessExternalArea();

            if (!canAccessExternalArea)
            {
                string message = "The user is not authorised to access the external area.";
                throw new SecurityException(message);
            }
        }

        /// <summary>
        /// Checks that the user can access the external area.
        /// </summary>
        public bool CheckCanAccessExternalArea()
        {
            Claim claim = new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessExternalArea);

            return HasClaim(claim);
        }

        /// <summary>
        /// Ensures that the principal represents a user with a claim
        /// granting them access to the specified organisation.
        /// </summary>
        public void EnsureOrganisationAccess(Guid organisationId)
        {
            bool access = CheckOrganisationAccess(organisationId);

            if (!access)
            {
                string message = string.Format(
                    "The user does not have access to the organisation with ID \"{0}\".",
                    organisationId);

                throw new SecurityException(message);
            }
        }

        /// <summary>
        /// Checks that the principal represents a user with a claim
        /// granting them access to the specified organisation.
        /// </summary>
        public bool CheckOrganisationAccess(Guid organisationId)
        {
            var userId = userContext.UserId.ToString();

            return
                context.OrganisationUsers.Any(
                    ou => ou.OrganisationId == organisationId
                       && ou.UserId == userId
                       && ou.UserStatus.Value == UserStatus.Active.Value);
        }

        /// <summary>
        /// Ensures that the principal represents a user with a claim
        /// granting them access to the specified scheme.
        /// </summary>
        public void EnsureSchemeAccess(Guid schemeId)
        {
            bool access = CheckSchemeAccess(schemeId);

            if (!access)
            {
                string message = string.Format(
                    "The user does not have access to the scheme with ID \"{0}\".",
                    schemeId);

                throw new SecurityException(message);
            }
        }

        /// <summary>
        /// Checks that the principal represents a user with a claim
        /// granting them access to the specified scheme.
        /// </summary>
        public bool CheckSchemeAccess(Guid schemeId)
        {
            var organisationId = context.Schemes.Where(s => s.Id == schemeId).Select(s => s.OrganisationId).FirstOrDefault();

            if (organisationId == Guid.Empty)
            {
                return false;
            }

            var userId = userContext.UserId.ToString();

            return
                context.OrganisationUsers.Any(
                    ou => ou.OrganisationId == organisationId
                       && ou.UserId == userId
                       && ou.UserStatus.Value == UserStatus.Active.Value);
        }

        /// <summary>
        /// Ensures that the principal represents a user with a claim
        /// granting them access to the internal area or the specified
        /// organisation.
        /// </summary>
        public void EnsureInternalOrOrganisationAccess(Guid organisationId)
        {
            bool access = CheckInternalOrOrganisationAccess(organisationId);

            if (!access)
            {
                string message = string.Format(
                    "The user does not have access to the internal area or the organisation with ID \"{0}\".",
                    organisationId);

                throw new SecurityException(message);
            }
        }

        /// <summary>
        /// Checks that the principal represents a user with a claim
        /// granting them access to the internal area or the specified
        /// organisation.
        /// </summary>
        public bool CheckInternalOrOrganisationAccess(Guid organisationId)
        {
            return CheckCanAccessInternalArea() || CheckOrganisationAccess(organisationId);
        }

        private bool HasClaim(Claim claim)
        {
            foreach (ClaimsIdentity identity in userContext.Principal.Identities)
            {
                if (identity.HasClaim(claim.Type, claim.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
