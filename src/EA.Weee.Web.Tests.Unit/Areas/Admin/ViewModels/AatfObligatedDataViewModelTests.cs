namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.Reports;
    using Xunit;
    public class AatfObligatedDataViewModelTests
    {
        [Fact]
        public void AatfAeReturnDataViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfAeReturnDataViewModel_Quarter_ShouldHaveRequiredAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("SelectedColumn").Should().BeDecoratedWith<RequiredAttribute>();
        }
    }
}
