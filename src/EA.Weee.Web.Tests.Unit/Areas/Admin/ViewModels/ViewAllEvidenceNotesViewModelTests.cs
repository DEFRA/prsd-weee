namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class ViewAllEvidenceNotesViewModelTests
    {
        [Fact]
        public void CheckViewAllEvidenceNotesViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(ViewAllEvidenceNotesViewModel).BaseType.Name.Should().Be(nameof(ManageEvidenceNotesViewModel));
        }
    }
}
