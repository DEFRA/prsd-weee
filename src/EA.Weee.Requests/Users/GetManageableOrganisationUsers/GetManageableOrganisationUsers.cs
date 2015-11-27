namespace EA.Weee.Requests.Users.GetManageableOrganisationUsers
{
    using System;
    using System.Collections.Generic;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class GetManageableOrganisationUsers : IRequest<List<OrganisationUserData>>
    {
        public Guid OrganisationId { get; set; }

        public GetManageableOrganisationUsers(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
