namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Security.Claims;
    using DataAccess;
    using Prsd.Core.Domain;
    public class UserContext : IUserContext
    {
        public Guid UserId => Guid.Parse("0311F8F5-6E14-490C-99CF-635D4E992C3A");

        public ClaimsPrincipal Principal { get; }
    }
}
