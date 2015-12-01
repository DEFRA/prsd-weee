namespace EA.Weee.Requests.Admin
{
    using System;
    using Prsd.Core.Mediator;

    public class UpdateUser : IRequest<Guid>
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UpdateUser(string userId, string firstName, string lastName)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
