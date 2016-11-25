namespace EA.Weee.Web.Areas.Admin.ViewModels.Submissions
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Weee.Requests.Shared;

    public class SubmissionsResultsViewModel
    {
        public int Year { get; set; }

        public Guid Scheme { get; set; }

        public SubmissionsHistoryOrderBy OrderBy { get; set; }

        public IList<SubmissionsHistorySearchData> Results { get; set; }
    }
}