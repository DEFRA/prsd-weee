﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfEvidence;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GGetEvidenceNotesSelectedForTransferRequestHandlerTests : SimpleUnitTestBase
    {
        private GetEvidenceNotesSelectedForTransferRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMapper mapper;
        private readonly GetEvidenceNotesSelectedForTransferRequest request;
        private readonly Organisation organisation;
        private readonly Guid organisationId;

        public GGetEvidenceNotesSelectedForTransferRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            mapper = A.Fake<IMapper>();
            TestFixture.Create<Guid>();
            organisation = A.Fake<Organisation>();
            organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => organisationDataAccess.GetById(organisationId)).Returns(organisation);

            request = new GetEvidenceNotesSelectedForTransferRequest(organisationId, TestFixture.CreateMany<Guid>().ToList(), TestFixture.CreateMany<int>().ToList());

            handler = new GetEvidenceNotesSelectedForTransferRequestHandler(weeeAuthorization, evidenceDataAccess, mapper, organisationDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetEvidenceNotesSelectedForTransferRequestHandler(authorization, evidenceDataAccess, mapper, organisationDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoBalancingSchemeAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
           
            handler = new GetEvidenceNotesSelectedForTransferRequestHandler(authorization, evidenceDataAccess, mapper, organisationDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckExternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckBalancingSchemeAccess()
        {
            //arrange
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisationDataAccess.GetById(A<Guid>._)).Returns(organisation);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_OrganisationDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => organisationDataAccess.GetById(request.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithEvidenceNotesAndCategories_DataAccessGetTransferSelectedNotesShouldBeCalled()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() =>
                    evidenceDataAccess.GetTransferSelectedNotes(organisationId, 
                        A<List<Guid>>.That.IsSameSequenceAs(request.EvidenceNotes.Select(w => (Guid)w).ToList()),
                        A<List<int>>.That.IsSameSequenceAs(request.Categories))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenEvidenceNotesData_ReturnedNotesDataShouldBeMapped()
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

            var evidenceNoteResults = new EvidenceNoteResults(noteList, 3);

            A.CallTo(() => evidenceDataAccess.GetTransferSelectedNotes(A<Guid>._, A<List<Guid>>._, A<List<int>>._)).Returns(evidenceNoteResults);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMapper>
                .That.Matches(e => e.Note.Equals(note1) &&
                                   e.CategoryFilter.Equals(request.Categories) &&
                                   e.IncludeTonnage == true &&
                                   e.IncludeHistory == false &&
                                   e.IncludeTotal == true))).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMapper>
                .That.Matches(e => e.Note.Equals(note2) &&
                                   e.CategoryFilter.Equals(request.Categories) &&
                                   e.IncludeTonnage == true &&
                                   e.IncludeHistory == false &&
                                   e.IncludeTotal == true))).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMapper>
                .That.Matches(e => e.Note.Equals(note3) &&
                                   e.CategoryFilter.SequenceEqual(request.Categories) &&
                                   e.IncludeTonnage == true &&
                                   e.IncludeHistory == false &&
                                   e.IncludeTotal == true))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMappedEvidenceNoteData_ListEvidenceNoteDataShouldBeReturn()
        {
            // arrange
            var noteList = TestFixture.CreateMany<Note>(2).ToList();

            var noteData = new List<EvidenceNoteData>()
            {
                A.Fake<EvidenceNoteData>(),
                A.Fake<EvidenceNoteData>()
            };

            var evidenceNoteResults = new EvidenceNoteResults(noteList, 2);

            A.CallTo(() => evidenceDataAccess.GetTransferSelectedNotes(A<Guid>._, A<List<Guid>>._, A<List<int>>._)).Returns(evidenceNoteResults);

            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMapper>._))
                .ReturnsNextFromSequence(noteData.ElementAt(0), noteData.ElementAt(1));

            // act
            var result = await handler.HandleAsync(request);

            // assert
            result.Results.Count.Should().Be(2);
            result.Results.ElementAt(0).Should().Be(noteData.ElementAt(0));
            result.Results.ElementAt(1).Should().Be(noteData.ElementAt(1));
            result.NoteCount.Should().Be(2);
        }

        [Fact]
        public async Task HandleAsync_GivenTransferNoteId_ReturnedNotesDataShouldBeMapped()
        {
            // arrange
            var noteList = TestFixture.CreateMany<Note>(2).ToList();

            var noteData = new List<EvidenceNoteData>()
            {
                A.Fake<EvidenceNoteData>(),
                A.Fake<EvidenceNoteData>()
            };

            var evidenceNoteResults = new EvidenceNoteResults(noteList, 2);

            A.CallTo(() => evidenceDataAccess.GetTransferSelectedNotes(A<Guid>._, A<List<Guid>>._, A<List<int>>._)).Returns(evidenceNoteResults);

            // act
            var result = await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMapper>
            .That.Matches(e => e.TransferNoteId.Equals(request.TransferNoteId)))).MustHaveHappened(noteList.Count, Times.Exactly);
        }
    }
}
