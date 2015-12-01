namespace EA.Weee.Web.Infrastructure
{
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AreaRegistrationContextExtensions
    {
        public static Route MapLowercaseDashedRoute(this AreaRegistrationContext context, string name, string url,
            object defaults, object constraints, string[] namespaces)
        {
            if (namespaces == null && context.Namespaces != null)
            {
                namespaces = context.Namespaces.ToArray();
            }

            var route = context.Routes.MapLowercaseDashedRoute(name, url, defaults, constraints, namespaces);
            route.DataTokens["area"] = context.AreaName;

            // disabling the namespace lookup fallback mechanism keeps this areas from accidentally picking up
            // controllers belonging to other areas
            var useNamespaceFallback = (namespaces == null || namespaces.Length == 0);
            route.DataTokens["UseNamespaceFallback"] = useNamespaceFallback;

            return route;
        }

        public static Route MapLowercaseDashedRoute(this AreaRegistrationContext context, string name, string url,
            object defaults, string[] namespaces)
        {
            return MapLowercaseDashedRoute(context, name, url, defaults, null, namespaces);
        }
    }
}