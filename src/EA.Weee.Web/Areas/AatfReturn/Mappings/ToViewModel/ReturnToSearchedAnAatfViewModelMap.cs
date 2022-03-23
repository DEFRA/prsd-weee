namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnToSearchedAnAatfViewModelMap : IMap<ReturnToSearchedAnAatfViewModelMapTransfer, SearchedAatfResultListViewModel>
    {
        public SearchedAatfResultListViewModel Map(ReturnToSearchedAnAatfViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new SearchedAatfResultListViewModel()
            {
                ReturnId = source.Return.Id,
                AatfId = source.AatfId,
                OrganisationId = source.Return.OrganisationData.Id,
            };

            return viewModel;
        }
    }
}