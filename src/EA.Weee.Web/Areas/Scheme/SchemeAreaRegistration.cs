namespace EA.Weee.Web.Areas.Scheme
{
    using Controllers;
    using System.Web.Mvc;
    using System.Web.Routing;
    using LowercaseDashedRouting;

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
            context.Routes.LowercaseUrls = true;
            string url = "Scheme/{pcsId}/{controller}/{action}/{entityId}";
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary(new { action = "Index", controller = "Home", entityId = UrlParameter.Optional });
            string[] namespaces = new[] { typeof(HomeController).Namespace };
            LowercaseDashedRoute dashedRoute = new LowercaseDashedRoute(url, routeValueDictionary, new DashedRouteHandler(), this, context, namespaces);
            context.Routes.Add(dashedRoute);
        }
    }
}