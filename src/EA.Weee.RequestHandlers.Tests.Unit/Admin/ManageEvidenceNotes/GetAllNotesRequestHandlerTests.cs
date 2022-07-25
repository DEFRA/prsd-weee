﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes.Requests
{
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
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
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetAllNotesRequestHandlerTests : SimpleUnitTestBase
    {
        private GetAllNotesInternalRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly GetAllNotesInternal message;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly GetAllNotesInternal messageWithEmptyNoteInternalTypeList;

        public GetAllNotesRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            noteDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            message = new GetAllNotesInternal(TestFixture.CreateMany<NoteType>().ToList(), TestFixture.CreateMany<NoteStatus>().ToList());
            messageWithEmptyNoteInternalTypeList = new GetAllNotesInternal(new List<NoteType>(), TestFixture.CreateMany<NoteStatus>().ToList());

            handler = new GetAllNotesInternalRequestHandler(weeeAuthorization, noteDataAccess, mapper, systemDataDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoInternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetAllNotesInternalRequestHandler(authorization, noteDataAccess, mapper, systemDataDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetAllNotes()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCallEnsureCanAccessInternalArea()
        {
            //act
            await handler.HandleAsync(message);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_NoteDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(message);

            var allowedStatuses = message.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            var noteTypeFilter = message.NoteTypeFilterList
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteType>()).ToList();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.AllowedStatuses.SequenceEqual(allowedStatuses) &&
                e.NoteTypeFilter.SequenceEqual(noteTypeFilter) &&
                e.PageNumber == 1 &&
                e.PageSize == int.MaxValue))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithEmptyNoteType_NoteDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(messageWithEmptyNoteInternalTypeList);

            var allowedStatuses = messageWithEmptyNoteInternalTypeList.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            var noteTypeFilter = new List<Domain.Evidence.NoteType>();

            // assert
            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>.That.Matches(e =>
                e.AllowedStatuses.SequenceEqual(allowedStatuses) &&
                e.NoteTypeFilter.SequenceEqual(noteTypeFilter) &&
                e.PageSize == int.MaxValue &&
                e.PageNumber == 1))).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteData);

            // act
            await handler.HandleAsync(message);

            // assert
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>.That.Matches(a =>
                a.ListOfNotes.ElementAt(0).Reference.Equals(6) &&
                a.ListOfNotes.ElementAt(1).Reference.Equals(2) &&
                a.ListOfNotes.ElementAt(2).Reference.Equals(4) &&
                a.ListOfNotes.Count.Equals(3) &&
                a.IncludeTonnage == false))).MustHaveHappenedOnceExactly();

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
            var noteData = new EvidenceNoteResults(noteList, noteList.Count);

            var listOfEvidenceNotes = new ListOfEvidenceNoteDataMap() { ListOfEvidenceNoteData = evidenceNoteDatas };

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).Returns(noteData);

            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).Returns(listOfEvidenceNotes);

            // act
            var result = await handler.HandleAsync(GetAllNotes());

            // assert
            result.Should().BeOfType<EvidenceNoteSearchDataResult>();
            result.NoteCount.Should().Be(evidenceNoteDatas.Count);
            result.Results.Should().BeEquivalentTo(listOfEvidenceNotes.ListOfEvidenceNoteData);

            A.CallTo(() => noteDataAccess.GetAllNotes(A<NoteFilter>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).MustHaveHappenedOnceExactly();
        }

        private GetAllNotesInternal GetAllNotes()
        {
            return new GetAllNotesInternal(TestFixture.CreateMany<NoteType>().ToList(), TestFixture.CreateMany<NoteStatus>().ToList());    
        }
    }
}
