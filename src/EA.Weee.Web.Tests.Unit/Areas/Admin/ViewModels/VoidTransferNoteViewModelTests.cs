namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Xunit;

    public class VoidTransferNoteViewModelTests
    {
        [Fact]
        public void VoidTransferNoteViewModel_ShouldDeriveFromVoidNoteViewModelBase()
        {
            typeof(VoidTransferNoteViewModel).Should().BeDerivedFrom<VoidNoteViewModelBase>();
        }
    }
}
