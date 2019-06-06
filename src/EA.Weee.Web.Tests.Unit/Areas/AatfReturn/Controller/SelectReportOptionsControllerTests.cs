namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class SelectReportOptionsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SelectReportOptionsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private readonly IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel> mapper;

        public SelectReportOptionsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddSelectReportOptionsRequestCreator>();
            mapper = A.Fake<IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel>>();

            controller = new SelectReportOptionsController(() => weeeClient, breadcrumb, cache, requestCreator, mapper);
        }

        [Fact]
        public void SelectReportOptionsController_InheritsBaseController()
        {
            typeof(SelectReportOptionsController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void SelectReportOptionsController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(SelectReportOptionsController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
        }

        [Fact]
        public void SelectReportOptionsController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SelectReportOptionsController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
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
        public async void IndexPost_OnSubmitWithPcsOptionSelectedAndNoPreviousPcsSelection_PageRedirectsToPcsSelect()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var viewModel = CreateSubmittedViewModel();
            viewModel.ReturnId = returnId;
            viewModel.OrganisationId = organisationId;
            viewModel.SelectedOptions.Add(1);

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteName.Should().Be(AatfRedirect.SelectPcsRouteName);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithPcsOptionSelectedAndPreviousPcsSelection_PageRedirectsToAatfTaskList()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var viewModel = CreateSubmittedViewModel();
            var returnReportsOn = new List<ReturnReportOn>() { new ReturnReportOn((int)ReportOnQuestionEnum.WeeeReceived, returnId) };
            var returnData = new ReturnData() { ReturnReportOns = returnReportsOn };
            viewModel.ReturnId = returnId;
            viewModel.OrganisationId = organisationId;
            viewModel.SelectedOptions.Add(1);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);
            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["returnId"].Should().Be(returnId);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithoutPcsOptionSelected_PageRedirectsToSelectReportOptionsNil()
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

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AatfRedirect.SelectReportOptionsNilRouteName);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = CreateSubmittedViewModel();
            model.SelectedOptions.Add(1);
            var request = new AddReturnReportOn();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModelWithNoSelectedOptions_ApiSendShouldNotBeCalled()
        {
            var model = CreateSubmittedViewModel();
            var request = new AddReturnReportOn();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Never);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            const string orgName = "orgName";
            var model = new SelectReportOptionsViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = returnId
            };

            controller.ModelState.AddModelError("error", "error");

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            await controller.Index(model);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async void SetSelected_OptionsSelected_ViewModelQuestionsUpdate()
        {
            var model = CreateSubmittedViewModel();
            var selectionOptions = new List<int>() { 1, 2, 3, 4 };
            model.SelectedOptions.AddRange(selectionOptions);

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.Index(model) as ViewResult;

            var outputModel = result.Model as SelectReportOptionsViewModel;

            foreach (var selectedOption in selectionOptions)
            {
                model.ReportOnQuestions.FirstOrDefault(r => r.Id == selectedOption).Selected.Should().BeTrue();
            }
        }

        [Fact]
        public async void SetSelected_DcfSelected_ViewModelQuestionsUpdate()
        {
            var model = CreateSubmittedViewModel();
            var selectionOptions = new List<int>() { 4 };
            model.SelectedOptions.AddRange(selectionOptions);
            model.DcfSelectedValue = "Yes";

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.Index(model) as ViewResult;

            var outputModel = result.Model as SelectReportOptionsViewModel;

            model.ReportOnQuestions.FirstOrDefault(r => r.Id == (int)ReportOnQuestionEnum.NonObligatedDcf).Selected.Should().BeTrue();
        }

        [Fact]
        public async void IndexPost_OnSubmitWithDeselectedOptionAndSelectedOption_PageRedirectsToSelectReportOptionsDeselect()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var viewModel = CreateSubmittedViewModel();
            viewModel.ReturnId = returnId;
            viewModel.OrganisationId = organisationId;
            var selectionOptions = new List<int>() { 1, 2, 3, 4 };
            viewModel.SelectedOptions.AddRange(selectionOptions);

            var returnData = new ReturnData();
            returnData.ReturnReportOns = new List<ReturnReportOn>();

            for (var i = 0; i < 5; i++)
            {
                returnData.ReturnReportOns.Add(new ReturnReportOn(i + 1, returnId));
            }

            A.CallTo(() => weeeClient.SendAsync(A<String>._, A<GetReturn>._)).Returns(returnData);

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AatfRedirect.SelectReportOptionsDeselectRouteName);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithDeselectedOptionAndNoSelectedOption_PageRedirectsToSelectReportOptionsNil()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var viewModel = CreateSubmittedViewModel();
            viewModel.ReturnId = returnId;
            viewModel.OrganisationId = organisationId;

            var returnData = new ReturnData();
            returnData.ReturnReportOns = new List<ReturnReportOn>();

            for (var i = 0; i < 5; i++)
            {
                returnData.ReturnReportOns.Add(new ReturnReportOn(i + 1, returnId));
            }

            A.CallTo(() => weeeClient.SendAsync(A<String>._, A<GetReturn>._)).Returns(returnData);

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AatfRedirect.SelectReportOptionsNilRouteName);
        }

        [Fact]
        public async void CheckHasDeselectedOptions_OptionsDeselected_ViewModelQuestionsUpdate()
        {
            var model = CreateSubmittedViewModel();
            var selectionOptions = new List<int>() { 2 };
            model.SelectedOptions.AddRange(selectionOptions);

            var returnData = new ReturnData();
            returnData.ReturnReportOns = new List<ReturnReportOn>();

            for (var i = 0; i < 5; i++)
            {
                returnData.ReturnReportOns.Add(new ReturnReportOn(i + 1, A.Dummy<Guid>()));
            }

            A.CallTo(() => weeeClient.SendAsync(A<String>._, A<GetReturn>._)).Returns(returnData);

            await controller.Index(model);

            var outputModel = controller.TempData["viewModel"] as SelectReportOptionsViewModel;

            var deselectedOptions = returnData.ReturnReportOns.Select(r => r.ReportOnQuestionId).ToList().Where(s => model.SelectedOptions.All(s2 => s2 != s)).ToList();

            foreach (var option in deselectedOptions)
            {
                model.ReportOnQuestions.FirstOrDefault(r => r.Id == option).Deselected.Should().BeTrue();
            }
        }

        [Fact]
        public async void CheckHasDeselectedOptions_OptionsDeselectedNoSelected_ViewModelQuestionsUpdate()
        {
            var model = CreateSubmittedViewModel();

            var returnData = new ReturnData();
            returnData.ReturnReportOns = new List<ReturnReportOn>();

            for (var i = 0; i < 5; i++)
            {
                returnData.ReturnReportOns.Add(new ReturnReportOn(i + 1, A.Dummy<Guid>()));
            }

            A.CallTo(() => weeeClient.SendAsync(A<String>._, A<GetReturn>._)).Returns(returnData);

            await controller.Index(model);

            var outputModel = controller.TempData["viewModel"] as SelectReportOptionsViewModel;

            var deselectedOptions = returnData.ReturnReportOns.Select(r => r.ReportOnQuestionId).ToList().Where(s => model.SelectedOptions.All(s2 => s2 != s)).ToList();

            foreach (var option in deselectedOptions)
            {
                model.ReportOnQuestions.FirstOrDefault(r => r.Id == option).Deselected.Should().BeTrue();
            }
        }

        private static SelectReportOptionsViewModel CreateSubmittedViewModel()
        {
            var model = new SelectReportOptionsViewModel();
            model.SelectedOptions = new List<int>();
            model.ReportOnQuestions = new List<ReportOnQuestion>();

            for (var i = 0; i < 5; i++)
            {
                model.ReportOnQuestions.Add(new ReportOnQuestion(i + 1, A.Dummy<string>(), A.Dummy<string>(), null, A.Dummy<string>()));
            }

            return model;
        }
    }
}
