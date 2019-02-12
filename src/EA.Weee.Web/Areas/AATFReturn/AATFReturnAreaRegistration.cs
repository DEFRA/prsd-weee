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
    name: "AatfReturn_SelectYourPCS",
    url: "AatfReturn/{organisationId}/{controller}/{action}/{entityId}",
    defaults: new { action = "Index", controller = "SelectYourPCS", entityId = UrlParameter.Optional },
    namespaces: new[] { typeof(SelectYourPCSController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_NonObligatedDcf",
                url: "AatfReturn/{organisationId}/{returnId}/{controller}/dcf/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", Dcf = true, entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_NonObligated",
                url: "AatfReturn/{organisationId}/{returnId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", Dcf = false, entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_submitted",
                url: "AatfReturn/{organisationId}/{returnId}/{controller}/{action}",
                defaults: new { action = "Index", controller = "SubmittedReturn" },
                namespaces: new[] { typeof(SubmittedReturnController).Namespace });

             context.MapLowercaseDashedRoute(
               name: "AatfReturn_holding",
               url: "AatfReturn/{controller}/{action}",
               defaults: new { action = "Index", controller = "Holding"},
               namespaces: new[] { typeof(HoldingController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_default",
                url: "AatfReturn/{organisationId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(NonObligatedController).Namespace });
        }
    }
}