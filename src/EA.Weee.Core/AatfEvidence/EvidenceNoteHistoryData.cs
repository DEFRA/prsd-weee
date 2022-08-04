namespace EA.Weee.Core.AatfEvidence
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using AatfReturn;
    using Organisations;
    using Prsd.Core;

    public class EvidenceNoteHistoryData : EvidenceNoteDataBase
    {
        public EvidenceNoteHistoryData(Guid id, NoteStatus status, int reference, NoteType type, DateTime? submittedDate)
        {
            Id = id;
            Status = status;
            Reference = reference;
            Type = type;
            SubmittedDate = submittedDate;
        }
    }
}
