namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
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
            var viewModel = CreateInitialViewModel();
            A.CallTo(() => mapper.Map(A<ReportOptionsToSelectReportOptionsViewModelMapTransfer>._)).Returns(viewModel);

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            const string reportingPeriod = "2019 Q1 Jan - Mar";
            var viewModel = CreateInitialViewModel();
            A.CallTo(() => mapper.Map(A<ReportOptionsToSelectReportOptionsViewModelMapTransfer>._)).Returns(viewModel);
           
            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);

            Assert.Contains(reportingPeriod, breadcrumb.QuarterDisplayInfo);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithPcsOptionSelectedAndNoPreviousPcsSelection_PageRedirectsToPcsSelect()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var model = A.Fake<SelectReportOptionsViewModel>();
            var reportsOn = A.CollectionOfFake<ReportOnQuestion>(1);
            var schemeDataItems = A.CollectionOfFake<SchemeData>(0);
            var returnData = new ReturnData() { SchemeDataItems = schemeDataItems };
            A.CallTo(() => reportsOn.ElementAt(0).Id).Returns((int)ReportOnQuestionEnum.WeeeReceived);
            A.CallTo(() => reportsOn.ElementAt(0).Selected).Returns(true);
            A.CallTo(() => model.ReportOnQuestions).Returns(reportsOn);
            A.CallTo(() => model.OrganisationId).Returns(organisationId);
            A.CallTo(() => model.ReturnId).Returns(returnId);
            
            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            var result = await controller.Index(model) as RedirectToRouteResult;

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

            var model = A.Fake<SelectReportOptionsViewModel>();
            var reportsOn = A.CollectionOfFake<ReportOnQuestion>(1);
            var schemeDataItems = A.CollectionOfFake<SchemeData>(1);
            var returnData = new ReturnData() { SchemeDataItems = schemeDataItems };
            A.CallTo(() => reportsOn.ElementAt(0).Id).Returns((int)ReportOnQuestionEnum.WeeeReceived);
            A.CallTo(() => reportsOn.ElementAt(0).Selected).Returns(true);
            A.CallTo(() => model.ReportOnQuestions).Returns(reportsOn);
            A.CallTo(() => model.OrganisationId).Returns(organisationId);
            A.CallTo(() => model.ReturnId).Returns(returnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);
            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["returnId"].Should().Be(returnId);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = CreateSubmittedViewModel();
            model.ReportOnQuestions.First().Selected = true;

            var request = new AddReturnReportOn();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiSendShouldNotBeCalled()
        {
            var model = CreateSubmittedViewModel();
            var request = new AddReturnReportOn();

            controller.ModelState.AddModelError("errror", "error");

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustNotHaveHappened();
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
        public async void IndexPost_OnSubmitWithDeselectedOptionAndSelectedOption_PageRedirectsToSelectReportOptionsDeselect()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var model = A.Fake<SelectReportOptionsViewModel>();

            A.CallTo(() => model.OrganisationId).Returns(organisationId);
            A.CallTo(() => model.ReturnId).Returns(returnId);
            A.CallTo(() => model.DeSelectedOptions).Returns(new List<int>() { 1 });
            A.CallTo(() => model.HasSelectedOptions).Returns(true);

            var returnData = new ReturnData { ReturnReportOns = new List<ReturnReportOn>() };

            for (var i = 0; i < 5; i++)
            {
                returnData.ReturnReportOns.Add(new ReturnReportOn(i + 1, returnId));
            }

            A.CallTo(() => weeeClient.SendAsync(A<String>._, A<GetReturn>._)).Returns(returnData);

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

           var result = await controller.Index(model) as RedirectToRouteResult;

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

            var model = A.Fake<SelectReportOptionsViewModel>();

            A.CallTo(() => model.OrganisationId).Returns(organisationId);
            A.CallTo(() => model.ReturnId).Returns(returnId);
            A.CallTo(() => model.DeSelectedOptions).Returns(new List<int>() { 1 });

            var returnData = new ReturnData { ReturnReportOns = new List<ReturnReportOn>() };

            for (var i = 0; i < 5; i++)
            {
                returnData.ReturnReportOns.Add(new ReturnReportOn(i + 1, returnId));
            }

            A.CallTo(() => weeeClient.SendAsync(A<String>._, A<GetReturn>._)).Returns(returnData);

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AatfRedirect.SelectReportOptionsNilRouteName);
        }

        [Fact]
        public async void IndexPost_GivenNonObligatedQuestionIsNotSelectedAndModelIsNotValid_DcfSelectedValueShouldBeRemovedFromModelState()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var model = CreateSubmittedViewModel();
            model.ReportOnQuestions.First(r => r.Id == (int)ReportOnQuestionEnum.NonObligated).Selected = false;

            controller.ModelState.AddModelError("error", "error");
            controller.ModelState.Add("DcfSelectedValue", new ModelState());
            controller.ModelState.SetModelValue("DcfSelectedValue", new ValueProviderResult("Yes", string.Empty, CultureInfo.InvariantCulture));

            await controller.Index(model);

            controller.ModelState.Where(m => m.Key.Equals("DcfSelectedValue")).Should().BeNullOrEmpty();
        }

        private SelectReportOptionsViewModel CreateSubmittedViewModel()
        {
            var model = new SelectReportOptionsViewModel { ReportOnQuestions = new List<ReportOnQuestion>() };

            for (var i = 0; i < 5; i++)
            {
                model.ReportOnQuestions.Add(new ReportOnQuestion(i + 1, A.Dummy<string>(), A.Dummy<string>(), null, A.Dummy<string>()));
            }

            return model;
        }

        private static SelectReportOptionsViewModel CreateInitialViewModel()
        {
            var model = new SelectReportOptionsViewModel()
            {
                OrganisationId = A.Dummy<Guid>(),
                ReturnId = A.Dummy<Guid>(),
                ReturnData = new ReturnData() { Id = Guid.NewGuid(), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30)) }
            };

            return model;
        }
    }
}
