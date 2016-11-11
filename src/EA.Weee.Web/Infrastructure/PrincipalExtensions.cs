namespace EA.Weee.Web.Infrastructure
{
    using System.Security.Claims;
    using System.Security.Principal;
    using IdentityModel;

    public static class PrincipalExtensions
    {
        public static string GetUserId(this IPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                foreach (var identity in claimsPrincipal.Identities)
                {
                    if (identity.AuthenticationType == Constants.WeeeAuthType)
                    {
                        var idClaim = identity.FindFirst(JwtClaimTypes.Subject);

                        if (idClaim != null)
                        {
                            return idClaim.Value;
                        }
                    }
                }
            }

            return null;
        }

        public static string GetAccessToken(this IPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal != null)
            {
                return GetClaimValue(claimsPrincipal, OidcConstants.AuthorizeResponse.AccessToken);
            }

            return null;
        }

        public static string GetClaimValue(this ClaimsPrincipal principal, string type)
        {
            Claim claim = principal.FindFirst(type);

            return claim != null ? claim.Value : null;
        }

        public static string GetEmailAddress(this IPrincipal principal)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal != null)
            {
                return GetClaimValue(claimsPrincipal, JwtClaimTypes.Email);
            }
            return null;
        }
    }
}