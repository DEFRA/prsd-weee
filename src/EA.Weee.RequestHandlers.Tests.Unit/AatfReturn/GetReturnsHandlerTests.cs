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
        private readonly IReturnFactory returnFactory;

        private readonly Fixture fixture;

        public GetReturnsHandlerTests()
        {
            populatedReturn = A.Fake<IGetPopulatedReturn>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            aatfDataAccess = A.Fake<IFetchAatfByOrganisationIdDataAccess>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            returnFactory = A.Fake<IReturnFactory>();

            fixture = new Fixture();

            handler = new GetReturnsHandler(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                populatedReturn,
                returnDataAccess,
                aatfDataAccess,
                quarterWindowFactory,
                systemDataDataAccess,
                returnFactory);
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
                A.Dummy<ISystemDataDataAccess>(),
                A.Dummy<IReturnFactory>());

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

            result.ReturnsList.Should().Contain(returnData.ElementAt(0));
            result.ReturnsList.Should().Contain(returnData.ElementAt(1));
            result.ReturnsList.Should().HaveCount(2);
        }

        [Fact]
        public void Task_HandleAsync_GivenOrganisation_ReturnQuarterShouldBeRetrieved()
        {
        }
    }
}
