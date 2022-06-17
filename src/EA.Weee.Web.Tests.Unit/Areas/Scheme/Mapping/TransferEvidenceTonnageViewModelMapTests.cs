namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.DataReturns;
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

    public class TransferEvidenceTonnageViewModelMapTests : SimpleUnitTestBase
    {
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper;
        private List<EvidenceNoteData> notes;
        private readonly TransferEvidenceTonnageViewModelMap map;

        public TransferEvidenceTonnageViewModelMapTests()
        {
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            transferNoteMapper = A.Fake<IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel>>();

            map = new TransferEvidenceTonnageViewModelMap(mapper, cache, transferNoteMapper);
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
        public void Map_GivenRequestSource_TransferEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var source = GetRequestTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.Should().BeOfType<TransferEvidenceTonnageViewModel>();
            result.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenTransferNoteDataSource_TransferEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var source = GetTransferNoteDataTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.Should().BeOfType<TransferEvidenceTonnageViewModel>();
            result.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenRequestSource_SchemeValuesShouldBeSet()
        {
            //arrange
            var source = GetRequestTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.PcsId.Should().Be(source.OrganisationId);
        }

        [Fact]
        public void Map_GivenTransferNoteDataSource_SchemeValuesShouldBeSet()
        {
            //arrange
            var source = GetTransferNoteDataTransferObject();

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
            var source = GetRequestTransferObject();

            A.CallTo(() => cache.FetchSchemeName(source.Request.SchemeId)).Returns(schemeName);

            //act
            var result = map.Map(source);

            //assert
            result.RecipientName.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenTransferNoteDataSource_SchemeNameShouldBeRetrievedFromCacheAndSet()
        {
            //arrange
            var schemeName = TestFixture.Create<string>();
            var source = GetTransferNoteDataTransferObject();

            A.CallTo(() => cache.FetchSchemeName(source.TransferEvidenceNoteData.RecipientSchemeData.Id)).Returns(schemeName);

            //act
            var result = map.Map(source);

            //assert
            result.RecipientName.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenTransferNoteDataSource_CategoryValuesShouldBeSet()
        {
            //arrange
            var source = GetTransferNoteDataTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.CategoryValues.Should().NotBeEmpty();
            source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData
                .ForEach(c => result.CategoryValues.Should()
                    .Contain(cv => cv.CategoryId.Equals(c.EvidenceTonnageData.CategoryId.ToInt())));
        }

        [Fact]
        public void Map_GivenRequestSource_EvidenceNoteDataShouldBeMappedAndReturned()
        {
            //arrange
            notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };

            var viewEvidenceNoteViewModel = TestFixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();
            var request = DefaultRequest();
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
            result.EvidenceNotesDataList.Should().ContainEquivalentOf(viewEvidenceNoteViewModel.ElementAt(0));
            result.EvidenceNotesDataList.Should().ContainEquivalentOf(viewEvidenceNoteViewModel.ElementAt(1));
            result.EvidenceNotesDataList.Should().BeInAscendingOrder(e => e.SubmittedBy).And
                .ThenBeInAscendingOrder(e => e.Id);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_GivenSourceHasTransferAllTonnages_TransferAllTonnagesPropertyShouldBeSet(bool transferAllTonnages)
        {
            //arrange
            var source = GetRequestTransferObject();
            source.TransferAllTonnage = transferAllTonnages;

            //act
            var result = map.Map(source);

            //assert
            result.TransferAllTonnage.Should().Be(transferAllTonnages);
        }

        [Fact]
        public void Map_GivenSourceEvidenceListsWithAatfs_DisplayAatfPropertyShouldBeSet()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>(4).ToList();
            var request = DefaultRequest();
            var organisationId = TestFixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);

            var viewEvidenceNoteViewModels = new List<ViewEvidenceNoteViewModel>()
            {
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF1").Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF2").Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF2").Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF3").Create()
            };

            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).ReturnsNextFromSequence(viewEvidenceNoteViewModels.ToArray());

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNotesDataList.ElementAt(0).DisplayAatfName.Should().BeTrue();
            result.EvidenceNotesDataList.ElementAt(1).DisplayAatfName.Should().BeTrue();
            result.EvidenceNotesDataList.ElementAt(2).DisplayAatfName.Should().BeFalse();
            result.EvidenceNotesDataList.ElementAt(3).DisplayAatfName.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSourceEvidenceListsWithTonnageAndTransferAllTonnageIsFalse_TransferTonnageCategoriesShouldBeSet()
        {
            //arrange
            var source = SetupTransferTonnages(out var viewEvidenceNoteViewModels);

            //act
            var result = map.Map(source);

            //assert
            result.TransferCategoryValues.ElementAt(0).CategoryId.Should().Be(WeeeCategory.MedicalDevices.ToInt());
            result.TransferCategoryValues.ElementAt(0).Received.Should().BeNull();
            result.TransferCategoryValues.ElementAt(0).Reused.Should().BeNull();
            result.TransferCategoryValues.ElementAt(0).TransferTonnageId.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(2).Id);
            result.TransferCategoryValues.ElementAt(0).AvailableReceived.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(2).Received);
            result.TransferCategoryValues.ElementAt(0).AvailableReused.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(2).Reused);
            result.TransferCategoryValues.ElementAt(1).CategoryId.Should().Be(WeeeCategory.AutomaticDispensers.ToInt());
            result.TransferCategoryValues.ElementAt(1).Received.Should().BeNull();
            result.TransferCategoryValues.ElementAt(1).Reused.Should().BeNull();
            result.TransferCategoryValues.ElementAt(1).TransferTonnageId.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(0).Id);
            result.TransferCategoryValues.ElementAt(1).AvailableReceived.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(0).Received);
            result.TransferCategoryValues.ElementAt(1).AvailableReused.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(0).Reused);
            result.TransferCategoryValues.ElementAt(2).CategoryId.Should().Be(WeeeCategory.GasDischargeLampsAndLedLightSources.ToInt());
            result.TransferCategoryValues.ElementAt(2).Received.Should().BeNull();
            result.TransferCategoryValues.ElementAt(2).Reused.Should().BeNull();
            result.TransferCategoryValues.ElementAt(2).TransferTonnageId.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(1).Id);
            result.TransferCategoryValues.ElementAt(2).AvailableReceived.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(1).Received);
            result.TransferCategoryValues.ElementAt(2).AvailableReused.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(1).Reused);
            result.TransferCategoryValues.ElementAt(3).CategoryId.Should().Be(WeeeCategory.AutomaticDispensers.ToInt());
            result.TransferCategoryValues.ElementAt(3).Received.Should().BeNull();
            result.TransferCategoryValues.ElementAt(3).Reused.Should().BeNull();
            result.TransferCategoryValues.ElementAt(3).TransferTonnageId.Should()
                .Be(notes.ElementAt(0).EvidenceTonnageData.ElementAt(0).Id);
            result.TransferCategoryValues.ElementAt(3).AvailableReceived.Should()
                .Be(notes.ElementAt(0).EvidenceTonnageData.ElementAt(0).Received);
            result.TransferCategoryValues.ElementAt(3).AvailableReused.Should()
                .Be(notes.ElementAt(0).EvidenceTonnageData.ElementAt(0).Reused);
            result.TransferCategoryValues.Count.Should()
                .Be(viewEvidenceNoteViewModels.SelectMany(v => v.CategoryValues).Count());
        }

        [Fact]
        public void Map_GivenSourceEvidenceListsWithTonnageAndTransferAllTonnageIsTrue_TransferTonnageCategoriesShouldBeSet()
        {
            //arrange
            var source = SetupTransferTonnages(out var viewEvidenceNoteViewModels);
            source.TransferAllTonnage = true;

            //act
            var result = map.Map(source);

            //assert
            result.TransferCategoryValues.ElementAt(0).CategoryId.Should().Be(WeeeCategory.MedicalDevices.ToInt());
            result.TransferCategoryValues.ElementAt(0).Received.Should().Be("12.000");
            result.TransferCategoryValues.ElementAt(0).Reused.Should().Be("4.000");
            result.TransferCategoryValues.ElementAt(0).TransferTonnageId.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(2).Id);
            result.TransferCategoryValues.ElementAt(0).AvailableReceived.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(2).Received);
            result.TransferCategoryValues.ElementAt(0).AvailableReused.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(2).Reused);
            result.TransferCategoryValues.ElementAt(1).CategoryId.Should().Be(WeeeCategory.AutomaticDispensers.ToInt());
            result.TransferCategoryValues.ElementAt(1).Received.Should().Be("2.000");
            result.TransferCategoryValues.ElementAt(1).Reused.Should().Be("1.000");
            result.TransferCategoryValues.ElementAt(1).TransferTonnageId.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(0).Id);
            result.TransferCategoryValues.ElementAt(1).AvailableReceived.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(0).Received);
            result.TransferCategoryValues.ElementAt(1).AvailableReused.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(0).Reused);
            result.TransferCategoryValues.ElementAt(2).CategoryId.Should().Be(WeeeCategory.GasDischargeLampsAndLedLightSources.ToInt());
            result.TransferCategoryValues.ElementAt(2).Received.Should().Be("10.000");
            result.TransferCategoryValues.ElementAt(2).Reused.Should().BeNull();
            result.TransferCategoryValues.ElementAt(2).TransferTonnageId.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(1).Id);
            result.TransferCategoryValues.ElementAt(2).AvailableReceived.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(1).Received);
            result.TransferCategoryValues.ElementAt(2).AvailableReused.Should()
                .Be(notes.ElementAt(1).EvidenceTonnageData.ElementAt(1).Reused);
            result.TransferCategoryValues.ElementAt(3).CategoryId.Should().Be(WeeeCategory.AutomaticDispensers.ToInt());
            result.TransferCategoryValues.ElementAt(3).Received.Should().Be("2.000");
            result.TransferCategoryValues.ElementAt(3).Reused.Should().Be("1.000");
            result.TransferCategoryValues.ElementAt(3).TransferTonnageId.Should()
                .Be(notes.ElementAt(0).EvidenceTonnageData.ElementAt(0).Id);
            result.TransferCategoryValues.ElementAt(3).AvailableReceived.Should()
                .Be(notes.ElementAt(0).EvidenceTonnageData.ElementAt(0).Received);
            result.TransferCategoryValues.ElementAt(3).AvailableReused.Should()
                .Be(notes.ElementAt(0).EvidenceTonnageData.ElementAt(0).Reused);
            result.TransferCategoryValues.Count.Should()
                .Be(viewEvidenceNoteViewModels.SelectMany(v => v.CategoryValues).Count());
        }

        [Fact]
        public void Map_GivenCategoriesAndEvidenceNoteTonnage_TotalsShouldBeInitialised()
        {
            //arrange
            var source = SetupTotals();

            //act
            var result = map.Map(source);

            //assert
            result.CategoryValues.ForEach(c => c.TotalReused.Should().Be("0.000"));
            result.CategoryValues.ForEach(c => c.TotalReceived.Should().Be("0.000"));
        }

        [Fact]
        public void Map_GivenCategoriesAndEvidenceNoteTonnageAndTransferAllTonnageIsSelected_TotalsShouldBeInitialisedAndCalculated()
        {
            //arrange
            var source = SetupTotals();
            source.TransferAllTonnage = true;

            //act
            var result = map.Map(source);

            //assert
            result.CategoryValues.ElementAt(0).TotalReceived.Should().Be("1.000"); // 3.000 - 2.000
            result.CategoryValues.ElementAt(0).TotalReused.Should().Be("3.000"); // 6.000 - 3.000
            result.CategoryValues.ElementAt(1).TotalReceived.Should().Be("15.000"); // 15.000 - 0
            result.CategoryValues.ElementAt(1).TotalReused.Should().Be("17.000"); // 17.000 - 0
            result.CategoryValues.Count.Should().Be(2);
        }

        private TransferEvidenceNotesViewModelMapTransfer SetupTotals()
        {
            notes = new List<EvidenceNoteData>()
            {
                TestFixture.Build<EvidenceNoteData>().With(e => e.EvidenceTonnageData, new List<EvidenceTonnageData>()
                {
                    new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ITAndTelecommsEquipment, 3, 2, 2, 0),
                    new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.GasDischargeLampsAndLedLightSources, 10, 7, 0, 0),
                    new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.LargeHouseholdAppliances, 100, 700, 0, 0)
                }).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.EvidenceTonnageData, new List<EvidenceTonnageData>()
                {
                    new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ITAndTelecommsEquipment, null, 4, 0, 3),
                }).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.EvidenceTonnageData, new List<EvidenceTonnageData>()
                {
                    new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.GasDischargeLampsAndLedLightSources, 5, 10, 0, 0),
                }).Create()
            };

            var viewEvidenceNoteViewModel = TestFixture.Create<ViewEvidenceNoteViewModel>();

            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._))
                .Returns(viewEvidenceNoteViewModel);

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds,
                    new List<int>()
                    {
                        WeeeCategory.ITAndTelecommsEquipment.ToInt(), WeeeCategory.GasDischargeLampsAndLedLightSources.ToInt()
                    }).Create();

            var organisationId = TestFixture.Create<Guid>();
            var source = new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);
            return source;
        }

        private TransferEvidenceNotesViewModelMapTransfer SetupTransferTonnages(out List<ViewEvidenceNoteViewModel> viewEvidenceNoteViewModels)
        {
            //arrange
            var noteId1 = TestFixture.Create<Guid>();
            var noteId2 = TestFixture.Create<Guid>();

            notes = new List<EvidenceNoteData>()
            {
                TestFixture.Build<EvidenceNoteData>().With(e => e.EvidenceTonnageData,
                        new List<EvidenceTonnageData>()
                        {
                            new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.AutomaticDispensers, 2, 1, 0, 0)
                        })
                    .With(e => e.AatfData, new AatfData() { Name = "Z" })
                    .With(e => e.Id, noteId2).Create(),
                TestFixture.Build<EvidenceNoteData>().With(e => e.EvidenceTonnageData,
                        new List<EvidenceTonnageData>()
                        {
                            new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.AutomaticDispensers, 2, 1, 0, 0),
                            new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.GasDischargeLampsAndLedLightSources, 10,
                                null, 0, 0),
                            new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.MedicalDevices, 12, 4, 0, 0)
                        }).With(e => e.AatfData, new AatfData() { Name = "A" })
                    .With(e => e.Id, noteId1).Create()
            };

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.CategoryIds, new List<int>())
                .Create();
            var organisationId = TestFixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(notes.ToList(), request, organisationId);

            viewEvidenceNoteViewModels = new List<ViewEvidenceNoteViewModel>()
            {
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.CategoryValues,
                        new List<EvidenceCategoryValue>()
                        {
                            new EvidenceCategoryValue(WeeeCategory.AutomaticDispensers)
                        })
                    .With(n => n.Id, noteId2)
                    .With(n => n.SubmittedBy, "Z")
                    .Create(),
                TestFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.CategoryValues,
                        new List<EvidenceCategoryValue>()
                        {
                            new EvidenceCategoryValue(WeeeCategory.AutomaticDispensers),
                            new EvidenceCategoryValue(WeeeCategory.GasDischargeLampsAndLedLightSources),
                            new EvidenceCategoryValue(WeeeCategory.MedicalDevices)
                        })
                    .With(n => n.SubmittedBy, "A")
                    .With(n => n.Id, noteId1)
                    .Create()
            };

            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._))
                .ReturnsNextFromSequence(viewEvidenceNoteViewModels.ToArray());
            return source;
        }

        private TransferEvidenceNotesViewModelMapTransfer GetRequestTransferObject()
        {
            notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };

            var viewEvidenceNoteViewModel = TestFixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();

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

            var request = DefaultRequest();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);
        }

        private TransferEvidenceNotesViewModelMapTransfer GetTransferNoteDataTransferObject()
        {
            notes = new List<EvidenceNoteData>()
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };

            var viewEvidenceNoteViewModel = TestFixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();

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

            var transferNoteTonnageData = TestFixture.CreateMany<TransferEvidenceNoteTonnageData>().ToList();

            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, transferNoteTonnageData)
                .Create();

            var organisationId = TestFixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(notes, transferNoteData, organisationId);
        }

        private TransferEvidenceNoteRequest DefaultRequest()
        {
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds,
                    new List<int>() { WeeeCategory.ConsumerEquipment.ToInt(), WeeeCategory.ITAndTelecommsEquipment.ToInt() })
                .Create();
            return request;
        }
    }
}
