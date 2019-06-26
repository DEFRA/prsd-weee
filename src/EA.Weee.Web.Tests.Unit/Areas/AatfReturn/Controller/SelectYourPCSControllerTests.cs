namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class SelectYourPcsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SelectYourPcsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        public List<SchemeData> SchemeList;
        private readonly IAddReturnSchemeRequestCreator requestCreator;

        public SelectYourPcsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddReturnSchemeRequestCreator>();

            controller = new SelectYourPcsController(() => weeeClient, breadcrumb, cache, requestCreator);
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
        public void SelectYourPcsController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SelectYourPcsController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
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
            var @return = A.Fake<ReturnData>();

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30));
            const string reportingPeriod = "2019 Q1 Jan - Mar";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);

            Assert.Contains(reportingPeriod, breadcrumb.QuarterDisplayInfo);
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
