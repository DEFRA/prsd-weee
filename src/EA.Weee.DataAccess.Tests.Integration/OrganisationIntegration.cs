namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Xunit;

    public class OrganisationIntegration : IDisposable
    {
        private readonly WeeeContext context;

        private Organisation testOrganisationToCleanUp;

        public OrganisationIntegration()
        {
            var userContext = A.Fake<IUserContext>();

            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            context = new WeeeContext(userContext);
        }

        [Fact]
        public async Task CanCreateRegisteredCompany()
        {
            var contact = MakeContact();

            string name = "Test Name" + Guid.NewGuid();
            string tradingName = "Test Trading Name" + Guid.NewGuid();
            string crn = new Random().Next(100000000).ToString();
            var status = OrganisationStatus.Incomplete;
            var type = OrganisationType.RegisteredCompany;

            var organisationAddress = MakeAddress("O");
            var businessAddress = MakeUKAddress("B");
            var notificationAddress = MakeAddress("N");

            var organisation = Organisation.CreateRegisteredCompany(name, crn, tradingName);
            organisation.AddMainContactPerson(contact);
            organisation.AddAddress(AddressType.OrganisationAddress, organisationAddress);
            organisation.AddAddress(AddressType.RegisteredOrPPBAddress, businessAddress);
            organisation.AddAddress(AddressType.ServiceOfNoticeAddress, notificationAddress);

            this.testOrganisationToCleanUp = organisation;

            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            var thisTestOrganisationArray =
                context.Organisations.Where(o => o.Name == name).ToArray();

            Assert.NotNull(thisTestOrganisationArray);
            Assert.NotEmpty(thisTestOrganisationArray);

            var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

            VerifyOrganisation(name, null, crn, status, type, thisTestOrganisation);
            VerifyAddress(organisationAddress, thisTestOrganisation.OrganisationAddress);

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task CanCreateSoleTrader()
        {
            var contact = MakeContact();

            string tradingName = "Test Trading Name" + Guid.NewGuid();
            var status = OrganisationStatus.Incomplete;
            var type = OrganisationType.SoleTraderOrIndividual;

            var organisationAddress = MakeAddress("O");
            var businessAddress = MakeUKAddress("B");
            var notificationAddress = MakeAddress("N");

            var organisation = Organisation.CreateSoleTrader(tradingName);

            organisation.AddMainContactPerson(contact);
            organisation.AddAddress(AddressType.OrganisationAddress, organisationAddress);
            organisation.AddAddress(AddressType.RegisteredOrPPBAddress, businessAddress);
            organisation.AddAddress(AddressType.ServiceOfNoticeAddress, notificationAddress);

            this.testOrganisationToCleanUp = organisation;

            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            var thisTestOrganisationArray =
                context.Organisations.Where(o => o.TradingName == tradingName).ToArray();

            Assert.NotNull(thisTestOrganisationArray);
            Assert.NotEmpty(thisTestOrganisationArray);

            var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

            VerifyOrganisation(null, tradingName, null, status, type, thisTestOrganisation);
            VerifyAddress(organisationAddress, thisTestOrganisation.OrganisationAddress);

            await context.SaveChangesAsync();
        }

        private void VerifyOrganisation(string expectedName, string expectedTradingName, string expectedCrn, OrganisationStatus expectedStatus, OrganisationType expectedType, Organisation fromDatabase)
        {
            Assert.NotNull(fromDatabase);

            if (expectedType == OrganisationType.RegisteredCompany)
            {
                Assert.Equal(expectedName, fromDatabase.Name);
            }
            else
            {
                Assert.Equal(expectedTradingName, fromDatabase.TradingName);
            }

            Assert.Equal(expectedCrn, fromDatabase.CompanyRegistrationNumber);
            Assert.Equal(expectedStatus, fromDatabase.OrganisationStatus);
            Assert.Equal(expectedType, fromDatabase.OrganisationType);

            var thisTestOrganisationAddress = fromDatabase.OrganisationAddress;
            Assert.NotNull(thisTestOrganisationAddress);
        }

        private void VerifyAddress(Address expected, Address fromDatabase)
        {
            Assert.NotNull(fromDatabase);

            // this wants to be in a compare method on the class, doesn't it? will have to exclude Id/RowVersion though
            Assert.Equal(expected.Address1, fromDatabase.Address1);
            Assert.Equal(expected.Address2, fromDatabase.Address2);
            Assert.Equal(expected.TownOrCity, fromDatabase.TownOrCity);
            Assert.Equal(expected.CountyOrRegion, fromDatabase.CountyOrRegion);
            Assert.Equal(expected.Postcode, fromDatabase.Postcode);
            Assert.Equal(expected.Country.Id, fromDatabase.Country.Id);
            Assert.Equal(expected.Telephone, fromDatabase.Telephone);
            Assert.Equal(expected.Email, fromDatabase.Email);
        }

        public void Dispose()
        {
            context.Organisations.Remove(testOrganisationToCleanUp);
            context.SaveChangesAsync();
        }

        private static Address MakeAddress(string identifier)
        {
            Country country = MakeCountry();

            return new Address(
                "Line 1 " + identifier,
                "Line 2 " + identifier,
                "Town " + identifier,
                "Region" + identifier,
                "Postcode " + identifier,
                 country,
                "Phone" + identifier,
                "Email" + identifier);
        }

        private static Address MakeUKAddress(string identifier)
        {
            Country country = new Country(Guid.Parse("FE1E7E10-D8AA-47BD-B8B7-F2C5C43643F3"), "England");
            return new Address(
                "Line 1 " + identifier,
                "Line 2 " + identifier,
                "Town " + identifier,
                "Region" + identifier,
                "Postcode " + identifier,
                country,
                "Phone" + identifier,
                "Email" + identifier);
        }

        private static Country MakeCountry()
        {
            return new Country(new Guid(), "Country");
        }

        private Contact MakeContact()
        {
            return new Contact("Test firstname", "Test lastname", "Test position");
        }
    }
}