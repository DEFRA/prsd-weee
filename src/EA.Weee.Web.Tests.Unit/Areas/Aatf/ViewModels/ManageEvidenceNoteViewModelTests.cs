namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using EA.Weee.Web.ViewModels.Shared;
    using FluentAssertions;
    using Xunit;

    public class ManageEvidenceNoteViewModelTests
    {
        [Fact]
        public void ManageEvidenceNoteViewModel_Constructor_ShouldInitialiseFilterViewModel()
        {
            //act
            var model = new ManageEvidenceNoteViewModel();

            //assert
            model.FilterViewModel.Should().NotBeNull();
        }

        [Fact]
        public void ManageEvidenceNoteViewModel_Constructor_ShouldInitialiseRecipientWasteStatusViewModel()
        {
            //act
            var model = new ManageEvidenceNoteViewModel();

            //assert
            model.RecipientWasteStatusFilterViewModel.Should().NotBeNull();
        }

        [Fact]
        public void ManageEvidenceNoteViewModel_Constructor_ShouldInitialiseSubmittedDatesViewModel()
        {
            //act
            var model = new ManageEvidenceNoteViewModel();

            //assert
            model.SubmittedDatesFilterViewModel.Should().NotBeNull();
        }
    }
}
