namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Castle.Core.Internal;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class GetEvidenceNoteForInternalUserRequestHandlerTests : SimpleUnitTestBase
    {
        private GetEvidenceNoteForInternalUserRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly GetEvidenceNoteForInternalUserRequest request;
        private readonly Note note;
        private readonly Guid evidenceNoteId;

        public GetEvidenceNoteForInternalUserRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();

            A.Fake<IUserContext>();

            note = A.Fake<Note>();
            evidenceNoteId = TestFixture.Create<Guid>();

            request = new GetEvidenceNoteForInternalUserRequest(evidenceNoteId);

            handler = new GetEvidenceNoteForInternalUserRequestHandler(weeeAuthorization,
                evidenceDataAccess,
                mapper);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetEvidenceNoteForInternalUserRequestHandler(authorization,
                evidenceDataAccess,
                mapper);

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
        public async Task HandleAsync_GivenRequest_NoteShouldBeMapped()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>.That.Matches(e => e.Note.Equals(note) && e.CategoryFilter.IsNullOrEmpty()))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNote_MappedNoteShouldBeReturned()
        {
            //arrange
            var evidenceNote = TestFixture.Create<EvidenceNoteData>();

            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>._)).Returns(evidenceNote);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(evidenceNote);
        }
    }
}
