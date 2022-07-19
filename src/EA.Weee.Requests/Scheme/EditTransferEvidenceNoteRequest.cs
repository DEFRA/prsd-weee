namespace EA.Weee.Requests.Scheme
{
    using System;
    using System.Collections.Generic;
    using AatfEvidence;
    using CuttingEdge.Conditions;

    public class EditTransferEvidenceNoteRequest : TransferEvidenceNoteRequest
    {
        public EditTransferEvidenceNoteRequest()
        {
        }
        public Guid TransferNoteId { get; }

        public EditTransferEvidenceNoteRequest(Guid transferNoteId,
            Guid organisationId,
            Guid recipientId,
            List<TransferTonnageValue> transferValues,
            Core.AatfEvidence.NoteStatus status)
        {
            Condition.Requires(transferNoteId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(recipientId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(transferValues).IsNotNull();

            OrganisationId = organisationId;
            RecipientId = recipientId;
            TransferValues = transferValues;
            Status = status;
            TransferNoteId = transferNoteId;
        }
    }
}
