namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using Core.AatfReturn;
    using Core.Scheme;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn.ObligatedReceived;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Requests.AatfReturn;
    using Xunit;

    public class ObligatedReceivedControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IObligatedReceivedWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly ObligatedReceivedController controller;
        private readonly IWeeeCache cache;

        public ObligatedReceivedControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            requestCreator = A.Fake<IObligatedReceivedWeeeRequestCreator>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();

            controller = new ObligatedReceivedController(cache, breadcrumb, () => weeeClient, requestCreator);
        }

        [Fact]
        public void CheckNonObligatedControllerInheritsExternalSiteController()
        {
            typeof(NonObligatedController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new ObligatedReceivedViewModel();
            var request = new AddObligatedReceived();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldNotBeCalled()
        {
            controller.ModelState.AddModelError("error", "error");

            await controller.Index(A.Dummy<ObligatedReceivedViewModel>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddObligatedReceived>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexPost_GivenObligatedReceivedValuesAreSubmitted_PageRedirectsToAatfTaskList()
        {
            var model = new ObligatedReceivedViewModel();

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["area"].Should().Be("AatfReturn");
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenAction_ViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            const string scheme = "scheme";
            const string aatfname = "aatfName";

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(new AatfData(A.Dummy<Guid>(), aatfname, A.Dummy<string>()));
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(new SchemePublicInfo() { Name = scheme });

            var result = await controller.Index(organisationId, returnId, aatfId, schemeId) as ViewResult;

            var receivedModel = result.Model as ObligatedReceivedViewModel;

            receivedModel.AatfName.Should().Be(aatfname);
            receivedModel.SchemeName.Should().BeEquivalentTo(scheme);
            receivedModel.ReturnId.Should().Be(returnId);
            receivedModel.AatfId.Should().Be(aatfId);
            receivedModel.SchemeId.Should().Be(schemeId);
            receivedModel.OrganisationId.Should().Be(organisationId);
        }
    }
}
