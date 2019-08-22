namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;
    using System;

    public class AddCompetentAuthorityUser : IRequest<Guid>
    {
        public AddCompetentAuthorityUser(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}