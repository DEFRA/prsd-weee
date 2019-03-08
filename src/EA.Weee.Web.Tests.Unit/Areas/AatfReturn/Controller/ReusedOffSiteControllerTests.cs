namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReusedOffSiteControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ReusedOffSiteController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ReusedOffSiteControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();

            controller = new ReusedOffSiteController(() => weeeClient, breadcrumb, cache);
        }

        [Fact]
        public void CheckReusedOffSiteControllerInheritsExternalSiteController()
        {
            typeof(ReusedOffSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, A.Dummy<Guid>(), A.Dummy<Guid>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_ReusedOffSiteViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            
            var result = await controller.Index(organisationId, returnId, aatfId) as ViewResult;

            var receivedModel = result.Model as ReusedOffSiteViewModel;

            receivedModel.OrganisationId.Should().Be(organisationId);
            receivedModel.ReturnId.Should().Be(returnId);
            receivedModel.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public async void IndexPost_OnSubmitNo_PageRedirectsToAatfTaskList()
        {
            var model = new ReusedOffSiteViewModel { SelectedValue = "No" };
            var returnId = new Guid();
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["returnId"].Should().Be(returnId);
        }

        [Fact]
        public async void IndexPost_OnSubmitYes_PageRedirectsToCreateSite()
        {
            var model = new ReusedOffSiteViewModel { SelectedValue = "Yes" };
            var returnId = new Guid();
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ReusedOffSiteCreateSite");
            result.RouteValues["returnId"].Should().Be(returnId);
        }
    }
}
