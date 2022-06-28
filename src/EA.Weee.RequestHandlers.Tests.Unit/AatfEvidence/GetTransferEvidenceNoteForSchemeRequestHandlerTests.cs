namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfEvidence;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetTransferEvidenceNoteForSchemeRequestHandlerTests
    {
        private readonly GetTransferEvidenceNoteForSchemeRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly IMapper mapper;
        private readonly GetTransferEvidenceNoteForSchemeRequest request;
        private readonly Note note;
        private readonly Scheme scheme;
        private readonly Guid evidenceNoteId;
        private readonly Guid organisationId;

        public GetTransferEvidenceNoteForSchemeRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();

            mapper = A.Fake<IMapper>();
            note = A.Fake<Note>();
            scheme = A.Fake<Scheme>();
            fixture.Create<Guid>();
            evidenceNoteId = fixture.Create<Guid>();
            organisationId = fixture.Create<Guid>();

            A.CallTo(() => note.OrganisationId).Returns(organisationId);
            A.CallTo(() => weeeAuthorization.CheckSchemeAccess(A<Guid>._)).Returns(true);

            request = new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId);

            handler = new GetTransferEvidenceNoteForSchemeRequestHandler(weeeAuthorization, evidenceDataAccess, mapper, schemeDataAccess);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckSchemeAccess()
        {
            //arrange
            var schemeId = fixture.Create<Guid>();
            A.CallTo(() => note.RecipientId).Returns(schemeId);
            
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.CheckSchemeAccess(schemeId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(organisationId))
                .MustHaveHappenedOnceExactly();
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
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferNote_SchemeShouldBeRetrieved()
        {
            //arrange
            var organisationId = fixture.Create<Guid>();
            A.CallTo(() => note.OrganisationId).Returns(organisationId);
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(organisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenTransferNoteAndSchemeIsNull_ArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(A<Guid>._)).Returns((Scheme)null);

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
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(A<Guid>._)).Returns(scheme);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(A<TransferNoteMapTransfer>.That.Matches(t => 
                t.Note.Equals(note) && t.Scheme.Equals(scheme)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNote_MappedNoteShouldBeReturned()
        {
            //arrange
            var evidenceNote = fixture.Create<TransferEvidenceNoteData>();

            A.CallTo(() => mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(A<TransferNoteMapTransfer>._)).Returns(evidenceNote);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(evidenceNote);
        }
    }
}