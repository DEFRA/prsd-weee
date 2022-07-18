namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.ComponentModel;
    using Core.AatfEvidence;
    using Extensions;

    public class ViewEvidenceNoteViewModel : EvidenceNoteViewModel
    {
        public string SuccessMessage { get; set; }

        public bool DisplayMessage => !string.IsNullOrWhiteSpace(SuccessMessage);

        [DisplayName("Actual or protocol")]
        public string ProtocolDisplay => ProtocolValue.HasValue ? ProtocolValue.Value.ToDisplayString() : string.Empty;

        [DisplayName("Type of waste")]
        public string WasteDisplay => WasteTypeValue.HasValue ? WasteTypeValue.Value.ToDisplayString() : string.Empty;

        [DisplayName("Compliance year")]
        public string ComplianceYearDisplay => ComplianceYear.ToString();

        public int ComplianceYear { get; set; }

        public string SiteAddress { get; set; }

        public string OperatorAddress { get; set; }

        public string RecipientAddress { get; set; }

        public bool DisplayEditButton { get; set; }

        public bool HasSubmittedDate => !string.IsNullOrWhiteSpace(SubmittedDate);

        public bool HasApprovedDate => !string.IsNullOrWhiteSpace(ApprovedDate);

        public bool HasRejectedDate => Status.Equals(NoteStatus.Rejected);

        public bool HasBeenReturned => Status.Equals(NoteStatus.Returned);

        public Guid SchemeId { get; set; }

        public string SubmittedBy { get; set; }

        public string TotalReceivedDisplay { get; set; }

        public string AatfApprovalNumber { get; set; }

        public bool DisplayAatfName { get; set; }

        public bool DisplayH2Title { get; set; }

        public int SelectedComplianceYear { get; set; }

        public virtual string TabName
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
    }
}