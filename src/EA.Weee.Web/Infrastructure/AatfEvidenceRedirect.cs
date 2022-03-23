namespace EA.Weee.Web.Infrastructure
{
    using EA.Weee.Web.Areas.AatfEvidence.Controllers;
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AatfEvidenceRedirect
    {
        public static string Create = "aatf-evidence-create";
        public static string Default = "aatf-evidence-default";
        public static string Holding = "aatf-holding";
        public static string SelectAatfRouteName = "AatfEvidence_ChooseSite";

        public static RedirectToRouteResult SelectYourAatf(Guid organisationId)
        {
            //return new RedirectToRouteResult(SelectAatfRouteName, new RouteValueDictionary(new { controller = "SelectYourAatf", action = "Index", area = "AatfEvidence", organisationId }));
            return new RedirectToRouteResult(SelectAatfRouteName, new RouteValueDictionary(new { controller = "SelectYourAatf", action = "Index", organisationId, type = "aatf", area = "AatfEvidence" }));
        }
    }
}