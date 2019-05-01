namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReportOptionsToSelectReportOptionsViewModelMap : IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel>
    {
        public SelectReportOptionsViewModel Map(ReportOptionsToSelectReportOptionsViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.ReportOnQuestions, source.ReportOnQuestions);
            Guard.ArgumentNotNull(() => source.ReturnData, source.ReturnData);

            var model = new SelectReportOptionsViewModel(
                source.OrganisationId,
                source.ReturnId,
                source.ReportOnQuestions,
                source.ReturnData,
                source.ReturnData.Quarter.Year)
            {
                QuarterWindowEndDate = source.ReturnData.QuarterWindow.EndDate,
                QuarterWindowStartDate = source.ReturnData.QuarterWindow.StartDate,
                Quarter = source.ReturnData.Quarter.Q.ToString()
            };

            return model;
        }
    }
}