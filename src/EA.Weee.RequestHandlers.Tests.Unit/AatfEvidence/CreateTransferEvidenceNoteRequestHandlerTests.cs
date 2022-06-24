namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class CreateTransferEvidenceNoteRequestHandlerTests : SimpleUnitTestBase
    {
        private CreateTransferEvidenceNoteRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly TransferEvidenceNoteRequest request;
        private readonly Organisation organisation;
        private readonly Scheme scheme;
        private readonly Guid userId;
        private readonly short complianceYear;

        public CreateTransferEvidenceNoteRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userContext = A.Fake<IUserContext>();
            transferTonnagesValidator = A.Fake<ITransferTonnagesValidator>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            organisation = A.Fake<Organisation>();
            scheme = A.Fake<Scheme>();
            userId = TestFixture.Create<Guid>();
            complianceYear = TestFixture.Create<short>();

            A.CallTo(() => scheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => organisation.Id).Returns(TestFixture.Create<Guid>());

            request = Request();

            handler = new CreateTransferEvidenceNoteRequestHandler(weeeAuthorization,
                genericDataAccess,
                userContext,
                evidenceDataAccess,
                transferTonnagesValidator,
                transactionAdapter,
                systemDataDataAccess);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => genericDataAccess.GetById<Scheme>(request.SchemeId)).Returns(scheme);
            A.CallTo(() => userContext.UserId).Returns(userId);
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task HandleAsync_GivenNoteStatusIsNotValid_ArgumentExceptionExpected(Core.AatfEvidence.NoteStatus status)
        {
            if (status.Equals(Core.AatfEvidence.NoteStatus.Submitted) ||
                status.Equals(Core.AatfEvidence.NoteStatus.Draft))
            {
                return;
            }

            //arrange
            var request = Request();
            request.Status = status;

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<InvalidEnumArgumentException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new CreateTransferEvidenceNoteRequestHandler(authorization,
                genericDataAccess,
                userContext,
                evidenceDataAccess,
                transferTonnagesValidator,
                transactionAdapter,
                systemDataDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldEnsureCanAccessExternalArea()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldGetSystemDateTime()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new CreateTransferEvidenceNoteRequestHandler(authorization,
                genericDataAccess,
                userContext,
                evidenceDataAccess,
                transferTonnagesValidator,
                transactionAdapter,
                systemDataDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNoOrganisationFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Organisation>(A<Guid>._)).Returns((Organisation)null);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNoSchemeFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Scheme>(A<Guid>._)).Returns((Scheme)null);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_TransferValuesShouldBeValidated()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => transferTonnagesValidator.Validate(request.TransferValues)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ComplianceYearShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.GetComplianceYearByNotes(request.EvidenceNoteIds)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenValidRequest_TransferNoteShouldBeAdded()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => genericDataAccess.GetById<Scheme>(request.SchemeId)).Returns(scheme);
            A.CallTo(() => evidenceDataAccess.GetComplianceYearByNotes(A<List<Guid>>._)).Returns(complianceYear);

            var currentDate = TestFixture.Create<DateTime>();
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(organisation, scheme,
                    A<List<NoteTransferCategory>>.That.Matches(t => t.Count.Equals(request.CategoryIds.Count)),
                    A<List<NoteTransferTonnage>>.That.Matches(t => 
                        t.Count.Equals(request.TransferValues.Count)), NoteStatus.Draft, complianceYear, userId.ToString(), currentDate)).MustHaveHappenedOnceExactly();

            foreach (var transferValue in request.TransferValues)
            {
                A.CallTo(() =>
                    evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Scheme>._,
                        A<List<NoteTransferCategory>>._,
                        A<List<NoteTransferTonnage>>.That.Matches(t => t.Count(
                            t2 => t2.NoteTonnageId.Equals(transferValue.TransferTonnageId) && 
                                  t2.Received.Equals(transferValue.FirstTonnage) &&
                                  t2.Reused.Equals(transferValue.SecondTonnage)).Equals(1)), A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._))
                    .MustHaveHappenedOnceExactly();
            }

            foreach (var transferCategories in request.CategoryIds)
            {
                A.CallTo(() =>
                        evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Scheme>._,
                            A<List<NoteTransferCategory>>.That.Matches(t => t.Count(
                                t2 => t2.CategoryId.ToInt().Equals(transferCategories)).Equals(1)),
                            A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async Task HandleAsync_GivenValidRequest_TransferNoteIdShouldBeReturned()
        {
            //arrange
            var transferNoteId = TestFixture.Create<Guid>();
            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Scheme>._,
                    A<List<NoteTransferCategory>>._, A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._)).Returns(transferNoteId);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(transferNoteId);
        }

        [Fact]
        public async Task HandleAsync_GivenValidRequest_TransactionShouldBeCreatedAndCommitted()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => transactionAdapter.BeginTransaction()).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Scheme>._,
                    A<List<NoteTransferCategory>>._, A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => transactionAdapter.Commit(null)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenErrorDuringValidation_TransactionShouldBeRolledBack()
        {
            //arrange
            A.CallTo(() => transferTonnagesValidator.Validate(A<List<TransferTonnageValue>>._)).ThrowsAsync(new Exception());
                
            //act
            await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenErrorDuringAddingOfTransferNote_TransactionShouldBeRolledBack()
        {
            //arrange
            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Scheme>._,
                    A<List<NoteTransferCategory>>._, A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._)).ThrowsAsync(new Exception());

            //act
            await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
        }

        private TransferEvidenceNoteRequest Request()
        {
            return new TransferEvidenceNoteRequest(organisation.Id, scheme.Id, 
                TestFixture.CreateMany<int>().ToList(), 
                TestFixture.CreateMany<TransferTonnageValue>().ToList(), 
                TestFixture.CreateMany<Guid>().ToList(),
                Core.AatfEvidence.NoteStatus.Draft);
        }
    }
}
