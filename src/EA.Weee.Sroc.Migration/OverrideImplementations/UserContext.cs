﻿namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Security.Claims;
    using Prsd.Core.Domain;

    public class UserContext : IUserContext
    {
        public Guid UserId { get; }

        public ClaimsPrincipal Principal { get; }
    }
}
