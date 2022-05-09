namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
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
    using Xunit;

    public class TransferEvidenceTonnageViewModelMapTests
    {
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly Fixture fixture;

        private readonly TransferEvidenceTonnageViewModelMap map;

        public TransferEvidenceTonnageViewModelMapTests()
        {
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            map = new TransferEvidenceTonnageViewModelMap(mapper, cache);
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
            result.Should().BeOfType<TransferEvidenceTonnageViewModel>();
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
            var schemeName = fixture.Create<string>();
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
                fixture.Create<EvidenceNoteData>(),
                fixture.Create<EvidenceNoteData>()
            };

            var viewEvidenceNoteViewModel = fixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();

            var request = fixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds, new List<int>() { Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = fixture.Create<Guid>();

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
            var source = GetTransferObject();
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
            var notes = fixture.CreateMany<EvidenceNoteData>(4).ToList();
            var request = fixture.Create<TransferEvidenceNoteRequest>();
            var organisationId = fixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);

            var viewEvidenceNoteViewModels = new List<ViewEvidenceNoteViewModel>()
            {
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF1").Create(),
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF2").Create(),
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF2").Create(),
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF3").Create()
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
            var notes = new List<EvidenceNoteData>()
            {
                fixture.Build<EvidenceNoteData>().With(e => e.EvidenceTonnageData,
                    new List<EvidenceTonnageData>()
                    {
                        new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ConsumerEquipment, 1, null),
                        new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.AutomaticDispensers, 2, 1)
                    }).Create(),
                fixture.Build<EvidenceNoteData>().With(e => e.EvidenceTonnageData,
                    new List<EvidenceTonnageData>()
                    {
                        new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.GasDischargeLampsAndLedLightSources, 10,
                            null),
                        new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.MedicalDevices, 12, 4)
                    }).Create()
            };

            var request = fixture.Create<TransferEvidenceNoteRequest>();
            var organisationId = fixture.Create<Guid>();

            var source = new TransferEvidenceNotesViewModelMapTransfer(notes.ToList(), request, organisationId);

            var viewEvidenceNoteViewModels = new List<ViewEvidenceNoteViewModel>()
            {
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF1").Create(),
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF2").Create(),
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF2").Create(),
                fixture.Build<ViewEvidenceNoteViewModel>().With(v => v.SubmittedBy, "AATF3").Create()
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

        private TransferEvidenceNotesViewModelMapTransfer GetTransferObject()
        {
            var notes = new List<EvidenceNoteData>()
            {
                fixture.Create<EvidenceNoteData>(),
                fixture.Create<EvidenceNoteData>()
            };

            var viewEvidenceNoteViewModel = fixture.CreateMany<ViewEvidenceNoteViewModel>(2).ToList();

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

            var request = fixture.Build<TransferEvidenceNoteRequest>()
                .With(e => e.CategoryIds, new List<int>() { Core.DataReturns.WeeeCategory.ConsumerEquipment.ToInt(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt() }).Create();

            var organisationId = fixture.Create<Guid>();

            return new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);
        }
    }
}
