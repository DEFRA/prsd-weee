namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Requests.AatfReturn;
    using System;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess.DataAccess;
    using Domain;
    using Prsd.Core;
    using RequestHandlers.Factories;
    using Weee.Tests.Core;
    using Xunit;

    public class GetReturnsHandlerTests
    {
        private GetReturnsHandler handler;
        private readonly IGetPopulatedReturn populatedReturn;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IFetchAatfByOrganisationIdDataAccess aatfDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly Fixture fixture;

        public GetReturnsHandlerTests()
        {
            populatedReturn = A.Fake<IGetPopulatedReturn>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            aatfDataAccess = A.Fake<IFetchAatfByOrganisationIdDataAccess>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            fixture = new Fixture();

            handler = new GetReturnsHandler(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                populatedReturn,
                returnDataAccess,
                aatfDataAccess,
                quarterWindowFactory,
                systemDataDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetReturnsHandler(authorization,
                A.Dummy<IGetPopulatedReturn>(),
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IFetchAatfByOrganisationIdDataAccess>(),
                A.Dummy<IQuarterWindowFactory>(),
                A.Dummy<ISystemDataDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturns>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisation_ReturnsForOrganisationShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturns(organisationId));

            A.CallTo(() => returnDataAccess.GetByOrganisationId(organisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisation_GetPopulatedReturnsShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var returns = A.CollectionOfFake<Return>(2);

            A.CallTo(() => returns.ElementAt(0).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returns.ElementAt(1).Id).Returns(Guid.NewGuid());

            A.CallTo(() => returnDataAccess.GetByOrganisationId(organisationId)).Returns(returns);

            var result = await handler.HandleAsync(new GetReturns(organisationId));

            A.CallTo((() => populatedReturn.GetReturnData(returns.ElementAt(0).Id))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo((() => populatedReturn.GetReturnData(returns.ElementAt(1).Id))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisation_PopulatedReturnsShouldBeReturned()
        {
            var returns = A.CollectionOfFake<Return>(2);
            var returnData = A.CollectionOfFake<ReturnData>(2).ToArray();

            A.CallTo(() => returnDataAccess.GetByOrganisationId(A<Guid>._)).Returns(returns);
            A.CallTo((() => populatedReturn.GetReturnData(A<Guid>._))).ReturnsNextFromSequence(returnData);

            var result = await handler.HandleAsync(A.Dummy<GetReturns>());

            result.Should().Contain(returnData.ElementAt(0));
            result.Should().Contain(returnData.ElementAt(1));
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisation_OrganisationAatfsShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturns(organisationId));

            A.CallTo(() => aatfDataAccess.FetchAatfByOrganisationId(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingCurrentDate()
        {
            SystemTime.Freeze(new DateTime(2019, 1, 2));

            var testSystemData = new TestSystemData();

            testSystemData.ToggleFixedCurrentDateUsage(false);

            var result = await handler.HandleAsync(A.Dummy<GetReturns>());

            A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);
            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2019)
            && d.Month.Equals(1) && d.Day.Equals(2)))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenNotUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingCurrentDate()
        {
            SystemTime.Freeze(new DateTime(2019, 1, 2));

            var testSystemData = new TestSystemData();
            testSystemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);

            var result = await handler.HandleAsync(A.Dummy<GetReturns>());

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2019)
                                                                                                       && d.Month.Equals(1) && d.Day.Equals(2)))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingFixedSystemDate()
        {
            SystemTime.Freeze(new DateTime(2019, 1, 2));

            var testSystemData = new TestSystemData();
            testSystemData.ToggleFixedCurrentDateUsage(true);
            testSystemData.UpdateFixedCurrentDate(new DateTime(2019, 1, 3));

            A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);

            var result = await handler.HandleAsync(A.Dummy<GetReturns>());

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2019)
                                                                                                       && d.Month.Equals(1) && d.Day.Equals(3)))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        public class TestSystemData : SystemData
        {
        }
    }
}
