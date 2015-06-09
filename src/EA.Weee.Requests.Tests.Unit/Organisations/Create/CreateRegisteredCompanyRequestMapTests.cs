namespace EA.Weee.Requests.Tests.Unit.Organisations.Create
{
    using Domain;
    using RequestHandlers.Mappings;
    using Requests.Organisations.Create;
    using Xunit;

    public class CreateRegisteredCompanyRequestMapTests
    {
        [Fact]
        public void MapFullyPopulatedCreateRegisteredCompanyRequest_SetsTypeToPartnership()
        {
            const string tradingName = "test trading name";
            const string businessName = "test business name";
            const string companyRegistrationNumber = "AB123456";

            var mapper = new CreateRegisteredCompanyRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreateRegisteredCompanyRequest(tradingName, businessName, companyRegistrationNumber));

            Assert.Equal(OrganisationType.RegisteredCompany, organisation.OrganisationType);
        }

        [Fact]
        public void MapFullyPopulatedCreateRegisteredCompanyRequest_SetsStatusToIncomplete()
        {
            const string tradingName = "test trading name";
            const string businessName = "test business name";
            const string companyRegistrationNumber = "AB123456";

            var mapper = new CreateRegisteredCompanyRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreateRegisteredCompanyRequest(tradingName, businessName, companyRegistrationNumber));

            Assert.Equal(OrganisationStatus.Incomplete, organisation.OrganisationStatus);
        }

        [Fact]
        public void MapFullyPopulatedCreateRegisteredCompanyRequest_MapsAllProperties()
        {
            const string tradingName = "test trading name";
            const string businessName = "test business name";
            const string companyRegistrationNumber = "AB123456";

            var mapper = new CreateRegisteredCompanyRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreateRegisteredCompanyRequest(tradingName, businessName, companyRegistrationNumber));

            Assert.Equal(tradingName, organisation.TradingName);
            Assert.Equal(businessName, organisation.Name);
            Assert.Equal(companyRegistrationNumber, organisation.CompanyRegistrationNumber);
        }

        private CreateRegisteredCompanyRequest FullyPopulatedCreateRegisteredCompanyRequest(string tradingName, string businessName, string companyRegistrationNumber)
        {
            return new CreateRegisteredCompanyRequest
            {
                TradingName = tradingName,
                BusinessName = businessName,
                CompanyRegistrationNumber = companyRegistrationNumber
            };
        }
    }
}
