namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class SiteAddressDataToReusedOffSiteCreateSiteViewModelMap : IMap<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer, ReusedOffSiteCreateSiteViewModel>
    {
        public ReusedOffSiteCreateSiteViewModel Map(SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new ReusedOffSiteCreateSiteViewModel()
            {
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId
            };

            viewModel.AddressData = source.ReturnedSites.AddressData.Where(s => s.Id == source.SiteId).FirstOrDefault();
            viewModel.AddressData.Countries = source.Countries;

            return viewModel;
        }
    }
}