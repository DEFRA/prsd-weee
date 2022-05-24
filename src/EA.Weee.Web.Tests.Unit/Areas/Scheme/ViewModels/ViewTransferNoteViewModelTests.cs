namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System.ComponentModel;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
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
            result.Should().Be(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void RedirectTab_GivenNoteStatusIsNotDraft_TabShouldBeViewAndTransferEvidence(NoteStatus status)
        {
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
    }
}
