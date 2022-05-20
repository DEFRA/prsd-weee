namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using Core.AatfEvidence;
    using Core.Helpers;

    public class EvidenceNoteRowViewModel 
    {
        public int ReferenceId { get; set; }

        public string Recipient { get; set; }

        public WasteType? TypeOfWaste { get; set; }

        public NoteStatus Status { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string ReturnedReason { get; set; }

        public string RejectedReason { get; set; }

        public string SubmittedBy { get; set; }

        public Guid Id { get; set; }

        public NoteType Type { get; set; }

        public bool DisplayViewLink { get; set; }

        public string ReferenceDisplay => $"{Type.ToDisplayString()}{ReferenceId}";

        public string SubmittedDateDisplay => SubmittedDate.HasValue ? SubmittedDate.Value.ToShortDateString() : "-";
    }
}
