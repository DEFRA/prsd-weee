namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Filters;
    using FluentAssertions;
    using Xunit;

    public class ObligationsBaseControllerTests
    {
        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(ObligationsBaseController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public void Controller_IsDecoratedWith_AuthorizeInternalClaimsAttribute()
        {
            typeof(ObligationsBaseController).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void Controller_IsDecoratedWith_ValidatePcsObligationsEnabledAttribute()
        {
            typeof(ObligationsBaseController).Should()
                .BeDecoratedWith<ValidatePcsObligationsEnabledAttribute>(a => a.Match(new ValidatePcsObligationsEnabledAttribute()));
        }
    }
}
