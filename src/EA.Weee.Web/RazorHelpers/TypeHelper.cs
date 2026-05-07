namespace EA.Weee.Web.RazorHelpers
{
    using System.Collections.Generic;
    using System.Web.Routing;

    public class TypeHelper
    {
        public static RouteValueDictionary ObjectToDictionary(object value)
        {
            if (value is RouteValueDictionary routeValueDictionary)
            {
                return routeValueDictionary;
            }

            var result = new RouteValueDictionary();
            if (value != null)
            {
                foreach (var property in PropertyHelper.GetProperties(value))
                {
                    result.Add(property.Name, property.GetValue(value));
                }
            }
            return result;
        }
    }
}