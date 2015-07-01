namespace EA.Weee.Web.Requests
{
    using Requests.Base;
    using ViewModels.OrganisationRegistration.Details;
    using Weee.Requests.Organisations.Create;

    public class RegisteredCompanyDetailsRequestCreator : RequestCreator<RegisteredCompanyDetailsViewModel, CreateRegisteredCompanyRequest>, IRegisteredCompanyDetailsRequestCreator
    {
        public override CreateRegisteredCompanyRequest ViewModelToRequest(RegisteredCompanyDetailsViewModel viewModel)
        {
            // Auto mappings
            var request = base.ViewModelToRequest(viewModel);

            // Manual mappings (where names differ)
            request.TradingName = viewModel.BusinessTradingName;
            request.BusinessName = viewModel.CompanyName;
            request.CompanyRegistrationNumber = viewModel.CompaniesRegistrationNumber;

            return request;
        }
    }
}