namespace EA.Weee.Core.Security
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Security;
    using EA.Weee.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides evaluation of claims-based authorisation for WEEE resources.
    /// </summary>
    public class WeeeAuthorization : IWeeeAuthorization
    {
        private readonly IUserContext userContext;

        public WeeeAuthorization(IUserContext userContext)
        {
            Guard.ArgumentNotNull(() => userContext, userContext);

            this.userContext = userContext;
        }

        /// <summary>
        /// Ensures that the user can access the internal area.
        /// </summary>
        public void EnsureCanAccessInternalArea()
        {
            bool canAccessInternalArea = CheckCanAccessInternalArea();

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
            Claim claim = new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea);

            return HasClaim(claim);
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
            Claim claim = new Claim(WeeeClaimTypes.OrganisationAccess, organisationId.ToString());

            return HasClaim(claim);
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
            Claim claim = new Claim(WeeeClaimTypes.SchemeAccess, schemeId.ToString());

            return HasClaim(claim);
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
