namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class GetEvidenceNotesSelectedForTransferRequest : IRequest<EvidenceNoteSearchDataResult>
    {
        public Guid OrganisationId { get; private set; }

        public List<Guid> EvidenceNotes { get; private set; }

        public List<int> Categories { get; private set; }

        public GetEvidenceNotesSelectedForTransferRequest(Guid organisationId,
            List<Guid> evidenceNotes,
            List<int> categories)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            OrganisationId = organisationId;
            Categories = categories ?? new List<int>();
            EvidenceNotes = evidenceNotes ?? new List<Guid>();
        }
    }
}
