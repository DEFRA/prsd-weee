namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public abstract class TransferEvidenceViewModelBase
    {
        public Guid PcsId { get; set; }

        public string RecipientName { get; set; }

        public List<ViewEvidenceNoteViewModel> EvidenceNotesDataList { get; set; }

        public List<TotalCategoryValue> CategoryValues { get; set; }

        protected TransferEvidenceViewModelBase()
        {
            EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>();
            CategoryValues = new List<TotalCategoryValue>();
        }
    }
}