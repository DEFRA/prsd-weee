namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Admin.AatfReports;
    using Core.DataReturns;
    using Domain.Admin.AatfReports;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.AatfReports;
    using Requests.Admin;
    using Requests.Admin.Aatf;
    using Xunit;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class GetAatfSubmissionHistoryHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfSubmissionHistoryDataAccess dataAccess;
        private readonly IMapper mapper;
        private readonly GetAatfSubmissionHistoryHandler handler;
        private readonly Fixture fixture;

        public GetAatfSubmissionHistoryHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetAatfSubmissionHistoryDataAccess>();
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            handler = new GetAatfSubmissionHistoryHandler(dataAccess, authorization, mapper);
        }

        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetAatfSubmissionHistoryHandler(dataAccess, authorization, mapper);

            Func<Task> data = async () => await handler.HandleAsync(A.Dummy<GetAatfSubmissionHistory>());

            await Assert.ThrowsAsync<SecurityException>(data);
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_AatfSubmissionHistoryDataAccessShouldBeCalled()
        {
            var message = new GetAatfSubmissionHistory(Guid.NewGuid());

            await handler.HandleAsync(message);

            A.CallTo(() => dataAccess.GetItemsAsync(message.AatfId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfSubmissionHistory_HistoryShouldBeMapped()
        {
            var aatfSubmissionHistory = A.CollectionOfFake<AatfSubmissionHistory>(2).ToList();
            var aatfSubmissionHistoryData = A.CollectionOfFake<AatfSubmissionHistoryData>(2).ToList();

            A.CallTo(() => dataAccess.GetItemsAsync(A<Guid>._)).Returns(aatfSubmissionHistory);
            A.CallTo(() => mapper.Map<AatfSubmissionHistory, AatfSubmissionHistoryData>(aatfSubmissionHistory.ElementAt(0)))
                .Returns(aatfSubmissionHistoryData.ElementAt(0));
            A.CallTo(() => mapper.Map<AatfSubmissionHistory, AatfSubmissionHistoryData>(aatfSubmissionHistory.ElementAt(1)))
                .Returns(aatfSubmissionHistoryData.ElementAt(1));

            var message = new GetAatfSubmissionHistory(Guid.NewGuid());

            var result = await handler.HandleAsync(message);

            result.Should().Contain(aatfSubmissionHistoryData.ElementAt(0));
            result.Should().Contain(aatfSubmissionHistoryData.ElementAt(1));
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfSubmissionHistoryData_ShouldBeOrderedByComplianceYearQuarterAndSubmittedDate()
        {
            var message = new GetAatfSubmissionHistory(Guid.NewGuid());

            var aatfSubmissionHistoryData = new List<AatfSubmissionHistoryData>()
            {
                new AatfSubmissionHistoryData() { ComplianceYear = 2017 },
                new AatfSubmissionHistoryData() { ComplianceYear = 2019, Quarter = QuarterType.Q1, SubmittedDate = new DateTime(2019, 01, 01) },
                new AatfSubmissionHistoryData() { ComplianceYear = 2019, Quarter = QuarterType.Q1, SubmittedDate = new DateTime(2019, 01, 02) },
                new AatfSubmissionHistoryData() { ComplianceYear = 2018, Quarter = QuarterType.Q3 },
                new AatfSubmissionHistoryData() { ComplianceYear = 2018, Quarter = QuarterType.Q1 },
                new AatfSubmissionHistoryData() { ComplianceYear = 2018, Quarter = QuarterType.Q2 },
            }.ToArray();

            A.CallTo(() => dataAccess.GetItemsAsync(A<Guid>._)).Returns(A.CollectionOfFake<AatfSubmissionHistory>(6).ToList());
            A.CallTo(() => mapper.Map<AatfSubmissionHistory, AatfSubmissionHistoryData>(A<AatfSubmissionHistory>._)).ReturnsNextFromSequence(aatfSubmissionHistoryData);

            var result = await handler.HandleAsync(message);

            result.ElementAt(0).Should().BeEquivalentTo(new AatfSubmissionHistoryData() { ComplianceYear = 2019, Quarter = QuarterType.Q1, SubmittedDate = new DateTime(2019, 01, 02) });
            result.ElementAt(1).Should().BeEquivalentTo(new AatfSubmissionHistoryData() { ComplianceYear = 2019, Quarter = QuarterType.Q1, SubmittedDate = new DateTime(2019, 01, 01) });
            result.ElementAt(2).Should().BeEquivalentTo(new AatfSubmissionHistoryData() { ComplianceYear = 2018, Quarter = QuarterType.Q3 });
            result.ElementAt(3).Should().BeEquivalentTo(new AatfSubmissionHistoryData() { ComplianceYear = 2018, Quarter = QuarterType.Q2 });
            result.ElementAt(4).Should().BeEquivalentTo(new AatfSubmissionHistoryData() { ComplianceYear = 2018, Quarter = QuarterType.Q1 });
            result.ElementAt(5).Should().BeEquivalentTo(new AatfSubmissionHistoryData() { ComplianceYear = 2017 });
        }
    }
}
