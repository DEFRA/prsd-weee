namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Organisations;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class AddContactPersonToSchemeHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        private readonly IWeeeAuthorization permissiveAuthorization =
            AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private readonly IWeeeAuthorization denyingAuthorization =
            AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

        [Fact]
        public async Task AddContactPersonToOrganisationHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new AddContactPersonToSchemeHandler(A.Fake<WeeeContext>(), denyingAuthorization);
            var message = new AddContactPersonToScheme(A.Dummy<Guid>(), A.Dummy<ContactData>());

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task AddContactPersonToOrganisationHandler_HappyPath_AddsContactPerson()
        {
            var schemeId = Guid.NewGuid();
            var scheme = GetSchemeWithId(schemeId);

            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Scheme>
            {
                scheme
            }));

            var handler = new AddContactPersonToSchemeHandler(context, permissiveAuthorization);
            var message = new AddContactPersonToScheme(schemeId, new ContactData
            {
                FirstName = "Some first name",
                LastName = "Some last name",
                Position = "Some position"
            });

            await handler.HandleAsync(message);

            Assert.True(scheme.HasContact);
            Assert.Equal(message.ContactPerson.FirstName, scheme.Contact.FirstName);
            Assert.Equal(message.ContactPerson.LastName, scheme.Contact.LastName);
            Assert.Equal(message.ContactPerson.Position, scheme.Contact.Position);
        }

        private Scheme GetSchemeWithId(Guid id)
        {
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Id).Returns(id);
            return scheme;
        }
    }
}
