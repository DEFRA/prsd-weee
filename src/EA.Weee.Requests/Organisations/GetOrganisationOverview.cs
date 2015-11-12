namespace EA.Weee.Requests.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Organisations;
    using EA.Prsd.Core.Mediator;

    public class GetOrganisationOverview : IRequest<OrganisationOverview>
    {
        public Guid OrganisationId { get; set; }

        public GetOrganisationOverview(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
