namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords
{
    using Core.Shared.Paging;
    using EA.Weee.DataAccess.StoredProcedure;

    public class RemovePCSRecordsListViewModel
    {
        public string SelectedYear { get; set; }

        public string SelectedSchemeName { get; set; }

        //public List<SchemeDataExceedingRetentionPeriod> SchemeData { get; set; }
        public IPagedList<SchemeDataExceedingRetentionPeriod> SchemeData { get; set; }

        public string SortOrder { get; set; }
    }
}