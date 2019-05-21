namespace EA.Weee.Web.Areas.AatfReturn
{
    using System.Security.Policy;
    using Areas.AatfReturn;
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class AatfReturnAreaRegistration : AreaRegistration
    {
        public override string AreaName => "AatfReturn";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: "AatfReturn_holding",
                url: "aatf-return/holding/{organisationId}",
                defaults: new { action = "Index", controller = "Holding" },
                namespaces: new[] { typeof(HoldingController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.NonObligatedDcfRouteName,
                url: "aatf-return/{returnId}/non-obligated-dcf/{action}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = true },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.NonObligatedRouteName,
                url: "aatf-return/{returnId}/non-obligated/{action}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = false },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.SelectPcsRouteName,
                url: "aatf-return/{organisationId}/select-pcs/{returnId}/{action}",
                defaults: new { action = "Index", controller = "SelectYourPcs" },
                namespaces: new[] { typeof(SelectYourPcsController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.SelectReportOptionsRouteName,
                url: "aatf-return/{organisationId}/select-report-options/{returnId}/{action}",
                defaults: new { action = "Index", controller = "SelectReportOptions" },
                namespaces: new[] { typeof(SelectReportOptionsController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.SelectReportOptionsDeselectRouteName,
                url: "aatf-return/{organisationId}/select-report-options-confirm/{returnId}/{action}",
                defaults: new { action = "Index", controller = "SelectReportOptionsDeselect" },
                namespaces: new[] { typeof(SelectReportOptionsDeselectController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.AatfSchemeSelectedRoute,
                url: "aatf-return/{returnId}/{controller}/{aatfId}/scheme/{schemeId}/{action}",
                defaults: new { action = "Index", controller = "ReceivedPcsList" },
                namespaces: new[] { typeof(ReceivedPcsListController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.AatfOrganisationSelectedRoute,
                url: "aatf-return/{returnId}/{controller}/{aatfId}/{organisationId}/{action}",
                defaults: new { action = "Index", controller = "ReusedOffSite" },
                namespaces: new[] { typeof(ReusedOffSiteController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.AatfSelectedRoute,
                url: "aatf-return/{returnId}/{controller}/{aatfId}/{action}",
                defaults: new { action = "Index", controller = "ReceivedPcsList" },
                namespaces: new[] { typeof(ReceivedPcsListController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.Default,
                url: "aatf-return/{controller}/{returnId}/{action}",
                defaults: new { action = "Index", controller = "AatfTaskList" },
                namespaces: new[] { typeof(AatfTaskListController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.OrganisationRouteName,
                url: "aatf-return/{organisationId}/{controller}/{action}",
                defaults: new { action = "Index", controller = "Returns" },
                namespaces: new[] { typeof(ReturnsController).Namespace });
        }
    }
}