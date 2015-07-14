namespace EA.Weee.Requests.Tests.Unit.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Mappings;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Xunit;

    public class GetContactPersonByOrganisationIdHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task GetContactPersonByOrganisationIdHandler_RequestContactPerson_ReturnsContactPerson()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();
            var contactMapper = new ContactMap();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new GetContactPersonByOrganisationIdHandler(context, contactMapper);

            var contactPerson = await handler.HandleAsync(new GetContactPersonByOrganisationId(organisations.FirstOrDefault().Id));

            Assert.NotNull(contactPerson);
            Assert.Equal(organisations.FirstOrDefault().Contact.FirstName, contactPerson.FirstName);
            Assert.Equal(organisations.FirstOrDefault().Contact.LastName, contactPerson.LastName);
            Assert.Equal(organisations.FirstOrDefault().Contact.Position, contactPerson.Position);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("SFW Ltd"),
            });
        }
    }
}
