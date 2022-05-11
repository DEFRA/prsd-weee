﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.Note;
    using Weee.Tests.Core;
    using Xunit;

    public class SetNoteStatusRequestHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;
        private readonly Fixture fixture;
        private readonly Note note;
        private Guid recipientId;

        public SetNoteStatusRequestHandlerTests()
        {
            fixture = new Fixture();
            context = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();
            authorization = A.Fake<IWeeeAuthorization>();
            fixture.Create<Guid>();
            fixture.Create<Scheme>();
            recipientId = fixture.Create<Guid>();
            note = A.Fake<Note>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.Internal)]
        public async Task SetNoteStatusRequestHandler_WithNonExternalUser_ThrowsSecurityException(
            AuthorizationBuilder.UserType userType)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var message = new SetNoteStatus(noteId, Core.AatfEvidence.NoteStatus.Submitted);

            //act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(message));

            // Assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task ExternalUser_CanSetApprovedStatus()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var status = Core.AatfEvidence.NoteStatus.Approved;

            var note = new Note(A.Fake<Organisation>(), A.Fake<Scheme>(), DateTime.Now, DateTime.Now,
                WasteType.HouseHold, Protocol.Actual, A.Fake<Aatf>(), "created", new List<NoteTonnage>());

            note.UpdateStatus(NoteStatus.Submitted, "updatedBy");

            var message = new SetNoteStatus(noteId, status);
            A.CallTo(() => context.Notes.FindAsync(noteId)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.Status.Should().Be(NoteStatus.Approved);
        }

        [Fact]
        public async Task ExternalUser_CanSetRejectedStatus()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var status = Core.AatfEvidence.NoteStatus.Rejected;

            var note = new Note(A.Fake<Organisation>(), A.Fake<Scheme>(), DateTime.Now, DateTime.Now,
                WasteType.HouseHold, Protocol.Actual, A.Fake<Aatf>(), "created",
                new List<NoteTonnage>());

            note.UpdateStatus(NoteStatus.Submitted, "updatedBy");

            var message = new SetNoteStatus(noteId, status);
            A.CallTo(() => context.Notes.FindAsync(noteId)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.Status.Should().Be(NoteStatus.Rejected);
        }

        [Fact]
        public async Task ExternalUser_CanSetReturnedStatus()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var status = Core.AatfEvidence.NoteStatus.Returned;

            var note = new Note(A.Fake<Organisation>(), A.Fake<Scheme>(), DateTime.Now, DateTime.Now,
                WasteType.HouseHold, Protocol.Actual, A.Fake<Aatf>(), "created",
                new List<NoteTonnage>());

            note.UpdateStatus(NoteStatus.Submitted, "updatedBy");

            var message = new SetNoteStatus(noteId, status);
            A.CallTo(() => context.Notes.FindAsync(noteId)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.Status.Should().Be(NoteStatus.Returned);
        }

        [Fact]
        public async Task HandleAsync_GivenNoSchemeAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenySchemeAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var request = new SetNoteStatus(fixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);
            
            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckExternalAccess()
        {
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var request = new SetNoteStatus(fixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckSchemeAccess()
        {
            //arrange
            recipientId = fixture.Create<Guid>();
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var request = new SetNoteStatus(fixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => authorization.EnsureSchemeAccess(recipientId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ExternalUser_WithNoteNotFound_ThrowArgumentNullException()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns((Note)null);

            var message = new SetNoteStatus(fixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(message));

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task ExternalUser_WithNoteFound_ReturnsCorrectNoteId()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var id = fixture.Create<Guid>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => context.Notes.FindAsync(id)).Returns(note);
            var message = new SetNoteStatus(id, Core.AatfEvidence.NoteStatus.Approved);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.Should().Be(id);
        }

        [Fact]
        public async Task ExternalUser_WithApprovedNoteUpdate_SaveChangesAsyncShouldBeCalled()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var userId = fixture.Create<Guid>();

            var message = new SetNoteStatus(note.Id, Core.AatfEvidence.NoteStatus.Approved);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => note.UpdateStatus(NoteStatus.Approved, userId.ToString(), null))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                    .MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task ExternalUser_WithRejectedNoteUpdate_SaveChangesAsyncShouldBeCalled()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var userId = fixture.Create<Guid>();

            var message = new SetNoteStatus(note.Id, Core.AatfEvidence.NoteStatus.Rejected);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => note.UpdateStatus(NoteStatus.Rejected, userId.ToString(), null))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                    .MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task ExternalUser_WithReturnedNoteUpdate_SaveChangesAsyncShouldBeCalled()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var userId = fixture.Create<Guid>();

            var message = new SetNoteStatus(note.Id, Core.AatfEvidence.NoteStatus.Returned);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => note.UpdateStatus(NoteStatus.Returned, userId.ToString(), null))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                    .MustHaveHappenedOnceExactly());
        }
    }
}
