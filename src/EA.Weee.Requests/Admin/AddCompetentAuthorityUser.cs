namespace EA.Weee.Requests.Admin
{
    using System;
    using Prsd.Core.Mediator;

    public class AddCompetentAuthorityUser : IRequest<Guid>
    {
        public AddCompetentAuthorityUser(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}