﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
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
    using RequestHandlers.Factories;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class EditTransferEvidenceNoteRequestHandlerTests : SimpleUnitTestBase
    {
        private EditTransferEvidenceNoteRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly EditTransferEvidenceNoteRequest request;
        private readonly Organisation organisation;
        private readonly Organisation recipientOrganisation;
        private readonly Scheme scheme;
        private readonly Guid userId;
        private Guid transferNoteId;

        public EditTransferEvidenceNoteRequestHandlerTests()
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
            transferNoteId = TestFixture.Create<Guid>();

            A.CallTo(() => scheme.Organisation).Returns(recipientOrganisation);
            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => scheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => organisation.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(A.Fake<Note>());

            request = Request();

            handler = new EditTransferEvidenceNoteRequestHandler(weeeAuthorization,
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

            handler = new EditTransferEvidenceNoteRequestHandler(authorization,
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

            handler = new EditTransferEvidenceNoteRequestHandler(authorization,
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
        public async Task HandleAsync_GivenRequest_TransferNoteShouldBeRetrieved()
        {
            //act
            var result = await handler.HandleAsync(Request());

            //assert
            A.CallTo(() => evidenceDataAccess.GetNoteById(request.TransferNoteId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_SchemeRecipientShouldBeRetrieved()
        {
            //act
            var result = await handler.HandleAsync(Request());

            //assert
            A.CallTo(() => genericDataAccess.GetById<Scheme>(request.RecipientId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_TransferNoteShouldBeUpdated()
        {
            //arrange
            var currentDate = TestFixture.Create<DateTime>();
            SystemTime.Freeze(currentDate);
            var note = A.Fake<Note>();
            A.CallTo(() => scheme.Organisation).Returns(recipientOrganisation);
            A.CallTo(() => genericDataAccess.GetById<Scheme>(A<Guid>._)).Returns(scheme);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() =>
                evidenceDataAccess.UpdateTransfer(note, recipientOrganisation,
                    A<List<NoteTransferTonnage>>.That.Matches(t =>
                        t.Count.Equals(request.TransferValues.Count)), NoteStatus.Draft,
                    CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate))).MustHaveHappenedOnceExactly();

            foreach (var transferValue in request.TransferValues)
            {
                A.CallTo(() =>
                        evidenceDataAccess.UpdateTransfer(A<Note>._, A<Organisation>._,
                            A<List<NoteTransferTonnage>>.That.Matches(t => t.Count(
                                t2 => t2.NoteTonnageId.Equals(transferValue.Id) &&
                                      t2.Received.Equals(transferValue.FirstTonnage) &&
                                      t2.Reused.Equals(transferValue.SecondTonnage)).Equals(1)), A<NoteStatus>._, A<DateTime>._))
                    .MustHaveHappenedOnceExactly();
            }

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_TransferValuesShouldBeValidated()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => transferTonnagesValidator.Validate(request.TransferValues, transferNoteId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenValidRequest_TransferNoteIdShouldBeReturned()
        {
            //arrange
            transferNoteId = TestFixture.Create<Guid>();
            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(transferNoteId);

            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);

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
            A.CallTo(() => transactionAdapter.BeginTransaction())
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => evidenceDataAccess.UpdateTransfer(A<Note>._, A<Organisation>._, A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<DateTime>._)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => transferTonnagesValidator.Validate(A<List<TransferTonnageValue>>._, A<Guid>._)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => transactionAdapter.Commit(null))
                    .MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenErrorDuringValidation_TransactionShouldBeRolledBack()
        {
            //arrange
            A.CallTo(() => transferTonnagesValidator.Validate(A<List<TransferTonnageValue>>._, A<Guid?>._)).ThrowsAsync(new Exception());
                
            //act
            await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionAdapter.Commit(null)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenErrorDuringUpdatingOfTransferNote_TransactionShouldBeRolledBack()
        {
            //arrange
            A.CallTo(() =>
                evidenceDataAccess.UpdateTransfer(A<Note>._, A<Organisation>._,
                    A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<DateTime>._)).ThrowsAsync(new Exception());

            //act
            await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionAdapter.Commit(null)).MustNotHaveHappened();
        }

        private EditTransferEvidenceNoteRequest Request()
        {
            return new EditTransferEvidenceNoteRequest(transferNoteId, organisation.Id, scheme.Id,
                TestFixture.CreateMany<TransferTonnageValue>().ToList(),
                Core.AatfEvidence.NoteStatus.Draft);
        }
    }
}