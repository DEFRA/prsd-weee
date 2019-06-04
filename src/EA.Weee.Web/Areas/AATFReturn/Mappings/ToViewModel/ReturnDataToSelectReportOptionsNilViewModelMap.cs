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

            var model = new SelectReportOptionsNilViewModel(source.ReturnData)
            {
                ReturnId = source.ReturnId,
                OrganisationId = source.OrganisationId,
            };

            return model;
        }
    }
}