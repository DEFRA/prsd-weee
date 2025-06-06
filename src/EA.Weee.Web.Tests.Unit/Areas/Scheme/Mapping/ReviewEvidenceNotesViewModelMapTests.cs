﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Linq;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Xunit;

    public class ReviewEvidenceNotesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly Fixture fixture;
        private readonly EvidenceNoteData note;
        private readonly IMapper mapper;
        private readonly ReviewEvidenceNoteViewModelMap map;

        public ReviewEvidenceNotesViewModelMapTests()
        {
            fixture = new Fixture();
            mapper = A.Fake<IMapper>();
            map = new ReviewEvidenceNoteViewModelMap(mapper);
            note = fixture.Create<EvidenceNoteData>();
        }

        [Fact]
        public void ReviewEvidenceNotesViewModelMap_ShouldBeDerivedFromIMap()
        {
            typeof(ReviewEvidenceNoteViewModelMap).Should()
                .Implement<IMap<ViewEvidenceNoteMapTransfer, ReviewEvidenceNoteViewModel>>();
        }

        [Fact]
        public void GivenValidModel_MapperReturnsViewModel()
        {
            //arrange
            var schemeId = fixture.Create<Guid>();

            // act
            ReviewEvidenceNoteViewModel modelCreated = map.Map(new ViewEvidenceNoteMapTransfer(note, note.Status, false, null)
            {
                SchemeId = schemeId
            });

            // asset
            modelCreated.Should().NotBeNull();
            modelCreated.ViewEvidenceNoteViewModel.Should().NotBeNull();
            modelCreated.OrganisationId.Should().Be(schemeId);
            modelCreated.QueryString.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void MapTransfer_GivenNullArguments_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ViewEvidenceNoteMapTransfer(null,  note.Status, false));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenAValidEvidenceNote_MapperShouldBeCalled()
        {
            // arrange
            var transfer = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, false);

            // act
            map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(transfer)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAMappedViewEvidenceNoteViewModel_MustReturnAModel()
        {
            //arrange
            EvidenceNoteData note = fixture.Create<EvidenceNoteData>();
            var transfer = new ViewEvidenceNoteMapTransfer(note, null, false, null);
            var viewEvidenceNoteViewModel = new ViewEvidenceNoteViewModel();
            A.CallTo(() => mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(viewEvidenceNoteViewModel);

            //act
            var result = map.Map(transfer);

            // assert 
            result.ViewEvidenceNoteViewModel.Should().Be(viewEvidenceNoteViewModel);
        }

        [Fact]
        public void Map_GivenValidEvidenceNoteData_ShouldReturnViewModel()
        {
            //arrange
            EvidenceNoteData note = fixture.Create<EvidenceNoteData>();
            ViewEvidenceNoteMapTransfer transfer = new ViewEvidenceNoteMapTransfer(note, note.Status, false);

            //act
            var result = map.Map(transfer);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenValidEvidenceNoteData_ShouldContainHintItems()
        {
            //arrange
            EvidenceNoteData note = fixture.Create<EvidenceNoteData>();
            ViewEvidenceNoteMapTransfer transfer = new ViewEvidenceNoteMapTransfer(note, note.Status, false);

            //act
            var result = map.Map(transfer);

            // assert
            result.HintItems.Count.Should().Be(3);
            result.HintItems.ElementAt(0).Key.Should().Be("Approve evidence note");
            result.HintItems.ElementAt(0).Value.Should().BeNull();
            result.HintItems.ElementAt(1).Key.Should().Be("Reject evidence note");
            result.HintItems.ElementAt(1).Value.Should().Be("Reject an evidence note to have it replaced. If the note has been sent to you by mistake, it must be rejected.");
            result.HintItems.ElementAt(2).Key.Should().Be("Return evidence note");
            result.HintItems.ElementAt(2).Value.Should().Be("Return an evidence note to have amendments made to it.");
        }

        [Fact]
        public void ReviewEvidenceNotesViewModelMap_ShouldSetDisplayH2TitleTrue()
        {
            // act
            ReviewEvidenceNoteViewModel modelCreated = map.Map(new ViewEvidenceNoteMapTransfer(note, note.Status, false)
            {
                SchemeId = note.RecipientId
            });

            // asset
            modelCreated.Should().NotBeNull();
            modelCreated.ViewEvidenceNoteViewModel.DisplayH2Title.Should().BeTrue();
        }

        [Fact]
        public void ReviewEvidenceNotesViewModelMap_GivenQueryString_ViewModelPropertyShouldBeSet()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(note, note.Status, false)
            {
                QueryString = TestFixture.Create<string>()
            };

            //act
            var result = map.Map(source);

            //assert
            result.QueryString.Should().Be(source.QueryString);
        }
    }
}
