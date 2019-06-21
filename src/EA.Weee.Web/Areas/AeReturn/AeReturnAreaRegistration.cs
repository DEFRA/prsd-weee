namespace EA.Weee.Web.Areas.AeReturn
{
    using EA.Weee.Web.Areas.AeReturn.Controllers;
    using EA.Weee.Web.Infrastructure;
    using System.Web.Mvc;

    public class AeReturnAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AeReturn";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.MapMvcAttributeRoutes();

            context.MapLowercaseDashedRoute(
                name: "AeReturn_holding",
                url: "ae-return/holding/{organisationId}",
                defaults: new { action = "Index", controller = "Holding" },
                namespaces: new[] { typeof(HoldingController).Namespace });

            context.MapLowercaseDashedRoute(
                name: AeRedirect.ReturnsRouteName,
                url: "ae-return/returns/{organisationId}/{action}",
                defaults: new { action = "Index", controller = "Returns" },
                namespaces: new[] { typeof(ReturnsController).Namespace });
        }
    }
}