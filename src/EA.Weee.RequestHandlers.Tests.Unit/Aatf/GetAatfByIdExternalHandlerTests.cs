namespace EA.Weee.RequestHandlers.Tests.Unit.Aatf
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAatfByIdExternalHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchAatfDataAccess dataAccess;
        private readonly IMap<Aatf, AatfData> mapper;
        private GetAatfByIdExternalHandler handler;

        public GetAatfByIdExternalHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IFetchAatfDataAccess>();
            this.mapper = A.Fake<IMap<Aatf, AatfData>>();

            this.handler = new GetAatfByIdExternalHandler(authorization, mapper, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new GetAatfByIdExternalHandler(authorization,
                A.Dummy<IMap<Aatf, AatfData>>(),
                this.dataAccess);

            var message = new GetAatfByIdExternal(Guid.NewGuid());
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => this.dataAccess.FetchById(message.AatfId)).Returns(aatf);

            Func<Task> action = () => this.handler.HandleAsync(message);

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();

            await handler.HandleAsync(new GetAatfByIdExternal(aatfId));

            A.CallTo(() => dataAccess.FetchById(aatfId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfData_ContactDataShouldBeMapped()
        {
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => dataAccess.FetchById(A<Guid>._)).Returns(aatf);

            await handler.HandleAsync(A.Dummy<GetAatfByIdExternal>());

            A.CallTo(() => mapper.Map(aatf)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenMappedAatfData_AatfDataExternalShouldBeReturn()
        {
            var aatf = A.Fake<Aatf>();
            var aatfData = A.Fake<AatfData>();

            A.CallTo(() => dataAccess.FetchById(A<Guid>._)).Returns(aatf);
            A.CallTo(() => mapper.Map(aatf)).Returns(aatfData);

            var result = await handler.HandleAsync(A.Dummy<GetAatfByIdExternal>());

            result.Should().Be(aatfData);
        }
    }
}
