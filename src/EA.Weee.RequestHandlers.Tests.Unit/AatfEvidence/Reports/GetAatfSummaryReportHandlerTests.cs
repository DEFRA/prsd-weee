namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Reports
{
    using AutoFixture;
    using Core.Constants;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using EA.Weee.DataAccess.DataAccess;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using RequestHandlers.AatfEvidence.Reports;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAatfSummaryReportHandlerTests : SimpleUnitTestBase
    {
        private readonly GetAatfSummaryReportHandler handler;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<AatfEvidenceSummaryTotalsData> reportWriter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetAatfSummaryReportHandlerTests()
        {
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();
            reportWriter = A.Fake<ICsvWriter<AatfEvidenceSummaryTotalsData>>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();

            handler = new GetAatfSummaryReportHandler(authorization, evidenceStoredProcedures, reportWriter);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ExternalAccessShouldBeChecked()
        {
            //arrange
            var request = new GetAatfSummaryReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_AatfAccessShouldBeChecked()
        {
            //arrange
            var aatfId = TestFixture.Create<Guid>();
            var request = new GetAatfSummaryReportRequest(aatfId, TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureAatfHasOrganisationAccess(aatfId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_GetAatfEvidenceSummaryTotalsStoredProcedureShouldBeCalled()
        {
            //arrange
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();
            var request = new GetAatfSummaryReportRequest(aatfId, complianceYear);

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatfId, complianceYear)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_AatfSummaryCsvShouldBeDefined()
        {
            //arrange
            var request = new GetAatfSummaryReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => reportWriter.DefineColumn(EvidenceReportConstants.Category,
                    A<Func<AatfEvidenceSummaryTotalsData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => reportWriter.DefineColumn(EvidenceReportConstants.ApprovedEvidence,
                    A<Func<AatfEvidenceSummaryTotalsData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => reportWriter.DefineColumn(EvidenceReportConstants.ApprovedReuse,
                    A<Func<AatfEvidenceSummaryTotalsData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => reportWriter.DefineColumn(EvidenceReportConstants.SubmittedEvidence,
                    A<Func<AatfEvidenceSummaryTotalsData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => reportWriter.DefineColumn(EvidenceReportConstants.SubmittedReuse,
                    A<Func<AatfEvidenceSummaryTotalsData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => reportWriter.DefineColumn(EvidenceReportConstants.DraftEvidence,
                    A<Func<AatfEvidenceSummaryTotalsData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => reportWriter.DefineColumn(EvidenceReportConstants.DraftReuse,
                    A<Func<AatfEvidenceSummaryTotalsData, object>>._, false)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenRequestReportData_CsvShouldBeCreated()
        {
            //arrange
            var request = new GetAatfSummaryReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>());

            var reportData = TestFixture.CreateMany<AatfEvidenceSummaryTotalsData>().ToList();

            A.CallTo(() => evidenceStoredProcedures.GetAatfEvidenceSummaryTotals(A<Guid>._, A<int>._)).Returns(reportData);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => reportWriter.Write(reportData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestReportData_CsvFileDataShouldBeReturned()
        {
            //arrange
            SystemTime.Freeze(new DateTime(2020, 12, 31, 11, 13, 14));

            var request = new GetAatfSummaryReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<int>());
            var content = TestFixture.Create<string>();

            A.CallTo(() => reportWriter.Write(A<IEnumerable<AatfEvidenceSummaryTotalsData>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_Summary report{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }
    }
}
