namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class GetEvidenceNotesForTransferRequest : IRequest<EvidenceNoteSearchDataResult>
    {
        public Guid OrganisationId { get; protected set; }

        public List<int> Categories { get; private set; }

        public List<Guid> EvidenceNotes { get; private set; }

        public int ComplianceYear { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public GetEvidenceNotesForTransferRequest(Guid organisationId, 
            List<int> categories, 
            int complianceYear, 
            List<Guid> evidenceNotes = null,
            int pageNumber = 1,
            int pageSize = 25)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categories).IsNotEmpty();

            OrganisationId = organisationId;
            Categories = categories;
            ComplianceYear = complianceYear;
            EvidenceNotes = evidenceNotes ?? new List<Guid>();
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
