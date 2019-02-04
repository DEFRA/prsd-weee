 namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using Requests.AatfReturn;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetReturnHandlerTests
    {
        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IMap<Return, ReturnData>>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            var dataAccess = A.Fake<IReturnDataAccess>();

            var handler = new GetReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IMap<Return, ReturnData>>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenUserHasAccess_MappedObjectShouldBeReturned()
        {
            var authorization = new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess()
                .Build();

            var @return = A.Fake<Return>();
            var dataAccess = A.Fake<IReturnDataAccess>();
            var returnData = A.Fake<ReturnData>();
            var mapper = A.Fake<IMap<Return, ReturnData>>();
            
            A.CallTo(() => dataAccess.GetById(A.Dummy<Guid>())).Returns(@return);
            A.CallTo(() => mapper.Map(@return)).Returns(returnData);

            var handler = new GetReturnHandler(authorization,
                dataAccess,
                A.Dummy<IOrganisationDataAccess>(),
                mapper);

            var result = await handler.HandleAsync(A.Dummy<GetReturn>());

            result.Should().Be(returnData);
        }
    }
}
