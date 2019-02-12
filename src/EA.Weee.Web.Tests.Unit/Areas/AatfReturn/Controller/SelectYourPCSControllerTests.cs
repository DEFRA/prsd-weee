namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class SelectYourPCSControllerTests
    {
        private readonly Func<IWeeeClient> weeeClient;
        private readonly SelectYourPCSController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public SelectYourPCSControllerTests()
        {
            weeeClient = A.Fake<Func<IWeeeClient>>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();

            controller = new SelectYourPCSController(weeeClient, breadcrumb, cache);
        }

        [Fact]
        public void CheckCheckYourReturnControllerInheritsExternalSiteController()
        {
            typeof(SelectYourPCSController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexGet_GivenReturn_SelectYourPCSViewModelShouldBeReturned()
        {
            var orgId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var model = new SelectYourPCSViewModel();

            var result = await controller.Index(orgId, returnId) as ViewResult;

            result.Model.Should().Be(model);
        }
    }
}
