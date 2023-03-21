namespace EA.Weee.Web.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;

    public static class ReturnUrlHelper
    {
        public static string TidyQueryString(NameValueCollection queryString)
        {
            if (queryString != null && queryString.HasKeys())
            {
                var filteredQuery = queryString.ToString()
                    .Split('&')
                    .Where(q => !string.IsNullOrEmpty(q.Split('=')[1]))
                    .ToList();

                var newQuery = string.Join("&", filteredQuery);

                return newQuery;
            }
           
            return string.Empty;
        }
    }
}