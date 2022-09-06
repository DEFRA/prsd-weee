namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Infrastructure;

    public class EvidenceNoteRowViewModel 
    {
        public int ReferenceId { get; set; }

        public string Recipient { get; set; }

        public WasteType? TypeOfWaste { get; set; }

        public NoteStatus Status { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string ReturnedReason { get; set; }

        public string RejectedReason { get; set; }

        public string SubmittedBy { get; set; }

        public Guid Id { get; set; }

        public NoteType Type { get; set; }

        public bool DisplayViewLink { get; set; }

        public bool DisplayEditLink { get; set; }

        public string ReferenceDisplay => $"{Type.ToDisplayString()}{ReferenceId}";

        public string SubmittedDateDisplay => SubmittedDate.HasValue ? SubmittedDate.Value.ToShortDateString() : "-";

        public string AatfViewRouteName
        {
            get
            {
                switch (Status)
                {
                    case NoteStatus.Approved:
                        return AatfEvidenceRedirect.ViewApprovedEvidenceRouteName;
                    case NoteStatus.Rejected:
                        return AatfEvidenceRedirect.ViewRejectedEvidenceRouteName;
                    case NoteStatus.Draft:
                        return AatfEvidenceRedirect.ViewDraftEvidenceRouteName;
                    case NoteStatus.Returned:
                        return AatfEvidenceRedirect.ViewReturnedEvidenceRouteName;
                    case NoteStatus.Submitted:
                        return AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName;
                    case NoteStatus.Void:
                        return AatfEvidenceRedirect.ViewVoidedEvidenceRouteName;
                }

                throw new InvalidOperationException("Invalid route name");
            }
        }

        public string SchemeViewRouteName
        {
            get
            {
                if (Type == NoteType.Transfer)
                {
                    switch (Status)
                    {
                        case NoteStatus.Draft:
                            return SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName;
                        case NoteStatus.Submitted:
                            return SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName;
                        case NoteStatus.Approved:
                            return SchemeTransferEvidenceRedirect.ViewApprovedTransferEvidenceRouteName;
                        case NoteStatus.Rejected:
                            return SchemeTransferEvidenceRedirect.ViewRejectedTransferEvidenceRouteName;
                        case NoteStatus.Returned:
                            return SchemeTransferEvidenceRedirect.ViewReturnedTransferEvidenceRouteName;
                        case NoteStatus.Void:
                            return SchemeTransferEvidenceRedirect.ViewVoidedTransferEvidenceRouteName;
                    }
                }

                switch (Status)
                {
                    case NoteStatus.Approved:
                        return SchemeTransferEvidenceRedirect.ViewApprovedEvidenceNoteRouteName;
                    case NoteStatus.Rejected:
                        return SchemeTransferEvidenceRedirect.ViewRejectedEvidenceNoteRouteName;
                    case NoteStatus.Returned:
                        return SchemeTransferEvidenceRedirect.ViewReturnedEvidenceNoteRouteName;
                    case NoteStatus.Submitted:
                        return SchemeTransferEvidenceRedirect.ViewSubmittedEvidenceNoteRouteName;
                    case NoteStatus.Void:
                        return SchemeTransferEvidenceRedirect.ViewVoidedEvidenceNoteRouteName;
                }

                return string.Empty;
            }
        }
    }
}
