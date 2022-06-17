namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class TransferredOutEvidenceNotesSchemeViewModelTests
    {
        [Fact]
        public void CheckTransferredOutEvidenceNotesSchemeViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(TransferredOutEvidenceNotesSchemeViewModel).BaseType.Name.Should().Be(nameof(ManageEvidenceNoteSchemeViewModel));
        }
    }
}
