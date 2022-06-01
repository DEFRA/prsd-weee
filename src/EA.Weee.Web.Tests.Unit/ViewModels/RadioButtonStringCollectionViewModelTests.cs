namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.ViewModels.Shared;
    using Xunit;

    public class RadioButtonStringCollectionViewModelTests
    {
        [Fact]
        public void SelectedValue_ShouldHaveRequiredAttribute()
        {
            typeof(RadioButtonStringCollectionViewModel).GetProperty("SelectedValue").Should()
                .BeDecoratedWith<RequiredAttribute>(r =>
                    r.ErrorMessage.Equals("Please answer this question"));
        }
    }
}
