namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class GetEvidenceNotesForTransferRequest : IRequest<EvidenceNoteSearchDataResult>
    {
        public Guid OrganisationId { get; private set; }

        public List<int> Categories { get; private set; }

        public List<Guid> ExcludeEvidenceNotes { get; private set; }

        public int ComplianceYear { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string SearchReference { get; private set; }

        public Guid? TransferNoteId { get; private set; }

        public GetEvidenceNotesForTransferRequest(Guid organisationId, 
            List<int> categories, 
            int complianceYear,
            List<Guid> excludeEvidenceNotes,
            string searchReference = null,
            int pageNumber = 1,
            int pageSize = 10,
            Guid? transferNoteId = null)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categories).IsNotEmpty();

            OrganisationId = organisationId;
            Categories = categories;
            ComplianceYear = complianceYear;
            ExcludeEvidenceNotes = excludeEvidenceNotes ?? new List<Guid>();
            SearchReference = searchReference;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TransferNoteId = transferNoteId;
        }
    }
}
