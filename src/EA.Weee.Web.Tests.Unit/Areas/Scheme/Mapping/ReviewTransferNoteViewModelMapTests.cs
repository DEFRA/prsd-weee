namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Linq;
    using System.Security.Principal;
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
            var user = A.Fake<IPrincipal>();
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var transfer = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceData, displayNotification, user);
      
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

        [Fact]
        public void Map_GivenValidEvidenceNoteData_ShouldContainHintItems()
        {
            //arrange
            var schemeId = TestFixture.Create<Guid>();
            TransferEvidenceNoteData note = TestFixture.Create<TransferEvidenceNoteData>();
            ViewTransferNoteViewModelMapTransfer transfer = new ViewTransferNoteViewModelMapTransfer(schemeId, note, null);

            //act
            var result = map.Map(transfer);

            // assert
            result.HintItems.Count.Should().Be(3);
            result.HintItems.ElementAt(0).Key.Should().Be("Approve evidence note transfer");
            result.HintItems.ElementAt(0).Value.Should().BeNull();
            result.HintItems.ElementAt(1).Key.Should().Be("Reject evidence note transfer");
            result.HintItems.ElementAt(1).Value.Should().Be("Reject an evidence note transfer if the evidence has been sent to you by mistake or if there is a large number of updates to make that it is quicker to create a new evidence note transfer");
            result.HintItems.ElementAt(2).Key.Should().Be("Return evidence note transfer");
            result.HintItems.ElementAt(2).Value.Should().Be("Return an evidence note transfer if there are some minor updates");
        }

        [Fact]
        public void Map_GivenAMappedTransferNoteViewModel_MustReturnMappedViewModel()
        {
            //arrange
            DateTime today = DateTime.UtcNow;
            note.ApprovedDate = today;
            note.RejectedDate = today;
            note.ReturnedDate = today;
            note.RejectedReason = "rejected reason";
            note.ReturnedReason = "returned reason";
            var transfer = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(), note, null);

            var mapper = new ViewTransferNoteViewModelMap(A.Fake<IAddressUtilities>(), A.Fake<ITonnageUtilities>());

            //act
            var result = mapper.Map(transfer);

            // assert 
            result.ApprovedDate.Should().Be(((DateTime?)today).ToDisplayGMTDateTimeString());
            result.RejectedDate.Should().Be(((DateTime?)today).ToDisplayGMTDateTimeString());
            result.RejectedReason.Should().Be("rejected reason");
            result.ReturnedDate.Should().Be(((DateTime?)today).ToDisplayGMTDateTimeString());
            result.ReturnedReason.Should().Be("returned reason");
        }
    }
}
