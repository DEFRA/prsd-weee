namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
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
    }
}
