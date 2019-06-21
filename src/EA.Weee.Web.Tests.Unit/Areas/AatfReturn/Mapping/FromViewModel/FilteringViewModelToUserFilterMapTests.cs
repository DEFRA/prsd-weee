namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.User;
    using FluentAssertions;
    using System;
    using Xunit;

    public class FilteringViewModelToUserFilterMapTests
    {
        private readonly FilteringViewModelToUserFilterMap map;
        private readonly Fixture fixture;

        public FilteringViewModelToUserFilterMapTests()
        {
            map = new FilteringViewModelToUserFilterMap();
            fixture = new Fixture();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            var viewModel = fixture.Create<FilteringViewModel>();

            var result = map.Map(viewModel);

            Assert.Equal(viewModel.Name, result.Name);
            Assert.Equal(viewModel.OrganisationName, result.OrganisationName);
            Assert.Equal(viewModel.Status, result.Status);
        }
    }
}
