namespace EA.Weee.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Controllers;
    using LowercaseDashedRouting;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.Add(new LowercaseDashedRoute("{controller}/{action}/{id}",
                new RouteValueDictionary(
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional }),
                new DashedRouteHandler(), new[] { typeof(HomeController).Namespace }));
        }
    }
}