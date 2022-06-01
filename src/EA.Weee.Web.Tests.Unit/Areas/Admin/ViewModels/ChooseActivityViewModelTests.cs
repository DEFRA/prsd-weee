namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.Home;
    using Xunit;

    public class ChooseActivityViewModelTests
    {
        [Fact]
        public void SelectedValue_ShouldHaveRequiredAttribute()
        {
            typeof(ChooseActivityViewModel).GetProperty("SelectedValue").Should()
                .BeDecoratedWith<RequiredAttribute>(r =>
                    r.ErrorMessage.Equals("Select the activity you would like to do"));
        }
    }
}
