namespace EA.Weee.RequestHandlers.Tests.Unit.Factories
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain;
    using FakeItEasy;
    using Prsd.Core;
    using RequestHandlers.Factories;
    using Requests.AatfReturn;
    using Xunit;

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

        [Theory]
        [InlineData(FacilityType.Aatf)]
        [InlineData(FacilityType.Ae)]
        public async Task GetReturnQuarter_GivenOrganisationAndFacilityAndNotUsingFixedSystemTime_OrganisationAatfsShouldBeRetrieved(FacilityType facilityType)
        {
            SystemTime.Freeze(new DateTime(2019, 1, 2));

            var organisationId = Guid.NewGuid();

            var result = await returnFactory.GetReturnQuarter(organisationId, facilityType);

            A.CallTo(() => returnFactoryDataAccess.FetchAatfsByOrganisationFacilityTypeListAndYear(organisationId, 2019, facilityType)).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData(FacilityType.Aatf)]
        [InlineData(FacilityType.Ae)]
        public async Task GetReturnQuarter_GivenOrganisationAndFacilityAndUsingFixedSystemTime_OrganisationAatfsShouldBeRetrieved(FacilityType facilityType)
        {
            SystemTime.Freeze(new DateTime(2019, 1, 2));

            var date = new DateTime(2020, 1, 3);

            SetupFixedDate(date);

            var organisationId = Guid.NewGuid();

            var result = await returnFactory.GetReturnQuarter(organisationId, facilityType);

            A.CallTo(() => returnFactoryDataAccess.FetchAatfsByOrganisationFacilityTypeListAndYear(organisationId, 2020, facilityType)).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
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
