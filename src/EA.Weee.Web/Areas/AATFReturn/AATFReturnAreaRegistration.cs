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
                name: "AatfReturn_NonObligatedDcf",
                url: "AatfReturn/{organisationId}/{controller}/dcf/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = true, entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_NonObligated",
                url: "AatfReturn/{organisationId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", dcf = false, entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(NonObligatedController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "AatfReturn_default",
                url: "AatfReturn/{organisationId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(NonObligatedController).Namespace });
        }
    }
}