namespace EA.Weee.Web.Infrastructure
{
    using LowercaseDashedRouting;
    using System.Web.Routing;

    public static class RouteCollectionExtensions
    {
        public static Route MapLowercaseDashedRoute(this RouteCollection routes, string name, string url,
            object defaults, object constraints, string[] namespaces)
        {
            var dataTokens = CreateRouteValueDictionaryWithNamespaces(namespaces);

            var route = new LowercaseDashedRoute(url,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                dataTokens,
                new DashedRouteHandler());

            routes.Add(name, route);

            return route;
        }

        public static Route MapLowercaseDashedRoute(this RouteCollection routes, string name, string url,
            object defaults, string[] namespaces)
        {
            var dataTokens = CreateRouteValueDictionaryWithNamespaces(namespaces);

            var route = new LowercaseDashedRoute(url,
                new RouteValueDictionary(defaults),
                null,
                dataTokens,
                new DashedRouteHandler());

            routes.Add(name, route);

            return route;
        }

        public static Route MapLowercaseDashedRoute(this RouteCollection routes, string name, string url,
            object defaults)
        {
            var route = new LowercaseDashedRoute(url,
                new RouteValueDictionary(defaults),
                new DashedRouteHandler());

            routes.Add(name, route);

            return route;
        }

        private static RouteValueDictionary CreateRouteValueDictionaryWithNamespaces(string[] namespaces)
        {
            var dataTokens = new RouteValueDictionary();
            dataTokens["Namespaces"] = namespaces;
            return dataTokens;
        }
    }
}