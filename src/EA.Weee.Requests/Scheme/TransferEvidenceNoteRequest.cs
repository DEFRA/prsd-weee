namespace EA.Weee.Requests.Scheme
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;

    [Serializable]
    public class TransferEvidenceNoteRequest 
    {
        public TransferEvidenceNoteRequest()
        { 
        }

        public TransferEvidenceNoteRequest(Guid schemeId, 
            List<int> categoryIds)
        {
            Guard.ArgumentNotDefaultValue(() => schemeId, schemeId);
            Guard.ArgumentNotNull(() => categoryIds, categoryIds);

            SchemeId = schemeId;
            CategoryIds = categoryIds;
        }

        public Guid SchemeId { get; set; } 

        public IList<int> CategoryIds { get; set; }
    }
}
