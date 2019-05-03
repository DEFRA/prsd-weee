namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.CheckYourReturn;
    using RequestHandlers.Factories;
    using Requests.AatfReturn;
    using Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using Weee.Tests.Core;
    using Xunit;

    public class GetReturnHandlerTests
    {
        private GetReturnHandler handler;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMap<ReturnQuarterWindow, ReturnData> mapper;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IFetchNonObligatedWeeeForReturnDataAccess fetchNonObligatedWeeeDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchObligatedWeeeDataAccess;
        private readonly IWeeeSentOnDataAccess sentOnAatfSiteDataAccess;
        private readonly IFetchAatfByOrganisationIdDataAccess fetchAatfByOrganisationIdDataAccess;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IGenericDataAccess genericDataAccess;

        public GetReturnHandlerTests()
        {
            returnDataAccess = A.Fake<IReturnDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            mapper = A.Fake<IMap<ReturnQuarterWindow, ReturnData>>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            sentOnAatfSiteDataAccess = A.Fake<IWeeeSentOnDataAccess>();
            fetchNonObligatedWeeeDataAccess = A.Fake<IFetchNonObligatedWeeeForReturnDataAccess>();
            fetchObligatedWeeeDataAccess = A.Fake<IFetchObligatedWeeeForReturnDataAccess>();
            fetchAatfByOrganisationIdDataAccess = A.Fake<IFetchAatfByOrganisationIdDataAccess>();
            returnSchemeDataAccess = A.Fake<IReturnSchemeDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();

            handler = new GetReturnHandler(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                returnDataAccess,
                organisationDataAccess,
                mapper,
                quarterWindowFactory,
                fetchNonObligatedWeeeDataAccess,
                fetchObligatedWeeeDataAccess,
                fetchAatfByOrganisationIdDataAccess,
                sentOnAatfSiteDataAccess,
                returnSchemeDataAccess,
                genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IMap<ReturnQuarterWindow, ReturnData>>(),
                A.Dummy<IQuarterWindowFactory>(),
                A.Dummy<IFetchNonObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchAatfByOrganisationIdDataAccess>(),
                A.Dummy<IWeeeSentOnDataAccess>(),
                A.Dummy<IReturnSchemeDataAccess>(),
                A.Dummy<IGenericDataAccess>());
                Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            var dataAccess = A.Fake<IReturnDataAccess>();

            handler = new GetReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IMap<ReturnQuarterWindow, ReturnData>>(),
                A.Dummy<IQuarterWindowFactory>(),
                A.Dummy<IFetchNonObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchAatfByOrganisationIdDataAccess>(),
                A.Dummy<IWeeeSentOnDataAccess>(),
                A.Dummy<IReturnSchemeDataAccess>(),
                A.Dummy<IGenericDataAccess>());
                Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandeAsync_GivenReturn_ReturnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturn(returnId));

            A.CallTo(() => returnDataAccess.GetById(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_QuarterWindowShouldBeRetrieved()
        {
            var @return = new Return(A.Fake<Operator>(), A.Fake<Quarter>(), A.Fake<ReturnStatus>());

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);

            var result = await handler.HandleAsync(A.Dummy<GetReturn>());

            A.CallTo(() => quarterWindowFactory.GetAnnualQuarter(@return.Quarter)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_NonObligatedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturn(returnId));

            A.CallTo(() => fetchNonObligatedWeeeDataAccess.FetchNonObligatedWeeeForReturn(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_ObligatedReceivedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturn(returnId));

            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReceivedForReturn(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_ObligatedReusedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturn(returnId));

            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReusedForReturn(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_ObligatedSentOnValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();
            var @return = A.Fake<Return>();
            var aatfList = A.Fake<List<Aatf>>();

            A.CallTo(() => returnDataAccess.GetById(returnId)).Returns(@return);
            A.CallTo(() => fetchAatfByOrganisationIdDataAccess.FetchAatfByOrganisationId(@return.Operator.Organisation.Id)).Returns(aatfList);

            var result = await handler.HandleAsync(new GetReturn(returnId));

            foreach (var aatf in aatfList)
            {
                A.CallTo(() => sentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(aatf.Id, returnId)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_AatfsShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturn(returnId));

            var organisationId = new Guid();

            A.CallTo(() => fetchAatfByOrganisationIdDataAccess.FetchAatfByOrganisationId(organisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_MapperShouldBeCalled()
        {
            var @return = new Return(A.Fake<Operator>(), A.Fake<Quarter>(), A.Fake<ReturnStatus>());
            var quarterWindow = new Domain.DataReturns.QuarterWindow(DateTime.MaxValue, DateTime.MaxValue);
            var nonObligatedValues = new List<NonObligatedWeee>();
            var obligatedReceivedValues = new List<WeeeReceivedAmount>();
            var obligatedReusedValues = new List<WeeeReusedAmount>();
            var obligatedSentOnValues = new List<WeeeSentOnAmount>();

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarter(A<Quarter>._)).Returns(quarterWindow);
            A.CallTo(() => fetchNonObligatedWeeeDataAccess.FetchNonObligatedWeeeForReturn(A<Guid>._)).Returns(nonObligatedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReceivedForReturn(A<Guid>._)).Returns(obligatedReceivedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReusedForReturn(A<Guid>._)).Returns(obligatedReusedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeSentOnForReturnByReturn(A<Guid>._)).Returns(obligatedSentOnValues);

            await handler.HandleAsync(A.Dummy<GetReturn>());

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(c => c.QuarterWindow.IsSameOrEqualTo(quarterWindow)
                                                                                && c.NonObligatedWeeeList.IsSameOrEqualTo(nonObligatedValues)
                                                                                && c.ObligatedWeeeReceivedList.IsSameOrEqualTo(obligatedReceivedValues)
                                                                                && c.ObligatedWeeeReusedList.IsSameOrEqualTo(obligatedReusedValues)
                                                                                && c.ObligatedWeeeSentOnList.IsSameOrEqualTo(obligatedSentOnValues)
                                                                                && c.Return.Equals(@return)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_MappedObjectShouldBeReturned()
        {
            var returnData = new ReturnData();

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>._)).Returns(returnData);

            var result = await handler.HandleAsync(A.Dummy<GetReturn>());

            result.Should().Be(returnData);
        }
    }
}
