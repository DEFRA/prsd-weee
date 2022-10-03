namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.ViewModels;

    [Serializable]
    public class ViewTransferNoteViewModel : ViewEvidenceNoteViewModel
    {
        public Guid EvidenceNoteId { get; set; }
        
        public string TransferredByAddress { get; set; }

        public IList<TotalCategoryValue> TotalCategoryValues { get; set; }

        public IList<ViewTransferEvidenceAatfDataViewModel> Summary { get; set; }

        public bool EditMode { get; set; }

        public override string TabName
        {
            get
            {
                switch (Status)
                {
                    case NoteStatus.Draft:
                        return "Draft evidence note transfer";
                    case NoteStatus.Rejected:
                        return "Rejected evidence note transfer";
                    case NoteStatus.Approved:
                        return "Approved evidence note transfer";
                    case NoteStatus.Returned:
                        return "Returned evidence note transfer";
                    case NoteStatus.Submitted:
                        return "Submitted evidence note transfer";
                    case NoteStatus.Void:
                        return "Voided evidence note transfer";
                    default:
                        return string.Empty;
                }
            }
        }

        public bool ReturnToView { get; set; }
    }
}