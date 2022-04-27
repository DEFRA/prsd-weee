namespace EA.Weee.Requests.Scheme
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

        public Guid SchemeId { get; set; } 

        public IList<int> CategoryIds { get; set; }
    }
}
