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

            return new SelectReportOptionsViewModel(source.OrganisationId, source.ReturnId, source.ReportOnQuestions);
        }
    }
}