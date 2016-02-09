namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using Domain.Organisation;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class OrganisationTests
    {
        [Fact]
        public void CreateSoleTrader_SetsStatusToIncomplete_AndTypeToSoleTrader_AndTradingName()
        {
            const string tradingName = "test trading name";

            var result = Organisation.CreateSoleTrader(tradingName);

            Assert.Equal(OrganisationType.SoleTraderOrIndividual, result.OrganisationType);
            Assert.Equal(OrganisationStatus.Incomplete, result.OrganisationStatus);
            Assert.Equal(tradingName, result.TradingName);
        }

        [Fact]
        public void CreatePartnership_SetsStatusToIncomplete_AndTypeToPartnership_AndTradingName()
        {
            const string tradingName = "test trading name";

            var result = Organisation.CreatePartnership(tradingName);

            Assert.Equal(OrganisationType.Partnership, result.OrganisationType);
            Assert.Equal(OrganisationStatus.Incomplete, result.OrganisationStatus);
            Assert.Equal(tradingName, result.TradingName);
        }

        [Fact]
        public void CreateRegisteredCompany_SetsStatusToIncomplete_AndTypeToRegisteredCompany_AndCompanyNameAndCompanyNumberAndTradingName()
        {
            const string tradingName = "test trading name";
            const string companyName = "test company name";
            const string companyRegistrationNumber = "AB123456";

            var result = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);

            Assert.Equal(OrganisationType.RegisteredCompany, result.OrganisationType);
            Assert.Equal(OrganisationStatus.Incomplete, result.OrganisationStatus);
            Assert.Equal(companyName, result.Name);
            Assert.Equal(companyRegistrationNumber, result.CompanyRegistrationNumber);
            Assert.Equal(tradingName, result.TradingName);
        }

        [Fact]
        public void CreateRegisteredCompany_RegistrationNumberIsLessThan7Characters_ThrowsException()
        {
            const string companyName = "test company name";
            const string companyRegistrationNumber = "6chars";

            Assert.Throws<InvalidOperationException>(() => Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber));
        }

        [Fact]
        public void CreateRegisteredCompany_RegistrationNumberIsMoreThan15Characters_ThrowsException()
        {
            const string companyName = "test company name";
            const string companyRegistrationNumber = "1234567890ABCDEF";

            Assert.Throws<InvalidOperationException>(() => Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber));
        }

        [Fact]
        public void UpdateRegisteredCompanyDetails_CompanyNameIsNull_ThrowsArgumentNullException()
        {
            const string companyName = "Company name";
            const string companyRegistrationNumber = "123456789";
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);

            Assert.Throws<ArgumentNullException>(() => organisation.UpdateRegisteredCompanyDetails(null, companyRegistrationNumber, tradingName));
        }

        [Fact]
        public void UpdateRegisteredCompanyDetails_HappyPath_UpdatedDetails()
        {
            const string companyName = "Company name";
            const string companyRegistrationNumber = "123456789";
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);

            organisation.UpdateRegisteredCompanyDetails("SFW Ltd.", "999999999", tradingName);

            Assert.Equal(organisation.Name, "SFW Ltd.");
            Assert.Equal(organisation.CompanyRegistrationNumber, "999999999");
        }

        [Fact]
        public void UpdateSoleTraderOrIndividualDetails_TradingNameIsNull_ThrowsArgumentNullException()
        {
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateSoleTrader(tradingName);

            Assert.Throws<ArgumentNullException>(() => organisation.UpdateSoleTraderOrIndividualDetails(null));
        }

        [Fact]
        public void UpdateSoleTraderOrIndividualDetails_HappyPath_UpdatedDetails()
        {
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateSoleTrader(tradingName);

            organisation.UpdateSoleTraderOrIndividualDetails("SFW Ltd.");

            Assert.Equal(organisation.TradingName, "SFW Ltd.");
        }
    }
}
