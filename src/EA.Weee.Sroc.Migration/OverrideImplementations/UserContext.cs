namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Security.Claims;
    using Prsd.Core.Domain;

    public class UserContext : IUserContext
    {
        public Guid UserId
        {
            get { return Guid.Empty; }
            set { UserId = Guid.Empty; }
        }

        public ClaimsPrincipal Principal
        {
            get { return null; }
            set { Principal = null; }
        }
    }
}
