namespace EA.Weee.Integration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Prsd.Core.Domain;

    public class TestUserContext : IUserContext
    {
        public ClaimsPrincipal BackingPrincipal;

        public TestUserContext(Guid userId)
        {
            UserId = userId;

            var identity = new ClaimsIdentity(new List<Claim>(), "integration");

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
