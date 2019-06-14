namespace EA.Weee.Web.Tests.Unit.Areas.Test
{
    using AutoFixture;
    using FluentAssertions;
    using Security;
    using Web.Areas.Test.Controllers;
    using Web.Filters;
    using Xunit;

    public class TestControllerBaseTests
    {
        [Fact]
        public void TestControllerBase_ShouldBeDecoratedWithAuthorizeInternalClaimsAttribute()
        {
            typeof(TestControllerBase).Should()
                .BeDecoratedWith<AuthorizeClaimsAttribute>(a => a.Match(new AuthorizeClaimsAttribute(Claims.CanAccessInternalArea)));
        }
    }
}
