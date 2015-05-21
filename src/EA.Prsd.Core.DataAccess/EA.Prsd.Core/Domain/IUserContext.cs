namespace EA.Prsd.Core.Domain
{
    using System;

    public interface IUserContext
    {
        Guid UserId { get; }
    }
}