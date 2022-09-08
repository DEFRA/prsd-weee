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
    }
}
