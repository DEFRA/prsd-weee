using System.Web.Mvc;
using System.Web.Routing;

namespace EA.Weee.Web.Maintenance
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Maintenance", action = "Maintenance", id = UrlParameter.Optional }
            );
        }
    }
}
