namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class AddContactPersonToSchemeHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        private readonly IWeeeAuthorization permissiveAuthorization =
            AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private readonly IWeeeAuthorization denyingAuthorization =
            AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

        [Fact]
        public async Task AddContactPersonHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new AddContactPersonHandler(A.Fake<WeeeContext>(), denyingAuthorization, A.Fake<IGenericDataAccess>());
            var message = new AddContactPerson(A.Dummy<Guid>(), A.Dummy<ContactData>(), A.Dummy<Guid?>());

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task AddContactPersonHandler_GivenNullContactId_AddsContactPerson()
        {
            var schemeId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var contact = A.Fake<Contact>();
            A.CallTo(() => contact.Id).Returns(contactId);
            var dataAccess = A.Fake<IGenericDataAccess>();
            var context = A.Fake<WeeeContext>();

            var handler = new AddContactPersonHandler(context, permissiveAuthorization, dataAccess);
            var message = new AddContactPerson(schemeId, new ContactData
            {
                FirstName = "Some first name",
                LastName = "Some last name",
                Position = "Some position"
            }, null);

            A.CallTo(() => dataAccess.Add<Contact>(A<Contact>.That.Matches(c => c.FirstName.Equals(message.ContactPerson.FirstName)
                                                                                && c.LastName.Equals(message.ContactPerson.LastName) &&
                                                                                c.Position.Equals(message.ContactPerson.Position))))
                .Returns(contact);

            var result = await handler.HandleAsync(message);

            A.CallTo(() => dataAccess.Add<Contact>(A<Contact>.That.Matches(c => c.FirstName.Equals(message.ContactPerson.FirstName)
            && c.LastName.Equals(message.ContactPerson.LastName) && c.Position.Equals(message.ContactPerson.Position)))).MustHaveHappened(1, Times.Exactly).Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));

            result.Should().Be(contactId);
        }

        [Fact]
        public async Task AddContactPersonHandler_GivenContactId_ContactPersonUpdated()
        {
            var schemeId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var dataAccess = A.Fake<IGenericDataAccess>();
            var context = A.Fake<WeeeContext>();
            var contact = A.Fake<Contact>();

            var handler = new AddContactPersonHandler(context, permissiveAuthorization, dataAccess);
            var message = new AddContactPerson(schemeId, new ContactData
            {
                FirstName = "Some first name",
                LastName = "Some last name",
                Position = "Some position"
            }, contactId);

            A.CallTo(() => dataAccess.GetById<Contact>(contactId)).Returns(contact);

            await handler.HandleAsync(message);

            contact.FirstName.Should().Be(message.ContactPerson.FirstName);
            contact.LastName.Should().Be(message.ContactPerson.LastName);
            contact.Position.Should().Be(message.ContactPerson.Position);

            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly);
        }
    }
}
