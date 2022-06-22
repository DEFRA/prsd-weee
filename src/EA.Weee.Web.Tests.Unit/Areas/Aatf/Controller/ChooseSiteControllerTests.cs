namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
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
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using Constant;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.AatfEvidence.Controllers;
    using Weee.Requests.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class ChooseSiteControllerTests : SimpleUnitTestBase
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

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfEvidenceToSelectYourAatfViewModelMapTransfer>._)).Returns(TestFixture.Create<SelectYourAatfViewModel>());
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
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
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

            result.Model.Should().BeOfType<SelectYourAatfViewModel>();
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

            await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId == organisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_CurrentDateApiShouldBeCalled()
        {
            //act
            await controller.Index(TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiDate>._)).MustHaveHappenedOnceExactly();
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
            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(A<AatfEvidenceToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["aatfId"].Should().Be(aatfList[0].Id);
        }

        [Fact]
        public async void IndexGet_GivenActionParameters_SelectYourAatfViewModelShouldBeBuiltAsync()
        {
            //arrange
            var organisationId = this.fixture.Create<Guid>();
            var currentDate = fixture.Create<DateTime>();
            var aatfs = fixture.CreateMany<AatfData>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfs);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = await controller.Index(organisationId) as ViewResult;
            
            //assert
            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(
                A<AatfEvidenceToSelectYourAatfViewModelMapTransfer>.That.Matches(
                    v => v.CurrentDate == currentDate &&
                         v.AatfList == aatfs &&
                         v.OrganisationId == organisationId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenActionParameters_SelectYourAatfViewModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<SelectYourAatfViewModel>();

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(
                A<AatfEvidenceToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await controller.Index(TestFixture.Create<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void IndexGet_ShouldBeDecoratedWith_HttpGetAttribute()
        {
            typeof(ChooseSiteController).GetMethod("Index", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(Guid) }, null)
            .Should()
            .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void IndexPost_ShouldBeDecoratedWith_Attributes()
        {
            typeof(ChooseSiteController).GetMethod("Index", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(SelectYourAatfViewModel) }, null)
            .Should()
            .BeDecoratedWith<HttpPostAttribute>();

            typeof(ChooseSiteController).GetMethod("Index", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(SelectYourAatfViewModel) }, null)
            .Should()
            .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
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
        public async void IndexPost_GivenInvalidModel_CurrentDateApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_SelectYourAatfViewModelShouldBeBuiltAsync()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var existingModel = TestFixture.Build<SelectYourAatfViewModel>()
                .With(v => v.OrganisationId, organisationId).Create();
            var currentDate = fixture.Create<DateTime>();
            var aatfs = fixture.CreateMany<AatfData>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfs);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            controller.ModelState.AddModelError("error", "error");

            //act
            await controller.Index(existingModel);

            //assert
            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(
                A<AatfEvidenceToSelectYourAatfViewModelMapTransfer>.That.Matches(
                    v => v.CurrentDate == currentDate &&
                         v.AatfList == aatfs &&
                         v.OrganisationId == organisationId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenInvalidViewModel_SelectYourAatfViewModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<SelectYourAatfViewModel>();

            A.CallTo(() => mapper.Map<SelectYourAatfViewModel>(
                A<AatfEvidenceToSelectYourAatfViewModelMapTransfer>._)).Returns(model);

            controller.ModelState.AddModelError("error", "error");

            //act
            var result = await controller.Index(model) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async void IndexPost_GivenInvalidModel_ApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();

            var result = await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId == organisationId))).MustHaveHappened(1, Times.Exactly);
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
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
        }
    }
}