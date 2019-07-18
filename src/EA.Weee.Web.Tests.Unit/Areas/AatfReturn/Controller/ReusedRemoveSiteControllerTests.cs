namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
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
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class ReusedRemoveSiteControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly ReusedRemoveSiteController controller;
        private readonly IMap<ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer, ReusedRemoveSiteViewModel> mapper;

        public ReusedRemoveSiteControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer, ReusedRemoveSiteViewModel>>();

            controller = new ReusedRemoveSiteController(() => apiClient, breadcrumb, cache, mapper);
        }

        [Fact]
        public void ReusedRemoveSiteControllerInheritsAATFReturnBaseController()
        {
            typeof(ReusedRemoveSiteController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void ReusedRemoveSiteController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(ReusedRemoveSiteController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var siteId = Guid.NewGuid();
            const string orgName = "orgName";

            var @return = A.Fake<ReturnData>();
            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30), (int)Core.DataReturns.QuarterType.Q1);
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();

            const string reportingQuarter = "2019 Q1 Jan - Mar";
            const string reportingPeriod = "Test (WEE/QW1234RE/ATF)";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;
            const string aatfName = "Test";
            aatfInfo.ApprovalNumber = "WEE/QW1234RE/ATF";

            var siteAddressData = new SiteAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST") {Id = siteId};
            var addressTonnage = new AddressTonnageSummary()
            {
                AddressData = new List<SiteAddressData>()
                {
                    siteAddressData
                }
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfSite>.Ignored)).Returns(addressTonnage);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(organisationId, A.Dummy<Guid>(), aatfId, siteId);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            const string orgName = "orgName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            controller.ModelState.AddModelError("error", "error");
            var viewModel = new ReusedRemoveSiteViewModel()
            {
                OrganisationId = organisationId
            };

            await controller.Index(viewModel);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async void IndexPost_GivenSelectedValueIsYes_RemoveAatfSiteIsCalled()
        {
            var viewModel = new ReusedRemoveSiteViewModel()
            {
                SelectedValue = "Yes",
                SiteId = Guid.NewGuid()
            };

            await controller.Index(viewModel);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<RemoveAatfSite>.That.Matches(r => r.SiteId == viewModel.SiteId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_MapperIsCalled()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var siteId = Guid.NewGuid();
            var siteAddressData = new SiteAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST") {Id = siteId};
            var addressTonnage = new AddressTonnageSummary()
            {
                AddressData = new List<SiteAddressData>()
                {
                    siteAddressData
                }
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfSite>.That.Matches(w => w.AatfId == aatfId && w.ReturnId == returnId))).Returns(addressTonnage);

            await controller.Index(organisationId, returnId, aatfId, siteId);

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer>.That.Matches(t => t.OrganisationId == organisationId
                && t.ReturnId == returnId
                && t.AatfId == aatfId
                && t.SiteId == siteAddressData.Id))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_ViewResultIsReturnedCorrectly()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var siteId = Guid.NewGuid();

            var siteAddressData = new SiteAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST") {Id = siteId};
            var addressTonnage = new AddressTonnageSummary()
            {
                AddressData = new List<SiteAddressData>()
                {
                    siteAddressData
                }
            };

            var viewModel = new ReusedRemoveSiteViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = returnId,
                AatfId = aatfId,
                SiteAddress = controller.GenerateAddress(siteAddressData),
                SiteId = siteAddressData.Id,
                Site = siteAddressData
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfSite>.That.Matches(w => w.AatfId == aatfId && w.ReturnId == returnId))).Returns(addressTonnage);
            A.CallTo(() => mapper.Map(A<ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer>._)).Returns(viewModel);

            var result = await controller.Index(organisationId, returnId, aatfId, siteId) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void IndexPost_GivenSelectedValueIsNo_RedirectToActionIsCalled()
        {
            var returnId = new Guid();
            var organisationId = new Guid();
            var aatfId = new Guid();
            var model = new ReusedRemoveSiteViewModel()
            {
                ReturnId = returnId,
                OrganisationId = organisationId,
                AatfId = aatfId
            };
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ReusedOffSiteSummaryList");
            result.RouteValues["area"].Should().Be("AatfReturn");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public void GenerateAddress_GivenAddressData_LongAddressNameShouldBeCreatedCorrectly()
        {
            var siteAddress = new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country");
            var siteAddressLong = "Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country";

            var siteAddressWithoutAddress2 = new AatfAddressData("Site name", "Site address 1", null, "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country");
            var siteAddressWithoutAddress2Long = "Site name<br/>Site address 1<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country";

            var siteAddressWithoutCounty = new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", null, "GU22 7UY", Guid.NewGuid(), "Site country");
            var siteAddressWithoutCountyLong = "Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>GU22 7UY<br/>Site country";

            var siteAddressWithoutPostcode = new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", null, Guid.NewGuid(), "Site country");
            var siteAddressWithoutPostcodeLong = "Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>Site country";

            var result = controller.GenerateAddress(siteAddress);
            var resultWithoutAddress2 = controller.GenerateAddress(siteAddressWithoutAddress2);
            var resultWithoutCounty = controller.GenerateAddress(siteAddressWithoutCounty);
            var resultWithoutPostcode = controller.GenerateAddress(siteAddressWithoutPostcode);

            result.Should().Be(siteAddressLong);
            resultWithoutAddress2.Should().Be(siteAddressWithoutAddress2Long);
            resultWithoutCounty.Should().Be(siteAddressWithoutCountyLong);
            resultWithoutPostcode.Should().Be(siteAddressWithoutPostcodeLong);
        }

        [Fact]
        public async void IndexGet_NoSitesReturned_RedirectsToSummaryList()
        {
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            Guid weeeSentOnId = Guid.NewGuid();

            AddressTonnageSummary addressesData = new AddressTonnageSummary()
            {
                AddressData = new List<SiteAddressData>()
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfSite>._)).Returns(addressesData);

            RedirectToRouteResult result = await controller.Index(organisationId, returnId, aatfId, weeeSentOnId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ReusedOffSiteSummaryList");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public async void IndexGet_SiteNotInReturnedList_RedirectsToSummaryList()
        {
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            Guid weeeSentOnId = Guid.NewGuid();

            AddressTonnageSummary addressesData = new AddressTonnageSummary()
            {
                AddressData = new List<SiteAddressData>()
                {
                    new SiteAddressData()
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfSite>._)).Returns(addressesData);

            RedirectToRouteResult result = await controller.Index(organisationId, returnId, aatfId, weeeSentOnId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ReusedOffSiteSummaryList");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }
    }
}
