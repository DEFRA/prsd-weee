namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;    
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class SearchedAatfResultListControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer, SearchedAatfResultListViewModel> mapper;
        private readonly SearchedAatfResultListController controller;
        private readonly ICreateWeeeSentOnAatfRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public SearchedAatfResultListControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer, SearchedAatfResultListViewModel>>();
            this.requestCreator = A.Fake<ICreateWeeeSentOnAatfRequestCreator>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();

            controller = new SearchedAatfResultListController(() => apiClient, mapper, requestCreator, breadcrumb, cache);
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
        public async Task IndexGet_GivenAction_ShouldBeReturned_CanNotFoundTreatmentFacility()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfName = "Test";

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAddressBySearchId>._)).Returns(returnAatfAddressResult);

            var model = new SearchedAatfResultListViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId,
                OrganisationId = organisationId,
                Sites = returnAatfAddressResult
            };

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSearchedAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId, returnId, aatfId, Guid.Empty, selectedAatfName) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("NoResultsFound");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
            result.RouteValues["aatfName"].Should().Be(selectedAatfName);
        }        

        [Fact]
        public async Task IndexGet_GivenActionAndParameters_SentOnSiteSummaryListViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var selectedAatfName = "Test";

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>()
            {
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1234RE/ATF", WeeeAatfId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() }
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
        public async Task IndexPost_OnSubmit_PageRedirectsToObligatedReceived()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var aatfName = "Test";

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>()
            {
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1234RE/ATF", WeeeAatfId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() }
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
