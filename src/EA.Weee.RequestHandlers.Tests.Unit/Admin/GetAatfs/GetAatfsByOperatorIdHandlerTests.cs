namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAatfsByOperatorIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly IMap<Aatf, AatfDataList> aatfmap;
        private GetAatfsByOperatorIdHandler handler;

        public GetAatfsByOperatorIdHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IGetAatfsDataAccess>();
            this.aatfmap = A.Fake<IMap<Aatf, AatfDataList>>();

            handler = new GetAatfsByOperatorIdHandler(authorization, aatfmap, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetAatfsByOperatorIdHandler(authorization, aatfmap, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_GetAatfsShouldBeCalled()
        {
            var result = await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            A.CallTo(() => dataAccess.GetAatfs()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ReturnedListShouldBeMappedType()
        {
            var result = await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            result.Should().BeOfType(typeof(List<AatfDataList>));
        }
    }
}
