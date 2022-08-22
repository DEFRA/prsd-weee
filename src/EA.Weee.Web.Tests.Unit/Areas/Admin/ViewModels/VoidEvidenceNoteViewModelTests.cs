namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Xunit;

    public class VoidEvidenceNoteViewModelTests
    {
        [Fact]
        public void VoidEvidenceNoteViewModel_ShouldDeriveFromVoidNoteViewModelBase()
        {
            typeof(VoidEvidenceNoteViewModel).Should().BeDerivedFrom<VoidNoteViewModelBase>();
        }
    }
}
