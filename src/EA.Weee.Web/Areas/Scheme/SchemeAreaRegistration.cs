namespace EA.Weee.Web.Areas.Scheme
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class SchemeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Scheme";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: "Scheme_default",
                url: "Scheme/{pcsId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "Home", entityId = UrlParameter.Optional },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}