namespace EA.Weee.Requests.Users.GetManageableOrganisationUsers
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetManageableOrganisationUsers : IRequest<List<OrganisationUserData>>
    {
        public Guid OrganisationId { get; set; }

        public GetManageableOrganisationUsers(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
