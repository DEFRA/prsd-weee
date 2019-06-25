namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using System;
    using System.Collections.Generic;

    public class GetActiveOrganisationUsers : IRequest<IEnumerable<OrganisationUserData>>
    {
        public Guid OrganisationId;

        public GetActiveOrganisationUsers(Guid orgId)
        {
            OrganisationId = orgId;
        }
    }
}
