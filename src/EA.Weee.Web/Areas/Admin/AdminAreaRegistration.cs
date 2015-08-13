namespace EA.Weee.Web.Areas.Admin
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Controllers;
    using Infrastructure;

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
            //context.Routes.LowercaseUrls = true;
            //string url = "admin/{controller}/{action}/{id}";
            //RouteValueDictionary routeValueDictionary = new RouteValueDictionary(new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            //string[] namespaces = new[] { typeof(HomeController).Namespace };
            //LowercaseDashedRoute dashedRoute = new LowercaseDashedRoute(url, routeValueDictionary, new DashedRouteHandler(), this, context, namespaces);
            //context.Routes.Add(dashedRoute);

            context.MapLowercaseDashedRoute(
                name: "admin_default",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { action = "Index", controller = "Home", id = UrlParameter.Optional },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}