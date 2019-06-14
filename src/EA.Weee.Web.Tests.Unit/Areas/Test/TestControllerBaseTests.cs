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
        public void TestControllerBase_SholdBeDecoratedWithAuthorizeInternalClaimsAttribute()
        {
            typeof(TestControllerBase).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.CanAccessInternalArea)));
        }
    }
}
