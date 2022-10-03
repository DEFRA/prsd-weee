namespace EA.Weee.Web.Infrastructure
{
    using Core.AatfEvidence;

    public static class SchemeTransferEvidenceRedirect
    {
        public static string ViewDraftTransferEvidenceRouteName = "Scheme_view_transfer";
        public static string ReviewSubmittedTransferEvidenceRouteName = "Scheme_review_transfer";
        public static string ViewSubmittedTransferEvidenceRouteName = "Scheme_submitted_transfer";
        public static string ViewApprovedTransferEvidenceRouteName = "Scheme_approved_transfer";
        public static string EditDraftTransferEvidenceRouteName = "Scheme_edit_transfer";
        public static string ViewReturnedTransferEvidenceRouteName = "Scheme_returned_transfer";
        public static string ViewRejectedTransferEvidenceRouteName = "Scheme_rejected_transfer";
        public static string ViewVoidedTransferEvidenceRouteName = "Scheme_voided_transfer";

        public static string ViewApprovedEvidenceNoteRouteName = "Scheme_approved_evidence_note";
        public static string ViewRejectedEvidenceNoteRouteName = "Scheme_rejected_evidence_note";
        public static string ViewReturnedEvidenceNoteRouteName = "Scheme_returned_evidence_note";
        public static string ViewSubmittedEvidenceNoteRouteName = "Scheme_submitted_evidence_note";
        public static string ViewVoidedEvidenceNoteRouteName = "Scheme_void_evidence_note";

        public static string SchemeViewRouteName(NoteType noteType, NoteStatus noteStatus)
        {
            if (noteType == NoteType.Transfer)
            {
                switch (noteStatus)
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

            switch (noteStatus)
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