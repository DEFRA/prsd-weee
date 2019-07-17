namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
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

    public class ReusedOffSiteCreateSiteControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ReusedOffSiteCreateSiteController controller;
        private readonly IObligatedReusedSiteRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer, ReusedOffSiteCreateSiteViewModel> mapper;

        public ReusedOffSiteCreateSiteControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IObligatedReusedSiteRequestCreator>();
            mapper = A.Fake<IMap<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer, ReusedOffSiteCreateSiteViewModel>>();
            controller = new ReusedOffSiteCreateSiteController(() => weeeClient, breadcrumb, cache, requestCreator, mapper);
        }

        [Fact]
        public void ReusedOffSiteCreateSiteControllerInheritsExternalSiteController()
        {
            typeof(ReusedOffSiteCreateSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public void ReusedOffSiteCreateSiteController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(ReusedOffSiteCreateSiteController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            const string orgName = "orgName";

            var @return = A.Fake<ReturnData>();
            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30));
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();

            const string reportingQuarter = "2019 Q1 Jan - Mar";
            const string reportingPeriod = "Test (WEE/QW1234RE/ATF)";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;
            const string aatfName = "Test";
            aatfInfo.ApprovalNumber = "WEE/QW1234RE/ATF";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(organisationId, A.Dummy<Guid>(), aatfId, A.Dummy<Guid?>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid?>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenAatfAndReturn_SitesShouldBeRetrieved()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var result = await controller.Index(A.Dummy<Guid>(), returnId, aatfId, A.Dummy<Guid?>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfSite>.That.Matches(c => c.AatfId.Equals(aatfId) && c.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_CountriesShouldBeRetrieved()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid?>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(c => c.UKRegionsOnly.Equals(false)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_ViewModelShouldBeMapped()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var siteId = Guid.NewGuid();

            var country = new List<CountryData>();
            var summary = new AddressTonnageSummary();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(country);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfSite>._)).Returns(summary);

            await controller.Index(organisationId, returnId, aatfId, siteId);

            A.CallTo(() => mapper.Map(A<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer>.That.Matches(x =>
                x.OrganisationId.Equals(organisationId) &&
                x.AatfId.Equals(aatfId) &&
                x.ReturnId.Equals(returnId) &&
                x.SiteId.Equals(siteId) &&
                x.Countries.Equals(country) &&
                x.ReturnedSites.Equals(summary)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_ReusedOffSiteCreateSiteViewModelShouldBeReturned()
        {
            var model = new ReusedOffSiteCreateSiteViewModel();

            A.CallTo(() => mapper.Map(A<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            Assert.Equal(result.Model, model);
        }

        [Fact]
        public async void IndexPost_OnSubmit_PageRedirectsToSiteList()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var viewModel = new ReusedOffSiteCreateSiteViewModel
            {
                OrganisationId = organisationId,
                ReturnId = returnId,
                AatfId = aatfId,
            };

            httpContext.RouteData.Values.Add("organisationId", organisationId);
            httpContext.RouteData.Values.Add("returnId", returnId);
            httpContext.RouteData.Values.Add("aatfId", aatfId);

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ReusedOffSiteSummaryList");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new ReusedOffSiteCreateSiteViewModel();
            var request = new AddAatfSite();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldBeCalled()
        {
            var model = new ReusedOffSiteCreateSiteViewModel() { AddressData = new SiteAddressData() };
            controller.ModelState.AddModelError("error", "error");
            
            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            const string orgName = "orgName";
            var model = new ReusedOffSiteCreateSiteViewModel() { OrganisationId = organisationId, AddressData = new SiteAddressData() };
            controller.ModelState.AddModelError("error", "error");

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            await controller.Index(model);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }
    }
}
