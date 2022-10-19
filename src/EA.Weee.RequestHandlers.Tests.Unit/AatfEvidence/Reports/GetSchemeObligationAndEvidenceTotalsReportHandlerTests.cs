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

    public class GetSchemeObligationAndEvidenceTotalsReportHandlerTests : SimpleUnitTestBase
    {
        private readonly GetSchemeObligationAndEvidenceTotalsReportHandler handler;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<ObligationAndEvidenceProgressSummaryData> evidenceWriter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetSchemeObligationAndEvidenceTotalsReportHandlerTests()
        {
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();
            evidenceWriter = A.Fake<ICsvWriter<ObligationAndEvidenceProgressSummaryData>>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();

            handler = new GetSchemeObligationAndEvidenceTotalsReportHandler(evidenceStoredProcedures, evidenceWriter, genericDataAccess, authorization);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestThatIsNotAnOrganisationRequest_InternalAccessShouldBeChecked()
        {
            //arrange
            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), null, TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenReportDataRequest_GetSchemeObligationAndEvidenceTotalsShouldBeCalled()
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();
            var schemeId = TestFixture.Create<Guid?>();
            var authority = TestFixture.Create<Guid?>();
            var organisationId = TestFixture.Create<Guid?>();

            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(schemeId, authority, organisationId, complianceYear);

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetSchemeObligationAndEvidenceProgress(schemeId, authority, organisationId, complianceYear)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_SchemeObligationAndEvidenceTotalsCsvShouldBeDefined()
        {
            //arrange
            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), null, TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.SchemeName,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.SchemeApprovalNumber,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Category,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.HouseholdObligation,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.HouseholdEvidence,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.HouseholdReuse,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredOut,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredIn,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Difference,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.NonHouseholdEvidence,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.NonHouseholdReuse,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_OrganisationObligationAndEvidenceTotalsCsvShouldBeDefined()
        {
            //arrange
            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Category,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.HouseholdObligation,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.HouseholdEvidence,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.HouseholdReuse,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredOut,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredIn,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Difference,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.NonHouseholdEvidence,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.NonHouseholdReuse,
                    A<Func<ObligationAndEvidenceProgressSummaryData, object>>._, false)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenReportData_CsvShouldBeCreated()
        {
            //arrange
            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), TestFixture.Create<int>());

            var reportData = TestFixture.CreateMany<ObligationAndEvidenceProgressSummaryData>().ToList();

            A.CallTo(() => evidenceStoredProcedures.GetSchemeObligationAndEvidenceProgress(A<Guid?>._, A<Guid?>._, A<Guid?>._, A<int>._)).Returns(reportData);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.Write(reportData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithScheme_SchemeShouldBeRetrieved()
        {
            //arrange
            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), TestFixture.Create<int>());
            
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => genericDataAccess.GetById<Scheme>(request.SchemeId.Value)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithScheme_CsvFileDataShouldBeReturned()
        {
            //arrange
            SystemTime.Freeze(new DateTime(2020, 12, 31, 11, 13, 14));
            var approvalNumber = TestFixture.Create<string>();
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber).Returns(approvalNumber);

            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<Guid?>(), null, TestFixture.Create<int>());
            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<ObligationAndEvidenceProgressSummaryData>>._)).Returns(content);
            A.CallTo(() => genericDataAccess.GetById<Scheme>(A<Guid>._)).Returns(scheme);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_{approvalNumber}_PCS evidence and obligation progress{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithOrganisation_CsvFileDataShouldBeReturned()
        {
            //arrange
            SystemTime.Freeze(new DateTime(2020, 12, 31, 11, 13, 14));
            var approvalNumber = TestFixture.Create<string>();
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber).Returns(approvalNumber);

            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), TestFixture.Create<int>());
            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<ObligationAndEvidenceProgressSummaryData>>._)).Returns(content);
            A.CallTo(() => genericDataAccess.GetById<Scheme>(A<Guid>._)).Returns(scheme);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_PCS Summary{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithNoScheme_CsvFileDataShouldBeReturned()
        {
            //arrange
            SystemTime.Freeze(new DateTime(2020, 12, 31, 11, 13, 14));

            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(null, TestFixture.Create<Guid?>(), null, TestFixture.Create<int>());
            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<ObligationAndEvidenceProgressSummaryData>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_PCS evidence and obligation progress{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithNoSchemeAndWithOrganisation_CsvFileDataShouldBeReturned()
        {
            //arrange
            SystemTime.Freeze(new DateTime(2020, 12, 31, 11, 13, 14));

            var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(null, TestFixture.Create<Guid?>(), TestFixture.Create<Guid?>(), TestFixture.Create<int>());
            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<ObligationAndEvidenceProgressSummaryData>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_PCS Summary{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }
    }
}
