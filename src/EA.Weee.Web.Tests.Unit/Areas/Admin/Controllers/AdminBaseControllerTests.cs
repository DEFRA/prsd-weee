namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using FluentAssertions;
    using Security;
    using Web.Areas.Admin.Controllers.Base;
    using Web.Filters;
    using Xunit;

    public class AdminBaseControllerTests
    {
        [Fact]
        public void ControllerMustHaveAuthorizeClaimsAttribute()
        {
            typeof(AdminController).Should().BeDecoratedWith<AuthorizeClaimsAttribute>(a => a.Match(new AuthorizeClaimsAttribute(Claims.CanAccessInternalArea)));
        }
    }
}
