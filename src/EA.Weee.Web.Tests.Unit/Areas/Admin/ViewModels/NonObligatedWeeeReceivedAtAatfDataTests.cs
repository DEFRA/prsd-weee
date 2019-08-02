namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Xunit;

    public class NonObligatedWeeeReceivedAtAatfDataTests
    {
        [Fact]
        public void NonObligatedWeeeReceivedAtAatfViewModel_ComplianceYear_ShouldHaveRequiredAttribute()
        {
            typeof(NonObligatedWeeeReceivedAtAatfViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void NonObligatedWeeeReceivedAtAatfViewModel_ComplianceYear__ShouldHaveDisplayAttribute()
        {
            typeof(NonObligatedWeeeReceivedAtAatfViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Should().Be("Compliance year");
        }

        [Fact]
        public void NonObligatedWeeeReceivedAtAatfViewModel_OrganisationName_ShouldHaveDisplayAttribute()
        {
            typeof(NonObligatedWeeeReceivedAtAatfViewModel).GetProperty("OrganisationName").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Should().Be("AATF name");
        }
    }
}
