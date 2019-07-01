namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.AatfReports;
    using Xunit;

    public class GetAatfSubmissionHistoryDataAccessTests
    {
        private readonly GetAatfSubmissionHistoryDataAccess dataAccess;
        private readonly WeeeContext context;

        public GetAatfSubmissionHistoryDataAccessTests()
        {
            context = A.Fake<WeeeContext>();

            dataAccess = new GetAatfSubmissionHistoryDataAccess(context);
        }

        [Fact]
        public async Task GetItemsAsync_GivenAatfId_ContextStoredProcedureShouldBeCalled()
        {
            var id = Guid.NewGuid();

            await dataAccess.GetItemsAsync(id);

            A.CallTo(() => context.StoredProcedures.GetAatfSubmissions(id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetItemsAsync_GivenAatfData_DataShouldBeReturned()
        {
            var aatfHistory = A.CollectionOfFake<AatfSubmissionHistory>(2).ToList();

            A.CallTo(() => context.StoredProcedures.GetAatfSubmissions(A<Guid>._)).Returns(aatfHistory);

            var results = await dataAccess.GetItemsAsync(A.Dummy<Guid>());

            results.Should().BeSameAs(aatfHistory);
        }
    }
}
