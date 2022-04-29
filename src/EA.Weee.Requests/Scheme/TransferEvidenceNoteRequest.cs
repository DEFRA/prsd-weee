﻿namespace EA.Weee.Requests.Scheme
{
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    [Serializable]
    public class TransferEvidenceNoteRequest 
    {
        public TransferEvidenceNoteRequest()
        { 
        }

        public TransferEvidenceNoteRequest(Guid schemeId, 
            List<int> categoryIds)
        {
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();

            SchemeId = schemeId;
            CategoryIds = categoryIds;
        }

        public TransferEvidenceNoteRequest(Guid schemeId,
            List<int> categoryIds,
            List<Guid> evidenceNoteIds)
        {
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categoryIds).IsNotEmpty().IsNotNull();
            Condition.Requires(evidenceNoteIds).IsNotEmpty().IsNotNull();

            SchemeId = schemeId;
            CategoryIds = categoryIds;
            EvidenceNoteIds = evidenceNoteIds;
        }

        public Guid SchemeId { get; set; } 

        public List<int> CategoryIds { get; set; }

        public List<Guid> EvidenceNoteIds { get; set; }
    }
}
