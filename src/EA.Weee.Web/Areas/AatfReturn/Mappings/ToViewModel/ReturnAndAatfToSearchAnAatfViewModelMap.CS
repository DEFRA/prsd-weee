namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnAndAatfToSearchAnAatfViewModelMap : IMap<ReturnAndAatfToSearchAnAatfViewModelMapTransfer, SearchAnAatfViewModel>
    {
        public SearchAnAatfViewModel Map(ReturnAndAatfToSearchAnAatfViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new SearchAnAatfViewModel()
            {
                ReturnId = source.Return.Id,
                AatfId = source.AatfId,
                OrganisationId = source.Return.OrganisationData.Id,
            };

            return viewModel;
        }
    }
}