namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Xunit;

    public class AatfReuseSitesViewModelTests
    {
        [Fact]
        public void AatfReuseSitesViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(AatfReuseSitesViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }
    }
}
