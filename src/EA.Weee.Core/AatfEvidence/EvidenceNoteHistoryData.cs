namespace EA.Weee.Core.AatfEvidence
{
    using System;

    public class EvidenceNoteHistoryData : EvidenceNoteDataBase
    {
        public string TransferredTo { get; set; }

        public EvidenceNoteHistoryData(Guid id, NoteStatus status, int reference, NoteType type, DateTime? submittedDate, string transferredTo)
        {
            Id = id;
            Status = status;
            Reference = reference;
            Type = type;
            SubmittedDate = submittedDate;
            TransferredTo = transferredTo;
        }
    }
}
