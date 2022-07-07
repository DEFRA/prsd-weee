namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using Core.Helpers;
    using Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceCategoriesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper;
        private readonly TransferEvidenceCategoriesViewModelMap map;

        public TransferEvidenceCategoriesViewModelMapTests()
        {
            var cache = A.Fake<IWeeeCache>();
            var mapper = A.Fake<IMapper>();
            transferNoteMapper = A.Fake<IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel>>();

            map = new TransferEvidenceCategoriesViewModelMap(cache, mapper, transferNoteMapper);
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
        public void Map_GivenSource_TransferEvidenceNoteCategoriesViewModelShouldBeReturned()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.Should().BeOfType<TransferEvidenceNoteCategoriesViewModel>();
            result.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenSource_SchemeValueShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.PcsId.Should().Be(source.OrganisationId);
        }

        [Fact]
        public void Map_GivenSource_CurrentSelectedSchemeShouldBeSet()
        {
            //arrange
            var source = GetTransferObject();

            //act
            var result = map.Map(source);

            //assert
            result.SelectedSchema.Should().Be(source.TransferEvidenceNoteData.RecipientSchemeData.Id);
        }

        [Fact]
        public void Map_GivenSource_SchemeListShouldBeSetAndCurrentOrganisationRemovedFromList()
        {
            //arrange
            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>().Create();
            var organisationId = TestFixture.Create<Guid>();
            var schemeData = TestFixture.CreateMany<SchemeData>(3).ToList();
            schemeData.ElementAt(1).OrganisationId = organisationId;
            
            var transfer = new TransferEvidenceNotesViewModelMapTransfer(transferNoteData, schemeData, organisationId);

            //act
            var result = map.Map(transfer);

            //assert
            result.SchemasToDisplay.Count.Should().Be(2);
            result.SchemasToDisplay.Should().NotContain(s => s.Id == organisationId);
            result.SchemasToDisplay.Should().Contain(s => s.Id == schemeData.ElementAt(0).Id);
            result.SchemasToDisplay.Should().Contain(s => s.Id == schemeData.ElementAt(2).Id);
        }

        [Fact]
        public void Map_GivenSource_SelectedCategoriesShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            var transferNoteTonnageData = new List<TransferEvidenceNoteTonnageData>()
            {
                TestFixture
                    .Build<TransferEvidenceNoteTonnageData>()
                    .With(e => e.EvidenceTonnageData, new EvidenceTonnageData(Guid.Empty, WeeeCategory.MedicalDevices, null, null, null, null)).Create(),
                TestFixture
                    .Build<TransferEvidenceNoteTonnageData>()
                    .With(e => e.EvidenceTonnageData, new EvidenceTonnageData(Guid.Empty, WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null, null, null)).Create(),
                TestFixture
                    .Build<TransferEvidenceNoteTonnageData>()
                    .With(e => e.EvidenceTonnageData, new EvidenceTonnageData(Guid.Empty, WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null, null, null)).Create(),
                TestFixture
                    .Build<TransferEvidenceNoteTonnageData>()
                    .With(e => e.EvidenceTonnageData, new EvidenceTonnageData(Guid.Empty, WeeeCategory.AutomaticDispensers, null, null, null, null)).Create()
            };

            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, transferNoteTonnageData)
                .Create();

            var transfer = new TransferEvidenceNotesViewModelMapTransfer(transferNoteData, schemeData, organisationId);

            //act
            var result = map.Map(transfer);

            //assert
            var expectedSelectedCategory = new List<int>
            {
                WeeeCategory.MedicalDevices.ToInt(), 
                WeeeCategory.CoolingApplicancesContainingRefrigerants.ToInt(),
                WeeeCategory.AutomaticDispensers.ToInt()
            };

            result.CategoryValues.Where(c => expectedSelectedCategory.Contains(c.CategoryId)).All(c => c.Selected)
                .Should().BeTrue();
            result.CategoryValues.Where(c => !expectedSelectedCategory.Contains(c.CategoryId)).All(c => c.Selected)
                .Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSource_ViewTransferNoteViewModelShouldBeMapped()
        {
            //arrange
            var source = GetTransferObject();

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
            var source = GetTransferObject();
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
            var transferNoteData = TestFixture.Build<TransferEvidenceNoteData>().Create();

            var organisationId = TestFixture.Create<Guid>();

            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();

            return new TransferEvidenceNotesViewModelMapTransfer(transferNoteData, schemeData, organisationId);
        }
    }
}
