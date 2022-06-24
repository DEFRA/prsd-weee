namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System.ComponentModel;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared;
    using Xunit;

    public class ViewTransferNoteViewModelTests
    {
        [Fact]
        public void ViewTransferNoteViewModel_ReferenceDisplay_ShouldHaveDisplayAttribute()
        {
            typeof(ViewTransferNoteViewModel).GetProperty("ReferenceDisplay").Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Reference ID"));
        }

        [Fact]
        public void ViewTransferNoteViewModel_ShouldBeDerivedFrom_ViewEvidenceNoteViewModel()
        {
            typeof(ViewTransferNoteViewModel).Should().BeDerivedFrom<ViewEvidenceNoteViewModel>();
        }

        [Fact]
        public void ReferenceDisplay_GivenReferenceAndType_ReferenceDisplayShouldBetValid()
        {
            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                Reference = 1,
                Type = NoteType.Transfer
            };

            //assert
            model.ReferenceDisplay.Should().Be("T1");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void DisplayMessage_GivenEmptySuccessMessage_ShouldReturnFalse(string value)
        {
            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                SuccessMessage = value
            };

            //act
            var result = model.DisplayMessage;

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void DisplayMessage_GivenSuccessMessage_ShouldReturnTrue()
        {
            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                SuccessMessage = "a"
            };

            //act
            var result = model.DisplayMessage;

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void RedirectTab_GivenNoteStatusIsDraft_TabShouldBeViewAndTransferEvidence()
        {
            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                Status = NoteStatus.Draft
            };

            //act
            var result = model.RedirectTab;

            //assert
            result.Should().Be(ManageEvidenceNotesDisplayOptions.OutgoingTransfers.ToDisplayString());
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void RedirectTab_GivenNoteStatusIsNotDraft_TabShouldBeViewAndTransferEvidence(NoteStatus status)
        {
            if (status == NoteStatus.Draft)
            {
                return;
            }

            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                Status = status
            };

            //act
            var result = model.RedirectTab;

            //assert
            result.Should().Be(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void DisplayEditButton_GivenNoteStatusIsNotDraft_ShouldBeFalse(NoteStatus status)
        {
            if (status == NoteStatus.Draft || status == NoteStatus.Returned)
            {
                return;
            }

            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                Status = status
            };

            //act
            var result = model.DisplayEditButton;

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void DisplayEditButton_GivenNoteStatusIsDraft_ShouldBeTrue()
        {
            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                Status = NoteStatus.Draft
            };

            //act
            var result = model.DisplayEditButton;

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ComplianceYearDisplay_Has_Correct_Attribute()
        {
            typeof(ViewTransferNoteViewModel).GetProperty("ComplianceYearDisplay").Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Compliance year"));
        }

        [Theory]
        [InlineData(NoteStatus.Draft, "Draft evidence note")]
        [InlineData(NoteStatus.Void, "")]
        [InlineData(NoteStatus.Rejected, "Rejected evidence note")]
        [InlineData(NoteStatus.Returned, "Returned evidence note")]
        [InlineData(NoteStatus.Submitted, "Submitted evidence note")]
        [InlineData(NoteStatus.Approved, "Approved evidence note")]
        public void TabName_CorrectlyReturnsTabName_GivenStatus(NoteStatus status, string expectedTabName)
        {
            //arrange
            var model = new ViewTransferNoteViewModel()
            {
                Status = status
            };

            //act
            var result = model.TabName;

            //assert
            result.Should().Be(expectedTabName);
        }
    }
}
