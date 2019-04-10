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
    using RequestHandlers.AatfReturn;
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
        public async Task AddContactPersonToSchemeHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new AddContactPersonHandler(A.Fake<WeeeContext>(), denyingAuthorization, A.Fake<IGenericDataAccess>());
            var message = new AddContactPerson(A.Dummy<Guid>(), A.Dummy<ContactData>(), A.Dummy<Guid?>());

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task AddContactPersonToSchemeHandler_GivenNullContactId_AddsContactPerson()
        {
            var schemeId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var scheme = GetSchemeWithId(schemeId);
            var dataAccess = A.Fake<IGenericDataAccess>();
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Scheme>
            {
                scheme
            }));

            var handler = new AddContactPersonHandler(context, permissiveAuthorization, dataAccess);
            var message = new AddContactPerson(schemeId, new ContactData
            {
                FirstName = "Some first name",
                LastName = "Some last name",
                Position = "Some position"
            }, null);

            await handler.HandleAsync(message);

            Assert.True(scheme.HasContact);
            Assert.Equal(message.ContactPerson.FirstName, scheme.Contact.FirstName);
            Assert.Equal(message.ContactPerson.LastName, scheme.Contact.LastName);
            Assert.Equal(message.ContactPerson.Position, scheme.Contact.Position);
        }

        [Fact]
        public async Task AddContactPersonToSchemeHandler_GivenContactId_ContactPersonUpdated()
        {
            var schemeId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var scheme = GetSchemeWithId(schemeId);
            var dataAccess = A.Fake<IGenericDataAccess>();
            var context = A.Fake<WeeeContext>();
            var contact = A.Fake<Contact>();

            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Scheme>
            {
                scheme
            }));

            var handler = new AddContactPersonHandler(context, permissiveAuthorization, dataAccess);
            var message = new AddContactPerson(schemeId, new ContactData
            {
                FirstName = "Some first name",
                LastName = "Some last name",
                Position = "Some position"
            }, null);

            A.CallTo(() => dataAccess.GetById<Contact>(contactId)).Returns(contact);

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
