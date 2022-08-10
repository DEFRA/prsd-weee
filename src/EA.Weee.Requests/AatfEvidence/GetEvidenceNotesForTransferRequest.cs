namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class GetEvidenceNotesForTransferRequest : IRequest<IList<EvidenceNoteData>>
    {
        public Guid OrganisationId { get; protected set; }

        public List<int> Categories { get; private set; }

        public List<Guid> EvidenceNotes { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetEvidenceNotesForTransferRequest(Guid organisationId, List<int> categories, int complianceYear, List<Guid> evidenceNotes = null)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categories).IsNotEmpty();

            OrganisationId = organisationId;
            Categories = categories;
            ComplianceYear = complianceYear;
            EvidenceNotes = evidenceNotes ?? new List<Guid>();
        }
    }
}
