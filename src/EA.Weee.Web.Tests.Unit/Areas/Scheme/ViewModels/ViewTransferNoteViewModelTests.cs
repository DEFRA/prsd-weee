namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using System.ComponentModel;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.ViewModels.Shared;
    using Xunit;

    public class ViewTransferNoteViewModelTests
    {
        [Fact]
        public void ViewTransferNoteViewModel_ShouldHaveSerializableAttribute()
        {
            typeof(ViewTransferNoteViewModel).Should().BeDecoratedWith<SerializableAttribute>();
        }

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
        public void ComplianceYearDisplay_Has_Correct_Attribute()
        {
            typeof(ViewTransferNoteViewModel).GetProperty("ComplianceYearDisplay").Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Compliance year"));
        }

        [Theory]
        [InlineData(NoteStatus.Draft, "Draft evidence note transfer")]
        [InlineData(NoteStatus.Void, "")]
        [InlineData(NoteStatus.Rejected, "Rejected evidence note transfer")]
        [InlineData(NoteStatus.Returned, "Returned evidence note transfer")]
        [InlineData(NoteStatus.Submitted, "Submitted evidence note transfer")]
        [InlineData(NoteStatus.Approved, "Approved evidence note transfer")]
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
