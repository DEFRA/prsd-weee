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
    using EA.Weee.Web.Areas.AatfReturn.Requests;
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

    public class SearchedAatfResultListControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer, SearchedAatfResultListViewModel> mapper;
        private readonly SearchedAatfResultListController controller;
        private readonly ICreateWeeeSentOnAatfRequestCreator requestCreator;

        public SearchedAatfResultListControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer, SearchedAatfResultListViewModel>>();
            this.requestCreator = A.Fake<ICreateWeeeSentOnAatfRequestCreator>();

            controller = new SearchedAatfResultListController(() => apiClient, breadcrumb, cache, mapper, requestCreator);
        }

        [Fact]
        public void CheckSearchedAatfResultListControllerInheritsFromExternalSiteController()
        {
            typeof(SearchedAatfResultListController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }
        [Fact]
        public void CheckSearchedAatfResultListController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SearchedAatfResultListController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
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
            var selectedAatfId = Guid.NewGuid();
            const string selectedAatfName = "Test";

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

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>()
            {
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1234RE/ATF", WeeeSentOnId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() }
            };
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAddressBySearchId>._)).Returns(returnAatfAddressResult);

            await controller.Index(organisationId, @return.Id, aatfId, selectedAatfId, selectedAatfName);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var selectedAatfName = "Test";

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAddressBySearchId>._)).Returns(returnAatfAddressResult);

            var model = new SearchedAatfResultListViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId,
                OrganisationId = organisationId,
                SelectedAatfId = selectedAatfId,
                Sites = returnAatfAddressResult
            };

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSearchedAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId, returnId, aatfId, selectedAatfId, selectedAatfName) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("CanNotFoundTreatmentFacility");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
            result.RouteValues["aatfName"].Should().Be(selectedAatfName);
        }

        [Fact]
        public async void IndexGet_GivenReturnIdAndAatfId_ApiShouldBeCalledWithReturnRequest()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var selectedAatfName = "Test";

            await controller.Index(A.Dummy<Guid>(), returnId, aatfId, selectedAatfId, selectedAatfName);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOn>.That.Matches(g => g.ReturnId.Equals(returnId) && g.AatfId.Equals(aatfId))))
                                    .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_SentOnSiteSummaryListViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var selectedAatfName = "Test";

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>()
            {
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1234RE/ATF", WeeeSentOnId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() }
            };
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAddressBySearchId>._)).Returns(returnAatfAddressResult);

            var model = new SearchedAatfResultListViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId,
                OrganisationId = organisationId,
                SelectedAatfId = selectedAatfId,
                Sites = returnAatfAddressResult
            };

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSearchedAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId, returnId, aatfId, selectedAatfId, selectedAatfName) as ViewResult;

            result.Model.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async void IndexPost_OnSubmit_PageRedirectsToObligatedReceived()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var aatfName = "Test";

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>()
            {
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1234RE/ATF", WeeeSentOnId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() }
            };
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAddressBySearchId>._)).Returns(returnAatfAddressResult);

            var model = new SearchedAatfResultListViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId,
                OrganisationId = organisationId,
                SelectedAatfId = selectedAatfId,
                Sites = returnAatfAddressResult,
                AatfName = aatfName,
                SelectedWeeeSentOnId = selectedAatfId,
                SelectedAatfName = aatfName
            };
            
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ObligatedSentOn");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }
    }
}
