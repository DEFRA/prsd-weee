namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Reports
{
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Constants;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using RequestHandlers.AatfEvidence.Reports;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using Domain.Scheme;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetEvidenceNoteReportHandlerTests : SimpleUnitTestBase
    {
        private readonly GetEvidenceNoteReportHandler handler;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<EvidenceNoteReportData> evidenceWriter;
        private readonly IEvidenceReportsAuthenticationCheck evidenceReportsAuthenticationCheck;
        private readonly IGenericDataAccess genericDataAccess;

        public GetEvidenceNoteReportHandlerTests()
        {
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();
            evidenceWriter = A.Fake<ICsvWriter<EvidenceNoteReportData>>();
            evidenceReportsAuthenticationCheck = A.Fake<IEvidenceReportsAuthenticationCheck>();
            genericDataAccess = A.Fake<IGenericDataAccess>();

            handler = new GetEvidenceNoteReportHandler(evidenceStoredProcedures, evidenceWriter, evidenceReportsAuthenticationCheck, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_EvidenceReportsAuthenticationCheckShouldBeCalled()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(null, null, null, TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>(), TestFixture.Create<bool>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => evidenceReportsAuthenticationCheck.EnsureIsAuthorised(request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithRecipientOrganisation_OrganisationShouldBeRetrieved()
        {
            //arrange
            var recipientOrganisationId = TestFixture.Create<Guid>();
            var request = new GetEvidenceNoteReportRequest(recipientOrganisationId, null, null, TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>(), TestFixture.Create<bool>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(recipientOrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(TonnageToDisplayReportEnum.OriginalTonnages, false)]
        [InlineData(TonnageToDisplayReportEnum.Net, true)]
        public async Task HandleAsync_GivenAllReportDataRequest_GetEvidenceNoteOriginalTonnagesReportShouldBeCalled(TonnageToDisplayReportEnum tonnageToDisplay, bool expected)
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();

            var request = new GetEvidenceNoteReportRequest(null,
                null,
                null,
                tonnageToDisplay,
                complianceYear,
                TestFixture.Create<bool>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(complianceYear,
                    null, null, null, expected)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(TonnageToDisplayReportEnum.OriginalTonnages, false)]
        [InlineData(TonnageToDisplayReportEnum.Net, true)]
        public async Task HandleAsync_GivenAatfReportDataRequest_GetEvidenceNoteOriginalTonnagesReportShouldBeCalled(TonnageToDisplayReportEnum tonnageToDisplay, bool expected)
        {
            //arrange
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var request = new GetEvidenceNoteReportRequest(null,
                null,
                aatfId,
                tonnageToDisplay,
                complianceYear,
                TestFixture.Create<bool>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(complianceYear,
                    null, null, aatfId, expected)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(TonnageToDisplayReportEnum.OriginalTonnages, false)]
        [InlineData(TonnageToDisplayReportEnum.Net, true)]
        public async Task HandleAsync_GivenOriginatorReportDataRequest_GetEvidenceNoteOriginalTonnagesReportShouldBeCalled(TonnageToDisplayReportEnum tonnageToDisplay, bool expected)
        {
            //arrange
            var originatorOrganisation = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var request = new GetEvidenceNoteReportRequest(null,
                originatorOrganisation,
                null,
                tonnageToDisplay,
                complianceYear,
                TestFixture.Create<bool>());

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(complianceYear,
                    originatorOrganisation, null, null, expected)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AatfCsvShouldBeDefined()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(null, 
                null,
                TestFixture.Create<Guid>(),
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>(),
                TestFixture.Create<bool>());

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
        public async Task HandleAsync_NonAatfCsvShouldBeDefined()
        {
            //arrange
            var request = new GetEvidenceNoteReportRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                null,
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>(),
                TestFixture.Create<bool>());

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
            var request = new GetEvidenceNoteReportRequest(null,
                null,
                null,
                TestFixture.Create<TonnageToDisplayReportEnum>(),
                TestFixture.Create<int>(),
                TestFixture.Create<bool>());

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
            var request = new GetEvidenceNoteReportRequest(null,
                null,
                null,
                tonnageToDisplay,
                TestFixture.Create<int>(),
                TestFixture.Create<bool>());

            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<EvidenceNoteReportData>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_Evidence notes {expected}{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfCsvData_CsvFileDataShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2020, 12, 31, 11, 13, 14);
            SystemTime.Freeze(date);
            var aatfId = TestFixture.Create<Guid>();
            var aatf = A.Fake<Aatf>();
            
            A.CallTo(() => aatf.ApprovalNumber).Returns("123456");
            A.CallTo(() => genericDataAccess.GetById<Aatf>(aatfId)).Returns(aatf);
            
            var request = new GetEvidenceNoteReportRequest(null,
                null,
                aatfId,
                TonnageToDisplayReportEnum.OriginalTonnages,
                TestFixture.Create<int>(),
                TestFixture.Create<bool>());

            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<EvidenceNoteReportData>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_{aatf.ApprovalNumber}_Evidence notes report{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData(TonnageToDisplayReportEnum.OriginalTonnages, "original tonnages")]
        [InlineData(TonnageToDisplayReportEnum.Net, "net of transfer")]
        public async Task HandleAsync_GivenRecipientOrganisationThatIsNotBalancingSchemeCsvData_CsvFileDataShouldBeReturned(TonnageToDisplayReportEnum tonnageToDisplay, string expected)
        {
            //arrange
            var date = new DateTime(2020, 12, 31, 11, 13, 14);
            SystemTime.Freeze(date);

            var recipientOrganisationId = TestFixture.Create<Guid>();
            var approvalNumber = TestFixture.Create<string>();
            var recipientOrganisation = A.Fake<Organisation>();
            var recipientScheme = A.Fake<Scheme>();

            A.CallTo(() => recipientOrganisation.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => recipientScheme.ApprovalNumber).Returns(approvalNumber);
            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { recipientScheme });
            A.CallTo(() => genericDataAccess.GetById<Organisation>(recipientOrganisationId)).Returns(recipientOrganisation);
            
            var request = new GetEvidenceNoteReportRequest(recipientOrganisationId,
                null,
                null,
                tonnageToDisplay,
                TestFixture.Create<int>(),
                TestFixture.Create<bool>());

            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<EvidenceNoteReportData>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_{approvalNumber}_Evidence notes {expected}{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }
    }
}
