namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Organisations;
    using Prsd.Core.Mapper;
    using ViewModels.Scheme.Overview.OrganisationDetails;
    using Web.ViewModels.Shared;

    public class OrganisationDataToSoleTraderDetailsOverviewViewModel : IMap<OrganisationData, SoleTraderDetailsOverviewViewModel>
    {
        public SoleTraderDetailsOverviewViewModel Map(OrganisationData source)
        {
            return new SoleTraderDetailsOverviewViewModel
            {
                Address = new AddressViewModel
                {
                    Address = source.BusinessAddress,
                    OrganisationId = source.Id,
                    OrganisationType = source.OrganisationType
                },
                BusinessTradingName = source.TradingName
            };
        }
    }
}