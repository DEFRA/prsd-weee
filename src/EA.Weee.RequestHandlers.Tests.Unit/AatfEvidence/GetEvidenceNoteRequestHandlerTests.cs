namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Castle.Core.Internal;
    using Core.AatfEvidence;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using RequestHandlers.Aatf;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetEvidenceNoteRequestHandlerTests
    {
        private GetEvidenceNoteRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly GetEvidenceNoteForAatfRequest request;
        private readonly Note note;
        private readonly Guid evidenceNoteId;
        private readonly Guid organisationId;

        public GetEvidenceNoteRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();

            A.Fake<IAatfDataAccess>();
            A.Fake<IUserContext>();

            A.Fake<Organisation>();
            A.Fake<Aatf>();
            A.Fake<Scheme>();
            note = A.Fake<Note>();
            fixture.Create<Guid>();
            evidenceNoteId = fixture.Create<Guid>();
            organisationId = fixture.Create<Guid>();

            A.CallTo(() => note.OrganisationId).Returns(organisationId);

            request = new GetEvidenceNoteForAatfRequest(evidenceNoteId);

            handler = new GetEvidenceNoteRequestHandler(weeeAuthorization,
                evidenceDataAccess,
                mapper);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetEvidenceNoteRequestHandler(authorization,
                evidenceDataAccess,
                mapper);

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
           
            handler = new GetEvidenceNoteRequestHandler(authorization,
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
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>.That.Matches(e => e.Note.Equals(note) && e.CategoryFilter.IsNullOrEmpty() && e.IncludeTonnage == true))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNote_MappedNoteShouldBeReturned()
        {
            //arrange
            var evidenceNote = fixture.Create<EvidenceNoteData>();

            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>._)).Returns(evidenceNote);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(evidenceNote);
        }
    }
}
