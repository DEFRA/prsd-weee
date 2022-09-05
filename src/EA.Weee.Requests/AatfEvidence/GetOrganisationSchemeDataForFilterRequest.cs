namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetOrganisationSchemeDataForFilterRequest : IRequest<List<OrganisationSchemeData>>
    {
        public Guid OrganisationId { get; private set; }

        public int ComplianceYear { get; private set; }
    
        public GetOrganisationSchemeDataForFilterRequest(Guid organisationId, int complianceYear)
        {
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
