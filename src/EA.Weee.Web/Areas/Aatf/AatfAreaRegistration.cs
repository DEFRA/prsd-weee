namespace EA.Weee.Web.Areas.Aatf
{
    using System.Security.Policy;
    using Aatf.Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class AatfAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Aatf";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();

            context.MapLowercaseDashedRoute(
                name: "Aatf_default",
                url: "Aatf/{organisationId}/{controller}/{action}/",
                defaults: new { action = "Index", controller = "Home"},
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}