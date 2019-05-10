namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Internal
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GetAatfContactHandlerTests
    {
        private readonly IMap<AatfContact, AatfContactData> mapper;
        private readonly IAatfContactDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly GetAatfContactHandler handler;

        public GetAatfContactHandlerTests()
        {
            mapper = A.Fake<IMap<AatfContact, AatfContactData>>();
            dataAccess = A.Fake<IAatfContactDataAccess>();
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

            A.CallTo(() => dataAccess.GetContact(id)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfId_AatfContactShouldBeRetrieved()
        {
            var aatfContact = A.Fake<AatfContact>();

            A.CallTo(() => dataAccess.GetContact(A<Guid>._)).Returns(aatfContact);

            var result = await handler.HandleAsync(A.Dummy<GetAatfContact>());

            A.CallTo(() => dataAccess.GetContact(A<Guid>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenAddressesAndWeeReusedAmounts_SummaryDataShouldBeMapped()
        {
            var aatfContact = A.Fake<AatfContact>();

            A.CallTo(() => dataAccess.GetContact(A<Guid>._)).Returns(aatfContact);

            await handler.HandleAsync(A.Dummy<GetAatfContact>());

            A.CallTo(() => mapper.Map(A<AatfContact>.That.IsSameAs(aatfContact))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
