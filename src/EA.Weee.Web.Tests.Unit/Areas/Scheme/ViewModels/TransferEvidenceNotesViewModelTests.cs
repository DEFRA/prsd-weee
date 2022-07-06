namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
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
            result.SelectedEvidenceNotePairs.Should().NotBeNull();
            result.TotalCategoryValues.Should().NotBeNull();
        }

        [Fact]
        public void Validate_GivenNoNotesSelected_ValidationResultExpected()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel();
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>());

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().Contain(v =>
                v.ErrorMessage.Equals("Select at least one evidence note to transfer from")
                && v.MemberNames.Contains("SelectedEvidenceNotePairs"));
        }

        [Fact]
        public void Validate_GivenMoreThanFiveNotesSelected_ValidationResultExpected()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel();
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), false));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), false));

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().Contain(v =>
                v.ErrorMessage.Equals("You cannot select more than 5 notes")
                && v.MemberNames.Contains("SelectedEvidenceNotePairs"));
        }

        [Fact]
        public void Validate_GivenFivesNotesSelected_ValidationResultShouldBeEmpty()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel();
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenSingleNoteSelected_ValidationResultShouldBeEmpty()
        {
            //arrange
            var result = new TransferEvidenceNotesViewModel();
            result.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(Guid.NewGuid(), true));

            //act
            var validationResults = result.Validate(new ValidationContext(result));

            //assert
            validationResults.Should().BeEmpty();
        }
    }
}
