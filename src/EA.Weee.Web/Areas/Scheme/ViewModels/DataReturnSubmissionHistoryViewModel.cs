namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using Core.DataReturns;
    using Core.Shared.Paging;
    using Weee.Requests.Shared;

    public class DataReturnSubmissionHistoryViewModel
    {
        public DataReturnSubmissionsHistoryOrderBy OrderBy { get; set; }

        public IPagedList<DataReturnSubmissionsHistoryData> Results { get; set; }

        public int ResultsCount { get; set; }
    }
}