namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.Linq;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.EvidenceReports;
    using Web.Areas.Admin.ViewModels.Reports;
    using Xunit;

    public class ChooseEvidenceReportViewModelTests
    {
        [Fact]
        public void ChooseEvidenceReportViewModel_ShouldHaveValidOptions()
        {
            var model = new ChooseEvidenceReportViewModel();

            model.PossibleValues.Count.Should().Be(2);
            model.PossibleValues.ElementAt(0).Should().Be(Reports.EvidenceNotesReports);
            model.PossibleValues.ElementAt(1).Should().Be(Reports.EvidenceNotesReports);
        }
    }
}
