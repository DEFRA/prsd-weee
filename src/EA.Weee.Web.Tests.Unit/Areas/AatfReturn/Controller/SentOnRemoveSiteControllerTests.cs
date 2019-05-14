namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
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
    using System.Web.Mvc;
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
        public void CheckSentOnCreateSiteOperatorControllerInheritsExternalSiteController()
        {
            typeof(SentOnRemoveSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var schemeInfo = A.Fake<SchemePublicInfo>();
            const string orgName = "orgName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            await controller.Index(organisationId, A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.SchemeInfo.Should().Be(schemeInfo);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_MapperIsCalled()
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

            await controller.Index(organisationId, returnId, aatfId, weeeSentOnId);

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer>.That.Matches(t => t.OrganisationId == organisationId
                && t.ReturnId == returnId
                && t.AatfId == aatfId
                && t.WeeeSentOn == weeeSentOn))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenSelectedValueIsYes_RemoveWeeeSentOnIsCalled()
        {
            var viewModel = new SentOnRemoveSiteViewModel()
            {
                SelectedValue = "Yes",
                WeeeSentOnId = Guid.NewGuid()
            };

            await controller.Index(viewModel);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<RemoveWeeeSentOn>.That.Matches(r => r.WeeeSentOnId == viewModel.WeeeSentOnId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_ModelStateNotValid_ReturnsView()
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
        public async void IndexPost_GivenSelectedValueIsNo_RedirectToActionIsCalled()
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
        public async void IndexGet_WeeeSentOnIdNoLongerExists_RedirectsToSummaryList()
        {
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            Guid weeeSentOnId = Guid.NewGuid();

            WeeeSentOnData weeeSentOnResult = null;

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOnById>._)).Returns(weeeSentOnResult);

            RedirectToRouteResult result = await controller.Index(organisationId, returnId, aatfId, weeeSentOnId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("SentOnSiteSummaryList");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }
    }
}
