namespace EA.Weee.Requests.Users
{
    using System;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;
    using Core.Organisations;

    public class GetUsersByOrganisationId : IRequest<List<OrganisationUserData>>
    {
        public Guid OrganisationId { get; set; }

        public GetUsersByOrganisationId(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
