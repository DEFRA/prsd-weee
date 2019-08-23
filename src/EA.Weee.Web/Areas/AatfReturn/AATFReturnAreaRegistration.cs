﻿namespace EA.Weee.Web.Areas.AatfReturn
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class AatfReturnAreaRegistration : AreaRegistration
    {
        public override string AreaName => "AatfReturn";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();

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
                name: AatfRedirect.SelectReportOptionsNilRouteName,
                url: "aatf-return/{organisationId}/nil-report-options-confirm/{returnId}/{action}",
                defaults: new { action = "Index", controller = "SelectReportOptionsNil" },
                namespaces: new[] { typeof(SelectReportOptionsNilController).Namespace });

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
                name: AatfRedirect.ReturnsCopyRouteName,
                url: "aatf-return/returns/{organisationId}/copy/{returnId}",
                defaults: new { action = "Copy", controller = "Returns" },
                namespaces: new[] { typeof(ReturnsController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.Download,
                url: "aatf-return/returns-download/{returnId}/",
                defaults: new { action = "Download", controller = "ReturnsSummary" },
                namespaces: new[] { typeof(ReturnsSummaryController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.ReturnsRouteName,
                url: "aatf-return/returns/{organisationId}/{action}",
                defaults: new { action = "Index", controller = "Returns" },
                namespaces: new[] { typeof(ReturnsController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.Default,
                url: "aatf-return/{controller}/{returnId}/{action}",
                defaults: new { action = "Index", controller = "AatfTaskList" },
                namespaces: new[] { typeof(AatfTaskListController).Namespace });
        }
    }
}