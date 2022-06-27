namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
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

            A.CallTo(() => cache.FetchSchemeName(source.Request.SchemeId)).Returns(schemeName);

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
        public void Map_GivenSource_EvidenceNoteDataShouldBeMappedAndReturned()
        {
            //arrange
            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };

            var viewEvidenceNoteViewModel = TestFixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds, new List<int>() { Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = TestFixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);

            A.CallTo(() =>
                    mapper.Map<ViewEvidenceNoteViewModel>(
                        A<ViewEvidenceNoteMapTransfer>.That.Matches(
                            v => v.IncludeAllCategories.Equals(false) && v.EvidenceNoteData.Equals(notes.ElementAt(0)) && v.NoteStatus == null)))
                .Returns(viewEvidenceNoteViewModel.ElementAt(0));

            A.CallTo(() =>
                    mapper.Map<ViewEvidenceNoteViewModel>(
                        A<ViewEvidenceNoteMapTransfer>.That.Matches(
                            v => v.IncludeAllCategories.Equals(false) && v.EvidenceNoteData.Equals(notes.ElementAt(1)) && v.NoteStatus == null)))
                .Returns(viewEvidenceNoteViewModel.ElementAt(1));

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNotesDataList.ElementAt(0).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(0));
            result.EvidenceNotesDataList.ElementAt(1).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(1));
        }

        [Fact]
        public void Map_GivenSource_SelectedEvidenceNotePairsShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.SelectedEvidenceNotePairs.Should().NotBeEmpty();
            foreach (var evidenceNoteData in source.Notes)
            {
                result.SelectedEvidenceNotePairs.Should()
                    .Contain(p => p.Value.Equals(false) && p.Key.Equals(evidenceNoteData.Id));
            }
        }

        [Fact]
        public void Map_GivenSource_ViewTransferNoteViewModelShouldBeMapped()
        {
            //arrange
            var source = GetTransferNoteDataTransferObject();

            //act
            map.Map(source);

            //assert
            A.CallTo(() => transferNoteMapper.Map(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(v =>
                v.TransferEvidenceNoteData == source.TransferEvidenceNoteData && v.DisplayNotification == null &&
                v.SchemeId == source.OrganisationId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
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
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds, new List<int>() { Core.DataReturns.WeeeCategory.ConsumerEquipment.ToInt(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);
        }

        private TransferEvidenceNotesViewModelMapTransfer GetTransferNoteDataTransferObject()
        {
            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };

            var transferNoteTonnageData = TestFixture.CreateMany<TransferEvidenceNoteTonnageData>().ToList();

            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, transferNoteTonnageData)
                .Create();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(notes, transferNoteData, organisationId);
        }
    }
}
