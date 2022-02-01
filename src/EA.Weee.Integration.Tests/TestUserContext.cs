using System;

namespace EA.Weee.Integration.Tests
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Prsd.Core.Domain;

    public class TestUserContext : IUserContext
    {
        public TestUserContext(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; }

        public ClaimsPrincipal Principal
        {
            get
            {
                var identity = new ClaimsIdentity(new List<Claim>(), "integration");
                
                var principal = new ClaimsPrincipal(identity);
                
                return principal;
            }
        }
    }
}
