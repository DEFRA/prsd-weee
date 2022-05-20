namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class SearchAnAatfControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSearchAnAatfViewModelMapTransfer, SearchAnAatfViewModel> mapper;
        private readonly SearchAnAatfController controller;

        public SearchAnAatfControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToSearchAnAatfViewModelMapTransfer, SearchAnAatfViewModel>>();

            controller = new SearchAnAatfController(() => apiClient, breadcrumb, cache, mapper);
        }

        [Fact]
        public void CheckSearchAnAatfControllerInheritsFromExternalSiteController()
        {
            typeof(SearchAnAatfController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public void CheckSearchAnAatfController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SearchAnAatfController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var organisationData = A.Fake<OrganisationData>();
            const string orgName = "orgName";

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();

            const string reportingQuarter = "2019 Q1 Jan - Mar";
            const string reportingPeriod = "Test (WEE/QW1234RE/ATF)";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;
            const string aatfName = "Test";
            aatfInfo.ApprovalNumber = "WEE/QW1234RE/ATF";

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => organisationData.Id).Returns(organisationId);
            A.CallTo(() => @return.OrganisationData).Returns(organisationData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(@return.Id, aatfId, A.Dummy<Guid>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_SearchAnAatfViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var model = new SearchAnAatfViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId,
                OrganisationId = organisationId
            };

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSearchAnAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(returnId, aatfId, A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async void IndexGet_GivenReturn_SearchAnAatfViewModelShouldBeBuilt()
        {
            var weeeSentOnList = A.Fake<List<WeeeSentOnData>>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOn>._)).Returns(weeeSentOnList);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());
            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSearchAnAatfViewModelMapTransfer>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_OnSubmit_PageRedirectsToSearchedAatfList()
        {
            var model = new SearchAnAatfViewModel()
            {
                SearchTerm = "Test Aatf"
            };
            var returnId = new Guid();
            var organisationId = new Guid();
            var aatfId = new Guid();
            var selectedAatfId = new Guid();
            var searchTerm = "Test Aatf";
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("SearchedAatfResultList");
            result.RouteValues["area"].Should().Be("AatfReturn");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
            result.RouteValues["selectedAatfId"].Should().Be(selectedAatfId);
            result.RouteValues["selectedAatfName"].Should().Be(searchTerm);
        }

        [Fact]
        public async Task FetchSearchResultsJson_GivenNotAnAjaxRequest_InvalidOperationExceptionExpected()
        {
            SetupControllerRequest();

            var exception = await Record.ExceptionAsync(() => controller.SearchAatf(A.Dummy<string>(), A.Dummy<Guid>()));

            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task FetchSearchResultsJson_GivenNotValidModel_EmptyJsonExpected()
        {
            SetupControllerAjaxRequest();

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.SearchAatf(A.Dummy<string>(), A.Dummy<Guid>()) as JsonResult;

            result.JsonRequestBehavior.Should().Be(JsonRequestBehavior.AllowGet);
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task FetchSearchResultsJson_GivenValidRequest_JsonResultExpected()
        {
            SetupControllerAjaxRequest();

            var returnAatfAddressResult = new List<ReturnAatfAddressResult>()
            {
                new ReturnAatfAddressResult() { SearchTermId = new Guid(), SearchTermName = "Test Aatf" }
            };
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetSearchAatfAddress>._)).Returns(returnAatfAddressResult);

            var jsonResult = await controller.SearchAatf("Test Aatf", A.Dummy<Guid>()) as JsonResult;

            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<ReturnAatfAddressResult>>(serializer.Serialize(jsonResult.Data));

            result.Count.Should().Be(1);
            result.Should().BeEquivalentTo(returnAatfAddressResult);
        }

        private void SetupControllerAjaxRequest()
        {
            var mocker = new HttpContextMocker();
            mocker.AttachToController(controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => mocker.HttpContextBase.Request).Returns(request);
            A.CallTo(() => request["X-Requested-With"]).Returns("XMLHttpRequest");
        }

        private void SetupControllerRequest()
        {
            var mocker = new HttpContextMocker();
            mocker.AttachToController(controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => mocker.HttpContextBase.Request).Returns(request);
        }
    }
}
