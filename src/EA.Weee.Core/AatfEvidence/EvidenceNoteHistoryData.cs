namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.Collections.Generic;

    public class EvidenceNoteHistoryData : EvidenceNoteDataBase
    {
        public string TransferredTo { get; set; }

        public IList<EvidenceTonnageData> TransferEvidenceTonnageData { get; set; }

        public EvidenceNoteHistoryData(Guid id, NoteStatus status, int reference, NoteType type, DateTime? submittedDate, string transferredTo, IList<EvidenceTonnageData> transferEvidenceTonnageData)
        {
            Id = id;
            Status = status;
            Reference = reference;
            Type = type;
            SubmittedDate = submittedDate;
            TransferredTo = transferredTo;
            TransferEvidenceTonnageData = transferEvidenceTonnageData;
        }
    }
}
