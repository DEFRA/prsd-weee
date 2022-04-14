namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class ReviewSubmittedEvidenceNotesViewModelTests
    {
        [Fact]
        public void CheckReviewSubmittedEvidenceNotesViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(ReviewSubmittedEvidenceNotesViewModel).BaseType.Name.Should().Be(nameof(ManageEvidenceNoteViewModel));
        }
    }
}
