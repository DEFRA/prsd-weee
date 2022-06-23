namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Internal
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Aatf;
    using Requests.Admin.Aatf;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Xunit;

    public class GetAatfContactHandlerTests
    {
        private readonly IMap<AatfContact, AatfContactData> mapper;
        private readonly IAatfDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly GetAatfContactHandler handler;

        public GetAatfContactHandlerTests()
        {
            mapper = A.Fake<IMap<AatfContact, AatfContactData>>();
            dataAccess = A.Fake<IAatfDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();
            handler = new GetAatfContactHandler(authorization, dataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetAatfContactHandler(authorization, dataAccess, mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfContact>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            var id = Guid.NewGuid();

            await handler.HandleAsync(new GetAatfContact(id));

            A.CallTo(() => dataAccess.GetContact(id)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfId_AatfContactShouldBeRetrieved()
        {
            var aatfContact = A.Fake<AatfContact>();

            A.CallTo(() => dataAccess.GetContact(A<Guid>._)).Returns(aatfContact);

            var result = await handler.HandleAsync(A.Dummy<GetAatfContact>());

            A.CallTo(() => dataAccess.GetContact(A<Guid>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void HandleAsync_GivenAatfId_MapperIsCalled()
        {
            var aatfContact = A.Fake<AatfContact>();

            A.CallTo(() => dataAccess.GetContact(A<Guid>._)).Returns(aatfContact);

            await handler.HandleAsync(A.Dummy<GetAatfContact>());

            A.CallTo(() => mapper.Map(A<AatfContact>.That.IsSameAs(aatfContact))).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void HandleAsync_GivenAatfId_CanEditContactDetailsSet(bool selectedValue)
        {
            var aatfContact = A.Fake<AatfContact>();

            A.CallTo(() => authorization.CheckUserInRole(A<Roles>._)).Returns(selectedValue);
            A.CallTo(() => dataAccess.GetContact(A<Guid>._)).Returns(aatfContact);

            var result = await handler.HandleAsync(A.Dummy<GetAatfContact>());

            result.CanEditContactDetails.Should().Be(selectedValue);
        }

        [Fact]
        public async void HandleAsync_GivenAatfId_MappedObjectShouldBeReturned()
        {
            var aatfContactData = new AatfContactData();

            A.CallTo(() => mapper.Map(A<AatfContact>._)).Returns(aatfContactData);

            var result = await handler.HandleAsync(A.Dummy<GetAatfContact>());

            result.Should().Be(aatfContactData);
        }
    }
}
