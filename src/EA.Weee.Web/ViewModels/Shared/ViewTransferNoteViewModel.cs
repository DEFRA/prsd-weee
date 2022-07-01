namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Core.Helpers;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;

    public class ViewTransferNoteViewModel : ViewEvidenceNoteViewModel
    {
        public Guid EvidenceNoteId { get; set; }
        
        public string TransferredByAddress { get; set; }

        public IList<TotalCategoryValue> TotalCategoryValues { get; set; }

        public IList<ViewTransferEvidenceAatfDataViewModel> Summary { get; set; }

        public string RedirectTab
        {
            get
            {
                if (Status.Equals(NoteStatus.Draft) || Status.Equals(NoteStatus.Submitted))
                {
                    return ManageEvidenceNotesDisplayOptions.OutgoingTransfers.ToDisplayString();
                }

                //TODO: this will get updated when viewing and editing of transfer notes is added
                return ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString();
            }
        }

        public bool EditMode { get; set; }

        public override string TabName
        {
            get
            {
                switch (Status)
                {
                    case NoteStatus.Draft:
                        return "Draft evidence note";
                    case NoteStatus.Rejected:
                        return "Rejected evidence note";
                    case NoteStatus.Approved:
                        return "Approved evidence note";
                    case NoteStatus.Returned:
                        return "Returned evidence note";
                    case NoteStatus.Submitted:
                        return "Submitted evidence note";
                    default:
                        return string.Empty;
                }
            }
        }

        public bool ReturnToView { get; set; }

        public new int? SelectedComplianceYear { get; set; }
    }
}