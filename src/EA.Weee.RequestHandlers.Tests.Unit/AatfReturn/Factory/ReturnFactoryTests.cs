namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Factory
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using FakeItEasy;
    using Prsd.Core;
    using Requests.AatfReturn;
    using Xunit;

    public class ReturnFactoryTests
    {
        public ReturnFactoryTests()
        {
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisation_OrganisationAatfsShouldBeRetrieved()
        {
            //var organisationId = Guid.NewGuid();

            //var result = await handler.HandleAsync(new GetReturns(organisationId));

            //A.CallTo(() => aatfDataAccess.FetchAatfByOrganisationId(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingCurrentDate()
        {
            //SystemTime.Freeze(new DateTime(2019, 1, 2));

            //var testSystemData = new TestSystemData();

            //testSystemData.ToggleFixedCurrentDateUsage(false);

            //var result = await handler.HandleAsync(A.Dummy<GetReturns>());

            //A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);
            //A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2019)
            //&& d.Month.Equals(1) && d.Day.Equals(2)))).MustHaveHappenedOnceExactly();

            //SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenNotUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingCurrentDate()
        {
            //SystemTime.Freeze(new DateTime(2019, 1, 2));

            //var testSystemData = new TestSystemData();
            //testSystemData.ToggleFixedCurrentDateUsage(false);

            //A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);

            //var result = await handler.HandleAsync(A.Dummy<GetReturns>());

            //A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2019)
            //                                                                                           && d.Month.Equals(1) && d.Day.Equals(2)))).MustHaveHappenedOnceExactly();

            //SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenUsingFixedSystemTime_QuarterWindowsShouldBeRetrievedUsingFixedSystemDate()
        {
            //SystemTime.Freeze(new DateTime(2019, 1, 2));

            //var testSystemData = new TestSystemData();
            //testSystemData.ToggleFixedCurrentDateUsage(true);
            //testSystemData.UpdateFixedCurrentDate(new DateTime(2019, 1, 3));

            //A.CallTo(() => systemDataDataAccess.Get()).Returns(testSystemData);

            //var result = await handler.HandleAsync(A.Dummy<GetReturns>());

            //A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>.That.Matches(d => d.Year.Equals(2019)
            //                                                                                           && d.Month.Equals(1) && d.Day.Equals(3)))).MustHaveHappenedOnceExactly();

            //SystemTime.Unfreeze();
        }

        public class TestSystemData : SystemData
        {
        }
    }
}
