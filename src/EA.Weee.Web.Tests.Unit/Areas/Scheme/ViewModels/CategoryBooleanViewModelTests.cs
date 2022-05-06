namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class CategoryBooleanViewModelTests
    {
        [Fact]
        public void CategoryBooleanViewModelInheritsCategoryValue()
        {
            typeof(CategoryBooleanViewModel).Should().BeDerivedFrom<CategoryValue>();
        }
    }
}
