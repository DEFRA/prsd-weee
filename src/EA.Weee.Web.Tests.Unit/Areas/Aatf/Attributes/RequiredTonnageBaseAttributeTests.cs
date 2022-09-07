namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Aatf.Attributes;
    using Xunit;

    public class RequiredTonnageBaseAttributeTests
    {
        [Fact]
        public void RequiredTonnageBaseAttribute_ShouldBeDerivedFromRequiredAttribute()
        {
            typeof(RequiredTonnageBaseAttribute).Should().BeDerivedFrom<RequiredAttribute>();
        }
    }
}
