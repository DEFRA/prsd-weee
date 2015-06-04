namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain;
    using FakeItEasy;
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

            var organisationAddress = MakeAddress("O");
            var businessAddress = MakeAddress("B");
            var notificationAddress = MakeAddress("N");

            var organisation = new Organisation("Test Organisation For CCOTest", "Registered company")
            {
                Contact = contact,
                OrganisationAddress = organisationAddress,
                BusinessAddress = businessAddress,
                NotificationAddress = notificationAddress,
                CompanyRegistrationNumber = "12345678",
                Status = "Incomplete",
                TradingName = "Test Trading Name"
            };

            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            var thisTestOrganisationArray = context.Organisations.Where(o => o.Name == "Test Organisation For CCOTest").ToArray();

            Assert.NotNull(thisTestOrganisationArray);
            Assert.NotEmpty(thisTestOrganisationArray);

            var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

            context.Addresses.Remove(thisTestOrganisation.OrganisationAddress);
            context.Addresses.Remove(thisTestOrganisation.BusinessAddress);
            context.Addresses.Remove(thisTestOrganisation.NotificationAddress);
            context.Contacts.Remove(thisTestOrganisation.Contact);

            context.Organisations.Remove(thisTestOrganisation);

            await context.SaveChangesAsync();
        }

        private Address MakeAddress(string identifier)
        {
            return new Address(
                "Building " + identifier,
                "Line 1 " + identifier,
                "Line 2 " + identifier,
                "Town " + identifier,
                "Postcode " + identifier,
                "Country " + identifier);
        }
    }
}