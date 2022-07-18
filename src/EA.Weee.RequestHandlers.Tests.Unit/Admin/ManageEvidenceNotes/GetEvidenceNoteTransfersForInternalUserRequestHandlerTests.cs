namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.Scheme;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GetEvidenceNoteTransfersForInternalUserRequestHandlerTests : SimpleUnitTestBase
    {
        private GetEvidenceNoteTransfersForInternalUserRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly GetEvidenceNoteTransfersForInternalUserRequest request;
        private readonly Note note;
        private readonly Scheme scheme;
        private readonly Guid evidenceNoteId;
        private readonly Guid organisationId;

        public GetEvidenceNoteTransfersForInternalUserRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();

            A.Fake<IUserContext>();

            note = A.Fake<Note>();
            scheme = A.Fake<Scheme>();

            evidenceNoteId = TestFixture.Create<Guid>();
            organisationId = TestFixture.Create<Guid>();

            request = new GetEvidenceNoteTransfersForInternalUserRequest(evidenceNoteId);

            handler = new GetEvidenceNoteTransfersForInternalUserRequestHandler(weeeAuthorization,
                evidenceDataAccess,
                mapper,
                schemeDataAccess);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetEvidenceNoteTransfersForInternalUserRequestHandler(authorization,
              evidenceDataAccess,
              mapper,
              schemeDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_TransferEvidenceNoteShouldBeRetrieved()
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
            A.CallTo(() => note.OrganisationId).Returns(organisationId);
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => weeeAuthorization.CheckOrganisationAccess(organisationId)).Returns(true);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(organisationId)).Returns(scheme);

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
            var transferNote = TestFixture.Create<TransferEvidenceNoteData>();
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(A<TransferNoteMapTransfer>._)).Returns(transferNote);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(transferNote);
        }
    }
}
