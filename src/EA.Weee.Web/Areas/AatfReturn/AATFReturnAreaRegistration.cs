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

            //context.MapLowercaseDashedRoute(
            //    name: AatfRedirect.CheckReturnRouteName,
            //    url: "aatf-return/{organisationId}/check-/{returnId}/{action}",
            //    defaults: new { action = "Index", controller = "SelectYourPcs" },
            //    namespaces: new[] { typeof(SelectYourPcsController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AatfRedirect.SelectPcsRouteName,
                url: "aatf-return/{organisationId}/select-pcs/{returnId}/{action}",
                defaults: new { action = "Index", controller = "SelectYourPcs" },
                namespaces: new[] { typeof(SelectYourPcsController).Namespace });

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

            //context.MapLowercaseDashedRoute(
            //    name: "aatf-default",
            //    url: "aatf-return/{returnId}/{controller}/{action}",
            //    defaults: new { action = "Index", controller = "AatfTaskList" },
            //    namespaces: new[] { typeof(AatfTaskListController).Namespace });

            //context.MapLowercaseDashedRoute(
            //    name: "aatf-default",
            //    url: "AatfReturn/{organisationId}/{returnId}/{controller}/{action}",
            //    defaults: new { action = "Index", controller = "AatfTaskList" },
            //    namespaces: new[] { typeof(AatfTaskListController).Namespace });

            //context.MapLowercaseDashedRoute(
            //    name: "aatf-received",
            //    url: "AatfReturn/received/{organisationId}/{returnId}/{aatfId}/{controller}/{action}",
            //    defaults: new { action = "Index", controller = "ReceivedPcsList" },
            //    namespaces: new[] { typeof(ReceivedPcsListController).Namespace });

            //context.MapLowercaseDashedRoute(
            //    name: "aatf-received-scheme",
            //    url: "AatfReturn/received-scheme/{organisationId}/{returnId}/{aatfId}/{schemeId}/{controller}/{action}",
            //    defaults: new { action = "Index", controller = "ObligatedReceived" },
            //    namespaces: new[] { typeof(ReceivedPcsListController).Namespace });

            //context.MapLowercaseDashedRoute(
            //    name: "aatf-reused",
            //    url: "AatfReturn/reused/{organisationId}/{returnId}/{aatfId}/{controller}/{action}",
            //    defaults: new { action = "Index", controller = "ObligatedReused" },
            //    namespaces: new[] { typeof(ObligatedReusedController).Namespace });
        }
    }
}