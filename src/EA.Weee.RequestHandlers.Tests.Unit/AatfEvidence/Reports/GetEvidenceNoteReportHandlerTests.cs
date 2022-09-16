namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using FakeItEasy;
    using RequestHandlers.AatfEvidence.Reports;
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
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<EvidenceNoteReportData> evidenceWriter;
        private readonly IEvidenceReportsAuthenticationCheck evidenceReportsAuthenticationCheck;

        public GetEvidenceNoteReportHandlerTests()
        {
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();
            evidenceWriter = A.Fake<ICsvWriter<EvidenceNoteReportData>>();
            evidenceReportsAuthenticationCheck = A.Fake<IEvidenceReportsAuthenticationCheck>();

            handler = new GetEvidenceNoteReportHandler(evidenceStoredProcedures, evidenceWriter, evidenceReportsAuthenticationCheck);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_EvidenceReportsAuthenticationCheckShouldBeCalled()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => evidenceReportsAuthenticationCheck.EnsureIsAuthorised(request)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(TonnageToDisplayReportEnum.OriginalTonnages, false)]
        [InlineData(TonnageToDisplayReportEnum.Net, true)]
        public async Task HandleAsync_GivenRequest_GetEvidenceNoteOriginalTonnagesReportShouldBeCalled(TonnageToDisplayReportEnum tonnageToDisplay, bool expected)
        {
            //arrange
            var originatingOrganisationId = TestFixture.Create<Guid>();
            var recipientOrganisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var request = new GetEvidenceNoteReportRequest(recipientOrganisationId, 
                originatingOrganisationId,
                aatfId,
                tonnageToDisplay,
                complianceYear);

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(complianceYear,
                    originatingOrganisationId, recipientOrganisationId, aatfId, expected)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AatfCsvShouldBeDefined()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), 
                TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Reference,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Status,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.SubmittedDate,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.ObligationType,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.ReceivedStartDate,
                    A<Func<EvidenceNoteReportData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.ReceivedEndDate,
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
        public async Task HandleAsync_NonAatfCsvShouldBeDefined()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                null,
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
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), 
                TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>());

            var reportData = TestFixture.CreateMany<EvidenceNoteReportData>().ToList();

            A.CallTo(() =>
                    evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(A<int>._, 
                        A<Guid?>._, 
                        A<Guid?>._,
                        A<Guid?>._,
                        A<bool>._)).Returns(reportData);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.Write(reportData)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(TonnageToDisplayReportEnum.OriginalTonnages, "original tonnages")]
        [InlineData(TonnageToDisplayReportEnum.Net, "net of transfer")]
        public async Task HandleAsync_GivenCsvData_CsvFileDataShouldBeReturned(TonnageToDisplayReportEnum tonnageToDisplay, string expected)
        {
            //arrange
            var date = new DateTime(2020, 12, 31, 11, 13, 14);
            SystemTime.Freeze(date);
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(), 
                TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                tonnageToDisplay,
                TestFixture.Create<int>());

            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<EvidenceNoteReportData>>._)).Returns(content);
            
            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should()
                .Be($"{request.ComplianceYear}_Evidence notes {expected}{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }
    }
}
