namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using Core.Organisations;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class GetContactHandlerTests
    {
        private readonly GetContactHandler handler;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<Contact, ContactData> mapper;

        public GetContactHandlerTests()
        {
            authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            dataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMap<Contact, ContactData>>();

            handler = new GetContactHandler(authorization, dataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_GivenNotOrganisationUser_ThrowsSecurityException()
        {
            var localAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var localHandler = new GetContactHandler(localAuthorization, dataAccess, mapper);

            Func<Task<ContactData>> action = async () => await localHandler.HandleAsync(A.Dummy<GetContact>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenContactId_ContactShouldBeRetrieved()
        {
            var id = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetContact(id, A.Dummy<Guid>()));

            A.CallTo(() => dataAccess.GetById<Contact>(id)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenContactNotFound_ArgumentExceptionExpected()
        {
            var id = Guid.NewGuid();

            A.CallTo(() => dataAccess.GetById<Contact>(id)).Returns((Contact)null);

            Func<Task<ContactData>> action = async () => await handler.HandleAsync(new GetContact(id, A.Dummy<Guid>()));

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task HandleAsync_GivenContactFound_ContactDataShouldBeMapped()
        {
            var id = Guid.NewGuid();
            var contact = A.Fake<Contact>();

            A.CallTo(() => dataAccess.GetById<Contact>(id)).Returns(contact);

            var result = await handler.HandleAsync(new GetContact(id, A.Dummy<Guid>()));

            A.CallTo(() => mapper.Map(contact)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenContactFound_ContactDataShouldBeReturned()
        {
            var id = Guid.NewGuid();
            var contact = A.Fake<Contact>();
            var contactData = new ContactData();

            A.CallTo(() => dataAccess.GetById<Contact>(id)).Returns(contact);
            A.CallTo(() => mapper.Map(contact)).Returns(contactData);

            var result = await handler.HandleAsync(new GetContact(id, A.Dummy<Guid>()));

            result.Should().Be(contactData);
        }
    }
}
