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
    using RequestHandlers.Factories;
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
        public async Task HandleAsync_GivenUserHasAccess_MappedObjectShouldBeBuilt()
        {
            var authorization = new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess()
                .Build();

            var returnId = Guid.NewGuid();
            var @return = new Return(A.Fake<Operator>(), A.Fake<Quarter>(), A.Fake<ReturnStatus>());
            var quarter = A.Fake<Quarter>();

            var dataAccess = A.Fake<IReturnDataAccess>();
            //var returnData = new ReturnData();
            var mapper = A.Fake<IMap<ReturnQuarterWindow, ReturnData>>();
            var quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            var quarterWindow = new Domain.DataReturns.QuarterWindow(DateTime.MaxValue, DateTime.MinValue);
            var getReturn = new GetReturn(returnId);

            //A.CallTo(() => @return.Quarter).Returns(quarter);
            //A.CallTo(() => @return.Id).Returns(returnId);
            A.CallTo(() => dataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => quarterWindowFactory.GetQuarter(A<Quarter>._)).Returns(quarterWindow);
            //A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(c => c.QuarterWindow == quarterWindow))).Returns(returnData);

            var handler = new GetReturnHandler(authorization,
                dataAccess,
                A.Dummy<IOrganisationDataAccess>(),
                mapper,
                quarterWindowFactory);

            var result = await handler.HandleAsync(getReturn);

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(c => c.QuarterWindow == quarterWindow && c.Return.Equals(@return))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenUserHasAccess_MappedObjectShouldBeReturned()
        {
            var authorization = new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess()
                .Build();

            var returnId = Guid.NewGuid();
            var @return = A.Fake<Return>();
            var quarter = A.Fake<Quarter>();

            var dataAccess = A.Fake<IReturnDataAccess>();
            var returnData = new ReturnData();
            var mapper = A.Fake<IMap<ReturnQuarterWindow, ReturnData>>();
            var quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            var quarterWindow = A.Fake<Domain.DataReturns.QuarterWindow>();
            var getReturn = new GetReturn(returnId);

            A.CallTo(() => @return.Quarter).Returns(quarter);
            A.CallTo(() => @return.Id).Returns(returnId);
            A.CallTo(() => dataAccess.GetById(returnId)).Returns(@return);
            A.CallTo(() => quarterWindowFactory.GetQuarter(@return.Quarter)).Returns(quarterWindow);
            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(c => c.QuarterWindow == quarterWindow && c.Return.Equals(@return)))).Returns(returnData);
            
            var handler = new GetReturnHandler(authorization,
                dataAccess,
                A.Dummy<IOrganisationDataAccess>(),
                mapper,
                quarterWindowFactory);

            var result = await handler.HandleAsync(getReturn);

            result.Should().Be(returnData);
        }
    }
}
