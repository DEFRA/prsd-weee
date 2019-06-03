namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnDataToSelectReportOptionsNilViewModelMap : IMap<ReturnDataToSelectReportOptionsNilViewModelMapTransfer, SelectReportOptionsNilViewModel>
    {
        public SelectReportOptionsNilViewModel Map(ReturnDataToSelectReportOptionsNilViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.ReturnData, source.ReturnData);

            var model = new SelectReportOptionsNilViewModel()
            {
                ReturnId = source.ReturnId,
                OrganisationId = source.OrganisationId,
                QuarterWindowEndDate = source.ReturnData.QuarterWindow.EndDate,
                QuarterWindowStartDate = source.ReturnData.QuarterWindow.StartDate,
                Quarter = source.ReturnData.Quarter.Q.ToString(),
                Year = source.ReturnData.Quarter.Year.ToString()
            };

            return model;
        }
    }
}