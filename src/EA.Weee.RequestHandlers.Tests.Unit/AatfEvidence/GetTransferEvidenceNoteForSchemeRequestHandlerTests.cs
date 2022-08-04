namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfEvidence;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class GetTransferEvidenceNoteForSchemeRequestHandlerTests
    {
        private readonly GetTransferEvidenceNoteForSchemeRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMapper mapper;
        private readonly GetTransferEvidenceNoteForSchemeRequest request;
        private readonly Note note;
        private readonly Organisation organisation;
        private readonly Organisation recipientOrganisation;
        private readonly Scheme recipientScheme;
        private readonly Guid evidenceNoteId;
        private readonly Guid organisationId;
        private readonly Guid recipientSchemeId;

        public GetTransferEvidenceNoteForSchemeRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();

            mapper = A.Fake<IMapper>();
            note = A.Fake<Note>();
            evidenceNoteId = fixture.Create<Guid>();
            organisationId = fixture.Create<Guid>();
            recipientSchemeId = fixture.Create<Guid>();
            organisation = A.Fake<Organisation>();
            recipientScheme = A.Fake<Scheme>();
            recipientOrganisation = A.Fake<Organisation>();

            A.CallTo(() => recipientScheme.Id).Returns(recipientSchemeId);
            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { recipientScheme });
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => note.OrganisationId).Returns(organisationId);
            A.CallTo(() => note.Recipient).Returns(recipientOrganisation);
            A.CallTo(() => weeeAuthorization.CheckSchemeAccess(A<Guid>._)).Returns(true);

            request = new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId);

            handler = new GetTransferEvidenceNoteForSchemeRequestHandler(weeeAuthorization, evidenceDataAccess, mapper, organisationDataAccess);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldEnsureCanAccessExternalArea()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_EnsureOrganisationAccess()
        {
            A.CallTo(() => organisationDataAccess.GetById(A<Guid>._)).Returns(organisation);
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(organisation.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_EnsureSchemeAccess()
        {
            A.CallTo(() => organisationDataAccess.GetById(A<Guid>._)).Returns(organisation);
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.CheckSchemeAccess(note.Recipient.Scheme.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationOrSchemeAccess_ShouldThrowSecurityException()
        {
            //arrange
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(A<Guid>._)).Returns(false);
            A.CallTo(() => weeeAuthorization.CheckSchemeAccess(A<Guid>._)).Returns(false);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationButNoSchemeAccess_ShouldNotThrowSecurityException()
        {
            //arrange
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(A<Guid>._)).Returns(false);
            A.CallTo(() => weeeAuthorization.CheckSchemeAccess(A<Guid>._)).Returns(true);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_GivenNoSchemeAccessButOrganisationAccess_ShouldNotThrowSecurityException()
        {
            //arrange
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(A<Guid>._)).Returns(true);
            A.CallTo(() => weeeAuthorization.CheckSchemeAccess(A<Guid>._)).Returns(false);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_EvidenceNoteShouldBeRetrieved()
        {
            //arrange
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(organisationId)).Returns(true);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferNote_OrganisationShouldBeRetrieved()
        {
            //arrange
            A.CallTo(() => note.OrganisationId).Returns(organisationId);
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(organisationId)).Returns(true);
            A.CallTo(() => organisationDataAccess.GetById(organisationId)).Returns(organisation);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => organisationDataAccess.GetById(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationIsNull_ArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => organisationDataAccess.GetById(A<Guid>._)).Returns((Organisation)null);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_NoteShouldBeMapped()
        {
            //arrange
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(A<TransferNoteMapTransfer>.That.Matches(t => t.Note.Equals(note)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNote_MappedNoteShouldBeReturned()
        {
            //arrange
            var evidenceNote = fixture.Create<TransferEvidenceNoteData>();
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(A<TransferNoteMapTransfer>._)).Returns(evidenceNote);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(evidenceNote);
        }
    }
}