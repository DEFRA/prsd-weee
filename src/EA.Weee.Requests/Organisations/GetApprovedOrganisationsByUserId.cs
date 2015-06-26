namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetApprovedOrganisationsByUserId : IRequest<List<OrganisationUserData>>
    {
        public string UserId { get; set; }

        public GetApprovedOrganisationsByUserId(string userId)
        {
            UserId = userId;
        }
    }
}
