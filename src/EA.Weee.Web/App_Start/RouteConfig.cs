namespace EA.Weee.Web
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapRoute("ListPayment", "payment/index",
                new { controller = "Payment", action = "Index" });

            routes.MapRoute("CreatePayment", "payment/create",
                new { controller = "Payment", action = "CreatePayment" });

            routes.MapRoute("Error403", "errors/403",
                new { controller = "Errors", action = "AccessDenied" });

            routes.MapRoute("Error404", "errors/404",
                new { controller = "Errors", action = "NotFound" });

            routes.MapRoute("Error500", "errors/500",
                new { controller = "Errors", action = "InternalError" });

            routes.MapRoute("Robots.txt", "robots.txt",
                new { controller = "Home", action = "Robots" },
                new[] { "EA.Weee.Web.Controllers" });

            routes.MapRoute("Login", "select-organisation",
                new { controller = "Organisation", action = "Index" });

            routes.MapRoute(name: "InternalLogin",
                url: "admin/home/index",
                defaults: new { controller = "Home", action = "Index", area = "Admin" });

            routes.MapRoute("LandingPage", string.Empty,
                new { controller = "Account", action = "SignIn" },
                namespaces: new[] { typeof(AccountController).Namespace });

            routes.MapLowercaseDashedRoute("Default", "{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}