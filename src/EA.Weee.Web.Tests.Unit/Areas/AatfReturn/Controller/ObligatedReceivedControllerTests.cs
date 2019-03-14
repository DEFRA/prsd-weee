namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
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
    using Prsd.Core.Mapper;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Weee.Requests.AatfReturn;
    using Xunit;

    public class ObligatedReceivedControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IObligatedReceivedWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly ObligatedReceivedController controller;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToObligatedViewModelTransfer, ObligatedViewModel> mapper;
        private readonly ICategoryValueTotalCalculator calculator;

        public ObligatedReceivedControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            requestCreator = A.Fake<IObligatedReceivedWeeeRequestCreator>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMap<ReturnToObligatedViewModelTransfer, ObligatedViewModel>>();
            calculator = A.Fake<ICategoryValueTotalCalculator>();

            controller = new ObligatedReceivedController(cache, breadcrumb, () => weeeClient, requestCreator, mapper);
        }

        [Fact]
        public void CheckObligatedReceivedControllerInheritsExternalSiteController()
        {
            typeof(ObligatedReceivedController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_ApiShouldBeCalled()
        {
            var returnId = Guid.NewGuid();

            await controller.Index(returnId, A.Dummy<Guid>(), A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_ViewModelShouldBeBuilt()
        {
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            A.CallTo(() => @return.ReturnOperatorData.OrganisationId).Returns(organisationId);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(@return);

            await controller.Index(returnId, aatfId, schemeId);

            A.CallTo(() => mapper.Map(A<ReturnToObligatedViewModelTransfer>.That.Matches(r => r.ReturnData.Equals(@return) && r.AatfId.Equals(aatfId) && r.OrganisationId.Equals(organisationId) && r.ReturnId.Equals(returnId) && r.SchemeId.Equals(schemeId)))).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public async void IndexGet_GivenReturnAndPastedValues_CategoryValuesShouldNotBeTheSame()
        {
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var pastedValue = A.Fake<List<ObligatedCategoryValue>>();

            A.CallTo(() => @return.ReturnOperatorData.OrganisationId).Returns(organisationId);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(@return);

            var result = await controller.Index(returnId, aatfId, schemeId) as ViewResult;

            var viewModel = result.Model as ObligatedViewModel;

            viewModel.CategoryValues.Should().NotBeSameAs(pastedValue);
        }

        [Fact]
        public async void IndexGet_GivenBuiltViewModel_ViewModelShouldBeReturned()
        {
            var model = A.Fake<ObligatedViewModel>();

            A.CallTo(() => mapper.Map(A<ReturnToObligatedViewModelTransfer>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new ObligatedViewModel(calculator);
            var request = A.Fake<ObligatedReceived>();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldNotBeCalled()
        {
            controller.ModelState.AddModelError("error", "error");

            await controller.Index(A.Dummy<ObligatedViewModel>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddObligatedReceived>._)).MustNotHaveHappened();
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

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

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
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["returnId"].Should().Be(model.ReturnId);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }
    }
}
