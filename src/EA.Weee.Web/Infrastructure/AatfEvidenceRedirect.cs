namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.AatfEvidence;

    public static class AatfEvidenceRedirect
    {
        public static string ManageEvidenceRouteName = "AATF_ManageEvidence";
        public static string ViewEvidenceRouteName = "AATF_ViewEvidence";
        public static string ViewSubmittedEvidenceRouteName = "AATF_SubmittedViewEvidence";

        public static RedirectToRouteResult ViewEvidenceNote(Guid organisationId, Guid aatfId, Guid evidenceNoteId, NoteStatus status)
        {
            var routeName = status == NoteStatus.Draft ? ViewEvidenceRouteName : ViewSubmittedEvidenceRouteName;
            return new RedirectToRouteResult(routeName, new RouteValueDictionary(new { action = "ViewDraftEvidenceNote", controller = "ManageEvidenceNotes", area = "Aatf", organisationId, aatfId, evidenceNoteId }));
        }
    }
}