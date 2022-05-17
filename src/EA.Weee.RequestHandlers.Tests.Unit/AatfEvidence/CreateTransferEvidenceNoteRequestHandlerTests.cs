﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Tests.Unit.Helpers;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class CreateTransferEvidenceNoteRequestHandlerTests
    {
        private CreateTransferEvidenceNoteRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly TransferEvidenceNoteRequest request;
        private readonly Organisation organisation;
        private readonly Scheme scheme;
        private readonly Guid userId;

        public CreateTransferEvidenceNoteRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userContext = A.Fake<IUserContext>();
            transferTonnagesValidator = A.Fake<ITransferTonnagesValidator>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();

            organisation = A.Fake<Organisation>();
            scheme = A.Fake<Scheme>();
            userId = fixture.Create<Guid>();

            A.CallTo(() => scheme.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => organisation.Id).Returns(fixture.Create<Guid>());

            request = Request();

            handler = new CreateTransferEvidenceNoteRequestHandler(weeeAuthorization,
                genericDataAccess,
                userContext,
                evidenceDataAccess,
                transferTonnagesValidator,
                transactionAdapter);

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
                transactionAdapter);

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
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new CreateTransferEvidenceNoteRequestHandler(authorization,
                genericDataAccess,
                userContext,
                evidenceDataAccess,
                transferTonnagesValidator,
                transactionAdapter);

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
        public async Task HandleAsync_GivenValidRequest_TransferNoteShouldBeAdded()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => genericDataAccess.GetById<Scheme>(request.SchemeId)).Returns(scheme);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(organisation, scheme, 
                    A<List<NoteTransferTonnage>>.That.Matches(t => 
                        t.Count.Equals(request.TransferValues.Count)), NoteStatus.Draft, userId.ToString())).MustHaveHappenedOnceExactly();

            foreach (var transferValue in request.TransferValues)
            {
                A.CallTo(() =>
                    evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Scheme>._,
                        A<List<NoteTransferTonnage>>.That.Matches(t => t.Count(
                            t2 => t2.NoteTonnageId.Equals(transferValue.TransferTonnageId) && 
                                  t2.Received.Equals(transferValue.FirstTonnage) &&
                                  t2.Reused.Equals(transferValue.SecondTonnage)).Equals(1)), A<NoteStatus>._, A<string>._))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async Task HandleAsync_GivenValidRequest_TransferNoteIdShouldBeReturned()
        {
            //arrange
            var transferNoteId = fixture.Create<Guid>();
            A.CallTo(() =>
                evidenceDataAccess.AddTransferNote(A<Organisation>._, A<Scheme>._,
                    A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<string>._)).Returns(transferNoteId);

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
                    A<List<NoteTransferTonnage>>._,
                    A<NoteStatus>._, A<string>._)).MustHaveHappenedOnceExactly())
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
                    A<List<NoteTransferTonnage>>._, A<NoteStatus>._, A<string>._)).ThrowsAsync(new Exception());

            //act
            await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
        }

        private TransferEvidenceNoteRequest Request()
        {
            return new TransferEvidenceNoteRequest(organisation.Id, scheme.Id, fixture.CreateMany<TransferTonnageValue>().ToList(), Core.AatfEvidence.NoteStatus.Draft);
        }
    }
}