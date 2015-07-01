namespace EA.Weee.Web.Tests.Unit.Requests
{
    using ViewModels.OrganisationRegistration.Details;
    using Weee.Web.Requests;
    using Xunit;

    public class RequestCreatorTests
    {
        [Fact]
        public void SoleTraderDetailsViewModel_AllPropertiesPopulated_ThenAllRequestPropertiesFullyPopulated()
        {
            const string businessTradingName = "Test business trading name";

            var soleTraderDetailsRequestCreator = new SoleTraderDetailsRequestCreator();
            var request = soleTraderDetailsRequestCreator.ViewModelToRequest(FullyPopulatedSoleTraderDetailsViewModel(businessTradingName));

            Assert.Equal(businessTradingName, request.TradingName);
        }

        [Fact]
        public void PartnershipDetailsViewModel_AllPropertiesPopulated_ThenAllRequestPropertiesFullyPopulated()
        {
            const string businessTradingName = "Test business trading name";

            var partnershipDetailsRequestCreator = new PartnershipDetailsRequestCreator();
            var request = partnershipDetailsRequestCreator.ViewModelToRequest(FullyPopulatedPartnershipDetailsViewModel(businessTradingName));

            Assert.Equal(businessTradingName, request.TradingName);
        }

        [Fact]
        public void RegisteredCopmanyDetailsViewModel_AllPropertiesPopulated_ThenAllRequestPropertiesFullyPopulated()
        {
            const string companyName = "Test company name";
            const string businessTradingName = "Test business trading name";
            const string companyRegistrationNumber = "AB123456";

            var registeredCompanyDetailsRequestCreator = new RegisteredCompanyDetailsRequestCreator();
            var request = registeredCompanyDetailsRequestCreator.ViewModelToRequest(FullyPopulatedRegisteredCompanyDetailsViewModel(companyName, businessTradingName, companyRegistrationNumber));

            Assert.Equal(businessTradingName, request.TradingName);
            Assert.Equal(companyName, request.BusinessName);
            Assert.Equal(companyRegistrationNumber, request.CompanyRegistrationNumber);
        }

        private SoleTraderDetailsViewModel FullyPopulatedSoleTraderDetailsViewModel(string businessTradingName)
        {
            return new SoleTraderDetailsViewModel
            {
                BusinessTradingName = businessTradingName
            };
        }

        private PartnershipDetailsViewModel FullyPopulatedPartnershipDetailsViewModel(string businessTradingName)
        {
            return new PartnershipDetailsViewModel
            {
                BusinessTradingName = businessTradingName
            };
        }

        private RegisteredCompanyDetailsViewModel FullyPopulatedRegisteredCompanyDetailsViewModel(string companyName, string businessTradingName, string companyRegistrationNumber)
        {
            return new RegisteredCompanyDetailsViewModel
            {
                CompanyName = companyName,
                BusinessTradingName = businessTradingName,
                CompaniesRegistrationNumber = companyRegistrationNumber
            };
        }
    }
}
