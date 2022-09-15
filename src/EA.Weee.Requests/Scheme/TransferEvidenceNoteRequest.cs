namespace EA.Weee.Requests.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    [Serializable]
    public class TransferEvidenceNoteRequest : IRequest<Guid>
    {
        public TransferEvidenceNoteRequest()
        {
            EvidenceNoteIds = new List<Guid>();
            DeselectedEvidenceNoteIds = new List<Guid>();
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid recipientId, 
            List<int> categoryIds)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(recipientId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();

            OrganisationId = organisationId;
            RecipientId = recipientId;
            CategoryIds = categoryIds;
            DeselectedEvidenceNoteIds = new List<Guid>();
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid recipientId,
            List<int> categoryIds,
            List<Guid> evidenceNoteIds)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(recipientId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();

            OrganisationId = organisationId;
            RecipientId = recipientId;
            CategoryIds = categoryIds;
            EvidenceNoteIds = evidenceNoteIds;
            DeselectedEvidenceNoteIds = new List<Guid>();
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid recipientId,
            List<int> categoryIds,
            List<TransferTonnageValue> transferValues,
            List<Guid> evidenceNoteIds,
            Core.AatfEvidence.NoteStatus status,
            int complianceYear)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(recipientId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();
            Condition.Requires(transferValues).IsNotNull();
            Condition.Requires(evidenceNoteIds).IsNotNull();

            OrganisationId = organisationId;
            RecipientId = recipientId;
            TransferValues = transferValues;
            Status = status;
            CategoryIds = categoryIds;
            EvidenceNoteIds = evidenceNoteIds;
            ComplianceYear = complianceYear;
            DeselectedEvidenceNoteIds = new List<Guid>();
        }

        public Guid RecipientId { get; set; } 

        public List<int> CategoryIds { get; set; }

        public List<Guid> EvidenceNoteIds { get; set; }

        public List<Guid> DeselectedEvidenceNoteIds { get; set; }

        public Guid OrganisationId { get; set; }

        public EA.Weee.Core.AatfEvidence.NoteStatus Status { get; set; }

        public List<TransferTonnageValue> TransferValues { get; set; }

        public int ComplianceYear { get; set; }

        public void UpdateSelectedNotes(List<Guid> selectedNotes)
        {
            selectedNotes.RemoveAll(s => DeselectedEvidenceNoteIds.Contains(s));

            EvidenceNoteIds = EvidenceNoteIds.Union(selectedNotes.Distinct()).ToList();
            //EvidenceNoteIds = new List<Guid>(selectedNotes.Distinct());
        }
    }
}
