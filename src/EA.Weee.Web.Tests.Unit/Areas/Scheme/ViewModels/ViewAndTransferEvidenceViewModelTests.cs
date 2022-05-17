namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class ViewAndTransferEvidenceViewModelTests
    {
        [Fact]
        public void CheckViewAndTransferEvidenceViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(SchemeViewAndTransferManageEvidenceSchemeViewModel).BaseType.Name.Should().Be(nameof(ManageManageEvidenceNoteSchemeViewModel));
        }
    }
}
