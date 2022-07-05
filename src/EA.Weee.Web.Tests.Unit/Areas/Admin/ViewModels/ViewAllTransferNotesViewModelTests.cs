namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class ViewAllTransferNotesViewModelTests
    {
        [Fact]
        public void CheckViewAllEvidenceTransfersNotesViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(ViewAllTransferNotesViewModel).BaseType.Name.Should().Be(nameof(ManageEvidenceNotesViewModel));
        }
    }
}
