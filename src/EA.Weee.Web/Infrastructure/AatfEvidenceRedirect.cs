namespace EA.Weee.Web.Infrastructure
{
    using Core.AatfEvidence;

    public static class AatfEvidenceRedirect
    {
        public static string ManageEvidenceRouteName = "AATF_ManageEvidence";
        public static string ViewDraftEvidenceRouteName = "AATF_ViewEvidence";
        public static string ViewSubmittedEvidenceRouteName = "AATF_SubmittedViewEvidence";
        public static string ViewRejectedEvidenceRouteName = "AATF_RejectedViewEvidence";
        public static string ViewReturnedEvidenceRouteName = "AATF_ReturnedViewEvidence";
        public static string ViewApprovedEvidenceRouteName = "AATF_ApprovedViewEvidence";
        public static string ViewVoidedEvidenceRouteName = "AATF_VoidedViewEvidence";
        public static string EditEvidenceRouteName = "AATF_EditEvidence";
        public static string ViewCancelEvidenceRouteName = "AATF_CancelViewEvidence";

        public static string AatfViewRouteName(NoteStatus noteStatus)
        {
            switch (noteStatus)
            {
                case NoteStatus.Approved:
                    return ViewApprovedEvidenceRouteName;
                case NoteStatus.Rejected:
                    return ViewRejectedEvidenceRouteName;
                case NoteStatus.Draft:
                    return ViewDraftEvidenceRouteName;
                case NoteStatus.Returned:
                    return ViewReturnedEvidenceRouteName;
                case NoteStatus.Submitted:
                    return ViewSubmittedEvidenceRouteName;
                case NoteStatus.Void:
                    return ViewVoidedEvidenceRouteName;
                case NoteStatus.Cancelled:
                    return ViewCancelEvidenceRouteName;
            }

            return string.Empty;
        }
    }
}