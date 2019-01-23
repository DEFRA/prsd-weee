namespace EA.Weee.Web.Areas.AATFReturn
{
    using System.Web.Mvc;
    using Controllers;
    using Infrastructure;

    public class AATFReturnAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AATFReturn";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: "AATFReturn_default",
                url: "AATFReturn/{organisationId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "NonObligated", entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}