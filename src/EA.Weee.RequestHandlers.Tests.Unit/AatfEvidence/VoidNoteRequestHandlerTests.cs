namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.Requests.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Factories;
    using RequestHandlers.Security;
    using Weee.Security;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;

    public class VoidNoteRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly Note note;
        private readonly DateTime currentDate;
        private readonly VoidNoteRequestHandler handler;

        public VoidNoteRequestHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();
            authorization = A.Fake<IWeeeAuthorization>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            note = A.Fake<Note>();
            currentDate = new DateTime(2020, 1, 1);

            var recipientOrganisation = A.Fake<Organisation>();
            var recipientScheme = A.Fake<Scheme>();

            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() {recipientScheme});
            A.CallTo(() => recipientScheme.SchemeStatus).Returns(SchemeStatus.Approved);
            A.CallTo(() => recipientScheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => note.Recipient).Returns(recipientOrganisation);
            A.CallTo(() => note.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => note.Status).Returns(NoteStatus.Approved);
            
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            handler = new VoidNoteRequestHandler(context, userContext, authorization, systemDataDataAccess);
        }

        [Fact]
        public void VoidNoteRequestHandler_ShouldDerivedFromSaveTransferNoteRequestBase()
        {
            typeof(VoidNoteRequestHandler).Should().BeDerivedFrom<SaveNoteRequestBase>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckInternalAccess()
        {
            //arrange
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(TestFixture.Create<VoidNoteRequest>());

            //assert
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNoInternalAccess_SecurityExceptionExpected()
        {
            //arrange
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).Throws<SecurityException>();

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(TestFixture.Create<VoidNoteRequest>()));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckAdminAccess()
        {
            //arrange
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(TestFixture.Create<VoidNoteRequest>());

            //assert
            A.CallTo(() => authorization.EnsureUserInRole(Roles.InternalAdmin)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestNoAdminAccess_SecurityExceptionExpected()
        {
            //arrange
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => authorization.EnsureUserInRole(A<Roles>._)).Throws<SecurityException>();

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(TestFixture.Create<VoidNoteRequest>()));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WithNoteNotFound_ThrowArgumentNullException()
        {
            // Arrange
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns((Note)null);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(TestFixture.Create<VoidNoteRequest>()));

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_WithNoteFound_ReturnsCorrectNoteId()
        {
            // Arrange
            var id = TestFixture.Create<Guid>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => context.Notes.FindAsync(id)).Returns(note);

            var request = new VoidNoteRequest(id);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().Be(id);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task HandleAsync_WithNoteFoundThatIsNotApproved_ShouldThrowInvalidOperationException(NoteStatus status)
        {
            if (status == NoteStatus.Approved)
            {
                return;
            }

            // Arrange
            var id = TestFixture.Create<Guid>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Status).Returns(status);
            A.CallTo(() => note.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => context.Notes.FindAsync(id)).Returns(note);

            var request = new VoidNoteRequest(id);

            // Act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            // Assert
            exception.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be($"Cannot void note with id {id}");
        }

        [Fact]
        public async Task HandleAsync_WithNoteFoundThatIsApprovedButHasEvidenceNoteWithApprovedTransfers_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var id = TestFixture.Create<Guid>();
            var noteTransferTonnages = new List<NoteTransferTonnage>();
            var transferNote = A.Fake<Note>();
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            var noteTransferTonnage = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage.TransferNote).Returns(transferNote);
            noteTransferTonnages.Add(noteTransferTonnage);

            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => note.NoteType).Returns(NoteType.EvidenceNote);
            A.CallTo(() => note.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => context.Notes.FindAsync(id)).Returns(note);

            var request = new VoidNoteRequest(id);

            // Act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            // Assert
            exception.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be($"Cannot void note with id {id} as its has approved transfers");
        }

        [Fact]
        public async Task HandleAsync_WithNoteFoundThatIsApprovedAndHasEvidenceNoteWithNonApprovedTransfers_ShouldNotThrowInvalidOperationException()
        {
            // Arrange
            var id = TestFixture.Create<Guid>();
            var noteTransferTonnages = new List<NoteTransferTonnage>();
            var transferNoteDraft = A.Fake<Note>();
            A.CallTo(() => transferNoteDraft.Status).Returns(NoteStatus.Draft);
            var noteTransferTonnageDraft = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnageDraft.TransferNote).Returns(transferNoteDraft);
            noteTransferTonnages.Add(noteTransferTonnageDraft);

            var transferNoteSubmitted = A.Fake<Note>();
            A.CallTo(() => transferNoteSubmitted.Status).Returns(NoteStatus.Submitted);
            var noteTransferTonnageSubmitted = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnageSubmitted.TransferNote).Returns(transferNoteSubmitted);
            noteTransferTonnages.Add(noteTransferTonnageSubmitted);

            var transferNoteVoid = A.Fake<Note>();
            A.CallTo(() => transferNoteVoid.Status).Returns(NoteStatus.Void);
            var noteTransferTonnageVoid = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnageVoid.TransferNote).Returns(transferNoteVoid);
            noteTransferTonnages.Add(noteTransferTonnageVoid);

            var transferNoteReturned = A.Fake<Note>();
            A.CallTo(() => transferNoteReturned.Status).Returns(NoteStatus.Returned);
            var noteTransferTonnageReturned = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnageReturned.TransferNote).Returns(transferNoteReturned);
            noteTransferTonnages.Add(noteTransferTonnageReturned);

            var transferNoteRejected = A.Fake<Note>();
            A.CallTo(() => transferNoteRejected.Status).Returns(NoteStatus.Returned);
            var noteTransferTonnageRejected = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnageRejected.TransferNote).Returns(transferNoteRejected);
            noteTransferTonnages.Add(noteTransferTonnageRejected);

            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => note.NoteType).Returns(NoteType.EvidenceNote);
            A.CallTo(() => note.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => context.Notes.FindAsync(id)).Returns(note);

            var request = new VoidNoteRequest(id);

            // Act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            // Assert
            exception.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WithVoidStatusNoteUpdate_UpdateStatusAndSaveChangesShouldBeCalled()
        {
            // Arrange
            SystemTime.Freeze(DateTime.UtcNow);
            var userId = TestFixture.Create<Guid>();

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            var request = new VoidNoteRequest(TestFixture.Create<Guid>());
            
            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => note.UpdateStatus(NoteStatus.Void, userId.ToString(), A<DateTime>.That.IsEqualTo(CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate)), null))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappenedOnceExactly());
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_WithReasonNoteUpdate_SaveChangesAsyncShouldBeCalled()
        {
            // Arrange
            SystemTime.Freeze(DateTime.UtcNow);
            var userId = TestFixture.Create<Guid>();

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            var request = new VoidNoteRequest(TestFixture.Create<Guid>(), "reason passed as parameter");

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => note.UpdateStatus(NoteStatus.Void, userId.ToString(), A<DateTime>.That.IsEqualTo(CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate)), "reason passed as parameter"))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappenedOnceExactly());
            SystemTime.Freeze(DateTime.UtcNow);
        }
    }
}
