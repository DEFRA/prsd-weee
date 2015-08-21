namespace EA.Weee.Requests.Users
{
    using System;
    using Core.Users;
    using Prsd.Core.Mediator;

    public class GetUserData : IRequest<UserData>
    {
        public readonly string Id;

        public GetUserData(string id)
        {
            Id = id;
        }
    }
}