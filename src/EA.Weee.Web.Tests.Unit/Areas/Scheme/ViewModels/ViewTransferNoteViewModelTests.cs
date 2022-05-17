namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System.ComponentModel;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
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
    }
}
