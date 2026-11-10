namespace EA.Weee.Requests.Users
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class GetUserPasswordHistory : IRequest<bool>
    {
        public Guid UserId;

        public string Password;

        public GetUserPasswordHistory(Guid userId, string password)
        {
            UserId = userId;
            Password = password;
        }
    }
}
