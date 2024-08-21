﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
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
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class SentOnRemoveSiteControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly SentOnRemoveSiteController controller;
        private readonly IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel> mapper;

        public SentOnRemoveSiteControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel>>();

            controller = new SentOnRemoveSiteController(() => apiClient, breadcrumb, cache, mapper);
        }

        [Fact]
        public void SentOnRemoveSiteOperatorControllerInheritsExternalSiteController()
        {
            typeof(SentOnRemoveSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public void SentOnRemoveSiteOperatorController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SentOnRemoveSiteController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async Task IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            const string orgName = "orgName";

            var @return = A.Fake<ReturnData>();
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

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(organisationId, A.Dummy<Guid>(), aatfId, A.Dummy<Guid>(), false);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async Task IndexPost_GivenInvalidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            const string orgName = "orgName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            controller.ModelState.AddModelError("error", "error");
            var viewModel = new SentOnRemoveSiteViewModel()
            {
                OrganisationId = organisationId
            };

            await controller.Index(viewModel);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task IndexGet_GivenValidViewModel_MapperIsCalled()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var weeeSentOnId = Guid.NewGuid();
            var siteAddressData = new AatfAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST");
            var operatorAddressData = new AatfAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST");
            var weeeSentOn = new WeeeSentOnData()
            {
                SiteAddress = siteAddressData,
                OperatorAddress = operatorAddressData
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOnById>.That.Matches(w => w.WeeeSentOnId == weeeSentOnId))).Returns(weeeSentOn);

            await controller.Index(organisationId, returnId, aatfId, weeeSentOnId, false);

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer>.That.Matches(t => t.OrganisationId == organisationId
                && t.ReturnId == returnId
                && t.AatfId == aatfId
                && t.WeeeSentOn == weeeSentOn))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexPost_GivenSelectedValueIsYes_RemoveWeeeSentOnIsCalled()
        {
            var viewModel = new SentOnRemoveSiteViewModel()
            {
                SelectedValue = "Yes",
                WeeeSentOnId = Guid.NewGuid()
            };

            await controller.Index(viewModel);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<RemoveWeeeSentOn>.That.Matches(r => r.WeeeSentOnId == viewModel.WeeeSentOnId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexPost_ModelStateNotValid_ReturnsView()
        {
            SentOnRemoveSiteViewModel viewModel = new SentOnRemoveSiteViewModel()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid()
            };
            controller.ModelState.AddModelError("error", "error");

            ViewResult result = await controller.Index(viewModel) as ViewResult;

            SentOnRemoveSiteViewModel outputModel = result.Model as SentOnRemoveSiteViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Index");
            Assert.Equal(viewModel, outputModel);
        }

        [Fact]
        public async Task IndexPost_GivenSelectedValueIsNo_RedirectToActionIsCalled()
        {
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            SentOnRemoveSiteViewModel model = new SentOnRemoveSiteViewModel()
            {
                ReturnId = returnId,
                OrganisationId = organisationId,
                AatfId = aatfId
            };

            RedirectToRouteResult result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("SentOnSiteSummaryList");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public async Task IndexGet_WeeeSentOnIdNoLongerExists_RedirectsToSummaryList()
        {
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            Guid weeeSentOnId = Guid.NewGuid();

            WeeeSentOnData weeeSentOnResult = null;

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOnById>._)).Returns(weeeSentOnResult);

            RedirectToRouteResult result = await controller.Index(organisationId, returnId, aatfId, weeeSentOnId, false) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("SentOnSiteSummaryList");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }
    }
}
