namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;
    using Core.Organisations;

    public class GetOrganisationsByUserId : IRequest<List<OrganisationUserData>>
    {
        public string UserId { get; set; }

        public int[] OrganisationUserStatus { get; set; }

        public GetOrganisationsByUserId(string userId, int[] organisationUserStatus)
        {
            UserId = userId;
            OrganisationUserStatus = organisationUserStatus;
        }
    }
}
