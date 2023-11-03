﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
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
        private readonly List<WasteType> wasteType;
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
            wasteType = TestFixture.Create<List<WasteType>>();
            noteStatus = TestFixture.Create<NoteStatus>();
            startDate = SystemTime.UtcNow;
            endDate = startDate.AddDays(2);
            currentYear = TestFixture.Create<int>();
            var systemSettings = A.Fake<SystemData>();
            systemSettings.ToggleFixedCurrentDateUsage(false);

            var allowedWasteType = new List<WasteType>() { WasteType.Household };

            A.CallTo(() => organisation.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => aatf.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => aatf.Organisation).Returns(organisation);

            request = new GetAatfNotesRequest(organisation.Id,
                aatf.Id, TestFixture.CreateMany<NoteStatus>().ToList(), TestFixture.Create<string>(), currentYear, recipientId, allowedWasteType, noteStatus, startDate, endDate, int.MaxValue, 1);

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
                e.RecipientId == request.RecipientId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithSearchRef_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var searchRef = TestFixture.Create<string>();
            var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();
            var request = GetAatfNotesRequest(SystemTime.UtcNow.Year, searchRef, null, wasteTypeList);

            // act
            await handler.HandleAsync(request);

            var status = request.AllowedStatuses.Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.OrganisationId.Equals(request.OrganisationId) &&
                e.AatfId.Equals(request.AatfId) &&
                e.AllowedStatuses.SequenceEqual(status) &&
                e.RecipientId == null &&
                e.SearchRef.Equals(searchRef)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithRecipientFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            Guid? recipientId = Guid.NewGuid();
            var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();
            var request = GetAatfNotesRequest(SystemTime.UtcNow.Year, null, recipientId, wasteTypeList);

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
                e.RecipientId.Equals(recipientId)))).MustHaveHappenedOnceExactly();
        }

        //[Fact]
        //public async void HandleAsync_GivenRequestWithWasteTypeFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        //{
        //    //arrange
        //    //WasteType? wasteType = TestFixture.Create<WasteType?>();
        //    var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();
        //    var allowedWasteType = new List<WasteType>() { WasteType.Household };

        //    var request = GetAatfNotesRequest(currentYear, null, null, wasteTypeList);

        //    // act
        //    await handler.HandleAsync(request);

        //    var status = request.AllowedStatuses
        //        .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

        //    // assert
        //    A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
        //        e.OrganisationId.Equals(request.OrganisationId) &&
        //        e.AatfId.Equals(request.AatfId) &&
        //        e.AllowedStatuses.SequenceEqual(status) &&
        //        e.SearchRef == null &&
        //        e.RecipientId == null && e.WasteTypeId.Equals(1) &&
        //        e.NoteTypeFilter.Contains(NoteType.EvidenceNote) &&
        //        e.NoteTypeFilter.Count == 1))).MustHaveHappenedOnceExactly();
        //}

        [Fact]
        public async void HandleAsync_GivenRequestWithNoteStatusFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            NoteStatus? noteStatus = TestFixture.Create<NoteStatus?>();
            var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();
            var request = GetAatfNotesRequest(currentYear, null, null, wasteTypeList, noteStatus);

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
                e.RecipientId == null &&
                e.WasteTypeId == null &&
                e.NoteStatusId.Equals((int?)noteStatus)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithStartDateFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var startDate = TestFixture.Create<DateTime?>();
            var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();
            var request = GetAatfNotesRequest(currentYear, null, null, wasteTypeList, null, startDate);

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
                e.RecipientId == null &&
                e.WasteTypeId == null &&
                e.NoteStatusId == null &&
                e.StartDateSubmitted.Equals(startDate)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithEndDateFilterSet_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var endDate = TestFixture.Create<DateTime?>();
            var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();
            var request = GetAatfNotesRequest(currentYear, null, null, wasteTypeList, null, null, endDate);

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
                e.RecipientId == null &&
                e.WasteTypeId == null &&
                e.NoteStatusId == null &&
                e.StartDateSubmitted == null &&
                e.EndDateSubmitted.Equals(endDate)))).MustHaveHappenedOnceExactly();
        }

        //[Fact]
        //public async void HandleAsync_GivenRequestAllFiltersSet_EvidenceDataAccessShouldBeCalledOnce()
        //{
        //    //arrange
        //    var searchRef = TestFixture.Create<string>();
        //    var receivedId = Guid.NewGuid();
        //    var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();
        //    var noteTypeList = TestFixture.CreateMany<NoteType>(2).ToList();
        //    var request = GetAatfNotesRequest(currentYear, searchRef, receivedId, wasteTypeList, noteStatus, startDate, endDate);

        //    // act
        //    await handler.HandleAsync(request);

        //    var status = request.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();            

        //    // assert
        //    A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
        //        e.OrganisationId.Equals(request.OrganisationId) &&
        //        e.AatfId.Equals(request.AatfId) &&
        //        e.AllowedStatuses.Equals(status) &&
        //        e.WasteTypeFilter.Equals(wasteTypeList) &&
        //        e.SearchRef.Equals(searchRef) &&
        //        e.RecipientId.Equals(receivedId) &&
        //        e.WasteTypeId.Equals(wasteType) &&
        //        e.NoteStatusId.Equals((int?)noteStatus) &&
        //        e.StartDateSubmitted.Equals(startDate) &&
        //        e.EndDateSubmitted.Equals(endDate)))).MustHaveHappenedOnceExactly();
        //}

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

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteData);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<List<Note>, List<EvidenceNoteData>>(A<List<Note>>
                .That.IsSameSequenceAs(noteList.OrderByDescending(n => n.CreatedDate)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenMappedEvidenceNoteData_ListEvidenceNoteDataShouldBeReturn()
        {
            // arrange
            var noteList = TestFixture.CreateMany<Note>(2).ToList();
            var wasteTypeList = TestFixture.CreateMany<WasteType>(2).ToList();

            var evidenceNoteDatas = new List<EvidenceNoteData>()
            {
                A.Fake<EvidenceNoteData>(),
                A.Fake<EvidenceNoteData>()
            };
            var noteData = new EvidenceNoteResults(noteList, noteList.Count);

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteData);

            A.CallTo(() => mapper.Map<List<Note>, List<EvidenceNoteData>>(A<List<Note>>._)).Returns(evidenceNoteDatas);

            // act
            var result = await handler.HandleAsync(GetAatfNotesRequest(currentYear, null, null, wasteTypeList));

            // assert
            result.Should().BeOfType<EvidenceNoteSearchDataResult>();
            result.NoteCount.Should().Be(evidenceNoteDatas.Count);
            result.Results.ToList().Should().BeEquivalentTo(evidenceNoteDatas);
        }

        private GetAatfNotesRequest GetAatfNotesRequest(int selectedComplianceYear, string searchRef = null, Guid? receivedId = null,
                                                        List<WasteType> wasteType = null, NoteStatus? noteStatus = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return new GetAatfNotesRequest(organisation.Id, aatf.Id, TestFixture.CreateMany<NoteStatus>().ToList(), searchRef, selectedComplianceYear,
                                           receivedId, wasteType, noteStatus, startDate, endDate, int.MaxValue, 1);
        }
    }
}
