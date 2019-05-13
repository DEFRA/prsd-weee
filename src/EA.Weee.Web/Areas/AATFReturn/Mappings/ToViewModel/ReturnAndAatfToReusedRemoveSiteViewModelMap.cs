namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnAndAatfToReusedRemoveSiteViewModelMap : IMap<ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer, ReusedRemoveSiteViewModel>
    {
        public ReturnAndAatfToReusedRemoveSiteViewModelMap()
        {
        }

        public ReusedRemoveSiteViewModel Map(ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new ReusedRemoveSiteViewModel()
            {
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                SiteAddress = source.SiteAddress,
                SiteId = source.SiteId,
                Site = source.Site,
                SiteAddressName = source.Site.Name
            };

            return viewModel;
        }
    }
}