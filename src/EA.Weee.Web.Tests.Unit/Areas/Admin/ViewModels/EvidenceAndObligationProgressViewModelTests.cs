namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.SchemeReports;
    using Xunit;

    public class EvidenceAndObligationProgressViewModelTests
    {
        [Theory]
        [InlineData("SelectedYear", "Compliance year")]
        [InlineData("SelectedSchemeId", "PCS")]
        public void EvidenceAndObligationProgressViewModel_PropertiesShouldHaveDisplayNameAttribute(string property, string display)
        {
            typeof(EvidenceAndObligationProgressViewModel).GetProperty(property).Should()
                .BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Should().Be(display);
        }

        [Theory]
        [InlineData("SelectedYear")]
        public void EvidenceAndObligationProgressViewModel_PropertiesShouldHaveRequiredAttribute(string property)
        {
            typeof(EvidenceAndObligationProgressViewModel).GetProperty(property).Should()
                .BeDecoratedWith<RequiredAttribute>();
        }
    }
}
