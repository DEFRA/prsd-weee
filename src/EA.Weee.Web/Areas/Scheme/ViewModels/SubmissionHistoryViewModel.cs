namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using Core.Admin;

    public class SubmissionHistoryViewModel
    {
        public IList<SubmissionsHistorySearchResult> Results { get; set; }
    }
}