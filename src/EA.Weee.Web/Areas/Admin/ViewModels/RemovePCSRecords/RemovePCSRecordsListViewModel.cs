namespace EA.Weee.Web.Areas.Admin.ViewModels.RemovePCSRecords
{
    using EA.Weee.Core.Shared.Paging;

    public class RemovePCSRecordsListViewModel
    {
        public IPagedList<RemovePCSRowViewModel> RemovePCSRowViews { get; set; }
    }
}