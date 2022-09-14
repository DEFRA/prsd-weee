﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceNotesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper;

        private readonly TransferEvidenceNotesViewModelMap map;

        public TransferEvidenceNotesViewModelMapTests()
        {
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            transferNoteMapper = A.Fake<IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel>>();

            map = new TransferEvidenceNotesViewModelMap(mapper, cache, transferNoteMapper);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));
            
            //asset
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_TransferEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.Should().BeOfType<TransferEvidenceNotesViewModel>();
            result.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenSource_SchemeValuesShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.PcsId.Should().Be(source.OrganisationId);
        }

        [Fact]
        public void Map_GivenSource_SchemeNameShouldBeRetrievedFromCacheAndSet()
        {
            //arrange
            var schemeName = TestFixture.Create<string>();
            var source = GetTransferObject();

            A.CallTo(() => cache.FetchSchemePublicInfo(source.Request.RecipientId)).Returns(new SchemePublicInfo()
            {
                Name = schemeName
            });

            //act
            var result = map.Map(source);

            //assert
            result.RecipientName.Should().Be(schemeName);
        }

        [Fact(Skip = "TO BE FIXED")]
        public void Map_GivenSourceWithTransferNoteData_SchemeNameShouldBeRetrievedFromCacheAndSet()
        {
            //arrange
            var schemeName = TestFixture.Create<string>();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var source = new TransferEvidenceNotesViewModelMapTransfer(
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3),
                null, noteData, TestFixture.Create<Guid>());

            A.CallTo(() => cache.FetchSchemePublicInfo(source.TransferEvidenceNoteData.RecipientOrganisationData.Id)).Returns(new SchemePublicInfo()
            {
                Name = schemeName
            });

            //act
            var result = map.Map(source);

            //assert
            result.RecipientName.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenSource_CategoryValuesShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.CategoryValues.Should().NotBeEmpty();
            source.Request.CategoryIds.ForEach(c => result.CategoryValues.Should().Contain(cv => cv.CategoryId.Equals(c)));
        }

        [Fact]
        public void Map_GivenSource_SelectedEvidenceNoteDataShouldBeMappedAndReturned()
        {
            //arrange
            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };

            var evidenceNoteData = new EvidenceNoteSearchDataResult(notes, 2);

            var viewEvidenceNoteViewModel = TestFixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds, new List<int>() { Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = TestFixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<int>(), evidenceNoteData, request, organisationId);

            A.CallTo(() =>
                    mapper.Map<ViewEvidenceNoteViewModel>(
                        A<ViewEvidenceNoteMapTransfer>.That.Matches(
                            v => v.IncludeAllCategories.Equals(false) && 
                                 v.EvidenceNoteData.Equals(notes.ElementAt(0)) && 
                                 v.NoteStatus == null &&
                                 v.User == null &&
                                 v.PrintableVersion == false)))
                .Returns(viewEvidenceNoteViewModel.ElementAt(0));

            A.CallTo(() =>
                    mapper.Map<ViewEvidenceNoteViewModel>(
                        A<ViewEvidenceNoteMapTransfer>.That.Matches(
                            v => v.IncludeAllCategories.Equals(false) && 
                                 v.EvidenceNoteData.Equals(notes.ElementAt(1)) && 
                                 v.NoteStatus == null &&
                                 v.User == null &&
                                 v.PrintableVersion == false)))
                .Returns(viewEvidenceNoteViewModel.ElementAt(1));

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNotesDataList.ElementAt(0).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(0));
            result.EvidenceNotesDataList.ElementAt(1).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(1));
        }

        [Fact(Skip = "CHECK AND FIX")]
        public void Map_GivenSource_SelectedEvidenceNotePairsShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            //result.SelectedEvidenceNotePairs.Should().NotBeEmpty();
            //foreach (var evidenceNoteData in source.Notes.Results)
            //{
            //    result.SelectedEvidenceNotePairs.Should()
            //        .Contain(p => p.Value.Equals(false) && p.Key.Equals(evidenceNoteData.Id));
            //}
        }

        [Fact(Skip = "CHECK AND FIX")]
        public void Map_GivenSourceWithExistingNoteData_SelectedEvidenceNotePairsShouldBeSetAndOrderedCorrectly()
        {
            //arrange
            var existingNoteId1 = TestFixture.Create<Guid>();
            var existingNoteId2 = TestFixture.Create<Guid>();
            var notExistingNoteId1 = TestFixture.Create<Guid>();
            var notExistingNoteId2 = TestFixture.Create<Guid>();

            var transferNoteTonnageData = new List<TransferEvidenceNoteTonnageData>()
            {
                TestFixture.Build<TransferEvidenceNoteTonnageData>().Create(),
                TestFixture.Build<TransferEvidenceNoteTonnageData>().With(n => n.OriginalNoteId, existingNoteId1).Create(),
                TestFixture.Build<TransferEvidenceNoteTonnageData>().Create(),
                TestFixture.Build<TransferEvidenceNoteTonnageData>().With(n => n.OriginalNoteId, existingNoteId2).Create()
            };

            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, transferNoteTonnageData)
                .Create();

            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, notExistingNoteId1).With(e => e.Reference, 1).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, existingNoteId2).With(e => e.Reference, 2).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, existingNoteId1).With(e => e.Reference, 3).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, notExistingNoteId2).With(e => e.Reference, 4).Create(),
            };
            var evidenceNoteData = new EvidenceNoteSearchDataResult(notes, 4);

            var viewEvidenceNoteModels = new List<ViewEvidenceNoteViewModel>()
            {
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, notExistingNoteId1).With(e => e.Reference, 1).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Reference, 2).With(e => e.Id, existingNoteId2).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Reference, 3).With(e => e.Id, existingNoteId1).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, notExistingNoteId2).With(e => e.Reference, 4).Create()
            };

            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).ReturnsNextFromSequence(viewEvidenceNoteModels.ToArray());

            var organisationId = TestFixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(evidenceNoteData, TransferEvidenceNoteRequest(), transferNoteData, organisationId);

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNotesDataList.ElementAt(0).Reference.Should().Be(3);
            result.EvidenceNotesDataList.ElementAt(1).Reference.Should().Be(2);
            result.EvidenceNotesDataList.ElementAt(2).Reference.Should().Be(4);
            result.EvidenceNotesDataList.ElementAt(3).Reference.Should().Be(1);
            //result.SelectedEvidenceNotePairs.ElementAt(0).Key.Should().Be(existingNoteId1);
            //result.SelectedEvidenceNotePairs.ElementAt(0).Value.Should().BeTrue();
            //result.SelectedEvidenceNotePairs.ElementAt(1).Key.Should().Be(existingNoteId2);
            //result.SelectedEvidenceNotePairs.ElementAt(1).Value.Should().BeTrue();
            //result.SelectedEvidenceNotePairs.ElementAt(2).Key.Should().Be(notExistingNoteId2);
            //result.SelectedEvidenceNotePairs.ElementAt(2).Value.Should().BeFalse();
            //result.SelectedEvidenceNotePairs.ElementAt(3).Key.Should().Be(notExistingNoteId1);
            //result.SelectedEvidenceNotePairs.ElementAt(3).Value.Should().BeFalse();
        }

        [Fact(Skip = "CHECK AND FIX")]
        public void Map_GivenSourceWithSessionEvidenceDataIds_SelectedEvidenceNotePairsShouldBeSetAndOrderedCorrectly()
        {
            //arrange
            var noteId1 = TestFixture.Create<Guid>();
            var noteId2 = TestFixture.Create<Guid>();
            var noteId3 = TestFixture.Create<Guid>();
            var noteId4 = TestFixture.Create<Guid>();
            var noteId5 = TestFixture.Create<Guid>();
            var noteId6 = TestFixture.Create<Guid>();
            var noteId7 = TestFixture.Create<Guid>();

            var organisationId = TestFixture.Create<Guid>();

            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, noteId1).With(e => e.Reference, 1).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, noteId2).With(e => e.Reference, 2).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, noteId3).With(e => e.Reference, 3).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, noteId4).With(e => e.Reference, 4).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, noteId5).With(e => e.Reference, 5).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, noteId6).With(e => e.Reference, 6).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Id, noteId7).With(e => e.Reference, 7).Create(),
            };

            var evidenceNoteData = new EvidenceNoteSearchDataResult(notes, 7);

            var viewEvidenceNoteModels = new List<ViewEvidenceNoteViewModel>()
            {
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, noteId1).With(e => e.Reference, 7).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, noteId2).With(e => e.Reference, 4).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, noteId3).With(e => e.Reference, 2).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, noteId4).With(e => e.Reference, 5).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, noteId5).With(e => e.Reference, 3).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, noteId6).With(e => e.Reference, 1).Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(e => e.Id, noteId7).With(e => e.Reference, 6).Create(),
            };

            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).ReturnsNextFromSequence(viewEvidenceNoteModels.ToArray());

            var source = new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<int>(), evidenceNoteData, TransferEvidenceNoteRequest(), organisationId);
            source.SessionEvidenceNotesId = new List<Guid>() { noteId2, noteId3, noteId5, noteId7};

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNotesDataList.ElementAt(0).Reference.Should().Be(6);
            //result.SelectedEvidenceNotePairs.ElementAt(0).Key.Should().Be(noteId7);
            //result.SelectedEvidenceNotePairs.ElementAt(0).Value.Should().BeTrue();

            result.EvidenceNotesDataList.ElementAt(1).Reference.Should().Be(4);
            //result.SelectedEvidenceNotePairs.ElementAt(1).Key.Should().Be(noteId2);
            //result.SelectedEvidenceNotePairs.ElementAt(1).Value.Should().BeTrue();

            result.EvidenceNotesDataList.ElementAt(2).Reference.Should().Be(3);
            //result.SelectedEvidenceNotePairs.ElementAt(2).Key.Should().Be(noteId5);
            //result.SelectedEvidenceNotePairs.ElementAt(2).Value.Should().BeTrue();

            result.EvidenceNotesDataList.ElementAt(3).Reference.Should().Be(2);
            //result.SelectedEvidenceNotePairs.ElementAt(3).Key.Should().Be(noteId3);
            //result.SelectedEvidenceNotePairs.ElementAt(3).Value.Should().BeTrue();

            //result.SelectedEvidenceNotePairs.ElementAt(4).Value.Should().BeFalse();
            //result.SelectedEvidenceNotePairs.ElementAt(5).Value.Should().BeFalse();
            //result.SelectedEvidenceNotePairs.ElementAt(6).Value.Should().BeFalse();
        }

        [Fact(Skip = "TO BE FIXED")]
        public void Map_GivenSource_ViewTransferNoteViewModelShouldBeMapped()
        {
            //arrange
            var source = GetTransferNoteDataTransferObject();

            //act
            map.Map(source);

            //assert
            A.CallTo(() => transferNoteMapper.Map(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(v =>
                v.TransferEvidenceNoteData == source.TransferEvidenceNoteData && v.DisplayNotification == null &&
                v.OrganisationId == source.OrganisationId))).MustHaveHappenedOnceExactly();
        }

        [Fact(Skip = "TO BE FIXED")]
        public void Map_GivenSource_ModelShouldBeReturned()
        {
            //arrange
            var source = GetTransferNoteDataTransferObject();
            var viewTransferNoteViewModel = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => transferNoteMapper.Map(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(viewTransferNoteViewModel);

            //act
            var result = map.Map(source);

            //assert
            result.ViewTransferNoteViewModel.Should().Be(viewTransferNoteViewModel);
        }

        private TransferEvidenceNotesViewModelMapTransfer GetTransferObject()
        {
            var selectedNotes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var pagedNotes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds, new List<int>() { Core.DataReturns.WeeeCategory.ConsumerEquipment.ToInt(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<int>(), pagedNotes, selectedNotes, request, organisationId);
        }

        private TransferEvidenceNotesViewModelMapTransfer GetTransferNoteDataTransferObject()
        {
            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };
            var evidenceNoteData = new EvidenceNoteSearchDataResult(notes, 2);

            var transferNoteTonnageData = TestFixture.CreateMany<TransferEvidenceNoteTonnageData>().ToList();

            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, transferNoteTonnageData)
                .Create();

            var request = TransferEvidenceNoteRequest();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(evidenceNoteData, request, transferNoteData, organisationId);
        }

        private TransferEvidenceNoteRequest TransferEvidenceNoteRequest()
        {
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(c => c.CategoryIds,
                    new List<int>()
                    {
                        Core.DataReturns.WeeeCategory.MedicalDevices.ToInt(),
                        Core.DataReturns.WeeeCategory.ToysLeisureAndSports.ToInt()
                    }).Create();
            return request;
        }
    }
}
