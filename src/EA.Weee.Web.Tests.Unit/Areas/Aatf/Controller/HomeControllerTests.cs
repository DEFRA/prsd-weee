namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public HomeControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel>>();

            controller = new HomeController(cache, breadcrumb, () => apiClient, mapper);
        }

        [Fact]
        public void HomeControllerInheritsExternalSiteController()
        {
            typeof(HomeController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationName = "Organisation";
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>()
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);
            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await controller.Index(A.Dummy<Guid>(), false);

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
        }

        [Fact]
        public async void IndexPost_GivenInValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationName = "organisation";
            controller.ModelState.AddModelError("error", "error");

            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await controller.Index(new HomeViewModel());

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>()
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), false) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_ApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>()
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            await controller.Index(organisationId, false);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId == organisationId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_MapperShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var model = new HomeViewModel()
            {
                AatfList = A.Fake<List<AatfData>>()
            };

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>._)).Returns(model);

            await controller.Index(organisationId, false);

            A.CallTo(() => mapper.Map(A<AatfDataToHomeViewModelMapTransfer>.That.Matches(a => a.IsAE == false && a.OrganisationId == organisationId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_OnSingleAatf_PageRedirectsContactDetails()
        {
            var organisationId = Guid.NewGuid();
            var aatfList = new List<AatfData>();

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
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

            var result = await controller.Index(organisationId, false) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ViewAatfContactDetails");
            result.RouteValues["organisationId"].Should().Be(organisationId);
        }

        [Fact]
        public async void IndexPost_ValidViewModel_PageRedirectsContactDetails()
        {
            var model = new HomeViewModel()
            {
                OrganisationId = Guid.NewGuid(),
                SelectedAatfId = Guid.NewGuid(),
                IsAE = false
            };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ViewAatfContactDetails");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            result.RouteValues["aatfId"].Should().Be(model.SelectedAatfId);
            result.RouteValues["isAE"].Should().Be(model.IsAE);
        }
    }
}
