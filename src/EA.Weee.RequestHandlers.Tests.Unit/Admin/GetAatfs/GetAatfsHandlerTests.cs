namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Requests.Admin;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAatfsHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        public GetAatfsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Dummy<IGetAatfsDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            
            var handler = new GetAatfsHandler(authorization, A.Dummy<IMap<Aatf, AatfDataList>>(), dataAccess);

            Func<Task> aatfs = async () => await handler.HandleAsync(A.Dummy<GetAatfs>());

            await Assert.ThrowsAsync<SecurityException>(aatfs);
        }

        [Fact]
        public async Task HandleAsync_GivenGetAatfsReturnRequest_DataAccessFetchIsCalled()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var handler = new GetAatfsHandler(authorization, A.Dummy<IMap<Aatf, AatfDataList>>(), dataAccess);

            await handler.HandleAsync(A.Dummy<GetAatfs>());
            A.CallTo(() => dataAccess.GetAatfs()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
