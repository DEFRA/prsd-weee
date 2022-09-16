namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Constant;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class TransferSelectEvidenceNoteModelTests
    {
        [Fact]
        public void TransferSelectEvidenceNoteModel_ShouldDeriveFromTransferSelectEvidenceNoteModelBase()
        {
            typeof(TransferSelectEvidenceNoteModel).Should().BeDerivedFrom<TransferSelectEvidenceNoteModelBase>();
        }

        [Fact]
        public void TransferSelectEvidenceNoteModel_ShouldImplementIValidatableObject()
        {
            typeof(TransferSelectEvidenceNoteModel).Should().Implement<IValidatableObject>();
        }

        [Fact]
        public void Validate_GivenNumberOfSelectedNotesIsLessThan5_ValidationResultShouldBeSuccess()
        {
            //arrange
            var model = new TransferSelectEvidenceNoteModel() { NumberOfSelectedNotes = 4 };

            //assert
            var result = model.Validate(new ValidationContext(model));

            //assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        public void Validate_GivenNumberOfSelectedNotesMoreOrEqualTo5_ValidationResultShouldFail(int numberOfSelectedNotes)
        {
            //arrange
            var model = new TransferSelectEvidenceNoteModel() { NumberOfSelectedNotes = numberOfSelectedNotes };

            //assert
            var result = model.Validate(new ValidationContext(model));

            //assert
            result.Should().ContainEquivalentOf(new ValidationResult("You cannot select more than 5 notes",
                new[] { ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError }));
        }

        [Fact]
        public void NewPage_GivenPageIsGreaterThan1AndPageCountIs1_PreviousPageShouldBeReturned()
        {
            //arrange & act
            var model = new TransferSelectEvidenceNoteModel() { Page = 3, PageCount = 1 };

            //assert
            model.NewPage.Should().Be(2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void NewPage_GivenPageIsNotGreaterThan1AndPageCountIs1_PageShouldBeReturned(int page)
        {
            //arrange & act
            var model = new TransferSelectEvidenceNoteModel() { Page = page, PageCount = 1 };

            //assert
            model.NewPage.Should().Be(1);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(0)]
        public void NewPage_GivePageCountIsNot1_PageShouldBeReturned(int pageCount)
        {
            //arrange & act
            var model = new TransferSelectEvidenceNoteModel() { Page = 3, PageCount = pageCount };

            //assert
            model.NewPage.Should().Be(3);
        }
    }
}
