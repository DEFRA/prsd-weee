namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Web.Areas.Admin.ViewModels.Reports;
    using Xunit;
    public class AatfObligatedDataViewModelTests
    {
        private AatfObligatedDataViewModel model;

        public AatfObligatedDataViewModelTests()
        {
            model = new AatfObligatedDataViewModel();
        }

        [Fact]
        public void AatfObligatedDataViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfObligatedDataViewModel_Quarter_ShouldHaveRequiredAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("SelectedColumn").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfObligatedDataViewModel_PanArea_ShouldHaveDisplayAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("PanAreaId").Should().BeDecoratedWith<DisplayAttribute>().Which.Name.Should().Be("WROS Pan Area Team");
        }

        [Fact]
        public void AatfObligatedDataViewModel_AppropriateAuthority_ShouldHaveDisplayAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("CompetentAuthorityId").Should().BeDecoratedWith<DisplayAttribute>().Which.Name.Should().Be("Appropriate authority");
        }

        [Fact]
        public void AatfObligatedDataViewModel_AatfName_ShouldHaveDisplayAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("AATFName").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Should().Be("AATF name");
        }

        [Fact]
        public void AatfObligatedDataViewModel_ObligationTypes_ShouldHaveValidValues()
        {
            model.ObligationTypes.ElementAt(0).Should().BeEquivalentTo(new SelectListItem() {Text = "B2B" });
            model.ObligationTypes.ElementAt(1).Should().BeEquivalentTo(new SelectListItem() {Text = "B2C" });
        }

        [Fact]
        public void AatfObligatedDataViewModel_SchemeColumnPossibleValues_ShouldHaveValidValues()
        {
            model.SchemeColumnPossibleValues.ElementAt(0).Should().BeEquivalentTo(new SelectListItem() { Text = "PCS names", Value = "1" });
            model.SchemeColumnPossibleValues.ElementAt(1).Should().BeEquivalentTo(new SelectListItem() { Text = "Approval numbers", Value = "2" });
        }

        [Fact]
        public void AatfObligatedDataViewModel_SelectedColumn_ShouldHaveDisplayAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("SelectedColumn").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Should().Be("Select a column name");
        }

        [Fact]
        public void AatfObligatedDataViewModel_SelectedColumn_ShouldHaveRequiredAttribute()
        {
            typeof(AatfObligatedDataViewModel).GetProperty("SelectedColumn").Should().BeDecoratedWith<RequiredAttribute>().Which.ErrorMessage.Should().Be("You must tell us if you want PCS names or Approval numbers as column headings in the report");
        }
    }
}
