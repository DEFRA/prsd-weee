namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using Core.Admin;
    using Core.Shared.Paging;
    using Weee.Requests.Shared;

    public class SubmissionHistoryViewModel
    {
        public SubmissionsHistoryOrderBy OrderBy { get; set; }

        public IPagedList<SubmissionsHistorySearchData> Results { get; set; }

        public int ResultsCount { get; set; }
    }
}