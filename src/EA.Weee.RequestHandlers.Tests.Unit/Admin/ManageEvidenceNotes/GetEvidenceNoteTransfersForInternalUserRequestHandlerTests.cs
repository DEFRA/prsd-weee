namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
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
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly GetEvidenceNoteTransfersForInternalUserRequest request;
        private readonly Note note;
        private readonly Guid evidenceNoteId;

        public GetEvidenceNoteTransfersForInternalUserRequestHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();
            note = A.Fake<Note>();
            evidenceNoteId = TestFixture.Create<Guid>();

            request = new GetEvidenceNoteTransfersForInternalUserRequest(evidenceNoteId);

            handler = new GetEvidenceNoteTransfersForInternalUserRequestHandler(weeeAuthorization,
                evidenceDataAccess,
                mapper);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetEvidenceNoteTransfersForInternalUserRequestHandler(authorization,
              evidenceDataAccess,
              mapper);

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
        public async Task HandleAsync_GivenRequest_NoteShouldBeMapped()
        {
            //arrange
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(
                A<TransferNoteMapTransfer>.That.Matches(t => t.Note.Equals(note)))).MustHaveHappenedOnceExactly();
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
