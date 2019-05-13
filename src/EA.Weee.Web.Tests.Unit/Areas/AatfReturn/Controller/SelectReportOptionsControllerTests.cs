namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentValidation.Results;
    using Xunit;
    using ValidationResult = FluentValidation.Results.ValidationResult;

    public class SelectReportOptionsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SelectReportOptionsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private readonly ISelectReportOptionsViewModelValidatorWrapper validator;
        private readonly IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel> mapper;

        public SelectReportOptionsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddSelectReportOptionsRequestCreator>();
            validator = A.Fake<ISelectReportOptionsViewModelValidatorWrapper>();
            mapper = A.Fake<IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel>>();

            controller = new SelectReportOptionsController(() => weeeClient, breadcrumb, cache, requestCreator, validator, mapper);
        }

        [Fact]
        public void SelectReportOptionsController_InheritsExternalSiteController()
        {
            typeof(SelectReportOptionsController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void SelectReportOptionsController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(SelectReportOptionsController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
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
        public async void IndexPost_OnSubmitWithPcsOptionSelected_PageRedirectsToPcsSelect()
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
        }

        [Fact]
        public async void IndexPost_OnSubmitWithoutPcsOptionSelected_PageRedirectsToAatfTaskList()
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
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["returnId"].Should().Be(returnId);
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
            var schemeInfo = A.Fake<SchemePublicInfo>();
            const string orgName = "orgName";
            var model = new SelectReportOptionsViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = returnId
            };

            controller.ModelState.AddModelError("error", "error");

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            await controller.Index(model);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.SchemeInfo.Should().Be(schemeInfo);
        }

        [Fact]
        public async void IndexPost_InvalidViewModel_ValidatorShouldNotBeCalled()
        {
            var model = new SelectReportOptionsViewModel();

            controller.ModelState.AddModelError("error", "error");

            await controller.Index(model);

            A.CallTo(() => validator.Validate(model)).MustNotHaveHappened();
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
        public async void NonObligatedWeeSelected_NoDcfOptionSelected_ViewModelShouldHaveDcfError()
        {
            SelectReportOptionsViewModel viewModel = new SelectReportOptionsViewModel();
            controller.ModelState.AddModelError("DcfSelectedValue-0", "You must tell us whether any of the non-obligated WEEE was retained by a DCF");

            var result = await controller.Index(viewModel) as ViewResult;

            var outputModel = result.Model as SelectReportOptionsViewModel;

            Assert.Equal(true, outputModel.HasDcfError);
        }

        [Fact]
        public async void NonObligatedWeeNotSelected_ViewModelShouldNotHaveDcfError()
        {
            SelectReportOptionsViewModel viewModel = new SelectReportOptionsViewModel();
            controller.ModelState.AddModelError("error", "error");

            var result = await controller.Index(viewModel) as ViewResult;

            var outputModel = result.Model as SelectReportOptionsViewModel;

            Assert.Equal(false, outputModel.HasDcfError);
        }

        private static SelectReportOptionsViewModel CreateSubmittedViewModel()
        {
            var model = new SelectReportOptionsViewModel();
            model.SelectedOptions = new List<int>();
            model.ReportOnQuestions = new List<ReportOnQuestion>();

            for (var i = 0; i < 5; i++)
            {
                model.ReportOnQuestions.Add(new ReportOnQuestion(i + 1, A.Dummy<string>(), A.Dummy<string>(), null));
            }

            model.ReportOnQuestions[0].Question = "PCS";

            return model;
        }
    }
}
