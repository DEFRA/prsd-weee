namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.EvidenceReports;
    using Xunit;

    public class EvidenceReportViewModelTests
    {
        [Theory]
        [InlineData("SelectedYear", "Compliance year")]
        [InlineData("SelectedTonnageToDisplay", "Tonnage values")]
        public void EvidenceReportViewModel_PropertiesShouldHaveDisplayNameAttribute(string property, string display)
        {
            typeof(EvidenceReportViewModel).GetProperty(property).Should()
                .BeDecoratedWith<DisplayAttribute>(e => e.Name.Equals(display));
        }

        [Theory]
        [InlineData("SelectedYear", "Select a compliance year")]
        [InlineData("SelectedTonnageToDisplay", "Select whether you want to view the original tonnages or net of transfers")]
        public void EvidenceReportViewModel_PropertiesShouldHaveRequiredAttribute(string property, string message)
        {
            typeof(EvidenceReportViewModel).GetProperty(property).Should()
                .BeDecoratedWith<RequiredAttribute>(e => e.ErrorMessage.Equals(message));
        }
    }
}
