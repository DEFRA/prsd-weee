namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;

    public class AddContactPersonToOrganisationHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        private readonly IWeeeAuthorization permissiveAuthorization =
            AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private readonly IWeeeAuthorization denyingAuthorization =
            AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

        [Fact]
        public async Task AddContactPersonToOrganisationHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new AddContactPersonToOrganisationHandler(A.Fake<WeeeContext>(), denyingAuthorization);
            var message = new AddContactPersonToOrganisation(A<Guid>._, A<ContactData>._);

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task AddContactPersonToOrganisationHandler_HappyPath_AddsContactPerson()
        {
            var organisationId = Guid.NewGuid();
            var organisation = GetOrganisationWithId(organisationId);

            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                organisation
            }));

            var handler = new AddContactPersonToOrganisationHandler(context, permissiveAuthorization);
            var message = new AddContactPersonToOrganisation(organisationId, new ContactData
            {
                FirstName = "Some first name",
                LastName = "Some last name",
                Position = "Some position"
            });

            await handler.HandleAsync(message);

            Assert.True(organisation.HasContact);
            Assert.Equal(message.ContactPerson.FirstName, organisation.Contact.FirstName);
            Assert.Equal(message.ContactPerson.LastName, organisation.Contact.LastName);
            Assert.Equal(message.ContactPerson.Position, organisation.Contact.Position);
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(id);
            return organisation;
        }
    }
}
