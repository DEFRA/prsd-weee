namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetOrganisationSchemeDataForFilterRequest : IRequest<List<EntityIdDisplayNameData>>
    {
        public Guid? AatfId { get; private set; }

        public int ComplianceYear { get; private set; }
    
        public GetOrganisationSchemeDataForFilterRequest(Guid? aatfId, int complianceYear)
        {
            AatfId = aatfId;
            ComplianceYear = complianceYear;
        }
    }
}
