﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.NonObligated;
    using RequestHandlers.AatfReturn.Specification;
    using RequestHandlers.Factories;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using Weee.Tests.Core;
    using Xunit;
    using FacilityType = Domain.AatfReturn.FacilityType;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;
    using ReturnStatus = Domain.AatfReturn.ReturnStatus;

    public class GetPopulatedReturnTests
    {
        private GetPopulatedReturn populatedReturn;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMap<ReturnQuarterWindow, ReturnData> mapper;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly INonObligatedDataAccess fetchNonObligatedWeeeDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchObligatedWeeeDataAccess;
        private readonly IFetchAatfDataAccess fetchAatfDataAccess;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        public GetPopulatedReturnTests()
        {
            returnDataAccess = A.Fake<IReturnDataAccess>();
            mapper = A.Fake<IMap<ReturnQuarterWindow, ReturnData>>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            fetchNonObligatedWeeeDataAccess = A.Fake<INonObligatedDataAccess>();
            fetchObligatedWeeeDataAccess = A.Fake<IFetchObligatedWeeeForReturnDataAccess>();
            fetchAatfDataAccess = A.Fake<IFetchAatfDataAccess>();
            returnSchemeDataAccess = A.Fake<IReturnSchemeDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            populatedReturn = new GetPopulatedReturn(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                returnDataAccess,
                mapper,
                quarterWindowFactory,
                fetchNonObligatedWeeeDataAccess,
                fetchObligatedWeeeDataAccess,
                fetchAatfDataAccess,
                returnSchemeDataAccess,
                genericDataAccess,
                systemDataDataAccess);
        }

        [Fact]
        public async Task GetReturnData_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            populatedReturn = new GetPopulatedReturn(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IMap<ReturnQuarterWindow, ReturnData>>(),
                A.Dummy<IQuarterWindowFactory>(),
                A.Dummy<INonObligatedDataAccess>(),
                A.Dummy<IFetchObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchAatfDataAccess>(),
                A.Dummy<IReturnSchemeDataAccess>(),
                A.Dummy<IGenericDataAccess>(),
                systemDataDataAccess);

            Func<Task> action = async () => await populatedReturn.GetReturnData(A.Dummy<Guid>(), A.Dummy<bool>());

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
                A.Dummy<INonObligatedDataAccess>(),
                A.Dummy<IFetchObligatedWeeeForReturnDataAccess>(),
                A.Dummy<IFetchAatfDataAccess>(),
                A.Dummy<IReturnSchemeDataAccess>(),
                A.Dummy<IGenericDataAccess>(),
                systemDataDataAccess);

            Func<Task> action = async () => await populatedReturn.GetReturnData(A.Dummy<Guid>(), A.Dummy<bool>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ReturnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId, A.Dummy<bool>());

            A.CallTo(() => returnDataAccess.GetById(returnId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_QuarterWindowShouldBeRetrieved()
        {
            var @return = new Return(A.Fake<Organisation>(), A.Fake<Quarter>(), "id", A.Fake<FacilityType>());

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);

            var result = await populatedReturn.GetReturnData(A.Dummy<Guid>(), A.Dummy<bool>());

            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(@return.Quarter)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_NonObligatedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId, A.Dummy<bool>());

            A.CallTo(() => fetchNonObligatedWeeeDataAccess.FetchNonObligatedWeeeForReturn(returnId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ObligatedReceivedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId, A.Dummy<bool>());

            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReceivedForReturn(returnId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ObligatedReusedValuesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId, A.Dummy<bool>());

            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReusedForReturn(returnId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturnNonSummary_AatfsForOrganisationShouldBeRetrieved()
        {
            var @return = new Return(Organisation.CreatePartnership("trading"), new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);

            A.CallTo(() => returnDataAccess.GetById(@return.Id)).Returns(@return);

            var result = await populatedReturn.GetReturnData(@return.Id, false);

            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnQuarterWindow(@return)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturnForSummaryAndReturnIsSubmitted_AatfsShouldBeRetrieved()
        {
            var @return = new Return(Organisation.CreatePartnership("trading"), new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf)
            {
                ReturnStatus = ReturnStatus.Submitted
            };
            A.CallTo(() => returnDataAccess.GetById(@return.Id)).Returns(@return);

            var result = await populatedReturn.GetReturnData(@return.Id, true);

            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnId(@return.Id)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnQuarterWindow(A<Return>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetReturnData_GivenReturnForSummaryAndReturnIsCreated_AatfsForOrganisationShouldBeRetrieved()
        {
            var @return = new Return(Organisation.CreatePartnership("trading"), new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);
            var quarterWindow = new EA.Weee.Domain.DataReturns.QuarterWindow(DateTime.Now, DateTime.Now.AddDays(1), QuarterType.Q1);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(@return.Quarter)).Returns(quarterWindow);
            A.CallTo(() => returnDataAccess.GetById(@return.Id)).Returns(@return);

            var result = await populatedReturn.GetReturnData(@return.Id, true);

            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnQuarterWindow(@return)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnId(A<Guid>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ReturnSchemesShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId, false);

            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(returnId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_ReturnReportsOnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await populatedReturn.GetReturnData(returnId, A.Dummy<bool>());

            A.CallTo(() => genericDataAccess.GetManyByExpression(A<ReturnReportOnByReturnIdSpecification>.That.Matches(s => s.ReturnId.Equals(returnId)))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturnForNonSummary_MapperShouldBeCalled()
        {
            var @return = new Return(A.Fake<Organisation>(), A.Fake<Quarter>(), "id", A.Fake<FacilityType>());
            var quarterWindow = new Domain.DataReturns.QuarterWindow(DateTime.MaxValue, DateTime.MaxValue, QuarterType.Q1);
            var nonObligatedValues = new List<NonObligatedWeee>();
            var obligatedReceivedValues = new List<WeeeReceivedAmount>();
            var obligatedReusedValues = new List<WeeeReusedAmount>();
            var obligatedSentOnValues = new List<WeeeSentOnAmount>();
            var returnSchemes = new List<ReturnScheme>();
            var reportsOn = new List<ReturnReportOn>();
            var aatfs = new List<Aatf>();

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._)).Returns(quarterWindow);
            A.CallTo(() => fetchNonObligatedWeeeDataAccess.FetchNonObligatedWeeeForReturn(A<Guid>._)).Returns(nonObligatedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReceivedForReturn(A<Guid>._)).Returns(obligatedReceivedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReusedForReturn(A<Guid>._)).Returns(obligatedReusedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeSentOnForReturnByReturn(A<Guid>._)).Returns(obligatedSentOnValues);
            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(A<Guid>._)).Returns(returnSchemes);
            A.CallTo(() => genericDataAccess.GetManyByExpression<ReturnReportOn>(A<ReturnReportOnByReturnIdSpecification>._)).Returns(reportsOn);
            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnQuarterWindow(A<Return>._)).Returns(aatfs);

            await populatedReturn.GetReturnData(A.Dummy<Guid>(), false);

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(c => c.QuarterWindow.Equals(quarterWindow)
                                                                               && c.Aatfs.Equals(aatfs)
                                                                                && c.NonObligatedWeeeList.Equals(nonObligatedValues)
                                                                                && c.ObligatedWeeeReceivedList.Equals(obligatedReceivedValues)
                                                                                && c.ObligatedWeeeReusedList.Equals(obligatedReusedValues)
                                                                                && c.ObligatedWeeeSentOnList.Equals(obligatedSentOnValues)
                                                                                && c.Return.Equals(@return)
                                                                                && c.ReturnSchemes.Equals(returnSchemes)
                                                                                && c.ReturnReportOns.Equals(reportsOn)))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturnForSummary_MapperShouldBeCalled()
        {
            var @return = new Return(A.Fake<Organisation>(), A.Fake<Quarter>(), "id", A.Fake<FacilityType>()) { ReturnStatus = ReturnStatus.Submitted };
            var quarterWindow = new Domain.DataReturns.QuarterWindow(DateTime.MaxValue, DateTime.MaxValue, QuarterType.Q1);
            var nonObligatedValues = new List<NonObligatedWeee>();
            var obligatedReceivedValues = new List<WeeeReceivedAmount>();
            var obligatedReusedValues = new List<WeeeReusedAmount>();
            var obligatedSentOnValues = new List<WeeeSentOnAmount>();
            var returnSchemes = new List<ReturnScheme>();
            var reportsOn = new List<ReturnReportOn>();
            var aatfs = new List<Aatf>();

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._)).Returns(quarterWindow);
            A.CallTo(() => fetchNonObligatedWeeeDataAccess.FetchNonObligatedWeeeForReturn(A<Guid>._)).Returns(nonObligatedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReceivedForReturn(A<Guid>._)).Returns(obligatedReceivedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeReusedForReturn(A<Guid>._)).Returns(obligatedReusedValues);
            A.CallTo(() => fetchObligatedWeeeDataAccess.FetchObligatedWeeeSentOnForReturnByReturn(A<Guid>._)).Returns(obligatedSentOnValues);
            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(A<Guid>._)).Returns(returnSchemes);
            A.CallTo(() => genericDataAccess.GetManyByExpression<ReturnReportOn>(A<ReturnReportOnByReturnIdSpecification>._)).Returns(reportsOn);
            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnId(A<Guid>._)).Returns(aatfs);

            await populatedReturn.GetReturnData(A.Dummy<Guid>(), true);

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(c => c.QuarterWindow.Equals(quarterWindow)
                                                                               && c.Aatfs.Equals(aatfs)
                                                                                && c.NonObligatedWeeeList.Equals(nonObligatedValues)
                                                                                && c.ObligatedWeeeReceivedList.Equals(obligatedReceivedValues)
                                                                                && c.ObligatedWeeeReusedList.Equals(obligatedReusedValues)
                                                                                && c.ObligatedWeeeSentOnList.Equals(obligatedSentOnValues)
                                                                                && c.Return.Equals(@return)
                                                                                && c.ReturnSchemes.Equals(returnSchemes)
                                                                                && c.ReturnReportOns.Equals(reportsOn)))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetReturnData_GivenReturnShouldUseSystemTime_MapperShouldBeCalledWithCorrectTime()
        {
            var systemDate = new DateTime(2019, 1, 2);
            SystemTime.Freeze(systemDate);
            var expectedDate = new DateTime(2019, 1, 1);

            var systemData = A.Fake<SystemData>();
            systemData.UpdateFixedCurrentDate(expectedDate);
            systemData.ToggleFixedCurrentDateUsage(true);

            A.CallTo(() => systemDataDataAccess.Get()).Returns(systemData);

            await populatedReturn.GetReturnData(A.Dummy<Guid>(), A.Dummy<bool>());

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(r => r.SystemDateTime.Equals(expectedDate))))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(r => r.SystemDateTime.Equals(systemDate)))).MustNotHaveHappened();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task GetReturnData_GivenReturnShouldNotUseSystemTime_MapperShouldBeCalledWithCorrectTime()
        {
            var expectedDate = new DateTime(2019, 1, 2);
            SystemTime.Freeze(expectedDate);

            var systemData = A.Fake<SystemData>();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get()).Returns(systemData);

            await populatedReturn.GetReturnData(A.Dummy<Guid>(), A.Dummy<bool>());

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>.That.Matches(r => r.SystemDateTime.Equals(expectedDate))))
                .MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task GetReturnData_GivenReturn_MappedObjectShouldBeReturned()
        {
            var returnData = new ReturnData();

            A.CallTo(() => mapper.Map(A<ReturnQuarterWindow>._)).Returns(returnData);

            var result = await populatedReturn.GetReturnData(A.Dummy<Guid>(), A.Dummy<bool>());

            result.Should().Be(returnData);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnNotFound_ArgumentExceptionExpected()
        {
            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns((Return)null);

            var exception = await Xunit.Record.ExceptionAsync(() => populatedReturn.GetReturnData(A.Dummy<Guid>(), A.Dummy<bool>()));

            exception.Should().BeOfType<ArgumentException>();
        }
    }
}
