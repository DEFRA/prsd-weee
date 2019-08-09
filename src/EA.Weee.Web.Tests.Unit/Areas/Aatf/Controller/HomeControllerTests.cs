namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Controllers;
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

        public HomeControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();

            controller = new HomeController(cache, breadcrumb, () => apiClient);
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
            var result = await controller.Index(A.Dummy<Guid>(), false) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_ApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, false);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId == organisationId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void IndexGet_IsAEIsTrue_OnlyAEsShouldBeReturned(bool isAE)
        {
            var organisationId = Guid.NewGuid();
            var aatfList = new List<AatfData>();

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            var aatfData2 = new AatfData(Guid.NewGuid(), "AATF2", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            var exporterData = new AatfData(Guid.NewGuid(), "AE", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Ae
            };

            var exporterData2 = new AatfData(Guid.NewGuid(), "AE", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
               Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
               A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Ae
            };

            aatfList.Add(aatfData);
            aatfList.Add(exporterData);
            aatfList.Add(aatfData2);
            aatfList.Add(exporterData2);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId == organisationId))).Returns(aatfList);

            var result = await controller.Index(organisationId, isAE) as ViewResult;

            var model = result.Model as HomeViewModel;

            if (isAE)
            {
                foreach (var aatf in model.AatfList)
                {
                    aatf.FacilityType.Should().Be(FacilityType.Ae);
                }
            }
            else
            {
                foreach (var aatf in model.AatfList)
                {
                    aatf.FacilityType.Should().Be(FacilityType.Aatf);
                }
            }
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

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfList);

            var result = await controller.Index(organisationId, false) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ViewAatfContactDetails");
            result.RouteValues["organisationId"].Should().Be(organisationId);
        }

        [Fact]
        public async void IndexGet_HomeViewModelShouldBeBuilt()
        {
            var organisationId = Guid.NewGuid();

            var aatfList = new List<AatfData>();

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            var aatfData2 = new AatfData(Guid.NewGuid(), "AATF2", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
               Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
               A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            aatfList.Add(aatfData);
            aatfList.Add(aatfData2);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfList);

            var result = await controller.Index(organisationId, false) as ViewResult;

            var model = result.Model as HomeViewModel;

            model.IsAE.Should().Be(false);
            model.AatfList.Should().BeEquivalentTo(aatfList);
            model.OrganisationId.Should().Be(organisationId);
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
