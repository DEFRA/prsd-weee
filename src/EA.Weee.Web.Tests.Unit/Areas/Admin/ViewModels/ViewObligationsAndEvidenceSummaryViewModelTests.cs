namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FluentAssertions;
    using Xunit;

    public class ViewObligationsAndEvidenceSummaryViewModelTests
    {
        [Fact]
        public void ViewObligationsAndEvidenceSummaryViewModelImplementsIObligationSummaryBase()
        {
            typeof(ViewObligationsAndEvidenceSummaryViewModel).Should().Implement<IObligationSummaryBase>();
        }
    }
}
