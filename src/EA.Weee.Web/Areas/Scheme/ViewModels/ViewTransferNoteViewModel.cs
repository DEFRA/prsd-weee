namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.AatfEvidence;
    using Core.Helpers;
    using EA.Weee.Core.Shared;
    using ManageEvidenceNotes;
    using NoteStatus = Core.AatfEvidence.NoteStatus;

    public class ViewTransferNoteViewModel
    {
        public Guid SchemeId { get; set; }

        public Guid EvidenceNoteId { get; set; }

        public int Reference { get; set; }

        public NoteType Type { get; set; }

        public Core.AatfEvidence.NoteStatus Status { get; set; }

        [DisplayName("Reference ID")]
        public string ReferenceDisplay => $"{Type.ToDisplayString()}{Reference}";

        public string SuccessMessage { get; set; }

        public bool DisplayMessage => !string.IsNullOrWhiteSpace(SuccessMessage);

        [DisplayName("Compliance year")]
        public string ComplianceYearDisplay => ComplianceYear.ToString();

        public int ComplianceYear { get; set; }

        public string RecipientAddress { get; set; }

        public string TransferredByAddress { get; set; }

        public IList<TotalCategoryValue> TotalCategoryValues { get; set; }

        public IList<ViewTransferEvidenceAatfDataViewModel> Summary { get; set; }

        public int? SelectedComplianceYear { get; set; }

        public string RedirectTab
        {
            get
            {
                if (Status.Equals(NoteStatus.Draft))
                {
                    return ManageEvidenceNotesDisplayOptions.OutgoingTransfers.ToDisplayString();
                }

                //TODO: this will get updated when viewing and editing of transfer notes is added
                return ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString();
            }
        }

        public bool EditMode { get; set; }

        public string TabName
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

        public bool DisplayEditButton => Status == NoteStatus.Draft;

        public bool ReturnToView { get; set; }
    }
}