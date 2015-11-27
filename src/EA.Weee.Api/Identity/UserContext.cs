namespace EA.Weee.Api.Identity
{
    using Prsd.Core.Domain;
    using System;
    using System.Security.Claims;
    using System.Web;

    public class UserContext : IUserContext
    {
        public Guid UserId
        {
            get
            {
                var claimsPrincipal = HttpContext.Current.User as ClaimsPrincipal;

                if (claimsPrincipal != null)
                {
                    foreach (var identity in claimsPrincipal.Identities)
                    {
                        if (identity.AuthenticationType.Equals("BEARER", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var idClaim = identity.FindFirst("sub");

                            if (idClaim != null)
                            {
                                return Guid.Parse(idClaim.Value);
                            }
                        }
                    }
                }

                return Guid.Empty;
            }
        }

        public ClaimsPrincipal Principal
        {
            get { return HttpContext.Current.User as ClaimsPrincipal; }
        }
    }
}