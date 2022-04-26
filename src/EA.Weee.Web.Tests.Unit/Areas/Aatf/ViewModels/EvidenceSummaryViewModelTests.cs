namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class EvidenceSummaryViewModelTests
    {
        [Fact]
        public void EvidenceSummaryViewModel_Constructor_ShouldSetTab()
        {
            var model = new EvidenceSummaryViewModel();

            model.ActiveOverviewDisplayOption.Should().Be(ManageEvidenceOverviewDisplayOption.EvidenceSummary);
        }

        [Fact]
        public void EvidenceSummaryViewModel_Constructor_ShouldInitialiseList()
        {
            var model = new EvidenceSummaryViewModel();

            model.EvidenceNotesDataList.Should().NotBeNull();
            model.EvidenceNotesDataList.Should().BeEmpty();
        }
    }
}
