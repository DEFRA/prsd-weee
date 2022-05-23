namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class ViewTransferEvidenceViewModelTests
    {
        [Fact]
        public void ViewAndTransferEvidenceViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(SchemeViewAndTransferManageEvidenceSchemeViewModel).Should().BeDerivedFrom<ManageEvidenceNoteSchemeViewModel>();
        }
    }
}
