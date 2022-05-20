namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AatfRedirect
    {
        public static string SelectPcsRouteName = "aatf-select-pcs";
        public static string RemovedPcsRouteName = "aatf-removed-pcs";
        public static string NonObligatedRouteName = "aatf-non-obligated";
        public static string NonObligatedDcfRouteName = "aatf-non-obligated-dcf";
        public static string Default = "aatf-default";
        public static string AatfSelectedRoute = "aatf-selected";
        public static string AatfSchemeSelectedRoute = "aatf-scheme-selected";
        public static string AatfOrganisationSelectedRoute = "aatf-selected-organisation";
        public static string SelectReportOptionsRouteName = "aatf-report-options";
        public static string SelectReportOptionsNilRouteName = "aatf-report-options-nil";
        public static string ReturnsRouteName = "aatf-returns";
        public static string ReturnsCopyRouteName = "aatf-returns-copy";
        public static string SelectReportOptionsDeselectRouteName = "aatf-report-options-deselect";
        public static string Download = "returns-download";              

        public static RedirectToRouteResult TaskList(Guid returnId)
        {
            return new RedirectToRouteResult(Default, new RouteValueDictionary(new { controller = "AatfTaskList", action = "Index", returnId = returnId }));
        }

        public static RedirectToRouteResult SelectPcs(Guid organisationId, Guid returnId, bool reselect = false)
        {
            return new RedirectToRouteResult(SelectPcsRouteName, new RouteValueDictionary(new { action = "Index", organisationId = organisationId, returnId = returnId, reselect = reselect }));
        }

        public static RedirectToRouteResult SelectReportOptions(Guid organisationId, Guid returnId)
        {
            return new RedirectToRouteResult(SelectReportOptionsRouteName, new RouteValueDictionary(new { action = "Index", organisationId = organisationId, returnId = returnId }));
        }

        public static RedirectToRouteResult SelectReportOptionDeselect(Guid organisationId, Guid returnId)
        {
            return new RedirectToRouteResult(SelectReportOptionsDeselectRouteName, new RouteValueDictionary(new { action = "Index", organisationId = organisationId, returnId = returnId }));
        }

        public static RedirectToRouteResult SelectReportOptionsNil(Guid organisationId, Guid returnId)
        {
            return new RedirectToRouteResult(SelectReportOptionsNilRouteName, new RouteValueDictionary(new { action = "Index", organisationId = organisationId, returnId = returnId }));
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

        public static RedirectToRouteResult ObligatedSentOn(string siteName, Guid organisationId, Guid aatfId, Guid returnId, Guid weeeSentOnId, bool? isEditDetails = false, bool? isEditTonnage = false)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "ObligatedSentOn", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId, weeeSentOnId = weeeSentOnId, siteName = siteName, isEditDetails = isEditDetails, isEditTonnage = isEditTonnage }));
        }

        public static RedirectToRouteResult SentOnCreateSiteOperator(Guid organisationId, Guid aatfId, Guid returnId, Guid weeeSentOnId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SentOnCreateSiteOperator", action = "Index", returnId = returnId, aatfId = aatfId, organisationId = organisationId, weeeSentOnId = weeeSentOnId }));
        }

        public static RedirectToRouteResult SentOnCreateSite(Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SentOnCreateSite", action = "Index", returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult SearchAnAatf(Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SearchAnAatf", action = "Index", returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult CanNotFoundTreatmentFacility(Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "CanNotFoundTreatmentFacility", action = "Index", returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult SentOnSummaryList(Guid organisationId, Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SentOnSiteSummaryList", action = "Index", organisationId = organisationId, returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult SearchedAatfResultList(Guid organisationId, Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(AatfOrganisationSelectedRoute, new RouteValueDictionary(new { controller = "SearchedAatfResultList", action = "Index", organisationId = organisationId, returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult ReturnsList(Guid organisationId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "Index", organisationId = organisationId }));
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

        public static RedirectToRouteResult ReturnsSummaryDownloadObligatedData(Guid returnId)
        {
            return new RedirectToRouteResult(Default, new RouteValueDictionary(new { controller = "ReturnsSummary", action = "DownloadAllObligatedData", returnId = returnId }));
        }

        public static RedirectToRouteResult SubmittedReturn(Guid returnId)
        {
            return new RedirectToRouteResult(Default, new RouteValueDictionary(new { controller = "SubmittedReturn", action = "Index", returnId = returnId }));
        }

        public static RedirectToRouteResult ObligatedReceived(Guid returnId, Guid aatfId, Guid schemeId)
        {
            return new RedirectToRouteResult(AatfSchemeSelectedRoute, new RouteValueDictionary(new { controller = "ObligatedReceived", action = "Index", schemeId = schemeId, returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult ObligatedReused(Guid returnId, Guid aatfId)
        {
            return new RedirectToRouteResult(Default, new RouteValueDictionary(new { controller = "ObligatedReused", action = "Index", returnId = returnId, aatfId = aatfId }));
        }

        public static RedirectToRouteResult NonObligated(Guid returnId, bool dcf)
        {
            return new RedirectToRouteResult((dcf) ? NonObligatedDcfRouteName : NonObligatedRouteName, new RouteValueDictionary(new { returnId = returnId, dcf = dcf }));
        }
    }
}