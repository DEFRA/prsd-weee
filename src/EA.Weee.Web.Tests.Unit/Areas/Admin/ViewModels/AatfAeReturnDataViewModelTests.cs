namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Xunit;

    public class AatfAeReturnDataViewModelTests
    {
        private readonly AatfAeReturnDataViewModel model;

        public AatfAeReturnDataViewModelTests()
        {
            model = new AatfAeReturnDataViewModel();
        }

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
            typeof(AatfAeReturnDataViewModel).GetProperty("SelectedFacilityType").Should().BeDecoratedWith<RequiredAttribute>().Which.ErrorMessage.Should().Be("Select AATF or AE");
        }

        [Fact]
        public void AatfAeReturnDataViewModel_IncludeResubmissions_ShouldHaveRequiredAttribute()
        {
            typeof(AatfAeReturnDataViewModel).GetProperty("IncludeResubmissions").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void AatfAeReturnDataViewModel_SubmissionStatus_ShouldHaveValidValues()
        {
            model.SubmissionStatus.ElementAt(0).Should().BeEquivalentTo(new SelectListItem() { Text = "Submitted", Value = "2" });
            model.SubmissionStatus.ElementAt(1).Should().BeEquivalentTo(new SelectListItem() { Text = "Started", Value = "1" });
            model.SubmissionStatus.ElementAt(2).Should().BeEquivalentTo(new SelectListItem() { Text = "Not Started", Value = "0" });
        }

        [Fact]
        public void AatfAeReturnDataViewModel_IncludeResubmissionsOptions_ShouldHaveValidValues()
        {
            model.IncludeResubmissionsOptions.ElementAt(0).Should().BeEquivalentTo(new SelectListItem() { Text = "Exclude resubmissions", Value = bool.FalseString, Selected = true });
            model.IncludeResubmissionsOptions.ElementAt(1).Should().BeEquivalentTo(new SelectListItem() { Text = "Include resubmissions", Value = bool.TrueString });
        }

        [Fact]
        public void AatfAeReturnDataViewModel_Quarters_ShouldHaveValidValues()
        {
            model.Quarters.ElementAt(0).Should().BeEquivalentTo(new SelectListItem() { Text = "Q1", Value = "1" });
            model.Quarters.ElementAt(1).Should().BeEquivalentTo(new SelectListItem() { Text = "Q2", Value = "2" });
            model.Quarters.ElementAt(2).Should().BeEquivalentTo(new SelectListItem() { Text = "Q3", Value = "3" });
            model.Quarters.ElementAt(3).Should().BeEquivalentTo(new SelectListItem() { Text = "Q4", Value = "4" });
        }
    }
}
