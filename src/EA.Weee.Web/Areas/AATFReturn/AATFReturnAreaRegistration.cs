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
                url: "AatfReturn/holding/{organisationId}",
                defaults: new { action = "Index", controller = "Holding" },
                namespaces: new[] { typeof(HoldingController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "aatf-non-obligated-dcf",
                url: "AatfReturn/{organisationId}/{returnId}/non-obligated-dcf/{action}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = true },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "aatf-non-obligated",
                url: "AatfReturn/{organisationId}/{returnId}/non-obligated/{action}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = false },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "aatf-default",
                url: "AatfReturn/{organisationId}/{returnId}/{controller}/{action}",
                defaults: new { action = "Index", controller = "AatfTaskList" },
                namespaces: new[] { typeof(AatfTaskListController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "aatf-received",
                url: "AatfReturn/{organisationId}/{returnId}/{aatfId}/received/{controller}/{action}",
                defaults: new { action = "Index", controller = "ReceivedPcsList" },
                namespaces: new[] { typeof(ReceivedPcsListController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "aatf-received-scheme",
                url: "AatfReturn/{organisationId}/{returnId}/{aatfId}/{schemeId}/received/{controller}/{action}",
                defaults: new { action = "Index", controller = "ReceivedPcsList" },
                namespaces: new[] { typeof(ReceivedPcsListController).Namespace });
        }
    }
}