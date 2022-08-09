namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Extensions;
    using System;
    using System.ComponentModel;

    public class EvidenceNoteHistoryViewModel
    {
        public Guid Id { get; set; }
        public int Reference { get; set; }
        public string TransferredTo { get; set; }
        [DisplayName("Reference ID")]
        public string ReferenceDisplay => $"{Type.ToDisplayString()}{Reference}";
        public NoteType Type { get; set; }
        public NoteStatus Status { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string SubmittedDateDisplay => SubmittedDate != null ? SubmittedDate.ToDisplayGMTDateTimeString() : "-";

        public EvidenceNoteHistoryViewModel(Guid id, int reference, string transferredTo, NoteType type, NoteStatus status, DateTime? submittedDate)
        {
            Id = id;
            Reference = reference;
            TransferredTo = transferredTo;
            Type = type;
            Status = status;
            SubmittedDate = submittedDate;
        }
    }
}