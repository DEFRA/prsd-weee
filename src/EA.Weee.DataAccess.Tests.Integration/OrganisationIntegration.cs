namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Data.Entity.Infrastructure;
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
            var organisationAddress = new Address(
                "Building O",
                "Line 1 O",
                "Line 2 O",
                "Town O",
                "Postcode O",
                "Country O");

            var businessAddress = new Address(
                "Building B",
                "Line 1 B",
                "Line 2 B",
                "Town B",
                "Postcode B",
                "Country B");

            var notificationAddress = new Address(
                "Building N",
                "Line 1 N",
                "Line 2 N",
                "Town N",
                "Postcode N",
                "Country N");

            var organisation = new Organisation("Test Organisation", "Registered company")
            {
                OrganisationAddress = organisationAddress,
                BusinessAddress = businessAddress,
                NotificationAddress = notificationAddress,
                CompanyRegistrationNumber = "12345678",
                Status = "Incomplete",
                TradingName = "Test Trading Name"
            };

            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            // it's good to know it doesn't crash at this point, but we ought to verify the org now exists!
        }
    }
}