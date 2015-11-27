namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using Domain.Organisation;
    using System;
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
        public void CreateRegisteredCompany_RegistrationNumberIsMoreThan10Characters_ThrowsException()
        {
            const string companyName = "test company name";
            const string companyRegistrationNumber = "12345678901";

            Assert.Throws<InvalidOperationException>(() => Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber));
        }
    }
}
