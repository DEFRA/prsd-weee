﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.Core.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.Note;
    using Weee.Tests.Core;
    using Xunit;

    public class SetNoteStatusRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly Note note;
        private Guid recipientId;

        public SetNoteStatusRequestHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();
            authorization = A.Fake<IWeeeAuthorization>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            recipientId = TestFixture.Create<Guid>();
            note = A.Fake<Note>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.Internal)]
        public async Task HandleAsync_SetNoteStatusRequestHandler_WithNonExternalUser_ThrowsSecurityException(
            AuthorizationBuilder.UserType userType)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var noteId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            var message = new SetNoteStatus(noteId, Core.AatfEvidence.NoteStatus.Submitted);

            //act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(message));

            // Assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldGetSystemDateTime()
        {
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var message = new SetNoteStatus(TestFixture.Create<Guid>(), TestFixture.Create<EA.Weee.Core.AatfEvidence.NoteStatus>());

            //act
            await handler.HandleAsync(message);

            //assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoSchemeAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenySchemeAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatus(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);
            
            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckExternalAccess()
        {
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatus(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckSchemeAccess()
        {
            //arrange
            recipientId = TestFixture.Create<Guid>();
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatus(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => authorization.EnsureSchemeAccess(recipientId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_ExternalUser_WithNoteNotFound_ThrowArgumentNullException()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns((Note)null);

            var message = new SetNoteStatus(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(message));

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_ExternalUser_WithNoteFound_ReturnsCorrectNoteId()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var id = TestFixture.Create<Guid>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => context.Notes.FindAsync(id)).Returns(note);
            var message = new SetNoteStatus(id, Core.AatfEvidence.NoteStatus.Approved);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.Should().Be(id);
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Approved)]
        [InlineData(Core.AatfEvidence.NoteStatus.Rejected)]
        [InlineData(Core.AatfEvidence.NoteStatus.Returned)]
        public async Task HandleAsync_ExternalUser_WithStatusNoteUpdate_UpdateStatusAndSaveChangesShouldBeCalled(Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var userId = TestFixture.Create<Guid>();

            var message = new SetNoteStatus(note.Id, status);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);

            var currentDate = TestFixture.Create<DateTime>();
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => note.UpdateStatus(status.ToDomainEnumeration<NoteStatus>(), userId.ToString(), A<DateTime>.That.IsEqualTo(new DateTime(currentDate.Year, SystemTime.UtcNow.Month, SystemTime.UtcNow.Day)), null))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappenedOnceExactly());
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Approved)]
        [InlineData(Core.AatfEvidence.NoteStatus.Rejected)]
        [InlineData(Core.AatfEvidence.NoteStatus.Returned)]
        public async Task HandleAsync_ExternalUser_WithReasonNoteUpdate_SaveChangesAsyncShouldBeCalled(Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var userId = TestFixture.Create<Guid>();

            var message = new SetNoteStatus(note.Id, status, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);

            var currentDate = TestFixture.Create<DateTime>();
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => note.UpdateStatus(status.ToDomainEnumeration<NoteStatus>(), userId.ToString(), A<DateTime>.That.IsEqualTo(new DateTime(currentDate.Year, SystemTime.UtcNow.Month, SystemTime.UtcNow.Day)), "reason passed as parameter"))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappenedOnceExactly());
        }
    }
}
