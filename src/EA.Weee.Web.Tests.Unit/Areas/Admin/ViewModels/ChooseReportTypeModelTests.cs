namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.Linq;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.Reports;
    using Web.Areas.Admin.ViewModels.SchemeReports;
    using Xunit;

    public class ChooseReportTypeModelTests
    {
        [Fact]
        public void ChooseReportTypeModel_ShouldHaveValidOptions()
        {
            var model = new ChooseReportTypeModel();

            model.PossibleValues.Count.Should().Be(4);
            model.PossibleValues.ElementAt(0).Should().Be(Reports.PcsReports);
            model.PossibleValues.ElementAt(1).Should().Be(Reports.AatfReports);
            model.PossibleValues.ElementAt(2).Should().Be(Reports.PcsAatfDataDifference);
            model.PossibleValues.ElementAt(3).Should().Be(Reports.AatfAeDetails);
            model.PossibleValues.ElementAt(4).Should().Be(Reports.EvidenceNotesReports);
        }
    }
}
