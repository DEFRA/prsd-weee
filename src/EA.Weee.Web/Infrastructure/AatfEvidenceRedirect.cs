namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.AatfEvidence;
    using RedirectResult = System.Web.Http.Results.RedirectResult;

    public static class AatfEvidenceRedirect
    {
        public static string ManageEvidenceRouteName = "AATF_ManageEvidence";
        public static string ViewDraftEvidenceRouteName = "AATF_ViewEvidence";
        public static string ViewSubmittedEvidenceRouteName = "AATF_SubmittedViewEvidence";
        public static string EditEvidenceRouteName = "AATF_EditEvidence";
    }
}