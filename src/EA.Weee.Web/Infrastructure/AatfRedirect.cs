namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AatfRedirect
    {
        public static string SelectPcsRouteName = "aatf-select-pcs";
        public static string NonObligatedRouteName = "aatf-non-obligated";
        public static string NonObligatedDcfRouteName = "aatf-non-obligated-dcf";
        public static string CheckReturnRouteName = "aatf-check";
        public static string Default = "aatf-default";
        public static string AatfSelectedRoute = "aatf-selected";
        public static string AatfSchemeSelectedRoute = "aatf-scheme-selected";
        public static string AatfOrganisationSelectedRoute = "aatf-selected-organisation";

        public static RedirectToRouteResult TaskList(Guid returnId)
        {
            return new RedirectToRouteResult(Default, new RouteValueDictionary(new { controller = "AatfTaskList", action = "Index", returnId = returnId }));
        }

        public static RedirectToRouteResult SelectPcs(Guid organsationId, Guid returnId)
        {
            return new RedirectToRouteResult(SelectPcsRouteName, new RouteValueDictionary(new { action = "Index", organisationId = organsationId, returnId = returnId }));
        }

        public static RedirectToRouteResult ReusedOffSite(Guid returnId, Guid aatfId, Guid organisationId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ReusedOffSite", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId }));
        }

        public static RedirectToRouteResult ReusedOffSiteCreate(Guid returnId, Guid aatfId, Guid organisationId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ReusedOffSiteCreateSite", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId }));
        }

        public static RedirectToRouteResult CheckReturn(Guid returnId)
        {
            return new RedirectToRouteResult(Default, new RouteValueDictionary(new { controller = "CheckYourReturn", action = "Index", returnId = returnId }));
        }

        public static RedirectToRouteResult SubmittedReturn(Guid returnId)
        {
            return new RedirectToRouteResult(Default, new RouteValueDictionary(new { controller = "SubmittedReturn", action = "Index", returnId = returnId }));
        }

        public static RedirectToRouteResult ObligatedReceived(Guid returnId, Guid aatfId, Guid schemeId)
        {
            return new RedirectToRouteResult(AatfSchemeSelectedRoute, new RouteValueDictionary(new { controller = "ObligatedReceived", action = "Index", schemeId = schemeId, returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult NonObligated(Guid returnId, bool dcf)
        {
            return new RedirectToRouteResult((dcf) ? NonObligatedDcfRouteName : NonObligatedRouteName, new RouteValueDictionary(new { returnId = returnId, dcf = dcf }));
        }
    }
}