namespace EA.Weee.Web.Infrastructure
{
    using System.Web.Routing;

    public static class RouteValueDictionaryExtensions
    {
        public static RouteValueDictionary FixListRouteDataValues(this RouteValueDictionary routes)
        {
            var newRv = new RouteValueDictionary();
            foreach (var key in routes.Keys)
            {
                var value = routes[key];
                if (value is System.Collections.IEnumerable && !(value is string))
                {
                    var index = 0;
                    foreach (var val in (System.Collections.IEnumerable)value)
                    {
                        newRv.Add(string.Format("{0}[{1}]", key, index), val);
                        index++;
                    }
                }
                else
                {
                    newRv.Add(key, value);
                }
            }
            return newRv;
        }
    }
}
