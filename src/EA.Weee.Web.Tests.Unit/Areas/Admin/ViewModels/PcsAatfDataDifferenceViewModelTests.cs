namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Xunit;

    public class PcsAatfDataDifferenceViewModelTests
    {
        [Fact]
        public void PcsAatfDataDifferenceViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(PcsAatfDataDifferenceViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }
    }
}
