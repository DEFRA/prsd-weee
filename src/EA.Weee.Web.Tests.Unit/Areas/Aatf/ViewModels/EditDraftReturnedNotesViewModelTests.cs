namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class EditDraftReturnedNotesViewModelTests
    {
        [Fact]
        public void EditDraftReturnedNotesViewModel_Constructor_ShouldSetTab()
        {
            var model = new EditDraftReturnedNotesViewModel();

            model.ActiveOverviewDisplayOption.Should().Be(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes);
        }

        [Fact]
        public void EditDraftReturnedNotesViewModel_Constructor_ShouldInitialiseList()
        {
            var model = new EditDraftReturnedNotesViewModel();

            model.EvidenceNotesDataList.Should().NotBeNull();
            model.EvidenceNotesDataList.Should().BeEmpty();
        }
    }
}
