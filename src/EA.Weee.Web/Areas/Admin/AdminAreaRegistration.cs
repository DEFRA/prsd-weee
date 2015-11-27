namespace EA.Weee.Web.Areas.Admin
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapLowercaseDashedRoute(
                name: "admin_producers_search",
                url: "admin/producers/find-producer",
                defaults: new { action = "Search", controller = "Producers" },
                namespaces: new[] { typeof(ProducersController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "admin_default",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { action = "Index", controller = "Home", id = UrlParameter.Optional },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}