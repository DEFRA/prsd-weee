namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class TotalCategoryValueTests
    {
        [Fact]
        public void TotalCategoryValue_ShouldHaveSerializableAttribute()
        {
            typeof(TotalCategoryValue).Should().BeDecoratedWith<SerializableAttribute>();
        }
    }
}
