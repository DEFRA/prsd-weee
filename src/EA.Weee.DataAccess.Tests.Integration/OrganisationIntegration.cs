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
        public async Task CanCreateOrganisation()
        {
            var contact = new Contact("Test firstname", "Test lastname", "Test position");

            string name = "Test Name" + Guid.NewGuid();
            string tradingName = "Test Trading Name" + Guid.NewGuid();
            string crn = new Random().Next(100000000).ToString();
            var status = OrganisationStatus.Incomplete;
            var type = OrganisationType.RegisteredCompany;

            var organisationAddress = MakeAddress("O");
            var businessAddress = MakeAddress("B");
            var notificationAddress = MakeAddress("N");

            var organisation = new Organisation(name, type, status)
            {
                Contact = contact,
                OrganisationAddress = organisationAddress,
                BusinessAddress = businessAddress,
                NotificationAddress = notificationAddress,
                CompanyRegistrationNumber = crn,
                OrganisationStatus = status,
                TradingName = tradingName
            };

            this.testOrganisationToCleanUp = organisation;

            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            var thisTestOrganisationArray =
                context.Organisations.Where(o => o.Name == name).ToArray();

            Assert.NotNull(thisTestOrganisationArray);
            Assert.NotEmpty(thisTestOrganisationArray);

            var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();
            Assert.NotNull(thisTestOrganisation);

            Assert.Equal(name, thisTestOrganisation.Name);
            Assert.Equal(tradingName, thisTestOrganisation.TradingName);
            Assert.Equal(crn, thisTestOrganisation.CompanyRegistrationNumber);
            Assert.Equal(status, thisTestOrganisation.OrganisationStatus);
            Assert.Equal(type, thisTestOrganisation.OrganisationType);

            var thisTestOrganisationAddress = thisTestOrganisation.OrganisationAddress;
            Assert.NotNull(thisTestOrganisationAddress);

            // this wants to be in a compare method on the class, doesn't it? will have to exclude Id/RowVersion though
            Assert.Equal(organisationAddress.Address1, thisTestOrganisationAddress.Address1);
            Assert.Equal(organisationAddress.Address2, thisTestOrganisationAddress.Address2);
            Assert.Equal(organisationAddress.TownOrCity, thisTestOrganisationAddress.TownOrCity);
            Assert.Equal(organisationAddress.CountyOrRegion, thisTestOrganisationAddress.CountyOrRegion);
            Assert.Equal(organisationAddress.PostalCode, thisTestOrganisationAddress.PostalCode);
            Assert.Equal(organisationAddress.Country, thisTestOrganisationAddress.Country);
            Assert.Equal(organisationAddress.Telephone, thisTestOrganisationAddress.Telephone);
            Assert.Equal(organisationAddress.Email, thisTestOrganisationAddress.Email);

            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Organisations.Remove(testOrganisationToCleanUp);
            context.SaveChangesAsync();
        }

        private Address MakeAddress(string identifier)
        {
            return new Address(
                "Line 1 " + identifier,
                "Line 2 " + identifier,
                "Town " + identifier,
                "Region" + identifier,
                "Postcode " + identifier,
                "Country " + identifier,
                "Phone" + identifier,
                "Email" + identifier);
        }
    }
}