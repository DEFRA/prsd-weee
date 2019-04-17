namespace EA.Weee.Web.RazorHelpers
{
    using global::System.Web.Routing;
    using global::System.Web.WebPages;

    public class TypeHelper
    {
        public static RouteValueDictionary ObjectToDictionary(object value)
        {
            var routeValueDictionary = new RouteValueDictionary();
            if (value != null)
            {
                foreach (var property in PropertyHelper.GetProperties(value))
                {
                    routeValueDictionary.Add(property.Name, property.GetValue(value));
                }
            }
            return routeValueDictionary;
        }
    }
}