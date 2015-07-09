namespace EA.Weee.Requests.NewUser
{
    using System;
    using Core.NewUser;
    using Prsd.Core.Mediator;

    public class UserById : IRequest<UserData>
    {
        public readonly string Id;

        public UserById(string id)
        {
            Id = id;
        }

        public UserById(Guid id)
        {
            Id = id.ToString();
        }
    }
}