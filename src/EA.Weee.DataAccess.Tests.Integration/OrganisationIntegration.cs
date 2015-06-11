namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Xunit;

    public class OrganisationIntegration
    {
        private readonly WeeeContext context;

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

            const string name = "Test Organisation For CCOTest";
            const string tradingName = "Test Trading Name";
            const string crn = "12345678";
            var status = OrganisationStatus.Incomplete;
            var type = OrganisationType.RegisteredCompany;

            var organisationAddress = MakeAddress("O");
            var businessAddress = MakeAddress("B");
            var notificationAddress = MakeAddress("N");

            var organisation = Organisation.CreateRegisteredCompany(name, crn, tradingName);
            organisation.AddMainContactPerson(contact);
            organisation.OrganisationAddress = organisationAddress;
            organisation.BusinessAddress = businessAddress;
            organisation.NotificationAddress = notificationAddress;

            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            var thisTestOrganisationArray =
                context.Organisations.Where(o => o.Name == "Test Organisation For CCOTest").ToArray();

            Assert.NotNull(thisTestOrganisationArray);
            Assert.NotEmpty(thisTestOrganisationArray);

            var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

            if (thisTestOrganisation != null)
            {
                Assert.Equal(name, thisTestOrganisation.Name);
                Assert.Equal(tradingName, thisTestOrganisation.TradingName);
                Assert.Equal(crn, thisTestOrganisation.CompanyRegistrationNumber);
                Assert.Equal(status, thisTestOrganisation.OrganisationStatus);
                Assert.Equal(type, thisTestOrganisation.OrganisationType);
                await CleanUp(thisTestOrganisation);
            }

            await context.SaveChangesAsync();
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

        private async Task CleanUp(Organisation organisation)
        {
            context.Organisations.Remove(organisation);

            await context.SaveChangesAsync();
        }
    }
}