namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using Core.Admin;
    using System.Collections.Generic;

    public class SubmissionHistoryViewModel
    {
        public IList<SubmissionsHistorySearchResult> Results { get; set; }
    }
}