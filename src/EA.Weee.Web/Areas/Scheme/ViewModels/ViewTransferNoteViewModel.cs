namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.ComponentModel;
    using Core.AatfEvidence;
    using Core.Helpers;
    using ManageEvidenceNotes;

    public class ViewTransferNoteViewModel
    {
        public Guid SchemeId { get; set; }

        public int Reference { get; set; }

        public NoteType Type { get; set; }

        public NoteStatus Status { get; set; }

        [DisplayName("Reference ID")]
        public string ReferenceDisplay => $"{Type.ToDisplayString()}{Reference}";

        public string SuccessMessage { get; set; }

        public bool DisplayMessage => !string.IsNullOrWhiteSpace(SuccessMessage);

        public ManageEvidenceNotesDisplayOptions RedirectTab
        {
            get
            {
                if (Status.Equals(NoteStatus.Draft))
                {
                    return ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence;
                }

                //TODO: this will get updated when viewing and editing of transfer notes is added
                return ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence;
            }
        }
    }
}