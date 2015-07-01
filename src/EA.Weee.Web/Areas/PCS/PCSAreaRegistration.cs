namespace EA.Weee.Web.Areas.PCS
{
    using Controllers;
    using System.Web.Mvc;

    public class PCSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PCS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "PCS_default",
                url: "PCS/{id}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "Home", entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}