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
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class SelectYourPcsControllerTests
    {
        private readonly Func<IWeeeClient> weeeClient;
        private readonly SelectYourPcsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        public List<SchemeData> SchemeList;
        private readonly IAddReturnSchemeRequestCreator requestCreator;

        public SelectYourPcsControllerTests()
        {
            weeeClient = A.Fake<Func<IWeeeClient>>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddReturnSchemeRequestCreator>();

            controller = new SelectYourPcsController(weeeClient, breadcrumb, cache, requestCreator);
        }

        [Fact]
        public void SelectYourPcsControllerInheritsExternalSiteController()
        {
            typeof(SelectYourPcsController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void SelectYourPcsController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(SelectYourPcsController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
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
        public async void IndexPost_GivenModel_RedirectShouldBeCorrect()
        {
            var returnId = Guid.NewGuid();

            var viewModel = new SelectYourPcsViewModel(A.Fake<List<SchemeData>>(), A.Fake<List<Guid>>())
            {
                ReturnId = returnId
            };

            var redirect = await controller.Index(viewModel) as RedirectToRouteResult;

            redirect.RouteValues["action"].Should().Be("Index");
            redirect.RouteValues["controller"].Should().Be("AatfTaskList");
            redirect.RouteValues["returnId"].Should().Be(returnId);
        }
    }
}
