namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using FluentAssertions;
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using Xunit;

    public class ObligationsControllerTests
    {
        [Fact]
        public void Controller_ShouldInheritFromObligationsBaseController()
        {
            typeof(ObligationsController).Should().BeDerivedFrom<ObligationsBaseController>();
        }

        [Fact]
        public void ChooseAuthorityGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(ObligationsController).GetMethod("SelectAuthority", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { }, null)
            .Should()
            .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void HoldingPost_IsDecoratedWith_HttpPostAttribute()
        {
            typeof(ObligationsController).GetMethod("SelectAuthority", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(SelectAuthorityViewModel) }, null)
            .Should()
            .BeDecoratedWith<HttpPostAttribute>();
        }
    }
}
