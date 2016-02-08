namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Organisations;
    using Prsd.Core.Mapper;
    using ViewModels.Scheme.Overview.OrganisationDetails;
    using Web.ViewModels.Shared;

    public class OrganisationDataToRegisteredCompanyDetailsOverviewViewModel : IMap<OrganisationData, RegisteredCompanyDetailsOverviewViewModel>
    {
        public RegisteredCompanyDetailsOverviewViewModel Map(OrganisationData source)
        {
            return new RegisteredCompanyDetailsOverviewViewModel
            {
                Address = new AddressViewModel
                {
                    Address = source.BusinessAddress,
                    OrganisationId = source.Id,
                    OrganisationType = source.OrganisationType
                },
                BusinessTradingName = source.TradingName,
                CompanyName = source.Name,
                CompanyRegistrationNumber = source.CompanyRegistrationNumber
            };
        }
    }
}