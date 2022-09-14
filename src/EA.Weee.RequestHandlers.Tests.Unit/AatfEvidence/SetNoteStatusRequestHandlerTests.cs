﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfEvidence;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.Core.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Factories;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class SetNoteStatusRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly Note note;
        private readonly Scheme recipientScheme;
        private readonly Organisation recipientOrganisation;
        private readonly DateTime currentDate;
        private const string Error = "You cannot manage evidence as scheme is not in a valid state";

        public SetNoteStatusRequestHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();
            authorization = A.Fake<IWeeeAuthorization>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            note = A.Fake<Note>();
            currentDate = new DateTime(2020, 1, 1);

            recipientOrganisation = A.Fake<Organisation>();
            recipientScheme = A.Fake<Scheme>();

            A.CallTo(() => recipientScheme.SchemeStatus).Returns(SchemeStatus.Approved);
            A.CallTo(() => recipientScheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { recipientScheme });
            A.CallTo(() => note.Recipient).Returns(recipientOrganisation);
            A.CallTo(() => note.ComplianceYear).Returns(currentDate.Year);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);
        }

        [Fact]
        public void SetNoteStatusRequestHandler_ShouldDerivedFromSaveTransferNoteRequestBase()
        {
            typeof(SetNoteStatusRequestHandler).Should().BeDerivedFrom<SaveNoteRequestBase>();
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearDataWithBalancingScheme))]
        public async Task HandleAsync_GivenRequestedYearIsClosed_InvalidOperationExceptionExpected(DateTime currentDate, int complianceYear, bool balancingScheme)
        {
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatusRequest(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => note.Recipient).Returns(recipientOrganisation);
            A.CallTo(() => recipientOrganisation.ProducerBalancingScheme).Returns(balancingScheme ? A.Fake<ProducerBalancingScheme>() : null);
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
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatusRequest(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => note.Recipient).Returns(recipientOrganisation);
            A.CallTo(() => recipientOrganisation.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => note.ComplianceYear).Returns(currentDate.Year);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);
            A.CallTo(() => recipientScheme.SchemeStatus).Returns(SchemeStatus.Withdrawn);
            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { recipientScheme });

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be(Error);
        }

        //Not a valid scenario but in as a check against the balancing scheme
        [Fact]
        public async Task HandleAsync_GivenOrganisationIsBalancingSchemeAndSchemeIsWithdrawn_NoExceptionExpected()
        {
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatusRequest(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => note.Recipient).Returns(recipientOrganisation);
            A.CallTo(() => recipientOrganisation.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => note.ComplianceYear).Returns(currentDate.Year);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);
            A.CallTo(() => recipientScheme.SchemeStatus).Returns(SchemeStatus.Withdrawn);
            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { recipientScheme });

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeNull();
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
            var message = new SetNoteStatusRequest(noteId, Core.AatfEvidence.NoteStatus.Submitted);

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
            var message = new SetNoteStatusRequest(TestFixture.Create<Guid>(), TestFixture.Create<EA.Weee.Core.AatfEvidence.NoteStatus>());

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(message);

            //assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact] public async Task HandleAsync_GivenNoBalancingSchemeAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatusRequest(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task HandleAsync_GivenRequestThatIsNotBeingSubmitted_ShouldCheckRecipientOrganisationAccess(Core.AatfEvidence.NoteStatus status)
        {
            if (status == Core.AatfEvidence.NoteStatus.Submitted)
            {
                return;
            }

            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatusRequest(TestFixture.Create<Guid>(), status);

            var organisation = A.Fake<Organisation>();
            var organisationId = TestFixture.Create<Guid>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => note.Recipient).Returns(organisation);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestThatIsBeingSubmitted_ShouldCheckOrganisationAccess()
        {
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatusRequest(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Submitted);

            var organisation = A.Fake<Organisation>();
            var organisationId = TestFixture.Create<Guid>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckExternalAccess()
        {
            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var request = new SetNoteStatusRequest(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_ExternalUser_WithNoteNotFound_ThrowArgumentNullException()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns((Note)null);

            var message = new SetNoteStatusRequest(TestFixture.Create<Guid>(), Core.AatfEvidence.NoteStatus.Approved);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(message));

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_ExternalUser_WithNoteFound_ReturnsCorrectNoteId()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var id = TestFixture.Create<Guid>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => context.Notes.FindAsync(id)).Returns(note);
            var message = new SetNoteStatusRequest(id, Core.AatfEvidence.NoteStatus.Approved);

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
            SystemTime.Freeze(DateTime.UtcNow);
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var userId = TestFixture.Create<Guid>();

            var message = new SetNoteStatusRequest(note.Id, status);
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => note.UpdateStatus(status.ToDomainEnumeration<NoteStatus>(), userId.ToString(), A<DateTime>.That.IsEqualTo(CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate)), null))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappenedOnceExactly());
            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Approved)]
        [InlineData(Core.AatfEvidence.NoteStatus.Rejected)]
        [InlineData(Core.AatfEvidence.NoteStatus.Returned)]
        public async Task HandleAsync_ExternalUser_WithReasonNoteUpdate_SaveChangesAsyncShouldBeCalled(Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            SystemTime.Freeze(DateTime.UtcNow);
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess);
            var userId = TestFixture.Create<Guid>();

            var message = new SetNoteStatusRequest(note.Id, status, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => note.UpdateStatus(status.ToDomainEnumeration<NoteStatus>(), userId.ToString(), A<DateTime>.That.IsEqualTo(CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate)), "reason passed as parameter"))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappenedOnceExactly());
            SystemTime.Freeze(DateTime.UtcNow);
        }
    }
}
