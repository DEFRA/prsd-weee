namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AatfRedirect
    {
        public static string AatfInitialSelect = "aatf-initial-select";
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
            return new RedirectToRouteResult(AatfInitialSelect, new RouteValueDictionary(new { controller = "SelectYourPcs", action = "Index", organisationId = organsationId, returnId = returnId }));
        }

        public static RedirectToRouteResult SelectReportOptions(Guid organsationId, Guid returnId)
        {
            return new RedirectToRouteResult(AatfInitialSelect, new RouteValueDictionary(new { controller = "SelectReportOptions", action = "Index", organisationId = organsationId, returnId = returnId }));
        }

        public static RedirectToRouteResult ReusedOffSite(Guid returnId, Guid aatfId, Guid organisationId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ReusedOffSite", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId }));
        }

        public static RedirectToRouteResult ReusedOffSiteCreate(Guid returnId, Guid aatfId, Guid organisationId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ReusedOffSiteCreateSite", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId }));
        }

        public static RedirectToRouteResult ReusedOffSiteCreateEdit(Guid returnId, Guid aatfId, Guid organisationId, Guid siteId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ReusedOffSiteCreateSite", action = "Edit", returnId = returnId, aatfId = aatfId, organisationId = organisationId, siteId = siteId }));
        }

        public static RedirectToRouteResult ObligatedSentOn(string operatorName, Guid organisationId, Guid aatfId, Guid returnId, Guid weeeSentOnId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ObligatedSentOn", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId, weeeSentOnId = weeeSentOnId, operatorName = operatorName }));
        }

        public static RedirectToRouteResult SentOnCreateSiteOperator(Guid organisationId, Guid aatfId, Guid returnId, Guid weeeSentOnId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SentOnCreateSiteOperator", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId, weeeSentOnId = weeeSentOnId }));
        }

        public static RedirectToRouteResult SentOnCreateSite(Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SentOnCreateSite", action = "Index", returnId = returnId, aatfId = aatfId}));
        }

        public static RedirectToRouteResult SentOnSummaryList(Guid organisationId, Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SentOnSiteSummaryList", action = "Index", organisationId = organisationId, returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult ReusedOffSiteSummaryList(Guid returnId, Guid aatfId, Guid organisationId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ReusedOffSiteSummaryList", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId }));
        }

        public static RedirectToRouteResult ReceivedPcsList(Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfSelectedRoute, new RouteValueDictionary(new { controller = "ReceivedPcsList", action = "Index", returnId = returnId, aatfId = aatfId }));
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