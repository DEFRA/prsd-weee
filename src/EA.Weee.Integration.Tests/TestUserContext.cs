namespace EA.Weee.Integration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Prsd.Core.Domain;
    using Security;

    public class TestUserContext : IUserContext
    {
        public ClaimsPrincipal BackingPrincipal;

        public TestUserContext(Guid userId, bool asExternal, bool asInternalAdmin = false)
        {
            UserId = userId;
            var identity = new ClaimsIdentity(new List<Claim>(), "integration");
            if (asExternal)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessExternalArea));
            }
            else
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));
                if (asInternalAdmin)
                {
                    identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.InternalAdmin));
                }
            }

            BackingPrincipal = new ClaimsPrincipal(identity);
        }

        public TestUserContext(ClaimsIdentity identity)
        {
            BackingPrincipal = new ClaimsPrincipal(identity);
        }

        public Guid UserId { get; }

        public ClaimsPrincipal Principal => BackingPrincipal;
    }
}
