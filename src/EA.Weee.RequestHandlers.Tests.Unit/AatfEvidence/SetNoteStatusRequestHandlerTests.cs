namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using AutoFixture;
    using DataAccess;
    using Domain.Scheme;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Note;
    using FakeItEasy;
    using RequestHandlers.AatfEvidence;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class SetNoteStatusRequestHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;
        private readonly Fixture fixture;
        private readonly Note note;
        private readonly Note nullNote;
        private readonly Guid evidenceNoteId;
        private readonly Scheme recipient;
        private readonly Guid recipientId;

        public SetNoteStatusRequestHandlerTests()
        {
            fixture = new Fixture();
            context = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();
            authorization = A.Fake<IWeeeAuthorization>();

            A.Fake<IUserContext>();
            A.Fake<Scheme>();
            note = A.Fake<Note>();
            nullNote = null;

            fixture.Create<Guid>();
            evidenceNoteId = fixture.Create<Guid>();
            recipient = fixture.Create<Scheme>();
            recipientId = fixture.Create<Guid>();
        }

        /// <summary>
        ///     This test ensures that a non-internal user cannot execute requests to set a scheme's status.
        /// </summary>
        /// <returns></returns>
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
            SetNoteStatus message = new SetNoteStatus(noteId, Core.AatfEvidence.NoteStatus.Submitted);

            // Act
            Func<Task<Guid>> action = () => handler.HandleAsync(message);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External, Core.AatfEvidence.NoteStatus.Approved)]
        public void ExternalUser_CanSetApprovedStatus(AuthorizationBuilder.UserType userType, Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var recipientId = new Guid("66d5b5fa-28a7-4fa3-8426-6f284288921f");
            SetNoteStatus message = new SetNoteStatus(noteId, status);
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Approved);
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);
            A.CallTo(() => context.Notes.FindAsync(evidenceNoteId)).Returns(note);

            // Act
            var result = handler.HandleAsync(message);

            // Assert
            var actual = note.Status;
            var expected = Domain.Evidence.NoteStatus.Approved;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External, Core.AatfEvidence.NoteStatus.Approved)]
        public void ExternalUser_WithIncorrectSchemeCannotSetApprovedStatus(AuthorizationBuilder.UserType userType, Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var recipientId = new Guid("66d5b5fa-28a7-4fa3-8426-6f284288921f");
            SetNoteStatus message = new SetNoteStatus(noteId, status);
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Approved);
            A.CallTo(() => note.Recipient.Id).Returns(Guid.Empty);     // this guarantees the recipient id is different than scheme id
            A.CallTo(() => context.Notes.FindAsync(evidenceNoteId)).Returns(note);

            // Act
            Func<Task<Guid>> action = () => handler.HandleAsync(message);

            // Assert
            Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External, Core.AatfEvidence.NoteStatus.Approved)]
        public void ExternalUser_WithNoteNotFound_ThrowsException(AuthorizationBuilder.UserType userType, Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var recipientId = new Guid("66d5b5fa-28a7-4fa3-8426-6f284288921f");
            SetNoteStatus message = new SetNoteStatus(noteId, status);
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Approved);
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);
            A.CallTo(() => context.Notes.FindAsync(evidenceNoteId)).Returns(nullNote);

            // Act
            Func<Task<Guid>> action = () => handler.HandleAsync(message);

            // Assert
            Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External, Core.AatfEvidence.NoteStatus.Approved)]
        public void ExternalUser_WithNoteFound_ReturnsCorrectNoteId(AuthorizationBuilder.UserType userType, Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var newNote = fixture.Create<Note>();
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Approved);
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);
            A.CallTo(() => context.Notes.FindAsync(newNote.Id)).Returns(newNote);
            SetNoteStatus message = new SetNoteStatus(newNote.Id, status);

            // Act
            var result = handler.HandleAsync(message);

            // Assert
            Assert.Equal(Guid.Empty, newNote.Id);
        }

        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External, Core.AatfEvidence.NoteStatus.Approved)]
        public void ExternalUser_WithNoteUpdate_SaveChangesAsyncShouldBeCalled(AuthorizationBuilder.UserType userType, Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization);
            var newNote = A.Fake<Note>();
            SetNoteStatus message = new SetNoteStatus(note.Id, status);
            A.CallTo(() => newNote.Status).Returns(Domain.Evidence.NoteStatus.Approved);
            A.CallTo(() => newNote.Recipient.Id).Returns(recipientId);
            A.CallTo(() => context.Notes.FindAsync(note.Id)).Returns(newNote);
            A.CallTo(() => newNote.UpdateStatus(NoteStatus.Approved, "changed by"));

            // Act
            var result = handler.HandleAsync(message);

            // Assert
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
