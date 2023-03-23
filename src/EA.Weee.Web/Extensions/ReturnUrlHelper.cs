namespace EA.Weee.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public static class ReturnUrlHelper
    {
        public static string TidyQueryString(NameValueCollection queryString)
        {
            if (queryString != null && queryString.HasKeys())
            {
                var propertiesToIgnore = new List<string>()
                {
                    "ManageEvidenceNoteViewModel_RecipientWasteStatusFilterViewModel_SubmittedBy-auto",
                    "ManageEvidenceNoteViewModel_RecipientWasteStatusFilterViewModel_ReceivedId-auto"
                };

                var filteredQuery = queryString.ToString()
                    .Split('&')
                    .Where(q => !string.IsNullOrEmpty(q.Split('=')[1]) && 
                                (!string.IsNullOrEmpty(q.Split('=')[0]) && 
                                 !propertiesToIgnore.Contains(q.Split('=')[0])))
                    .ToList();

                var newQuery = string.Join("&", filteredQuery);

                return newQuery;
            }
           
            return string.Empty;
        }
    }
}