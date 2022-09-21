namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
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
        public void Map_GivenSourceWithPageNumber_PageNumberShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.PageNumber.Should().Be(source.PageNumber);
        }

        [Fact]
        public void Map_GivenSourceWithSearchRef_SearchRefShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.SearchRef.Should().Be(source.SearchRef);
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

        [Fact]
        public void Map_GivenSourceWithTransferNoteData_SchemeNameShouldBeRetrievedFromCacheAndSet()
        {
            //arrange
            var schemeName = TestFixture.Create<string>();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var source = new TransferEvidenceNotesViewModelMapTransfer(
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3),
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3),
                null, noteData, TestFixture.Create<Guid>(), TestFixture.Create<string>());

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
                TestFixture.Build<EvidenceNoteData>().With(e => e.Reference, 1).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.Reference, 2).Create(),
            };

            var selectedEvidenceNoteData = new EvidenceNoteSearchDataResult(notes, 2);

            var viewEvidenceNoteViewModel = TestFixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>().With(e => e.CategoryIds, 
                    new List<int>() { Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = TestFixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(selectedEvidenceNoteData,
                TestFixture.Create<EvidenceNoteSearchDataResult>(), request,
                TestFixture.Create<TransferEvidenceNoteData>(), organisationId, TestFixture.Create<string>());

            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                            v => v.IncludeAllCategories.Equals(false) && 
                                 v.EvidenceNoteData.Equals(notes.ElementAt(0)) && 
                                 v.NoteStatus == null &&
                                 v.User == null &&
                                 v.PrintableVersion == false))).Returns(viewEvidenceNoteViewModel.ElementAt(0));

            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                            v => v.IncludeAllCategories.Equals(false) && 
                                 v.EvidenceNoteData.Equals(notes.ElementAt(1)) && 
                                 v.NoteStatus == null &&
                                 v.User == null &&
                                 v.PrintableVersion == false))).Returns(viewEvidenceNoteViewModel.ElementAt(1));

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNotesDataList.ElementAt(0).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(1)); // ordered by ref
            result.EvidenceNotesDataList.ElementAt(1).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(0)); // ordered by ref
        }

        [Fact]
        public void Map_GivenSourceWithAvailableNotes_AvailableEvidenceNoteDataShouldBeMappedAndReturned()
        {
            //arrange
            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Build<EvidenceNoteData>().Create(),
                TestFixture.Build<EvidenceNoteData>().Create(),
            };

            var availableEvidenceNoteData = new EvidenceNoteSearchDataResult(notes, 2);

            var viewEvidenceNoteViewModel = TestFixture.CreateMany<EvidenceNoteRowViewModel>(2).ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>().With(e => e.CategoryIds,
                    new List<int>() { Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = TestFixture.Create<Guid>();

            const int pageNumber = 1;
            const int pageSize = 3;

            var source = new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<EvidenceNoteSearchDataResult>(),
                availableEvidenceNoteData, request,
                TestFixture.Create<TransferEvidenceNoteData>(), 
                organisationId,
                TestFixture.Create<string>(),
                pageNumber,
                pageSize);

            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(notes.ElementAt(0))).Returns(viewEvidenceNoteViewModel.ElementAt(0));
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(notes.ElementAt(1))).Returns(viewEvidenceNoteViewModel.ElementAt(1));

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNotesDataListPaged.ElementAt(0).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(0)); 
            result.EvidenceNotesDataListPaged.ElementAt(1).Should().BeEquivalentTo(viewEvidenceNoteViewModel.ElementAt(1));
            result.EvidenceNotesDataListPaged.PageSize.Should().Be(pageSize);
            result.EvidenceNotesDataListPaged.PageNumber.Should().Be(pageNumber);
            result.EvidenceNotesDataListPaged.PageIndex.Should().Be(pageNumber - 1);
            result.EvidenceNotesDataListPaged.TotalItemCount.Should().Be(2);
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
                v.OrganisationId == source.OrganisationId))).MustHaveHappenedOnceExactly();
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
            var selectedNotes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var pagedNotes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var pageNumber = TestFixture.Create<int>();
            var pageSize = TestFixture.Create<int>();
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds, new List<int>() { Core.DataReturns.WeeeCategory.ConsumerEquipment.ToInt(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();
            var searchRef = TestFixture.Create<string>();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<int>(), pagedNotes, selectedNotes, request, organisationId, searchRef, pageNumber, pageSize);
        }

        private TransferEvidenceNotesViewModelMapTransfer GetTransferNoteDataTransferObject()
        {
            var notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };
            var selectedEvidenceNotes = new EvidenceNoteSearchDataResult(notes, 2);
            var availableEvidenceNotes = new EvidenceNoteSearchDataResult(notes, 2);

            var transferNoteTonnageData = TestFixture.CreateMany<TransferEvidenceNoteTonnageData>().ToList();

            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, transferNoteTonnageData)
                .Create();

            var request = TransferEvidenceNoteRequest();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(selectedEvidenceNotes, availableEvidenceNotes, request, transferNoteData, organisationId, TestFixture.Create<string>());
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
