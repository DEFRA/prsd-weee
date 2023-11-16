namespace EA.Weee.Requests.AatfEvidence
{
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Weee.Core.DataReturns;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetTransferEvidenceNoteAvailableCategoriesRequest : IRequest<List<WeeeCategory>>
    {
        public Guid OrganisationId { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetTransferEvidenceNoteAvailableCategoriesRequest(Guid organisationId, int complianceYear)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}