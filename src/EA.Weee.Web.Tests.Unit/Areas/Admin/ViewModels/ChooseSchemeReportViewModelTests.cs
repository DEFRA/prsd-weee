namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.Linq;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.Reports;
    using Web.Areas.Admin.ViewModels.SchemeReports;
    using Xunit;

    public class ChooseSchemeReportViewModelTests
    {
        [Fact]
        public void ChooseSchemeReportViewModel_ShouldHaveValidOptions()
        {
            var model = new ChooseSchemeReportViewModel();

            model.PossibleValues.Count.Should().Be(9);
            model.PossibleValues.ElementAt(0).Should().Be(Reports.ProducerDetails);
            model.PossibleValues.ElementAt(1).Should().Be(Reports.ProducerEeeData);
            model.PossibleValues.ElementAt(2).Should().Be(Reports.SchemeWeeeData);
            model.PossibleValues.ElementAt(3).Should().Be(Reports.UkEeeData);
            model.PossibleValues.ElementAt(4).Should().Be(Reports.UkWeeeData);
            model.PossibleValues.ElementAt(5).Should().Be(Reports.ProducerPublicRegister);
            model.PossibleValues.ElementAt(6).Should().Be(Reports.SchemeObligationData);
            model.PossibleValues.ElementAt(7).Should().Be(Reports.MissingProducerData);
            model.PossibleValues.ElementAt(8).Should().Be(Reports.PcsEvidenceAndObligationProgressData);
        }
    }
}
