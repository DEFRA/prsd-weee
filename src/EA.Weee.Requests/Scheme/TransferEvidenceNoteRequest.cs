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
            Guid schemeId, 
            List<int> categoryIds)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();

            OrganisationId = organisationId;
            SchemeId = schemeId;
            CategoryIds = categoryIds;
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid schemeId,
            List<int> categoryIds,
            List<Guid> evidenceNoteIds)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();
            Condition.Requires(evidenceNoteIds).IsNotEmpty().IsNotNull();

            OrganisationId = organisationId;
            SchemeId = schemeId;
            CategoryIds = categoryIds;
            EvidenceNoteIds = evidenceNoteIds;
        }

        public TransferEvidenceNoteRequest(Guid organisationId,
            Guid schemeId,
            List<int> categoryIds,
            List<TransferTonnageValue> transferValues,
            Core.AatfEvidence.NoteStatus status)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();
            Condition.Requires(transferValues).IsNotNull();

            OrganisationId = organisationId;
            SchemeId = schemeId;
            TransferValues = transferValues;
            Status = status;
            CategoryIds = categoryIds;
        }

        public Guid SchemeId { get; set; } 

        public List<int> CategoryIds { get; set; }

        public List<Guid> EvidenceNoteIds { get; set; }

        public Guid OrganisationId { get; set; }

        public EA.Weee.Core.AatfEvidence.NoteStatus Status { get; set; }

        public List<TransferTonnageValue> TransferValues { get; set; }
    }
}
