namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using Core.Helpers;
    using EA.Weee.Core.AatfEvidence;

    public class EditDraftReturnedNote 
    {
        public int ReferenceId { get; set; }

        public string Recipient { get; set; }

        public WasteType? TypeOfWaste { get; set; }

        public NoteStatus Status { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string SubmittedBy { get; set; }

        public Guid Id { get; set; }

        public NoteType Type { get; set; }

        public bool DisplayViewLink { get; set; }

        public string ReferenceDisplay => $"{Type.ToDisplayString()}{ReferenceId}";

        public string SubmittedDateDisplay => SubmittedDate.HasValue ? SubmittedDate.Value.ToShortDateString() : "-";
    }
}
