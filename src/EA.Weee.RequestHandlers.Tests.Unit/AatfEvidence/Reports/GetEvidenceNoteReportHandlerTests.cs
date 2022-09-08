namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using FakeItEasy;
    using RequestHandlers.AatfEvidence.Reports;
    using RequestHandlers.Security;
    using System.Threading.Tasks;
    using Core.Constants;
    using FluentAssertions;
    using Prsd.Core;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetEvidenceNoteReportHandlerTests : SimpleUnitTestBase
    {
        private readonly GetEvidenceNoteReportHandler handler;
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<EvidenceNoteReportData> evidenceWriter;

        public GetEvidenceNoteReportHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();
            evidenceWriter = A.Fake<ICsvWriter<EvidenceNoteReportData>>();

            handler = new GetEvidenceNoteReportHandler(authorization, evidenceStoredProcedures, evidenceWriter);
        }

        [Fact]
        public async Task HandleAsync_GivenNullOrganisationIds_InternalAccessShouldBeChecked()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(null, null, TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRecipientOrganisationId_OrganisationAccessShouldBeChecked()
        {
            //arrange
            var recipientOrganisationId = TestFixture.Create<Guid>();

            var request = new GetEvidenceNoteReportRequest(recipientOrganisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureOrganisationAccess(recipientOrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenOriginatingOrganisationId_OrganisationAccessShouldBeChecked()
        {
            //arrange
            var originatingOrganisationId = TestFixture.Create<Guid>();

            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), originatingOrganisationId,
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureOrganisationAccess(originatingOrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_GetEvidenceNoteOriginalTonnagesReportShouldBeCalled()
        {
            //arrange
            var originatingOrganisationId = TestFixture.Create<Guid>();
            var recipientOrganisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var request = new GetEvidenceNoteReportRequest(recipientOrganisationId, originatingOrganisationId,
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                complianceYear);

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(complianceYear,
                    originatingOrganisationId, recipientOrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CsvShouldBeDefined()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Reference,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Status,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.AppropriateAuthority,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.SubmittedDate,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.SubmittedAatf,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.SubmittedAatfApprovalNumber,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.ObligationType,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.ReceivedStartDate,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.ReceivedEndDate,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Recipient,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.RecipientApprovalNumber,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Protocol,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14Received,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalReceived,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14Reused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalReused,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenReportData_CsvShouldBeCreated()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            var reportData = TestFixture.CreateMany<EvidenceNoteReportData>().ToList();

            A.CallTo(() =>
                    evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(A<int>._, A<Guid?>._, A<Guid?>._)).Returns(reportData);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.Write(reportData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenCsvData_CsvFileDataShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2020, 12, 31, 11, 13, 14);
            SystemTime.Freeze(date);
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<EvidenceNoteReportData>>._)).Returns(content);
            
            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should()
                .Be($"{request.ComplianceYear}_Evidence notes original tonnages{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }
    }
}
