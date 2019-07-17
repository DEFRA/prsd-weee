namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.Reports;
    using Xunit;

    public class AatfAeReturnDataViewModelTests
    {
        [Fact]
        public void AatfAeReturnDataViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(AatfAeReturnDataViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfAeReturnDataViewModel_Quarter_ShouldHaveRequiredAttribute()
        {
            typeof(AatfAeReturnDataViewModel).GetProperty("Quarter").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfAeReturnDataViewModel_SelectedFacilityType_ShouldHaveRequiredAttribute()
        {
            typeof(AatfAeReturnDataViewModel).GetProperty("SelectedFacilityType").Should().BeDecoratedWith<RequiredAttribute>().Which.ErrorMessage.Equals("Enter AATF or AE");
        }
    }
}
