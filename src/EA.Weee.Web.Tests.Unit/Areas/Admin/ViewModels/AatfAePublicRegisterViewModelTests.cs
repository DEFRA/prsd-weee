namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Xunit;

    public class AatfAePublicRegisterViewModelTests
    {
        [Fact]
        public void AatfAePublicRegisterViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(AatfAePublicRegisterViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfAePublicRegisterViewModel_AppropriateAuthority_ShouldHaveDisplayAttribute()
        {
            typeof(AatfAePublicRegisterViewModel).GetProperty("CompetentAuthorityId").Should().BeDecoratedWith<DisplayAttribute>().Which.Name.Should().Be("Appropriate authority");
        }

        [Fact]
        public void AatfAePublicRegisterViewModel_SelectedFacilityType_ShouldHaveRequiredAttribute()
        {
            typeof(AatfAePublicRegisterViewModel).GetProperty("SelectedFacilityType").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfAePublicRegisterViewModel_SelectedFacilityType_ShouldHaveDisplayAttribute()
        {
            typeof(AatfAePublicRegisterViewModel).GetProperty("SelectedFacilityType").Should().BeDecoratedWith<DisplayAttribute>().Which.Name.Should().Be("AATF or AE");
        }
    }
}
