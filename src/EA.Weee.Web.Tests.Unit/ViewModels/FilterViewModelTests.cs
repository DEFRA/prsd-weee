namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System.ComponentModel;
    using EA.Weee.Web.ViewModels.Shared;
    using FluentAssertions;
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

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void SearchPerformed_GivenNoSearchRef_ShouldReturnFalse(string search)
        {
            //arrange
            var model = new FilterViewModel() { SearchRef = search };

            //act
            var result = model.SearchPerformed;

            //assert
            result.Should().BeFalse();
        }

        [Fact]
       public void SearchPerformed_GivenSearchRef_ShouldReturnTrue()
        {
            //arrange
            var model = new FilterViewModel() { SearchRef = "search" };

            //act
            var result = model.SearchPerformed;

            //assert
            result.Should().BeTrue();
        }
    }
}
