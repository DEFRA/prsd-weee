namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class ReviewSubmittedEvidenceNotesViewModelTests
    {
        [Fact]
        public void ReviewSubmittedEvidenceNotesViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(ReviewSubmittedManageEvidenceNotesSchemeViewModel).Should().BeDerivedFrom<ManageManageEvidenceNoteSchemeViewModel>();
        }
    }
}
