namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Constant;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Web.ViewModels.Shared;
    using Xunit;

    public class TransferEvidenceNotesViewModelTests
    {
        [Fact]
        public void TransferEvidenceNotesViewModel_ShouldImplement_IValidatableObject()
        {
            typeof(TransferEvidenceNotesViewModel).Should().Implement<IValidatableObject>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModel_Constructor_ShouldInitialiseLists()
        {
            //act
            var result = new TransferEvidenceNotesViewModel();

            //assert
            result.EvidenceNotesDataList.Should().NotBeNull();
            result.CategoryValues.Should().NotBeNull();
        }

        [Fact]
        public void Validate_GivenNoNotesSelected_ValidationResultExpected()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel();

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().Contain(v =>
                v.ErrorMessage.Equals("Select at least one evidence note to transfer from")
                && v.MemberNames.Contains(ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError));
        }

        [Fact]
        public void Validate_GivenMoreThanFiveNotesSelected_ValidationResultExpected()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel()
            {
                EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>()
                {
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel()
                }
            };

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().Contain(v =>
                v.ErrorMessage.Equals("You cannot select more than 5 notes")
                && v.MemberNames.Contains(ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError));
        }

        [Fact]
        public void Validate_GivenFivesNotesSelected_ValidationResultShouldBeEmpty()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel()
            {
                EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>()
                {
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel()
                }
            };

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenSingleNoteSelected_ValidationResultShouldBeEmpty()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel()
            {
                EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>()
                {
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel(),
                    new ViewEvidenceNoteViewModel()
                }
            };

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().BeEmpty();
        }
    }
}
