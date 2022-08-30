namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class SummaryEvidenceViewModelTests
    {
        [Fact]
        public void SummaryEvidenceViewModelInheritsManageEvidenceNoteViewModel()
        {
            typeof(SummaryEvidenceViewModel).Should().BeDerivedFrom<ManageEvidenceNoteSchemeViewModel>();
        }

        [Fact]
        public void SummaryEvidenceViewModelImplementsIObligationSummaryBase()
        {
            typeof(SummaryEvidenceViewModel).Should().Implement<IObligationSummaryBase>();
        }
    }
}
