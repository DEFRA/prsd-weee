namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    [Serializable]
    public abstract class TransferEvidenceViewModelBase
    {
        public Guid PcsId { get; set; }

        public string RecipientName { get; set; }

        public Guid RecipientId { get; set; }

        public List<ViewEvidenceNoteViewModel> EvidenceNotesDataList { get; set; }

        public List<TotalCategoryValue> CategoryValues { get; set; }

        public ViewTransferNoteViewModel ViewTransferNoteViewModel { get; set; }

        public int ComplianceYear { get; set; }

        protected TransferEvidenceViewModelBase()
        {
            EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>();
            CategoryValues = new List<TotalCategoryValue>();
        }
    }
}