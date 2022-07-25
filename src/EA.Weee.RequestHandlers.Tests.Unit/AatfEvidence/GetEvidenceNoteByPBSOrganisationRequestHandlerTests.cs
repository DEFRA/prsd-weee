namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.AatfEvidence;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using Core.Helpers;
    using Domain.Scheme;
    using Xunit;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetEvidenceNoteByPbsOrganisationRequestHandlerTests : SimpleUnitTestBase
    {
        private GetEvidenceNoteByPbsOrganisationRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly Guid organisationId;
        private readonly GetEvidenceNoteByPbsOrganisationRequest request;

        public GetEvidenceNoteByPbsOrganisationRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();

            organisationId = Guid.NewGuid();

            request = new GetEvidenceNoteByPbsOrganisationRequest(organisationId, TestFixture.CreateMany<NoteStatus>().ToList(), TestFixture.Create<short>(), new List<NoteType>() { NoteType.Evidence }, false);

            handler = new GetEvidenceNoteByPbsOrganisationRequestHandler(weeeAuthorization,
                evidenceDataAccess,
                mapper);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            handler = new GetEvidenceNoteByPbsOrganisationRequestHandler(authorization, evidenceDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetEvidenceNoteByPBSOrganisationRequest()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            handler = new GetEvidenceNoteByPbsOrganisationRequestHandler(authorization, evidenceDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetEvidenceNoteByPBSOrganisationRequest()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldExternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList();

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => evidenceDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e => 
                                                              e.AllowedStatuses.SequenceEqual(status) &&
                                                              e.ComplianceYear == request.ComplianceYear &&
                                                              e.NoteTypeFilter.Contains(Domain.Evidence.NoteType.EvidenceNote) &&
                                                              e.NoteTypeFilter.Count == 1 && 
                                                              e.OrganisationId == organisationId &&
                                                              e.PageNumber == 1 &&
                                                              e.PageSize == int.MaxValue))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenTransferredOutRequest_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var scheme = A.Fake<Scheme>();
            var schemeId = TestFixture.Create<Guid>();
            var request = new GetEvidenceNoteByPbsOrganisationRequest(organisationId, TestFixture.CreateMany<NoteStatus>().ToList(), TestFixture.Create<short>(), new List<NoteType>() { NoteType.Transfer }, true);

            A.CallTo(() => scheme.Id).Returns(schemeId);
            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList();

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => evidenceDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.RecipientId == null &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.AatfId == null &&
                e.ComplianceYear == request.ComplianceYear &&
                e.NoteTypeFilter.Contains(Domain.Evidence.NoteType.TransferNote) &&
                e.NoteTypeFilter.Count == 1 &&
                e.OrganisationId == request.OrganisationId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenNotesData_ReturnedNotesDataShouldBeMapped()
        {
            // arrange
            var note1 = A.Fake<Note>();
            var note2 = A.Fake<Note>();
            var note3 = A.Fake<Note>();

            A.CallTo(() => note1.Reference).Returns(2);
            A.CallTo(() => note1.CreatedDate).Returns(DateTime.Now.AddDays(1));
            A.CallTo(() => note2.Reference).Returns(4);
            A.CallTo(() => note2.CreatedDate).Returns(DateTime.Now);
            A.CallTo(() => note3.Reference).Returns(6);
            A.CallTo(() => note3.CreatedDate).Returns(DateTime.Now.AddDays(2));

            var noteList = new List<Note>()
            {
                note1,
                note2,
                note3
            };
            var noteData = new EvidenceNoteResults(noteList, noteList.Count);

            A.CallTo(() => evidenceDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteData);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>.That.Matches(a =>
                a.ListOfNotes.ElementAt(0).Reference.Equals(6) &&
                a.ListOfNotes.ElementAt(1).Reference.Equals(2) &&
                a.ListOfNotes.ElementAt(2).Reference.Equals(4) &&
                a.ListOfNotes.Count.Equals(3) &&
                a.CategoryFilter.IsNullOrEmpty() &&
                a.IncludeTonnage == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenMappedEvidenceNoteData_ListEvidenceNoteDataShouldBeReturn()
        {
            // arrange
            var noteList = TestFixture.CreateMany<Note>().ToList();
            var noteData = new EvidenceNoteResults(noteList, noteList.Count);

            var mappedNoteData = new List<EvidenceNoteData>()
            {
                A.Fake<EvidenceNoteData>(),
                A.Fake<EvidenceNoteData>()
            };

            var listOfEvidenceNotes = new ListOfEvidenceNoteDataMap() { ListOfEvidenceNoteData = mappedNoteData };

            A.CallTo(() => evidenceDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteData);

            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).Returns(listOfEvidenceNotes);

            // act
            var result = await handler.HandleAsync(request);

            // assert
            result.Results.Should().BeEquivalentTo(mappedNoteData);
            result.NoteCount.Should().Be(noteList.Count);
        }

        private GetEvidenceNoteByPbsOrganisationRequest GetEvidenceNoteByPBSOrganisationRequest()
        {
            return new GetEvidenceNoteByPbsOrganisationRequest(organisationId, TestFixture.CreateMany<NoteStatus>().ToList(), TestFixture.Create<short>(), new List<NoteType>() { NoteType.Evidence }, false);
        }
    }
}
