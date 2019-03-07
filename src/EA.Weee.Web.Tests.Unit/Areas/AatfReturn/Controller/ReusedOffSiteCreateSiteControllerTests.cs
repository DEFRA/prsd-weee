namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReusedOffSiteCreateSiteControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ReusedOffSiteCreateSiteController controller;
        private readonly IAddObligatedReusedSiteRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ReusedOffSiteCreateSiteControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddObligatedReusedSiteRequestCreator>();

            controller = new ReusedOffSiteCreateSiteController(() => weeeClient, breadcrumb, cache, requestCreator);
        }

        [Fact]
        public void CheckReuseOffSiteCreateSiteControllerInheritsExternalSiteController()
        {
            typeof(ReusedOffSiteCreateSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
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
        public async void IndexGet_GivenActionAndParameters_ReusedOffSiteCreateSiteViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            
            var result = await controller.Index(organisationId, returnId, aatfId) as ViewResult;

            var receivedModel = result.Model as ReusedOffSiteCreateSiteViewModel;

            receivedModel.OrganisationId.Should().Be(organisationId);
            receivedModel.ReturnId.Should().Be(returnId);
            receivedModel.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public async void IndexPost_OnSubmit_PageRedirectsToHolding()
        {
            var organisationId = Guid.NewGuid();
            var model = new ReusedOffSiteCreateSiteViewModel() { OrganisationId = organisationId};
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Holding");
            result.RouteValues["area"].Should().Be("AatfReturn");
            result.RouteValues["organisationId"].Should().Be(organisationId);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new ReusedOffSiteCreateSiteViewModel();
            var request = new AddAatfSite();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
