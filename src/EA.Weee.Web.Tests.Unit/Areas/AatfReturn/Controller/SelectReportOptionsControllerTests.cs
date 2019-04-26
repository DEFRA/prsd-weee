namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
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
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class SelectReportOptionsControllerTests
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly SelectReportOptionsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private readonly ISelectReportOptionsViewModelValidatorWrapper validator;

        public SelectReportOptionsControllerTests()
        {
            apiClient = A.Fake<Func<IWeeeClient>>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddSelectReportOptionsRequestCreator>();
            validator = A.Fake<ISelectReportOptionsViewModelValidatorWrapper>();

            controller = new SelectReportOptionsController(apiClient, breadcrumb, cache, requestCreator, validator);
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
        public async void IndexPost_OnSubmit_PageRedirectsToPcsSelect()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var viewModel = new SelectReportOptionsViewModel
            {
                OrganisationId = organisationId,
                ReturnId = returnId
            };

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("SelectYourPcs");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
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
    }
}
