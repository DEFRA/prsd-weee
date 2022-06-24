namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
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
    using Xunit;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;
    using WasteType = Core.AatfEvidence.WasteType;

    public class GetAatfNotesRequestHandlerTests : SimpleUnitTestBase
    {
        private GetAatfNotesRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly GetAatfNotesRequest request;
        private readonly Organisation organisation;
        private readonly Aatf aatf;
        private readonly WasteType wasteType;
        private readonly NoteStatus noteStatus;
        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private readonly int currentYear;

        public GetAatfNotesRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            noteDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();

            organisation = A.Fake<Organisation>();
            aatf = A.Fake<Aatf>();
            var recipientId = TestFixture.Create<Guid>();
            wasteType = TestFixture.Create<WasteType>();
            noteStatus = TestFixture.Create<NoteStatus>();
            startDate = SystemTime.UtcNow;
            endDate = startDate.AddDays(2);
            currentYear = TestFixture.Create<int>();
            var systemSettings = A.Fake<SystemData>();
            systemSettings.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => organisation.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => aatf.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => aatf.Organisation).Returns(organisation);

            request = new GetAatfNotesRequest(organisation.Id,
                aatf.Id, TestFixture.CreateMany<NoteStatus>().ToList(), TestFixture.Create<string>(), currentYear, recipientId, wasteType, noteStatus, startDate, endDate);

            handler = new GetAatfNotesRequestHandler(weeeAuthorization,
                noteDataAccess,
                mapper);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetAatfNotesRequestHandler(authorization, noteDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetAatfNotesRequest(currentYear)));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            handler = new GetAatfNotesRequestHandler(authorization, noteDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetAatfNotesRequest(currentYear)));

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
        public async void HandleAsync_GivenRequest_EvidenceDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e => 
                e.ComplianceYear.Equals(request.ComplianceYear) && 
                e.OrganisationId.Equals(request.OrganisationId) && 
                e.AatfId.Equals(request.AatfId) && 
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SchemeId == request.RecipientId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithSearchRef_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var searchRef = TestFixture.Create<string>();
            var request = GetAatfNotesRequest(SystemTime.UtcNow.Year, searchRef);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SchemeId == null &&
                e.SearchRef.Equals(searchRef)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithRecipientFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            Guid? recipientId = Guid.NewGuid();
            var request = GetAatfNotesRequest(SystemTime.UtcNow.Year, null, recipientId);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SearchRef == null &&
                e.SchemeId.Equals(recipientId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithWasteTypeFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            WasteType? wasteType = TestFixture.Create<WasteType?>();
            var request = GetAatfNotesRequest(currentYear, null, null, wasteType);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SearchRef == null && 
                e.SchemeId == null && e.WasteTypeId.Equals((int?)wasteType) &&
                e.NoteTypeFilter.Contains(NoteType.EvidenceNote) &&
                e.NoteTypeFilter.Count == 1))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithNoteStatusFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            NoteStatus? noteStatus = TestFixture.Create<NoteStatus?>();
            var request = GetAatfNotesRequest(currentYear, null, null, null, noteStatus);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SearchRef == null &&
                e.SchemeId == null && 
                e.WasteTypeId == null && 
                e.NoteStatusId.Equals((int?)noteStatus)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithStartDateFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var startDate = TestFixture.Create<DateTime?>();
            var request = GetAatfNotesRequest(currentYear, null, null, null, null, startDate);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SearchRef == null &&
                e.SchemeId == null &&
                e.WasteTypeId == null &&
                e.NoteStatusId == null &&
                e.StartDateSubmitted.Equals(startDate)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithEndDateFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var endDate = TestFixture.Create<DateTime?>();
            var request = GetAatfNotesRequest(currentYear, null, null, null, null, null, endDate);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SearchRef == null &&
                e.SchemeId == null &&
                e.WasteTypeId == null &&
                e.NoteStatusId == null &&
                e.StartDateSubmitted == null &&
                e.EndDateSubmitted.Equals(endDate)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestAllFiltersSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var searchRef = TestFixture.Create<string>();
            var receivedId = Guid.NewGuid();
            var request = GetAatfNotesRequest(currentYear, searchRef, receivedId, wasteType, noteStatus, startDate, endDate);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.SearchRef.Equals(searchRef) &&
                e.SchemeId.Equals(receivedId) &&
                e.WasteTypeId.Equals((int?)wasteType) &&
                e.NoteStatusId.Equals((int?)noteStatus) &&
                e.StartDateSubmitted.Equals(startDate) &&
                e.EndDateSubmitted.Equals(endDate)))).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteList);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>.That.Matches(a => 
                a.ListOfNotes.ElementAt(0).Reference.Equals(6) &&
                a.ListOfNotes.ElementAt(1).Reference.Equals(2) &&
                a.ListOfNotes.ElementAt(2).Reference.Equals(4) &&
                a.ListOfNotes.Count.Equals(3)))).MustHaveHappenedOnceExactly();

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenMappedEvidenceNoteData_ListEvidenceNoteDataShouldBeReturn()
        {
            // arrange
            var noteList = TestFixture.CreateMany<Note>().ToList();

            var evidenceNoteDatas = new List<EvidenceNoteData>()
            {
                A.Fake<EvidenceNoteData>(),
                A.Fake<EvidenceNoteData>()
            };

            var listOfEvidenceNotes = new ListOfEvidenceNoteDataMap() { ListOfEvidenceNoteData = evidenceNoteDatas };

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteList);

            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).Returns(listOfEvidenceNotes);

            // act
            var result = await handler.HandleAsync(GetAatfNotesRequest(currentYear));

            // assert
            result.Should().BeOfType<List<EvidenceNoteData>>();
            result.Count().Should().Be(evidenceNoteDatas.Count);

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).MustHaveHappenedOnceExactly();
        }

        private GetAatfNotesRequest GetAatfNotesRequest(int selectedComplianceYear, string searchRef = null, Guid? receivedId = null, WasteType? wasteType = null, NoteStatus? noteStatus = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            return new GetAatfNotesRequest(organisation.Id, 
                aatf.Id, 
                TestFixture.CreateMany<NoteStatus>().ToList(),
                searchRef, selectedComplianceYear, receivedId, wasteType, noteStatus, startDate, endDate);
        }
    }
}
