namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;

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
        private readonly Organisation recipientOrganisation;
        private readonly Scheme scheme;
        private readonly Guid userId;
        private const string Error = "You cannot manage evidence as scheme is not in a valid state";
        
        public CreateTransferEvidenceNoteRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userContext = A.Fake<IUserContext>();
            transferTonnagesValidator = A.Fake<ITransferTonnagesValidator>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            recipientOrganisation = A.Fake<Organisation>();
            organisation = A.Fake<Organisation>();
            scheme = A.Fake<Scheme>();
            userId = TestFixture.Create<Guid>();

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(new DateTime(2020, 1, 1));
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => scheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Approved);
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
            A.CallTo(() => genericDataAccess.GetById<Scheme>(request.RecipientId)).Returns(scheme);
            A.CallTo(() => userContext.UserId).Returns(userId);
        }

        [Fact]
        public void CreateTransferEvidenceNoteRequestHandler_ShouldDerivedFromSaveTransferNoteRequestBase()
        {
            typeof(CreateTransferEvidenceNoteRequestHandler).Should().BeDerivedFrom<SaveTransferNoteRequestBase>();
        }

        public static IEnumerable<object[]> OutOfComplianceYear =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 2, 1), 2019, true },
                new object[] { new DateTime(2020, 1, 1), 2022, false },
                new object[] { new DateTime(2020, 2, 1), 2019, false },
                new object[] { new DateTime(2020, 1, 1), 2022, true },
            };

        [Theory]
        [MemberData(nameof(OutOfComplianceYear))]
        public async Task HandleAsync_GivenRequestedYearIsClosed_InvalidOperationExceptionExpected(DateTime currentDate, int complianceYear, bool balancingScheme)
        {
            //arrange
            var request = Request();
            request.ComplianceYear = complianceYear;
            var note = A.Fake<Note>();
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(balancingScheme ? A.Fake<ProducerBalancingScheme>() : null);
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => note.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be(Error);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationIsNotBalancingSchemeAndSchemeIsWithdrawn_InvalidOperationExceptionExpected()
        {
            var request = Request();
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Withdrawn);
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be(Error);
        }

        //Not a valid scenario but in as a check against the balancing scheme
        [Fact]
        public async Task HandleAsync_GivenOrganisationIsBalancingSchemeAndSchemeIsWithdrawn_NoExceptionExpected()
        {
            var request = Request();
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Withdrawn);
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeNull();
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
        public async Task HandleAsync_GivenRequestAndRecipientOrganisationFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Organisation>(A<Guid>._)).ReturnsNextFromSequence(A.Fake<Organisation>(), null);

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
            A.CallTo(() => transferTonnagesValidator.Validate(request.TransferValues, null)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(SchemeStatusData))]
        public async Task HandleAsync_GivenValidRequest_TransferNoteShouldBeAdded(SchemeStatus status)
        {
            if (status == SchemeStatus.Withdrawn)
            {
                return;
            }

            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            SystemTime.Freeze(currentDate);
            A.CallTo(() => scheme.SchemeStatus).Returns(status);
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.RecipientId)).Returns(recipientOrganisation);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(organisation, recipientOrganisation,
                    A<List<NoteTransferTonnage>>.That.Matches(t => 
                        t.Count.Equals(request.TransferValues.Count)), NoteStatus.Draft, request.ComplianceYear, userId.ToString(), 
                    A<DateTime>.That.Matches(d => d.Year == currentDate.Year && d.Month == currentDate.Month && d.Day == currentDate.Day))).MustHaveHappenedOnceExactly();

            foreach (var transferValue in request.TransferValues)
            {
                A.CallTo(() =>
                    evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Organisation>._,
                        A<List<NoteTransferTonnage>>.That.Matches(t => t.Count(
                            t2 => t2.NoteTonnageId.Equals(transferValue.Id) && 
                                  t2.Received.Equals(transferValue.FirstTonnage) &&
                                  t2.Reused.Equals(transferValue.SecondTonnage)).Equals(1)), A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._))
                    .MustHaveHappenedOnceExactly();
            }

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenValidRequest_TransferNoteIdShouldBeReturned()
        {
            //arrange
            var transferNoteId = TestFixture.Create<Guid>();
            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(transferNoteId);

            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Organisation>._,
                    A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._)).Returns(note);

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
                .Then(A.CallTo(() => evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Organisation>._,
                    A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => transactionAdapter.Commit(null)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenErrorDuringValidation_TransactionShouldBeRolledBack()
        {
            //arrange
            A.CallTo(() => transferTonnagesValidator.Validate(A<List<TransferTonnageValue>>._, null)).ThrowsAsync(new Exception());
                
            //act
            await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionAdapter.Commit(null)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenErrorDuringAddingOfTransferNote_TransactionShouldBeRolledBack()
        {
            //arrange
            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Organisation>._,
                    A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<int>._, A<string>._, A<DateTime>._)).ThrowsAsync(new Exception());

            //act
            await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionAdapter.Commit(null)).MustNotHaveHappened();
        }

        private TransferEvidenceNoteRequest Request()
        {
            return new TransferEvidenceNoteRequest(organisation.Id, scheme.Id, 
                TestFixture.CreateMany<int>().ToList(), 
                TestFixture.CreateMany<TransferTonnageValue>().ToList(), 
                TestFixture.CreateMany<Guid>().ToList(),
                Core.AatfEvidence.NoteStatus.Draft,
                2020);
        }
    }
}
