namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using Web.ViewModels.Shared;

    public class TransferEvidenceNotesViewModel
    {
        public Guid PcsId { get; set; }

        public Guid RecipientId { get; set; }

        public string RecipientName { get; set; }

        public List<CategoryValue> CategoryValues { get; set; }

        public List<ViewEvidenceNoteViewModel> EvidenceNotesDataList { get; set; }

        public List<bool> SelectedEvidenceNotes { get; set; }

        public TransferEvidenceNotesViewModel()
        {
            CategoryValues = new List<CategoryValue>();
            EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>();
            SelectedEvidenceNotes = new List<bool>();
        }
    }
}