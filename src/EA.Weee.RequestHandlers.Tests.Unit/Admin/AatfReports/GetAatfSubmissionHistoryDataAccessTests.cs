namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.AatfReturn;
    using Domain.Admin.AatfReports;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.AatfReports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAatfSubmissionHistoryDataAccessTests
    {
        private readonly GetAatfSubmissionHistoryDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        public GetAatfSubmissionHistoryDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();

            dataAccess = new GetAatfSubmissionHistoryDataAccess(context);
        }

        [Fact]
        public async Task GetItemsAsync_GivenAatfIsAatf_GetAatfSubmissionsStoredProcedureShouldBeCalled()
        {
            var id = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            await dataAccess.GetItemsAsync(id);

            A.CallTo(() => context.StoredProcedures.GetAatfSubmissions(id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetItemsAsync_GivenAatfIsAatf_DataFromStoredProcedureShouldBeReturned()
        {
            var aatfHistory = A.CollectionOfFake<AatfSubmissionHistory>(2).ToList();
            var id = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));
            A.CallTo(() => context.StoredProcedures.GetAatfSubmissions(A<Guid>._)).Returns(aatfHistory);

            var results = await dataAccess.GetItemsAsync(id);

            results.Should().BeSameAs(aatfHistory);
        }

        [Fact]
        public async Task GetItemsAsync_GivenAatfIsAe_GetAeSubmissionsStoredProcedureShouldBeCalled()
        {
            var id = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Ae);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            await dataAccess.GetItemsAsync(id);

            A.CallTo(() => context.StoredProcedures.GetAeSubmissions(id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetItemsAsync_GivenAatfIsAe_DataFromStoredProcedureShouldBeReturned()
        {
            var aatfHistory = A.CollectionOfFake<AatfSubmissionHistory>(2).ToList();
            var id = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Ae);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));
            A.CallTo(() => context.StoredProcedures.GetAeSubmissions(A<Guid>._)).Returns(aatfHistory);

            var results = await dataAccess.GetItemsAsync(id);

            results.Should().BeSameAs(aatfHistory);
        }
    }
}
