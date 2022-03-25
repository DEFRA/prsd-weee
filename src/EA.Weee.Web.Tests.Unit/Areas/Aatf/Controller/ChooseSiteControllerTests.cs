﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using Api.Client;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.AatfEvidence.Controllers;
    using Xunit;

    public class ChooseSiteControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ChooseSiteController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public ChooseSiteControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            fixture = new Fixture();
            mapper = A.Fake<IMapper>();

            controller = new ChooseSiteController(cache, breadcrumb, () => weeeClient, mapper);
        }

        [Fact]
        public void SelectYourPcsControllerInheritsExternalSiteController()
        {
            typeof(ChooseSiteController).BaseType.Name.Should().Be(nameof(AatfEvidenceBaseController));
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationName = "Organisation";
            var model = new SelectYourAatfViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
            };

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);
            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await controller.Index(A.Dummy<Guid>());

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be($"Manage AATF Evidence");
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var model = new SelectYourAatfViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
            };

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_ApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var model = new SelectYourAatfViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
            };

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId == organisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_MapperShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var model = new SelectYourAatfViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
            };

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var facilityType = FacilityType.Aatf;

            await controller.Index(organisationId);

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>.That.Matches(a => a.FacilityType == facilityType && a.OrganisationId == organisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_OnSingleAatf_PageRedirectsToManageEvidence()
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

            var model = new SelectYourAatfViewModel()
            {
                OrganisationId = organisationId,
                AatfList = aatfList,
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfList);
            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfList[0].Id);
        }

        [Fact]
        public async void IndexGet_GivenActionParameters_SelectYourAatfViewModelShouldBeBuiltAsync()
        {
            var organisationId = this.fixture.Create<Guid>();
            var model = new SelectYourAatfViewModel()
            {
                OrganisationId = organisationId,
                AatfList = A.Fake<List<AatfData>>(),
            };

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var result = await this.controller.Index(organisationId) as ViewResult;

            var modelResult = result.Model as SelectYourAatfViewModel;

            modelResult.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async void IndexPost_ValidViewModel_PageRedirectsManageEvidence()
        {
            var model = new SelectYourAatfViewModel()
            {
                OrganisationId = fixture.Create<Guid>(),
                SelectedId = fixture.Create<Guid>(),
            };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            result.RouteValues["aatfId"].Should().Be(model.SelectedId);
        }

        [Fact]
        public async void IndexPost_GivenInvalidModel_ApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var model = new SelectYourAatfViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
            };

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId == organisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexPost_GivenInvalid_MapperShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var model = new SelectYourAatfViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
            };

            this.controller.ModelState.AddModelError("error", "error");

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var facilityType = FacilityType.Aatf;

            await controller.Index(organisationId);

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>.That.Matches(a => a.FacilityType == facilityType && a.OrganisationId == organisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationName = "Organisation";
            var model = new SelectYourAatfViewModel()
            {
                AatfList = A.Fake<List<AatfData>>(),
            };

            this.controller.ModelState.AddModelError("error", "error");

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._)).Returns(model);
            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await controller.Index(model);

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be($"Manage AATF Evidence");
        }
    }
}