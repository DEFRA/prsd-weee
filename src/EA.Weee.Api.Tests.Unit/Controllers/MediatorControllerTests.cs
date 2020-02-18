namespace EA.Weee.Api.Tests.Unit.Controllers
{
    using System.Web.Http;
    using Api.Controllers;
    using EA.Weee.Tests.Core;
    using FluentAssertions;
    using Xunit;

    public class MediatorControllerTests
    {
        [Fact]
        public void Controller_ShouldHaveAuthorizeAttribute()
        {
            typeof(MediatorController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void Controller_ShouldNotHaveAnonymousActions()
        {
            AttributeHelper.ShouldNotHaveAnonymousMethods(typeof(MediatorController));
        }
    }
}
