namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;

    public class SchemeViewAndTransferManageEvidenceSchemeViewModel : ManageManageEvidenceNoteSchemeViewModel
    {
        public Guid? SelectedId { get; set; }

        public bool DisplayTransferButton { get; set; }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel()
            : base(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence)
        {
        }
    }
}