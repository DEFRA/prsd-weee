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

        public TestUserContext(Guid userId, bool asExternal)
        {
            UserId = userId;
            var identity = new ClaimsIdentity(new List<Claim>(), "integration");
            if (asExternal)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessExternalArea));
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
