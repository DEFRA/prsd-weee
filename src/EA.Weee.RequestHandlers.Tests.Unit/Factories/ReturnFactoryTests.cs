namespace EA.Weee.RequestHandlers.Tests.Unit.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Prsd.Core;
    using RequestHandlers.Factories;
    using Requests.AatfReturn;
    using Xunit;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class ReturnFactoryTests
    {
        private readonly ReturnFactory returnFactory;
        private readonly IReturnFactoryDataAccess returnFactoryDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;

        public ReturnFactoryTests()
        {
            returnFactoryDataAccess = A.Fake<IReturnFactoryDataAccess>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();

            SetupSystemTime();

            returnFactory = new ReturnFactory(returnFactoryDataAccess,
                systemDataDataAccess,
                quarterWindowFactory);
        }

        [Fact]
        public async Task GetReturnQuarter_GivenNotUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingCurrentDate()
        {
            SystemTime.Freeze(new DateTime(2019, 1, 2));

            var result = await returnFactory.GetReturnQuarter(A.Dummy<Guid>(), A.Dummy<FacilityType>());

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2019)
            && d.Month.Equals(1) && d.Day.Equals(2)))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task GetReturnQuarter_GivenUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingFixedSystemDate()
        {
            SystemTime.Freeze(new DateTime(2019, 1, 2));

            var date = new DateTime(2020, 1, 3);

            SetupFixedDate(date);

            var result = await returnFactory.GetReturnQuarter(A.Dummy<Guid>(), A.Dummy<FacilityType>());

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2020)
                                                                                                       && d.Month.Equals(1) && d.Day.Equals(3)))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task GetReturnQuarter_GivenNoOpenWindows_NullShouldBeReturned()
        {
            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>());

            var result = await returnFactory.GetReturnQuarter(A.Dummy<Guid>(), A.Dummy<FacilityType>());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetReturnQuarter_GivenQuarter1OpenWindow_ValidateAatfApprovalDateIsFalse_NullShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var startDate = new DateTime(2019, 4, 1);
            var quarterWindow = new QuarterWindow(startDate, new DateTime(2019, 7, 16), QuarterType.Q1);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarterWindow });
            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, startDate, 2019, FacilityType.Aatf)).Returns(false);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(A<Guid>._, A<int>._, A<QuarterType>._, A<FacilityType>._)).Returns(false);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetReturnQuarter_GivenQuarter1OpenWindow_HasReturnQuarterIsTrue_NullShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var startDate = new DateTime(2019, 4, 1);
            var quarterWindow = new QuarterWindow(startDate, new DateTime(2019, 7, 16), QuarterType.Q1);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarterWindow });
            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(A<Guid>._, A<DateTime>._, A<int>._, A<FacilityType>._)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q1, FacilityType.Aatf)).Returns(true);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetReturnQuarter_GivenQuarter1OpenWindow_NoPreviousReturnExists_AatfApprovalDateIsPreWindow_QuarterWindowShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var quarter1 = QuarterWindow(1);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarter1 });
            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter1.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q1, FacilityType.Aatf)).Returns(false);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.ComplianceYear.Should().Be(2019);
            result.Quarter.Should().Be(Core.DataReturns.QuarterType.Q1);
        }

        [Fact]
        public async Task GetReturnQuarter_GivenMultipleQuarterWindows_NoReturnExistsInFirstQuarter_AatfApprovalDateIsPreFirstWindow_FirstQuarterWindowsShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var quarter1 = QuarterWindow(1);
            var quarter2 = QuarterWindow(2);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarter1, quarter2 });

            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter1.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q1, FacilityType.Aatf)).Returns(false);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.ComplianceYear.Should().Be(2019);
            result.Quarter.Should().Be(Core.DataReturns.QuarterType.Q1);
        }

        [Fact]
        public async Task GetReturnQuarter_GivenMultipleQuarterWindows_ReturnExistsInFirstQuarterDoesNotExistInSecondWindow_AatfApprovalDateIsPreFirstWindow_SecondQuarterWindowShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var quarter1 = QuarterWindow(1);
            var quarter2 = QuarterWindow(2);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarter1, quarter2 });

            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter1.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter2.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q1, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q2, FacilityType.Aatf)).Returns(false);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.ComplianceYear.Should().Be(2019);
            result.Quarter.Should().Be(Core.DataReturns.QuarterType.Q2);
        }

        [Fact]
        public async Task GetReturnQuarter_GivenMultipleQuarterWindows_ReturnExistsInFirstAndSecondQuarterWindow_AatfApprovalDateIsPreSecondWindow_NullShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var quarter1 = QuarterWindow(1);
            var quarter2 = QuarterWindow(2);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarter1, quarter2 });

            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter1.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter2.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q1, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q2, FacilityType.Aatf)).Returns(true);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetReturnQuarter_GivenMultipleQuarterWindows_ReturnExistsInSecondButNotFirstQuarterWindow_AatfApprovalDateIsPreSecondWindow_FirstQuarterWindowShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var quarter1 = QuarterWindow(1);
            var quarter2 = QuarterWindow(2);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarter1, quarter2 });

            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter1.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter2.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q1, FacilityType.Aatf)).Returns(false);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q2, FacilityType.Aatf)).Returns(true);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.ComplianceYear.Should().Be(2019);
            result.Quarter.Should().Be(Core.DataReturns.QuarterType.Q1);
        }

        [Fact]
        public async Task GetReturnQuarter_GivenFourthQuarterWindow_ReturnDoesNotExistInQuarterWindow_AatfApprovalDateIsPreFourthWindow_FourthQuarterWindowShouldBeReturnedWithCorrectYear()
        {
            var organisationId = Guid.NewGuid();
            var quarter4 = QuarterWindow(4);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._)).Returns(new List<QuarterWindow>() { quarter4 });

            A.CallTo(() => returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarter4.EndDate, 2019, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(organisationId, 2019, QuarterType.Q4, FacilityType.Aatf)).Returns(false);

            var result = await returnFactory.GetReturnQuarter(organisationId, FacilityType.Aatf);

            result.ComplianceYear.Should().Be(2019);
            result.Quarter.Should().Be(Core.DataReturns.QuarterType.Q4);
        }

        private QuarterWindow QuarterWindow(int quarter)
        {
            var startDate = quarter != 4 ? new DateTime(2019, 1 + (3 * quarter), 1) : new DateTime(2020, 1, 1);

            return new QuarterWindow(startDate, new DateTime(2020, 3, 16), (QuarterType)quarter);
        }

        private void SetupFixedDate(DateTime date)
        {
            var testSystemData = new TestSystemData();
            testSystemData.ToggleFixedCurrentDateUsage(true);
            testSystemData.UpdateFixedCurrentDate(date);

            A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);
        }

        private void SetupSystemTime()
        {
            var testSystemData = new TestSystemData();

            testSystemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);
        }

        public class TestSystemData : SystemData
        {
        }
    }
}
