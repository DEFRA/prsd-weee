﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Mappings;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class GetContactPersonByOrganisationIdHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task GetContactPersonByOrganisationIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new GetContactPersonByOrganisationIdHandler(authorization, A.Dummy<WeeeContext>(), new ContactMap());
            var message = new GetContactPersonByOrganisationId(Guid.NewGuid());

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task GetContactPersonByOrganisationIdHandler_RequestContactPerson_ReturnsContactPerson()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();
            var contactMapper = new ContactMap();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            var handler = new GetContactPersonByOrganisationIdHandler(authorization, context, contactMapper);

            var contactPerson =
                await handler.HandleAsync(new GetContactPersonByOrganisationId(organisations.FirstOrDefault().Id));

            Assert.NotNull(contactPerson);
            Assert.True(false);
            //Assert.Equal(organisations.FirstOrDefault().Contact.FirstName, contactPerson.FirstName);
            //Assert.Equal(organisations.FirstOrDefault().Contact.LastName, contactPerson.LastName);
            //Assert.Equal(organisations.FirstOrDefault().Contact.Position, contactPerson.Position);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("TEST Ltd")
            });
        }
    }
}
