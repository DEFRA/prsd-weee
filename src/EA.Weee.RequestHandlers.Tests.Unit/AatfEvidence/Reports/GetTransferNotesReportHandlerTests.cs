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
    using Domain.Organisation;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetTransferNotesReportHandlerTests : SimpleUnitTestBase
    {
        private readonly GetTransferNotesReportHandler handler;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<TransferNoteData> evidenceWriter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetTransferNotesReportHandlerTests()
        {
            evidenceStoredProcedures = A.Fake<IEvidenceStoredProcedures>();
            evidenceWriter = A.Fake<ICsvWriter<TransferNoteData>>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();

            handler = new GetTransferNotesReportHandler(evidenceStoredProcedures, evidenceWriter, genericDataAccess, authorization);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithNoOrganisationId_InternalAccessShouldBeChecked()
        {
            //arrange
            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), null);

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithOrganisationId_ExternalAndOrganisationAccessShouldBeChecked()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), organisationId);

            //act
            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferReportDataRequest_GetTransferNoteReportShouldBeCalled()
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();
            var organisationId = TestFixture.Create<Guid>();

            var request = new GetTransferNoteReportRequest(complianceYear, organisationId);

            //act
            await handler.HandleAsync(request);

            A.CallTo(() =>
                evidenceStoredProcedures.GetTransferNoteReport(complianceYear, organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferReportRequestWithOrganisation_OrganisationShouldBeRetrieved()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), organisationId);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => genericDataAccess.GetById<Organisation>(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferReportRequestWithoutOrganisationAndData_TransferNoteDataCsvShouldBeDefined()
        {
            //arrange
            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), null);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferReference,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Status,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferApprovalDate,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredByName,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredByApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Recipient,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.RecipientApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteReference,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteApprovedDate,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByName,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Protocol,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenTransferReportRequestWithOrganisationIdThatIsNotPbs_TransferNoteDataCsvShouldBeDefined()
        {
            //arrange
            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), TestFixture.Create<Guid?>());

            var organisation = Organisation.CreateRegisteredCompany("companyname", "1234567");

            A.CallTo(() => genericDataAccess.GetById<Organisation>(A<Guid>._)).Returns(organisation);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferReference,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Status,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferApprovalDate,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredByName,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredByApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Recipient,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.RecipientApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteReference,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteApprovedDate,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByName,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Protocol,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenTransferReportRequestWithOrganisationIdThatIsPbs_TransferNoteDataCsvShouldBeDefined()
        {
            //arrange
            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), TestFixture.Create<Guid?>());

            var organisation = Organisation.CreateRegisteredCompany("companyname", "1234567");
            ObjectInstantiator<Organisation>.SetProperty(o => o.ProducerBalancingScheme, new ProducerBalancingScheme(), organisation);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(A<Guid>._)).Returns(organisation);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferReference,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Status,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferApprovalDate,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Recipient,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.RecipientApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteReference,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteApprovedDate,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByName,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Protocol,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14Transferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat1ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat2ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat3ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat4ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat5ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat6ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat7ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat8ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat9ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat10ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat11ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat12ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat13ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.Cat14ReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TotalReusedTransferred,
                    A<Func<TransferNoteData, object>>._, false)).MustHaveHappenedOnceExactly());

            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredByName,
                    A<Func<TransferNoteData, object>>._, false)).MustNotHaveHappened();
            A.CallTo(() => evidenceWriter.DefineColumn(EvidenceReportConstants.TransferredByApprovalNumber,
                    A<Func<TransferNoteData, object>>._, false)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferReportData_CsvShouldBeCreated()
        {
            //arrange
            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), TestFixture.Create<Guid?>());

            var reportData = TestFixture.CreateMany<TransferNoteData>().ToList();

            A.CallTo(() => evidenceStoredProcedures.GetTransferNoteReport(A<int>._, A<Guid?>._)).Returns(reportData);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceWriter.Write(reportData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferNoteRequest_CsvFileDataShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2020, 12, 31, 11, 13, 14);
            SystemTime.Freeze(date);

            var request = new GetTransferNoteReportRequest(TestFixture.Create<int>(), TestFixture.Create<Guid?>());
            var content = TestFixture.Create<string>();
            A.CallTo(() => evidenceWriter.Write(A<IEnumerable<TransferNoteData>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
            result.FileName.Should().Be($"{request.ComplianceYear}_Transfer notes report{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
        }
    }
}
