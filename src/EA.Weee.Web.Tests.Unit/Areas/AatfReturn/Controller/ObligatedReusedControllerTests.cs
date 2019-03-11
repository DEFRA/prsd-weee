namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using Core.AatfReturn;
    using Core.Scheme;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Helpers;
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
    using Weee.Requests.AatfReturn;
    using Xunit;

    public class ObligatedReusedControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IObligatedReusedWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly ObligatedReusedController controller;
        private readonly IWeeeCache cache;
        private readonly ICategoryValueTotalCalculator calculator;

        public ObligatedReusedControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            requestCreator = A.Fake<IObligatedReusedWeeeRequestCreator>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            calculator = A.Fake<ICategoryValueTotalCalculator>();

            controller = new ObligatedReusedController(cache, breadcrumb, () => weeeClient, requestCreator, calculator);
        }

        [Fact]
        public void CheckObligatedReusedControllerInheritsExternalSiteController()
        {
            typeof(ObligatedReusedController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new ObligatedViewModel(calculator);
            var request = new AddObligatedReused();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldNotBeCalled()
        {
            controller.ModelState.AddModelError("error", "error");

            await controller.Index(A.Dummy<ObligatedViewModel>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddObligatedReused>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var schemeInfo = A.Fake<SchemePublicInfo>();
            var operatorData = A.Fake<OperatorData>();

            const string orgName = "orgName";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => operatorData.OrganisationId).Returns(organisationId);
            A.CallTo(() => @return.ReturnOperatorData).Returns(operatorData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.SchemeInfo.Should().Be(schemeInfo);
        }

        [Fact]
        public async void IndexPost_GivenObligatedReceivedValuesAreSubmitted_PageRedirectsToAatfTaskList()
        {
            var model = new ObligatedViewModel(calculator) { ReturnId = Guid.NewGuid() };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ReusedOffSite");
            result.RouteValues["returnId"].Should().Be(model.ReturnId);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }
    }
}
