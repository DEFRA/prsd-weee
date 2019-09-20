namespace EA.Prsd.Core.Domain
{
    using System;
    using System.Security.Claims;

    public interface IUserContext
    {
        Guid UserId { get; }
        ClaimsPrincipal Principal { get; }
    }
}