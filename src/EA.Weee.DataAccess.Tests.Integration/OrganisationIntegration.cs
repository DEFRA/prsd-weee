namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
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

            const string Name = "Test Organisation For CCOTest";
            const string TradingName = "Test Trading Name";
            const string Crn = "12345678";
            var status = OrganisationStatus.Incomplete;
            var type = OrganisationType.RegisteredCompany;

            var organisationAddress = MakeAddress("O");
            var businessAddress = MakeAddress("B");
            var notificationAddress = MakeAddress("N");

            var organisation = new Organisation(Name, type, status)
            {
                Contact = contact,
                OrganisationAddress = organisationAddress,
                BusinessAddress = businessAddress,
                NotificationAddress = notificationAddress,
                CompanyRegistrationNumber = Crn,
                OrganisationStatus = status,
                TradingName = TradingName
            };

            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            var thisTestOrganisationArray = context.Organisations.Where(o => o.Name == "Test Organisation For CCOTest").ToArray();

            Assert.NotNull(thisTestOrganisationArray);
            Assert.NotEmpty(thisTestOrganisationArray);

            var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

            Assert.Equal(Name, thisTestOrganisation.Name);
            Assert.Equal(TradingName, thisTestOrganisation.TradingName);
            Assert.Equal(Crn, thisTestOrganisation.CompanyRegistrationNumber);
            Assert.Equal(status, thisTestOrganisation.OrganisationStatus);
            Assert.Equal(type, thisTestOrganisation.OrganisationType);

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