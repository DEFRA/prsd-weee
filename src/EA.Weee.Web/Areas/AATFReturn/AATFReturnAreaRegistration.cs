namespace EA.Weee.Web.Areas.AatfReturn
{
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
                name: "AatfReturn_NonObligatedDcf",
                url: "AatfReturn/{organisationId}/{returnId}/non-obligated-dcf/{action}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = true },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_NonObligated",
                url: "AatfReturn/{organisationId}/{returnId}/non-obligated/{action}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = false },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_default",
                url: "AatfReturn/{organisationId}/{returnId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "AatfTaskList", entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(AatfTaskListController).Namespace });
        }
    }
}