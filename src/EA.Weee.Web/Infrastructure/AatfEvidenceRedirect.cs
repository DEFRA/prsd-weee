namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AatfEvidenceRedirect
    {
        public static string ManageEvidenceRouteName = "AATF_ManageEvidence";
        public static string ViewEvidenceRouteName = "AATF_ViewEvidence";

        public static RedirectToRouteResult SelectYourAatf(Guid organisationId)
        {
            return new RedirectToRouteResult(ManageEvidenceRouteName, new RouteValueDictionary(new { controller = "SelectYourAatf", action = "Index", organisationId, type = "aatf", area = "AatfEvidence" }));
        }
    }
}