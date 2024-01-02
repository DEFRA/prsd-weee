namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
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
    using AddressType = Domain.AddressType;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;
    using SchemeStatus = Domain.Scheme.SchemeStatus;

    public class SetNoteStatusRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IAddressUtilities addressUtilities;
        private readonly IEvidenceDataAccess evidenceDataAccess;
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
            addressUtilities = A.Fake<IAddressUtilities>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
            var message = new SetNoteStatusRequest(TestFixture.Create<Guid>(), TestFixture.Create<EA.Weee.Core.AatfEvidence.NoteStatus>());

            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(message);

            //assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact] 
        public async Task HandleAsync_GivenNoBalancingSchemeAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            if (status == Core.AatfEvidence.NoteStatus.Submitted || status == Core.AatfEvidence.NoteStatus.Cancelled)
            {
                return;
            }

            //arrange
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
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

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task HandleAsync_ExternalUser_WithStatusNotApprovedOrRejected_ApprovedDetailsShouldNotBeSet(Core.AatfEvidence.NoteStatus status)
        {
            if (status == Core.AatfEvidence.NoteStatus.Approved || status == Core.AatfEvidence.NoteStatus.Rejected)
            {
                return;
            }

            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var recipient = A.Fake<Organisation>();
            A.CallTo(() => recipient.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => recipient.Schemes).Returns(new List<Scheme>() { A.Fake<Scheme>() });
            A.CallTo(() => note.ApprovedRecipientAddress).Returns(null);
            A.CallTo(() => note.ApprovedRecipientSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererAddress).Returns(null);
            A.CallTo(() => note.Recipient).Returns(recipient);

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._,
                A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).Returns("address");

            var message = new SetNoteStatusRequest(note.Id, status, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.ApprovedRecipientAddress.Should().BeNull();
            note.ApprovedRecipientSchemeName.Should().BeNull();
            note.ApprovedTransfererAddress.Should().BeNull();
            note.ApprovedTransfererSchemeName.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_ExternalUser_WithStatusApprovedAndRecipientIsBalancingScheme_ApprovedRecipientDetailsShouldNotBeSet()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var recipient = A.Fake<Organisation>();
            A.CallTo(() => recipient.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => note.ApprovedRecipientAddress).Returns(null);
            A.CallTo(() => note.ApprovedRecipientSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererAddress).Returns(null);
            A.CallTo(() => note.Recipient).Returns(recipient);
            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._,
                A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).Returns("address");

            var message = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.ApprovedRecipientAddress.Should().BeNull();
            note.ApprovedRecipientSchemeName.Should().BeNull();
            note.ApprovedTransfererAddress.Should().BeNull();
            note.ApprovedTransfererSchemeName.Should().BeNull();
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Approved)]
        [InlineData(Core.AatfEvidence.NoteStatus.Rejected)]
        public async Task HandleAsync_ExternalUser_WithStatusApprovedOrRejectedAndTransferNoteAndTransferOrganisationIsBalancingScheme_ApprovedDetailsShouldNotBeSet(Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var transferOrganisation = A.Fake<Organisation>();
            A.CallTo(() => transferOrganisation.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => note.ApprovedRecipientAddress).Returns(null);
            A.CallTo(() => note.ApprovedRecipientSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererAddress).Returns(null);
            A.CallTo(() => note.Organisation).Returns(transferOrganisation);

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._,
                A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).Returns("address");

            var message = new SetNoteStatusRequest(note.Id, status, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.ApprovedRecipientAddress.Should().BeNull();
            note.ApprovedRecipientSchemeName.Should().BeNull();
            note.ApprovedTransfererAddress.Should().BeNull();
            note.ApprovedTransfererSchemeName.Should().BeNull();
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Approved)]
        [InlineData(Core.AatfEvidence.NoteStatus.Rejected)]
        public async Task HandleAsync_ExternalUser_WithStatusOrRejectedApprovedAndNotTransferNoteAndTransferOrganisationIsNotBalancingScheme_ApprovedDetailsShouldNotBeSet(Core.AatfEvidence.NoteStatus status)
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var transferOrganisation = A.Fake<Organisation>();
            A.CallTo(() => note.ApprovedRecipientAddress).Returns(null);
            A.CallTo(() => note.ApprovedRecipientSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererSchemeName).Returns(null);
            A.CallTo(() => note.ApprovedTransfererAddress).Returns(null);
            A.CallTo(() => note.Organisation).Returns(transferOrganisation);
            A.CallTo(() => note.NoteType).Returns(Domain.Evidence.NoteType.TransferNote);

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._,
                A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).Returns("address");

            var message = new SetNoteStatusRequest(note.Id, status, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.ApprovedRecipientAddress.Should().BeNull();
            note.ApprovedRecipientSchemeName.Should().BeNull();
            note.ApprovedTransfererAddress.Should().BeNull();
            note.ApprovedTransfererSchemeName.Should().BeNull();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        public async Task HandleAsync_ExternalUser_WithConditionToSetApprovedDetailsAndRecipientHasBusinessAddress_ApprovedRecipientDetailsShouldBeSet(int addressType, int noteType)
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var evidenceNote = new Note();
            var address = TestFixture.Create<string>();
            var recipient = Organisation.CreateRegisteredCompany(TestFixture.Create<string>(), "12345678");
            var recipientAddress = new Address("address1", "address2",
                "town", "county", "postcode",
                new Country(TestFixture.Create<Guid>(), "address name"), "01483676767",
                "email");
            var scheme = A.Fake<Scheme>();
            var transferOrganisation = A.Fake<Organisation>();

            A.CallTo(() => scheme.SchemeName).Returns(TestFixture.Create<string>());
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Approved);

            recipient.AddOrUpdateAddress(Enumeration.FromValue<AddressType>(addressType), recipientAddress);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Schemes, new List<Scheme>() { scheme }, recipient);
            ObjectInstantiator<Organisation>.SetProperty(o => o.ProducerBalancingScheme, null, recipient);
            ObjectInstantiator<Organisation>.SetProperty(o => o.BusinessAddress, recipientAddress, recipient);
            ObjectInstantiator<Note>.SetProperty(o => o.ApprovedRecipientAddress, null, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.ApprovedRecipientSchemeName, null, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Recipient, recipient, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Organisation, transferOrganisation, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.ComplianceYear, currentDate.Year, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.NoteType, Enumeration.FromValue<Domain.Evidence.NoteType>(noteType), evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Status, NoteStatus.Submitted, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.NoteStatusHistory, new List<NoteStatusHistory>(), evidenceNote);

            var message = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(evidenceNote);
            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(scheme.SchemeName, recipient.OrganisationName,
                recipientAddress.Address1, recipientAddress.Address2, recipientAddress.TownOrCity,
                recipientAddress.CountyOrRegion, recipientAddress.Postcode, null)).Returns(address);

            // Act
            await handler.HandleAsync(message);

            // Assert
            evidenceNote.ApprovedRecipientSchemeName.Should().Be(scheme.SchemeName);
            evidenceNote.ApprovedRecipientAddress.Should().Be(address);
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task HandleAsync_ExternalUser_WithStatusNotApprovedOrRejectedAndRecipientIsNotBalancingScheme_ApprovedTransfererDetailsShouldNotBeSet(Core.AatfEvidence.NoteStatus status)
        {
            if (status == Core.AatfEvidence.NoteStatus.Approved || status == Core.AatfEvidence.NoteStatus.Rejected)
            {
                return;
            }

            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var transferer = A.Fake<Organisation>();
            A.CallTo(() => transferer.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => transferer.Schemes).Returns(new List<Scheme>() { A.Fake<Scheme>() });
            A.CallTo(() => note.ApprovedTransfererAddress).Returns(null);
            A.CallTo(() => note.ApprovedTransfererSchemeName).Returns(null);
            A.CallTo(() => note.NoteType).Returns(Domain.Evidence.NoteType.TransferNote);
            A.CallTo(() => note.Organisation).Returns(transferer);

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._,
                A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).Returns("address");

            var message = new SetNoteStatusRequest(note.Id, status, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.ApprovedTransfererAddress.Should().BeNull();
            note.ApprovedTransfererSchemeName.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_ExternalUser_WithStatusApprovedAndRecipientIsBalancingScheme_ApprovedTransfererDetailsShouldNotBeSet()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var transferer = A.Fake<Organisation>();
            A.CallTo(() => transferer.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => note.ApprovedTransfererAddress).Returns(null);
            A.CallTo(() => note.ApprovedTransfererSchemeName).Returns(null);
            A.CallTo(() => note.Organisation).Returns(transferer);
            A.CallTo(() => note.NoteType).Returns(Domain.Evidence.NoteType.TransferNote);

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._,
                A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).Returns("address");

            var message = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.ApprovedTransfererAddress.Should().BeNull();
            note.ApprovedTransfererSchemeName.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task HandleAsync_ExternalUser_WithConditionToSetApprovedDetailsAndTransfererHasAddress_ApprovedTransfererDetailsShouldBeSet(int addressType)
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var evidenceNote = new Note();
            var address = TestFixture.Create<string>();
            var transfererOrganisation = Organisation.CreateRegisteredCompany(TestFixture.Create<string>(), "12345678");
            var transfererAddress = new Address("address1", "address2",
                "town", "county", "postcode",
                new Country(TestFixture.Create<Guid>(), "address name"), "01483676767",
                "email");
            var scheme = A.Fake<Scheme>();
            var recipientOrganisation = A.Fake<Organisation>();

            A.CallTo(() => scheme.SchemeName).Returns(TestFixture.Create<string>());
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Approved);

            transfererOrganisation.AddOrUpdateAddress(Enumeration.FromValue<AddressType>(addressType), transfererAddress);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Schemes, new List<Scheme>() { scheme }, transfererOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.ProducerBalancingScheme, null, transfererOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.BusinessAddress, transfererAddress, transfererOrganisation);
            ObjectInstantiator<Note>.SetProperty(o => o.ApprovedTransfererAddress, address, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.ApprovedTransfererSchemeName, scheme.SchemeName, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Organisation, transfererOrganisation, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Recipient, recipientOrganisation, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.NoteType, Domain.Evidence.NoteType.TransferNote, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.ComplianceYear, currentDate.Year, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Status, NoteStatus.Submitted, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.NoteStatusHistory, new List<NoteStatusHistory>(), evidenceNote);

            var message = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved, "reason passed as parameter");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(evidenceNote);
            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(scheme.SchemeName, transfererOrganisation.OrganisationName,
                transfererAddress.Address1, transfererAddress.Address2, transfererAddress.TownOrCity,
                transfererAddress.CountyOrRegion, transfererAddress.Postcode, null)).Returns(address);

            // Act
            await handler.HandleAsync(message);

            // Assert
            evidenceNote.ApprovedTransfererAddress.Should().Be(address);
            evidenceNote.ApprovedTransfererSchemeName.Should().Be(scheme.SchemeName);
        }

        [Fact]
        public async void HandleAsync_Calls_DeleteZeroTonnageFromSubmittedTransferNote_InTheCorrectOrder()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);

            var evidenceNote = new Note();
            var address = TestFixture.Create<string>();
            var recipient = Organisation.CreateRegisteredCompany(TestFixture.Create<string>(), "12345678");
            var recipientAddress = new Address("address1", "address2",
                "town", "county", "postcode",
                new Country(TestFixture.Create<Guid>(), "address name"), "01483676767",
                "email");
            var scheme = A.Fake<Scheme>();

            A.CallTo(() => scheme.SchemeName).Returns(TestFixture.Create<string>());
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Approved);

            recipient.AddOrUpdateAddress(Enumeration.FromValue<AddressType>(1), recipientAddress);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Schemes, new List<Scheme>() { scheme }, recipient);
            ObjectInstantiator<Organisation>.SetProperty(o => o.ProducerBalancingScheme, null, recipient);
            ObjectInstantiator<Organisation>.SetProperty(o => o.BusinessAddress, recipientAddress, recipient);
            ObjectInstantiator<Note>.SetProperty(o => o.ApprovedRecipientAddress, null, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.ApprovedRecipientSchemeName, null, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Recipient, recipient, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.ComplianceYear, currentDate.Year, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.Status, NoteStatus.Submitted, evidenceNote);
            ObjectInstantiator<Note>.SetProperty(o => o.NoteStatusHistory, new List<NoteStatusHistory>(), evidenceNote);

            var message = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved, "test");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(evidenceNote);
            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(scheme.SchemeName, recipient.OrganisationName,
                recipientAddress.Address1, recipientAddress.Address2, recipientAddress.TownOrCity,
                recipientAddress.CountyOrRegion, recipientAddress.Postcode, null)).Returns(address);

            // Act
            await handler.HandleAsync(message);

            // Assert
            A.CallTo(() => evidenceDataAccess.DeleteZeroTonnageFromSubmittedTransferNote(A<Note>._, A<NoteStatus>._, A<Domain.Evidence.NoteType>._))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_ExternalUser_WithConditionToSetApprovedDetailsAndEvidenceNoteTypeIsEvidenceNote_ApprovedTransfererDetailsShouldNotBeSet()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            var authorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
            var handler = new SetNoteStatusRequestHandler(context, userContext, authorization, systemDataDataAccess, addressUtilities, evidenceDataAccess);
            var recipient = Organisation.CreateRegisteredCompany(TestFixture.Create<string>(), "12345678");
            var recipientAddress = new Address("address1", "address2",
                "town", "county", "postcode",
                new Country(TestFixture.Create<Guid>(), "address name"), "01483676767",
                "email");
            var notificationAddress = new Address("address1", "address2",
                "town", "county", "postcode",
                new Country(TestFixture.Create<Guid>(), "address name"), "01483676767",
                "email");
            ObjectInstantiator<Organisation>.SetProperty(o => o.ProducerBalancingScheme, null, recipient);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Schemes, new List<Scheme>() { scheme }, recipient);
            ObjectInstantiator<Organisation>.SetProperty(o => o.NotificationAddress, notificationAddress, recipient);
            A.CallTo(() => scheme.SchemeName).Returns(TestFixture.Create<string>());
            A.CallTo(() => note.NoteType).Returns(Domain.Evidence.NoteType.EvidenceNote);
            A.CallTo(() => note.Recipient).Returns(recipient);
            A.CallTo(() => note.ComplianceYear).Returns(2020);
            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(scheme.SchemeName, recipient.OrganisationName,
                recipientAddress.Address1, recipientAddress.Address2, recipientAddress.TownOrCity,
                recipientAddress.CountyOrRegion, recipientAddress.Postcode, null)).Returns("address");
            A.CallTo(() => context.Notes.FindAsync(A<Guid>._)).Returns(note);
            var message = new SetNoteStatusRequest(note.Id, Core.AatfEvidence.NoteStatus.Approved, "reason passed as parameter");

            // Act
            await handler.HandleAsync(message);

            // Assert
            note.ApprovedTransfererAddress.Should().BeNullOrEmpty();
            note.ApprovedTransfererSchemeName.Should().BeNullOrEmpty();
            note.ApprovedRecipientAddress.Should().NotBeNullOrEmpty();
            note.ApprovedRecipientSchemeName.Should().NotBeNullOrEmpty();
        }
    }
}
