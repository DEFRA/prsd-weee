namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.Aatf;
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
        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetAatfsHandler handler = new GetAatfsHandler(
                authorization,
                A.Dummy<IMap<Aatf, AatfDataList>>(),
                A.Dummy<IGetAatfsDataAccess>());

            // Act
            Func<Task> aatfs = async () => await handler.HandleAsync(A.Dummy<GetAatfs>());

            // Asert
            await Assert.ThrowsAsync<SecurityException>(aatfs);
        }
    }
}
