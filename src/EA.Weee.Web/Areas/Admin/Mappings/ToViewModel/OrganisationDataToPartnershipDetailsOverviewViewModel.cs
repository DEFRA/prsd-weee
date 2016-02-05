namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Organisations;
    using Prsd.Core.Mapper;
    using ViewModels.Scheme.Overview.OrganisationDetails;
    using Web.ViewModels.Shared;

    public class OrganisationDataToPartnershipDetailsOverviewViewModel : IMap<OrganisationData, PartnershipDetailsOverviewViewModel>
    {
        public PartnershipDetailsOverviewViewModel Map(OrganisationData source)
        {
            return new PartnershipDetailsOverviewViewModel
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