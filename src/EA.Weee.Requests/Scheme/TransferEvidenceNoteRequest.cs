namespace EA.Weee.Requests.Scheme
{
    using System;
    using System.Collections.Generic;
    using AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    [Serializable]
    public class TransferEvidenceNoteRequest : IRequest<Guid>
    {
        public TransferEvidenceNoteRequest()
        { 
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid recipientId, 
            List<int> categoryIds,
            int selectedComplianceYear)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(recipientId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();

            OrganisationId = organisationId;
            RecipientId = recipientId;
            CategoryIds = categoryIds;
            SelectedComplianceYear = selectedComplianceYear;
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid recipientId,
            List<int> categoryIds,
            List<Guid> evidenceNoteIds)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(recipientId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();
            Condition.Requires(evidenceNoteIds).IsNotEmpty().IsNotNull();

            OrganisationId = organisationId;
            RecipientId = recipientId;
            CategoryIds = categoryIds;
            EvidenceNoteIds = evidenceNoteIds;
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid recipientId,
            List<int> categoryIds,
            List<TransferTonnageValue> transferValues,
            List<Guid> evidenceNoteIds,
            Core.AatfEvidence.NoteStatus status)
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
        }

        public Guid RecipientId { get; set; } 

        public List<int> CategoryIds { get; set; }

        public List<Guid> EvidenceNoteIds { get; set; }

        public Guid OrganisationId { get; set; }

        public EA.Weee.Core.AatfEvidence.NoteStatus Status { get; set; }

        public List<TransferTonnageValue> TransferValues { get; set; }

        public int SelectedComplianceYear { get; set; }
    }
}
