namespace EA.Weee.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web.Routing;

    public static class ReturnUrlHelper
    {
        private static readonly HashSet<string> ReturnParameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "tab", "page", "selectedComplianceYear", "startDate", "endDate",
            "searchRef", "receivedId", "wasteTypeValue", "evidenceNoteTypeValue",
            "noteStatusValue", "submittedBy"
        };

        private static readonly List<string> PropertiesToIgnore = new List<string>()
        {
            "ManageEvidenceNoteViewModel_RecipientWasteStatusFilterViewModel_SubmittedBy-auto",
            "ManageEvidenceNoteViewModel_RecipientWasteStatusFilterViewModel_ReceivedId-auto"
        };

        public const string ReturnPrefix = "return_";

        /// <summary>
        /// Extracts filter parameters from the current request query string and returns
        /// them as a RouteValueDictionary with "return_" prefixed keys.
        /// </summary>
        public static RouteValueDictionary BuildReturnRouteValues(NameValueCollection queryString)
        {
            var result = new RouteValueDictionary();

            if (queryString == null || !queryString.HasKeys())
            {
                return result;
            }

            foreach (string key in queryString.AllKeys)
            {
                if (string.IsNullOrEmpty(key) || PropertiesToIgnore.Contains(key))
                {
                    continue;
                }

                var value = queryString[key];
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (ReturnParameterNames.Contains(key))
                {
                    result[ReturnPrefix + key] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Merges an anonymous object of primary route values with a RouteValueDictionary of return parameters.
        /// </summary>
        public static RouteValueDictionary MergeRouteValues(object primaryValues, RouteValueDictionary returnValues)
        {
            var result = new RouteValueDictionary(primaryValues);

            if (returnValues != null)
            {
                foreach (var kvp in returnValues)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Legacy method - kept for backward compatibility with views not yet migrated (Admin, Aatf).
        /// </summary>
        public static string TidyQueryString(NameValueCollection queryString)
        {
            if (queryString != null && queryString.HasKeys())
            {
                var filteredQuery = queryString.ToString()
                    .Split('&')
                    .Where(q => !string.IsNullOrEmpty(q.Split('=')[1]) &&
                                (!string.IsNullOrEmpty(q.Split('=')[0]) &&
                                 !PropertiesToIgnore.Contains(q.Split('=')[0])))
                    .ToList();

                var newQuery = string.Join("&", filteredQuery);

                return newQuery;
            }

            return string.Empty;
        }
    }
}