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
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class SentOnSiteSummaryListControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSentOnSummaryListViewModelMapTransfer, SentOnSiteSummaryListViewModel> mapper;
        private readonly SentOnSiteSummaryListController controller;

        public SentOnSiteSummaryListControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToSentOnSummaryListViewModelMapTransfer, SentOnSiteSummaryListViewModel>>();

            controller = new SentOnSiteSummaryListController(() => apiClient, breadcrumb, cache, mapper);
        }

        [Fact]
        public void CheckSentOnSiteSummaryListControllerInheritsFromExternalSiteController()
        {
            typeof(SentOnSiteSummaryListController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }
        [Fact]
        public void CheckSentOnSiteSummaryListController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SentOnSiteSummaryListController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
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

            await controller.Index(organisationId, @return.Id, aatfId);

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
        public async void IndexGet_GivenReturnIdAndAatfId_ApiShouldBeCalledWithReturnRequest()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            await controller.Index(A.Dummy<Guid>(), returnId, aatfId);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOn>.That.Matches(g => g.ReturnId.Equals(returnId) && g.AatfId.Equals(aatfId))))
            .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_SentOnSiteSummaryListViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var weeeSentOnList = A.Fake<List<WeeeSentOnSummaryListData>>();

            var model = new SentOnSiteSummaryListViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId,
                OrganisationId = organisationId,
                Sites = weeeSentOnList
            };

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSentOnSummaryListViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId, returnId, aatfId) as ViewResult;

            result.Model.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async void IndexGet_GivenReturn_SentOnSiteSummaryListViewModelShouldBeBuilt()
        {
            var weeeSentOnList = A.Fake<List<WeeeSentOnData>>();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOn>._)).Returns(weeeSentOnList);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSentOnSummaryListViewModelMapTransfer>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();            

            controller.ModelState.AddModelError("error", "Please select copy selection from previous quarter");
            var viewModel = new SentOnSiteSummaryListViewModel()
            {
                OrganisationId = organisationId,
                AatfId = aatfId,
                ReturnId = returnId
            };

            await controller.Index(viewModel);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexPost_OnSubmit_CopyPreviousQuarterData_And_PageRedirectsToSentOnSiteSummaryList()
        {
            var returnId = new Guid();
            var organisationId = new Guid();
            var aatfId = new Guid();
            var model = new SentOnSiteSummaryListViewModel()
            {
                ReturnId = returnId,
                OrganisationId = organisationId,
                AatfId = aatfId
            };
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("SentOnSiteSummaryList");
            result.RouteValues["area"].Should().Be("AatfReturn");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }
    }
}
