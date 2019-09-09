namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.Linq;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Web.Areas.Admin.ViewModels.Reports;
    using Xunit;

    public class ChooseAatfReportViewModelTests
    {
        [Fact]
        public void ChooseAatfReportViewModelTests_ShouldHaveValidOptions()
        {
            var model = new ChooseAatfReportViewModel();

            model.PossibleValues.Count.Should().Be(8);
            model.PossibleValues.ElementAt(0).Should().Be(Reports.AatfObligatedData);
            model.PossibleValues.ElementAt(1).Should().Be(Reports.AatfReuseSitesData);
            model.PossibleValues.ElementAt(2).Should().Be(Reports.AatfSentOnData);
            model.PossibleValues.ElementAt(3).Should().Be(Reports.AatfNonObligatedData);
            model.PossibleValues.ElementAt(4).Should().Be(Reports.UkWeeeDataAtAatfs);
            model.PossibleValues.ElementAt(5).Should().Be(Reports.UkNonObligatedWeeeData);
            model.PossibleValues.ElementAt(6).Should().Be(Reports.AatfAePublicRegister);
            model.PossibleValues.ElementAt(7).Should().Be(Reports.AatfAeReturnData);
        }
    }
}
