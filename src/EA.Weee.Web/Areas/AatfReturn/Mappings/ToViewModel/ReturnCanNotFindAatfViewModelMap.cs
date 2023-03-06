namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnCanNotFindAatfViewModelMap : IMap<ReturnCanNotFindAatfViewModelMapTransfer, CanNotFindAatfViewModel>
    {
        public CanNotFindAatfViewModel Map(ReturnCanNotFindAatfViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new CanNotFindAatfViewModel()
            {
                ReturnId = source.Return.Id,
                AatfId = source.AatfId,
                OrganisationId = source.Return.OrganisationData.Id,
                IsCanNotFindLinkClick = source.IsCanNotFindLinkClick,
                AatfName = source.AatfName
            };

            return viewModel;
        }
    }
}