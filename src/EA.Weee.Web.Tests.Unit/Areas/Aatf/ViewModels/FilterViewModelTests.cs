namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System.ComponentModel;
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class FilterViewModelTests
    {
        [Fact]
        public void FilterViewModel_SearchRef_ShouldHaveDisplayAttribute()
        {
            typeof(FilterViewModel)
                .GetProperty("SearchRef")
                .Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Search by reference ID"));
        }
    }
}
