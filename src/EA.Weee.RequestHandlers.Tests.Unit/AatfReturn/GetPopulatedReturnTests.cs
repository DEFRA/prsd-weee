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

    public class GetPopulatedReturnTests
    {
        private GetPopulatedReturn populatedReturn;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMap<ReturnQuarterWindow, ReturnData> mapper;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IFetchNonObligatedWeeeForReturnDataAccess fetchNonObligatedWeeeDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchObligatedWeeeDataAccess;
        private readonly ISentOnAatfSiteDataAccess sentOnAatfSiteDataAccess;
        private readonly IFetchAatfByOrganisationIdDataAccess fetchAatfByOrganisationIdDataAccess;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;

        public GetPopulatedReturnTests()
        {
            returnDataAccess = A.Fake<IReturnDataAccess>();
            mapper = A.Fake<IMap<ReturnQuarterWindow, ReturnData>>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            sentOnAatfSiteDataAccess = A.Fake<ISentOnAatfSiteDataAccess>();
            fetchNonObligatedWeeeDataAccess = A.Fake<IFetchNonObligatedWeeeForReturnDataAccess>();
            fetchObligatedWeeeDataAccess = A.Fake<IFetchObligatedWeeeForReturnDataAccess>();
            fetchAatfByOrganisationIdDataAccess = A.Fake<IFetchAatfByOrganisationIdDataAccess>();
            returnSchemeDataAccess = A.Fake<IReturnSchemeDataAccess>();

            populatedReturn = new GetPopulatedReturn(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                returnDataAccess,
                mapper,
                quarterWindowFactory,
                fetchNonObligatedWeeeDataAccess,
                fetchObligatedWeeeDataAccess,
                sentOnAatfSiteDataAccess,
                fetchAatfByOrganisationIdDataAccess,
                returnSchemeDataAccess);
        }

        [Fact]
        public async Task GetReturnData_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            populatedReturn = new GetPopulatedReturn(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IMap<ReturnQuarterWindow, ReturnData>>(),
                A.Dummy<IQuarterWindowFactory>(),
                A.Dummy<IFetchNonObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchObligatedWeeeForReturnDataAccess>(),
                A.Dummy<ISentOnAatfSiteDataAccess>(),
                A.Dummy<IFetchAatfByOrganisationIdDataAccess>(),
                A.Dummy<IReturnSchemeDataAccess>());

            Func<Task> action = async () => await populatedReturn.GetReturnData(A.Dummy<Guid>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task GetReturnData_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            var dataAccess = A.Fake<IReturnDataAccess>();

            populatedReturn = new GetPopulatedReturn(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IMap<ReturnQuarterWindow, ReturnData>>(),
                A.Dummy<IQuarterWindowFactory>(),
                A.Dummy<IFetchNonObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchObligatedWeeeForReturnDataAccess>(),
                A.Dummy<ISentOnAatfSiteDataAccess>(),
                A.Dummy<IFetchAatfByOrganisationIdDataAccess>(),
                A.Dummy<IReturnSchemeDataAccess>());

            Func<Task> action = async () => await populatedReturn.GetReturnData(A.Dummy<Guid>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ReturnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId);

            A.CallTo(() => returnDataAccess.GetById(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_QuarterWindowShouldBeRetrieved()
        {
            var @return = new Return(A.Fake<Operator>(), A.Fake<Quarter>(), "id");

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);

            var result = await populatedReturn.GetReturnData(A.Dummy<Guid>());

            A.CallTo(() => quarterWindowFactory.GetAnnualQuarter(@return.Quarter)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_NonObligatedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId);

            A.CallTo(() => fetchNonObligatedWeeeDataAccess.FetchNonObligatedWeeeForReturn(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ObligatedReceivedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId);

            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReceivedForReturn(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ObligatedReusedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId);

            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReusedForReturn(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ObligatedSentOnValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();
            var @return = A.Fake<Return>();
            var aatfList = A.Fake<List<Aatf>>();

            A.CallTo(() => returnDataAccess.GetById(returnId)).Returns(@return);
            A.CallTo(() => fetchAatfByOrganisationIdDataAccess.FetchAatfByOrganisationId(@return.Operator.Organisation.Id)).Returns(aatfList);

            var result = await populatedReturn.GetReturnData(returnId);

            foreach (var aatf in aatfList)
            {
                A.CallTo(() => sentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(aatf.Id, returnId)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_AatfsShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId);

            var organisationId = new Guid();

            A.CallTo(() => fetchAatfByOrganisationIdDataAccess.FetchAatfByOrganisationId(organisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ReturnSchemesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId);

            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }
    
        [Fact]
        public async Task GetReturnData_GivenReturn_MapperShouldBeCalled()
        {
            var @return = new Return(A.Fake<Operator>(), A.Fake<Quarter>(), "id");
            var quarterWindow = new Domain.DataReturns.QuarterWindow(DateTime.MaxValue, DateTime.MaxValue);
            var nonObligatedValues = new List<NonObligatedWeee>();
            var obligatedReceivedValues = new List<WeeeReceivedAmount>();
            var obligatedReusedValues = new List<WeeeReusedAmount>();
            var obligatedSentOnValues = new List<WeeeSentOnAmount>();
            var returnSchemes = new List<ReturnScheme>();

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarter(A<Quarter>._)).Returns(quarterWindow);
            A.CallTo(() => fetchNonObligatedWeeeDataAccess.FetchNonObligatedWeeeForReturn(A<Guid>._)).Returns(nonObligatedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReceivedForReturn(A<Guid>._)).Returns(obligatedReceivedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReusedForReturn(A<Guid>._)).Returns(obligatedReusedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeSentOnForReturnByReturn(A<Guid>._)).Returns(obligatedSentOnValues);
            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(A<Guid>._)).Returns(returnSchemes);

            await populatedReturn.GetReturnData(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(c => c.QuarterWindow.IsSameOrEqualTo(quarterWindow)
                                                                                && c.NonObligatedWeeeList.IsSameOrEqualTo(nonObligatedValues)
                                                                                && c.ObligatedWeeeReceivedList.IsSameOrEqualTo(obligatedReceivedValues)
                                                                                && c.ObligatedWeeeReusedList.IsSameOrEqualTo(obligatedReusedValues)
                                                                                && c.ObligatedWeeeSentOnList.IsSameOrEqualTo(obligatedSentOnValues)
                                                                                && c.Return.Equals(@return)
                                                                                && c.ReturnSchemes.Equals(returnSchemes)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_MappedObjectShouldBeReturned()
        {
            var returnData = new ReturnData();

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>._)).Returns(returnData);

            var result = await populatedReturn.GetReturnData(A.Dummy<Guid>());

            result.Should().Be(returnData);
        }
    }
}
