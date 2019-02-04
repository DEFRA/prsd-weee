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
    using Domain.DataReturns;
    using Factories;
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
                A.Dummy<IMap<ReturnQuarterWindow, ReturnData>>(),
                A.Dummy<IQuarterWindowFactory>());

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
                A.Dummy<IMap<ReturnQuarterWindow, ReturnData>>(),
                A.Dummy<IQuarterWindowFactory>());

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

            //var returnQuarterWindow = A.Fake<ReturnQuarterWindow>();
            var @return = A.Fake<Return>();
            var dataAccess = A.Fake<IReturnDataAccess>();
            var returnData = A.Fake<ReturnData>();
            var mapper = A.Fake<IMap<ReturnQuarterWindow, ReturnData>>();
            var quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
           // var quarterWindow = A.Fake<Domain.DataReturns.QuarterWindow>();

            A.CallTo(() => dataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>._)).Returns(returnData);
            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._)).Returns(A.Fake<Domain.DataReturns.QuarterWindow>());

            var handler = new GetReturnHandler(authorization,
                dataAccess,
                A.Dummy<IOrganisationDataAccess>(),
                mapper,
                quarterWindowFactory);

            var result = await handler.HandleAsync(A.Dummy<GetReturn>());

            result.Should().Be(returnData);
        }
    }
}
