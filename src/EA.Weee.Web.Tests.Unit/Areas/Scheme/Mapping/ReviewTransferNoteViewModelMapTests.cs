namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using Web.Areas.Scheme.ViewModels;
    using Weee.Tests.Core;
    using Xunit;

    public class ReviewTransferNoteViewModelMapTests : SimpleUnitTestBase
    {
        private readonly TransferEvidenceNoteData note;
        private readonly IMapper mapper;
        private readonly ReviewTransferNoteViewModelMap map;

        public ReviewTransferNoteViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();
            map = new ReviewTransferNoteViewModelMap(mapper);
            note = TestFixture.Create<TransferEvidenceNoteData>();
        }

        [Fact]
        public void ReviewTransferNoteViewModelMap_ShouldBeDerivedFromIMap()
        {
            typeof(ReviewTransferNoteViewModelMap).Should()
                .Implement<IMap<ViewTransferNoteViewModelMapTransfer, ReviewTransferNoteViewModel>>();
        }

        [Fact]
        public void GivenSource_MapperReturnsViewModel()
        {
            //arrange
            var schemeId = TestFixture.Create<Guid>();
            
            // act
            var result = map.Map(new ViewTransferNoteViewModelMapTransfer(schemeId, note, note.Status));

            // asset
            result.Should().NotBeNull();
            result.ViewTransferNoteViewModel.Should().NotBeNull();
            result.OrganisationId.Should().Be(schemeId);
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
        public void Map_GivenATransferNote_MapperShouldBeCalled()
        {
            // arrange
           var transfer = TestFixture.Create<ViewTransferNoteViewModelMapTransfer>();

            // act
            map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(transfer)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAMappedTransferNoteViewModel_MustReturnAModel()
        {
            //arrange
            var transfer = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(), note, null);

            var transferNoteViewModel = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(transferNoteViewModel);

            //act
            var result = map.Map(transfer);

            // assert 
            result.ViewTransferNoteViewModel.Should().Be(transferNoteViewModel);
        }

        //TODO: put these back in
        //[Fact]
        //public void Map_GivenValidEvidenceNoteData_ShouldContainHintItems()
        //{
        //    //arrange
        //    EvidenceNoteData note = fixture.Create<EvidenceNoteData>();
        //    ViewEvidenceNoteMapTransfer transfer = new ViewEvidenceNoteMapTransfer(note, note.Status);

        //    //act
        //    var result = map.Map(transfer);

        //    // assert
        //    result.HintItems.Count.Should().Be(3);
        //    result.HintItems.ElementAt(0).Key.Should().Be("Approve evidence note");
        //    result.HintItems.ElementAt(0).Value.Should().BeNull();
        //    result.HintItems.ElementAt(1).Key.Should().Be("Reject evidence note");
        //    result.HintItems.ElementAt(1).Value.Should().Be("Reject an evidence note if the evidence has been sent to you by mistake or if there is a large number of updates to make that it is quicker to create a new evidence note");
        //    result.HintItems.ElementAt(2).Key.Should().Be("Return evidence note");
        //    result.HintItems.ElementAt(2).Value.Should().Be("Return an evidence note if there are some minor updates to be made by the AATF");
        //}
    }
}
