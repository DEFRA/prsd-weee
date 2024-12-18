namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using Api.Client;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly HomeController controller;
        private readonly IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel> mapper;
        private readonly Fixture fixture;

        public HomeControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel>>();

            fixture = new Fixture();

            controller = new HomeController(cache, breadcrumb, () => apiClient, mapper);
        }

        [Fact]
        public void HomeControllerInheritsExternalSiteController()
        {
            typeof(HomeController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "AATF")]
        [InlineData(FacilityType.Ae, "AE")]
        public async Task IndexGet_GivenValidViewModelAndIsAE_BreadcrumbShouldBeSet(FacilityType facilityType, string expected)
        {
            var organisationName = "Organisation";
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
                FacilityType = facilityType
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);
            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await controller.Index(A.Dummy<Guid>(), facilityType);

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be($"Manage {expected} contact details");
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "AATF")]
        [InlineData(FacilityType.Ae, "AE")]
        public async Task IndexPost_GivenInValidViewModelAndIsAE_BreadcrumbShouldBeSet(FacilityType facilityType, string expected)
        {
            var organisationName = "organisation";
            var model = new HomeViewModel()
            {
                FacilityType = facilityType
            };

            controller.ModelState.AddModelError("error", "error");

            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);
            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            await controller.Index(new HomeViewModel() { FacilityType = facilityType });

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be($"Manage {expected} contact details");
        }

        [Fact]
        public async Task IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>()
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), fixture.Create<FacilityType>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task IndexGet_GivenOrganisationId_AatfsForOrganisationShouldBeRetrievedFromCache()
        {
            var organisationId = Guid.NewGuid();
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>()
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            await controller.Index(organisationId, fixture.Create<FacilityType>());

            A.CallTo(() => cache.FetchAatfDataForOrganisationData(organisationId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenOrganisationId_MapperShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>()
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            var facilityType = fixture.Create<FacilityType>();

            await controller.Index(organisationId, facilityType);

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>.That.Matches(a => a.FacilityType == facilityType && a.OrganisationId == organisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_OnSingleAatf_PageRedirectsContactDetails()
        {
            var organisationId = Guid.NewGuid();
            var aatfList = new List<AatfData>();

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   AatfStatus.Approved, A.Dummy<AatfAddressData>(), AatfSize.Large, DateTime.Now,
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            aatfList.Add(aatfData);

            var model = new HomeViewModel()
            {
                AatfList = aatfList
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfList);
            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId, fixture.Create<FacilityType>()) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ContactDetails");
            result.RouteValues["organisationId"].Should().Be(organisationId);
        }

        [Fact]
        public async Task IndexPost_ValidViewModel_PageRedirectsContactDetails()
        {
            var model = new HomeViewModel()
            {
                OrganisationId = fixture.Create<Guid>(),
                SelectedId = fixture.Create<Guid>(),
                FacilityType = fixture.Create<FacilityType>()
            };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ContactDetails");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            result.RouteValues["aatfId"].Should().Be(model.SelectedId);
            result.RouteValues["facilityType"].Should().Be(model.FacilityType);
        }
    }
}
