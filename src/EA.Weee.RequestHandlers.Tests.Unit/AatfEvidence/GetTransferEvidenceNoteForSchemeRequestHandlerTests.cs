namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfEvidence;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetTransferEvidenceNoteForSchemeRequestHandlerTests
    {
        private GetTransferEvidenceNoteForSchemeRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly GetTransferEvidenceNoteForSchemeRequest request;
        private readonly Note note;
        private readonly Guid evidenceNoteId;
        private readonly Guid organisationId;

        public GetTransferEvidenceNoteForSchemeRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();
            note = A.Fake<Note>();
            fixture.Create<Guid>();
            evidenceNoteId = fixture.Create<Guid>();
            organisationId = fixture.Create<Guid>();

            A.CallTo(() => note.OrganisationId).Returns(organisationId);

            request = new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId);

            handler = new GetTransferEvidenceNoteForSchemeRequestHandler(weeeAuthorization, evidenceDataAccess, mapper);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetTransferEvidenceNoteForSchemeRequestHandler(authorization, evidenceDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new GetTransferEvidenceNoteForSchemeRequestHandler(authorization, evidenceDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
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
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(organisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_NoteShouldBeMapped()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<Note, TransferEvidenceNoteData>(note)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNote_MappedNoteShouldBeReturned()
        {
            //arrange
            var evidenceNote = fixture.Create<TransferEvidenceNoteData>();

            A.CallTo(() => mapper.Map<Note, TransferEvidenceNoteData>(A<Note>._)).Returns(evidenceNote);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(evidenceNote);
        }
    }
}