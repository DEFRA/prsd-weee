namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class ReviewTransferNoteViewModelTests
    {
        [Theory]
        [InlineData("Approve evidence note transfer", NoteStatus.Approved)]
        [InlineData("Reject evidence note transfer", NoteStatus.Rejected)]
        [InlineData("Return evidence note transfer", NoteStatus.Returned)]
        public void SelectedEnumValue_GivenSelectedValue_CorrectValueShouldBeReturned(string value, NoteStatus expectedStatus)
        {
            //arrange
            var model = new ReviewTransferNoteViewModel()
            {
                SelectedValue = value
            };

            //act
            var result = model.SelectedEnumValue;

            //assert
            result.Should().Be(expectedStatus);
        }

        [Fact]
        public void SelectedEnumValue_GivenSelectedValue_Unknown_InvalidOperationExceptionExpected()
        {
            //arrange
            var model = new ReviewTransferNoteViewModel()
            {
                SelectedValue = "unknown"
            };

            //act
            var exception = Record.Exception(() => model.SelectedEnumValue);

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void SelectedValue_ShouldBeDecoratedWithRequiredAttribute()
        {
            typeof(ReviewTransferNoteViewModel).GetProperty("SelectedValue").Should()
                .BeDecoratedWith<RequiredAttribute>(r =>
                    r.ErrorMessage.Equals("Select whether you want to approve, reject or return the evidence note transfer"));
        }

        [Fact]
        public void Reason_ShouldBeDecoratedWithDisplayAttribute()
        {
            typeof(ReviewTransferNoteViewModel).GetProperty("Reason").Should()
                .BeDecoratedWith<DisplayNameAttribute>(r =>
                    r.DisplayName.Equals("What is the reason you are rejecting or returning the evidence note transfer?"));
        }

        [Fact]
        public void ReviewEvidenceNoteViewModel_ShouldBeInitialisedWithPossibleValues()
        {
            //act
            var model = new ReviewTransferNoteViewModel();

            //assert
            model.PossibleValues.ElementAt(0).Should().Be("Approve evidence note transfer");
        }

        [Fact]
        public void ReviewEvidenceNoteViewModel_ShouldBeInitialisedWithHintItems()
        {
            //act
            var model = new ReviewTransferNoteViewModel();

            //assert
            model.HintItems.ElementAt(0).Value.Should().Be(null);
            model.HintItems.ElementAt(0).Key.Should().Be("Approve evidence note transfer");
            model.HintItems.ElementAt(1).Value.Should().Be("Reject an evidence note transfer if the evidence has been sent to you by mistake or if there is a large number of updates to make that it is quicker to create a new evidence note transfer");
            model.HintItems.ElementAt(1).Key.Should().Be("Reject evidence note transfer");
            model.HintItems.ElementAt(2).Value.Should().Be("Return an evidence note transfer if there are some minor updates");
            model.HintItems.ElementAt(2).Key.Should().Be("Return evidence note transfer");
        }

        [Theory]
        [InlineData("Reject evidence note transfer")]
        [InlineData("Return evidence note transfer")]
        public void Validate_GivenNotApproved_ValidationResultShouldBeEmpty(string value)
        {
            //arrange
            var model = new ReviewTransferNoteViewModel()
            {
                SelectedValue = value
            };

            //act
            var result = model.Validate(new ValidationContext(model));

            //assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_GivenApprovedAndReasonIsEmpty_ValidationResultShouldBeEmpty(string reason)
        {
            //arrange
            var model = new ReviewTransferNoteViewModel()
            {
                SelectedValue = "Approve evidence note transfer",
                Reason = reason
            };

            //act
            var result = model.Validate(new ValidationContext(model));

            //assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("reason")]
        public void Validate_GivenApprovedAndReasonIsNotEmpty_ValidationResultShouldHaveCorrectResult(string reason)
        {
            //arrange
            var model = new ReviewTransferNoteViewModel()
            {
                SelectedValue = "Approve evidence note transfer",
                Reason = reason
            };

            //act
            var result = model.Validate(new ValidationContext(model));

            //assert
            result.Count().Should().Be(1);
            result.Should().Contain(v =>
                v.ErrorMessage.Equals(
                    "A reason can only be entered if you are rejecting or returning the evidence note transfer. Delete any text you have entered.") &&
                v.MemberNames.Contains("reason"));
        }
    }
}
