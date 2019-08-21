namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Xunit;
    public class AatfSentOnDataViewModelTests
    {
        [Fact]
        public void AatfSentOnDataViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(AatfSentOnDataViewModel).GetProperty("SelectedYear").Should().BeDecoratedWith<RequiredAttribute>();
        }
    }
}
