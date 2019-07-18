namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Infrastructure;
    using Xunit;

    public class SelectReportOptionsDeselectControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SelectReportOptionsDeselectController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private readonly IMap<SelectReportOptionsViewModel, SelectReportOptionsDeselectViewModel> mapper;
        private const string ConfirmSelectedValue = "Yes";

        public SelectReportOptionsDeselectControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddSelectReportOptionsRequestCreator>();
            mapper = A.Fake<IMap<SelectReportOptionsViewModel, SelectReportOptionsDeselectViewModel>>();

            controller = new SelectReportOptionsDeselectController(() => weeeClient, breadcrumb, cache, requestCreator, mapper);
        }

        [Fact]
        public void SelectReportOptionsController_InheritsExternalSiteController()
        {
            typeof(SelectReportOptionsDeselectController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void SelectReportOptionsController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(SelectReportOptionsDeselectController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
        }

        [Fact]
        public void SelectReportOptionsController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SelectReportOptionsDeselectController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
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
            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30), (int)Core.DataReturns.QuarterType.Q1);
            const string reportingPeriod = "2019 Q1 Jan - Mar";
            var @return = A.Fake<ReturnData>();
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);

            Assert.Contains(reportingPeriod, breadcrumb.QuarterDisplayInfo);
        }

        [Fact]
        public async void IndexPost_OnSubmitYesSelectedAndWeeeReceivedReSelected_PageRedirectsToPcsSelection()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var model = A.Fake<SelectReportOptionsDeselectViewModel>();

            A.CallTo(() => model.OrganisationId).Returns(organisationId);
            A.CallTo(() => model.ReturnId).Returns(returnId);
            A.CallTo(() => model.SelectedValue).Returns(model.YesValue);

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            A.CallTo(() => mapper.Map(A<SelectReportOptionsViewModel>._)).Returns(model);

            var selectReportOptionsViewModel = A.Fake<SelectReportOptionsViewModel>();
            var reportsOn = A.CollectionOfFake<ReportOnQuestion>(1);
            A.CallTo(() => selectReportOptionsViewModel.ReportOnQuestions).Returns(reportsOn);
            A.CallTo(() => reportsOn.ElementAt(0).Id).Returns((int)ReportOnQuestionEnum.WeeeReceived);
            A.CallTo(() => reportsOn.ElementAt(0).ReSelected).Returns(true);

            controller.TempData["viewModel"] = selectReportOptionsViewModel;

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteName.Should().Be(AatfRedirect.SelectPcsRouteName);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["reselect"].Should().Be(false);
        }

        [Fact]
        public async void IndexPost_OnSubmitYesSelectedAndWeeeReceivedNotReSelected_PageRedirectsToAatfTaskList()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var model = A.Fake<SelectReportOptionsDeselectViewModel>();
            var reportsOn = A.CollectionOfFake<ReportOnQuestion>(1);
            A.CallTo(() => model.OrganisationId).Returns(organisationId);
            A.CallTo(() => model.ReturnId).Returns(returnId);
            A.CallTo(() => reportsOn.ElementAt(0).Id).Returns((int)ReportOnQuestionEnum.WeeeReceived);
            A.CallTo(() => reportsOn.ElementAt(0).ReSelected).Returns(false);
            A.CallTo(() => model.SelectedValue).Returns(model.YesValue);
            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            A.CallTo(() => mapper.Map(A<SelectReportOptionsViewModel>._)).Returns(model);

            controller.TempData["viewModel"] = CreateTempData();

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["returnId"].Should().Be(returnId);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithNoSelected_PageRedirectsToSelectReportOptions()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var viewModel = CreateSubmittedViewModel();
            viewModel.ReturnId = returnId;
            viewModel.OrganisationId = organisationId;

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            A.CallTo(() => mapper.Map(A<SelectReportOptionsViewModel>._)).Returns(viewModel);

            controller.TempData["viewModel"] = CreateTempData();

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = CreateSubmittedViewModel();
            model.SelectedOptions.Add(1);
            model.SelectedValue = ConfirmSelectedValue;
            var request = new AddReturnReportOn();

            controller.TempData["viewModel"] = CreateTempData();
            A.CallTo(() => mapper.Map(A<SelectReportOptionsViewModel>._)).Returns(model);
            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            const string orgName = "orgName";
            var model = CreateSubmittedViewModel();
            model.ReturnId = returnId;
            model.OrganisationId = organisationId;

            controller.ModelState.AddModelError("error", "error");
            controller.TempData["viewModel"] = CreateTempData();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);
            A.CallTo(() => mapper.Map(A<SelectReportOptionsViewModel>._)).Returns(model);

            await controller.Index(model);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        private static SelectReportOptionsDeselectViewModel CreateSubmittedViewModel()
        {
            var model = new SelectReportOptionsDeselectViewModel { ReportOnQuestions = new List<ReportOnQuestion>() };

            for (var i = 0; i < 5; i++)
            {
                model.ReportOnQuestions.Add(new ReportOnQuestion(i + 1, A.Dummy<string>(), A.Dummy<string>(), null, A.Dummy<string>()) { Selected = true });
            }

            return model;
        }

        private static SelectReportOptionsViewModel CreateTempData()
        {
            var model = new SelectReportOptionsViewModel { ReportOnQuestions = new List<ReportOnQuestion>() };

            for (var i = 0; i < 5; i++)
            {
                model.ReportOnQuestions.Add(new ReportOnQuestion(i + 1, A.Dummy<string>(), A.Dummy<string>(), null, A.Dummy<string>()));
            }

            return model;
        }
    }
}
