namespace EA.Weee.Requests.Scheme
{
    using System;
    using System.Collections.Generic;
    using AatfEvidence;
    using CuttingEdge.Conditions;

    public class EditTransferEvidenceNoteRequest : TransferEvidenceNoteRequest
    {
        public Guid TransferNoteId { get; }

        public EditTransferEvidenceNoteRequest(Guid transferNoteId,
            Guid organisationId,
            Guid schemeId,
            List<int> categoryIds,
            List<TransferTonnageValue> transferValues,
            List<Guid> evidenceNoteIds,
            Core.AatfEvidence.NoteStatus status) : base(organisationId, schemeId, categoryIds, transferValues, evidenceNoteIds, status)
        {
            Condition.Requires(transferNoteId).IsNotEqualTo(Guid.Empty);

            TransferNoteId = transferNoteId;
        }
    }
}
