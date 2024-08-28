namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using Domain.Organisation;
    using EA.Weee.Tests.Core;
    using System;
    using Xunit;

    public class OrganisationTests
    {
        [Fact]
        public void CreateSoleTrader_SetsStatusToIncomplete_AndTypeToSoleTrader_AndCompanyName()
        {
            const string companyName = "test company name";
            const string tradingName = "test trading name";

            var result = Organisation.CreateSoleTrader(companyName, tradingName);

            Assert.Equal(OrganisationType.SoleTraderOrIndividual, result.OrganisationType);
            Assert.Equal(OrganisationStatus.Incomplete, result.OrganisationStatus);
            Assert.Equal(companyName, result.Name);
            Assert.Equal(tradingName, result.TradingName);
        }

        [Fact]
        public void CreateSoleTrader_SetsStatusToIncomplete_AndTypeToSoleTrader_AndCompanyName_NullTradingName()
        {
            const string companyName = "test company name";

            var result = Organisation.CreateSoleTrader(companyName);

            Assert.Equal(OrganisationType.SoleTraderOrIndividual, result.OrganisationType);
            Assert.Equal(OrganisationStatus.Incomplete, result.OrganisationStatus);
            Assert.Equal(companyName, result.Name);
            Assert.Null(result.TradingName);
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
        public void CreateDirectRegistrantOrganisation_SetsStatusToIncomplete_AndCompanyNames()
        {
            const string companyName = "test company name";
            const string tradingName = "test trading name";
            const string companyRegistrationNumber = "1";

            var result = Organisation.CreateDirectRegistrantCompany(OrganisationType.DirectRegistrantPartnership, companyName, tradingName, companyRegistrationNumber);

            Assert.Equal(OrganisationType.DirectRegistrantPartnership, result.OrganisationType);
            Assert.Equal(OrganisationStatus.Incomplete, result.OrganisationStatus);
            Assert.Equal(companyName, result.Name);
            Assert.Equal(tradingName, result.TradingName);
            Assert.Equal(companyRegistrationNumber, result.CompanyRegistrationNumber);
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

            Assert.Equal("SFW Ltd.", organisation.Name);
            Assert.Equal("999999999", organisation.CompanyRegistrationNumber);
        }

        [Fact]
        public void UpdateSoleTraderDetails_CompanyNameIsNull_ThrowsArgumentNullException()
        {
            const string companyName = "Company name";
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateSoleTrader(companyName, tradingName);

            Assert.Throws<ArgumentNullException>(() => organisation.UpdateSoleTraderDetails(null, tradingName));
        }

        [Fact]
        public void UpdateSoleTraderDetails_TradingNameIsNull_UpdatedDetails()
        {
            const string companyName = "Company name";
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateSoleTrader(companyName, tradingName);

            organisation.UpdateSoleTraderDetails("SFW Ltd.", null);

            Assert.Equal("SFW Ltd.", organisation.Name);
            Assert.Null(organisation.TradingName);
        }

        [Fact]
        public void UpdateSoleTraderDetails_HappyPath_UpdatedDetails()
        {
            const string companyName = "Company name";
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateSoleTrader(companyName, tradingName);

            organisation.UpdateSoleTraderDetails("SFW Ltd.", "SFW");

            Assert.Equal("SFW Ltd.", organisation.Name);
            Assert.Equal("SFW", organisation.TradingName);
        }

        [Fact]
        public void UpdatePartnershipDetails_TradingNameIsNull_ThrowsArgumentNullException()
        {
            const string tradingName = "Trading name";

            var organisation = Organisation.CreatePartnership(tradingName);

            Assert.Throws<ArgumentNullException>(() => organisation.UpdatePartnershipDetails(null));
        }

        [Fact]
        public void UpdatePartnershipDetails_HappyPath_UpdatedDetails()
        {
            const string tradingName = "Trading name";

            var organisation = Organisation.CreateSoleTrader(tradingName);

            organisation.UpdatePartnershipDetails("SFW Ltd.");

            Assert.Equal("SFW Ltd.", organisation.TradingName);
        }
    }
}
