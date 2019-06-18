﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Admin.DeleteAatf;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Requests;
    using Web.Areas.Admin.ViewModels.Aatf;
    using Xunit;

    public class AatfControllerTests
    {
        private readonly Fixture fixture;
        private readonly IWeeeClient weeeClient;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IMapper mapper;
        private readonly IEditFacilityDetailsRequestCreator detailsRequestCreator;
        private readonly IEditAatfContactRequestCreator contactRequestCreator;
        private readonly IWeeeCache cache;
        private readonly AatfController controller;

        public AatfControllerTests()
        {
            fixture = new Fixture();
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            detailsRequestCreator = A.Fake<IEditFacilityDetailsRequestCreator>();
            contactRequestCreator = A.Fake<IEditAatfContactRequestCreator>();
            cache = A.Fake<IWeeeCache>();

            controller = new AatfController(() => weeeClient, breadcrumbService, mapper, detailsRequestCreator, contactRequestCreator, cache);
        }

        [Fact]
        public async Task ManageSchemesPost_ModelError_ReturnsView()
        {
            SetUpControllerContext(false);
            controller.ModelState.AddModelError(string.Empty, "Validation message");

            var result = await controller.ManageAatfs(new ManageAatfsViewModel());

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ManageAatfsPost_ReturnsSelectedGuid()
        {
            var selectedGuid = Guid.NewGuid();

            var result = await controller.ManageAatfs(new ManageAatfsViewModel { Selected = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("Details", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["Id"]);
        }

        [Fact]
        public async Task ManageAatfsPost_ModelError_WithFilter_GetAatfsMustBeRun()
        {
            SetUpControllerContext(false);
            controller.ModelState.AddModelError(string.Empty, "Validation message");

            await controller.ManageAatfs(new ManageAatfsViewModel());

            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfs>.That.Matches(a => a.Filter == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ManageAatfsPost_ModelError_GetAatfsMustBeRun()
        {
            SetUpControllerContext(false);
            controller.ModelState.AddModelError(string.Empty, "Validation message");
            var filter = fixture.Create<FilteringViewModel>();
            var mappedFilter = fixture.Create<AatfFilter>();

            var mapperCall = A.CallTo(() => mapper.Map<AatfFilter>(filter));
            mapperCall.Returns(mappedFilter);

            var result = await controller.ManageAatfs(new ManageAatfsViewModel { Filter = filter, CanAddAatf = false });

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var viewResult = (ViewResult)result;
            Assert.IsType<ManageAatfsViewModel>(viewResult.Model);

            var viewResultModel = (ManageAatfsViewModel)viewResult.Model;
            Assert.Equal(filter, viewResultModel.Filter);
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfs>.That.Matches(a => a.Filter == mappedFilter))).MustHaveHappenedOnceExactly();
            mapperCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ApplyFilterPost_RedirectsToManageView()
        {
            SetUpControllerContext(fixture.Create<bool>());
            var filter = fixture.Create<FilteringViewModel>();
            var mappedFilter = fixture.Create<AatfFilter>();

            var mapperCall = A.CallTo(() => mapper.Map<AatfFilter>(filter));
            mapperCall.Returns(mappedFilter);

            var result = await controller.ApplyFilter(filter);

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var viewResult = (ViewResult)result;
            Assert.Equal("ManageAatfs", viewResult.ViewName);
            Assert.IsType<ManageAatfsViewModel>(viewResult.Model);

            var viewResultModel = (ManageAatfsViewModel)viewResult.Model;
            Assert.Equal(filter, viewResultModel.Filter);
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfs>.That.Matches(a => a.Filter == mappedFilter))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ManageAatfsPost_InvalidModel_ChecksUserIsAllowed_ViewModelSetCorrectly(bool userHasInternalAdminClaims)
        {
            SetUpControllerContext(userHasInternalAdminClaims);
            controller.ModelState.AddModelError(string.Empty, "Validation message");

            ManageAatfsViewModel viewModel = new ManageAatfsViewModel();

            ViewResult result = await controller.ManageAatfs(viewModel) as ViewResult;

            ManageAatfsViewModel resultViewModel = result.Model as ManageAatfsViewModel;

            Assert.Equal(userHasInternalAdminClaims, resultViewModel.CanAddAatf);
        }

        [Fact]
        public async Task ManageAatfsPost_Always_SetsInternalBreadcrumbToManageAATFs()
        {
            var selectedGuid = Guid.NewGuid();

            var result = await controller.ManageAatfs(new ManageAatfsViewModel { Selected = selectedGuid, FacilityType = FacilityType.Aatf });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumbService.InternalAatf.Should().Be(null);
        }

        [Theory]
        [MemberData("FacilityTypeEnumValues")]
        public async Task ManageAatfsPost_InvalidModel_CheckViewModelFacilityTypeSetCorrectly(FacilityType type)
        {
            SetUpControllerContext(false);
            controller.ModelState.AddModelError(string.Empty, "Validation message");

            ManageAatfsViewModel viewModel = new ManageAatfsViewModel()
            {
                FacilityType = type
            };

            ViewResult result = await controller.ManageAatfs(viewModel) as ViewResult;

            ManageAatfsViewModel resultViewModel = result.Model as ManageAatfsViewModel;

            Assert.Equal(type, resultViewModel.FacilityType);
        }

        [Fact]
        public async Task GetAatfsList_Always_SetsInternalBreadcrumbToManageAATFs()
        {
            SetUpControllerContext(true);

            ActionResult result = await controller.ManageAatfs(FacilityType.Aatf);

            Assert.Equal("Manage AATFs", breadcrumbService.InternalActivity);
            Assert.Equal(null, breadcrumbService.InternalAatf);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetManageAatfs_ChecksUserIsAllowed_ViewModelSetCorrectly(bool userHasInternalAdminClaims)
        {
            SetUpControllerContext(userHasInternalAdminClaims);

            ViewResult result = await controller.ManageAatfs(FacilityType.Aatf) as ViewResult;

            var viewModel = result.Model as ManageAatfsViewModel;

            Assert.Equal(userHasInternalAdminClaims, viewModel.CanAddAatf);
        }

        [Theory]
        [MemberData("FacilityTypeEnumValues")]
        public async Task GetManageAatfs_TypeParameterSent_ViewModelSetCorrectly(FacilityType facilityType)
        {
            SetUpControllerContext(false);

            ViewResult result = await controller.ManageAatfs(facilityType) as ViewResult;

            ManageAatfsViewModel viewModel = result.Model as ManageAatfsViewModel;

            Assert.Equal(facilityType, viewModel.FacilityType);
        }

        [Theory]
        [MemberData("FacilityTypeEnumValues")]
        [MemberData("FacilityTypeEnumValues")]
        public async void DetailsGet_GivenValidAatfId_BreadcrumbShouldBeSet(FacilityType type)
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData()
            {
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country",
                    Telephone = "9367282",
                    Email = "test@test.com"
                }
            };

            var organisation = new OrganisationData() { Id = Guid.NewGuid(), Name = "TEST" };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Contact = A.Fake<AatfContactData>(),
                FacilityType = type
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            await controller.Details(aatfId);

            if (type == FacilityType.Aatf)
            {
                Assert.Equal(breadcrumbService.InternalActivity, InternalUserActivity.ManageAatfs);
                Assert.Equal(breadcrumbService.InternalAatf, aatfData.Name);
            }
            else
            {
                Assert.Equal(breadcrumbService.InternalActivity, InternalUserActivity.ManageAes);
                Assert.Equal(breadcrumbService.InternalAe, aatfData.Name);
            }
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_AatfsByOperatorIdShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData
            {
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country",
                    Telephone = "9367282",
                    Email = "test@test.com"
                }
            };

            AatfContactData contactData = new AatfContactData
            {
                AddressData = new AatfContactAddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country"
                }
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Contact = contactData,
                FacilityType = FacilityType.Aatf
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            await controller.Details(aatfId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfsByOrganisationId>.That.Matches(a => a.OrganisationId == aatfData.Organisation.Id))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_MapperShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData
            {
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country",
                    Telephone = "9367282",
                    Email = "test@test.com"
                }
            };

            AatfContactData contactData = new AatfContactData
            {
                AddressData = new AatfContactAddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country"
                }
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Contact = contactData
            };

            var associatedAatfs = new List<AatfDataList>();
            var associatedSchemes = new List<Core.Scheme.SchemeData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfsByOrganisationId>.That.Matches(a => a.OrganisationId == aatfData.Organisation.Id))).Returns(associatedAatfs);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesByOrganisationId>._)).Returns(associatedSchemes);

            await controller.Details(aatfId);

            A.CallTo(() => mapper.Map<AatfDetailsViewModel>(A<AatfDataToAatfDetailsViewModelMapTransfer>.That.Matches(a => a.AssociatedAatfs == associatedAatfs
            && a.AssociatedSchemes == associatedSchemes
            && a.OrganisationString == controller.GenerateSharedAddress(aatfData.Organisation.BusinessAddress)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_SchemesByOrganisationIdShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData
            {
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country",
                    Telephone = "9367282",
                    Email = "test@test.com"
                }
            };

            AatfContactData contactData = new AatfContactData
            {
                AddressData = new AatfContactAddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country"
                }
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Contact = contactData
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            await controller.Details(aatfId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesByOrganisationId>.That.Matches(a => a.OrganisationId == aatfData.Organisation.Id))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_ViewModelShouldBeCreatedWithApprovalDate()
        {
            var viewModel = A.Fake<AatfDetailsViewModel>();

            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData
            {
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country",
                    Telephone = "9367282",
                    Email = "test@test.com"
                }
            };

            AatfContactData contactData = new AatfContactData
            {
                AddressData = new AatfContactAddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country"
                }
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Contact = contactData,
                FacilityType = FacilityType.Aatf
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            var result = await controller.Details(aatfId) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfIdButNoApprovalDate_ViewModelShouldBeCreatedWithNullApprovalDate()
        {
            var viewModel = A.Fake<AatfDetailsViewModel>();
            viewModel.ApprovalDate = null;

            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData
            {
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country",
                    Telephone = "9367282",
                    Email = "test@test.com"
                }
            };

            AatfContactData contactData = new AatfContactData
            {
                AddressData = new AatfContactAddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country"
                }
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Contact = contactData,
                ApprovalDate = default(DateTime),
                FacilityType = FacilityType.Aatf
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            var result = await controller.Details(aatfId) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void ManageAatfDetailsGet_CanNotEdit_ReturnsForbiddenResult()
        {
            var id = fixture.Create<Guid>();
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, false).Create();

            var clientCall = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfById>.That.Matches(a => a.AatfId == id)));
            clientCall.Returns(aatf);

            var result = await controller.ManageAatfDetails(id);

            Assert.IsType<HttpForbiddenResult>(result);
            clientCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsGet_CanEdit_BreadcrumbShouldBeSet()
        {
            var id = fixture.Create<Guid>();
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create();
            var aatfViewModel = fixture.Create<AatfEditDetailsViewModel>();

            var clientCall = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfById>.That.Matches(a => a.AatfId == id)));
            clientCall.Returns(aatf);
            var mapperCall = A.CallTo(() => mapper.Map<AatfEditDetailsViewModel>(aatf));
            mapperCall.Returns(aatfViewModel);

            await controller.ManageAatfDetails(id);

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumbService.InternalAatf.Should().Be(aatf.Name);
            clientCall.MustHaveHappenedOnceExactly();
            mapperCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsGet_CanEdit_ViewModelShouldBeReturned()
        {
            var id = fixture.Create<Guid>();
            var countries = fixture.CreateMany<CountryData>().ToList();
            var authorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create();
            var aatfViewModel = fixture.Create<AatfEditDetailsViewModel>();

            var clientCallAatf = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfById>.That.Matches(a => a.AatfId == id)));
            clientCallAatf.Returns(aatf);
            var mapperCall = A.CallTo(() => mapper.Map<AatfEditDetailsViewModel>(aatf));
            mapperCall.Returns(aatfViewModel);
            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(authorities);
            var clientCallCountries = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetCountries>.That.Matches(a => a.UKRegionsOnly == false)));
            clientCallCountries.Returns(countries);

            var result = await controller.ManageAatfDetails(id) as ViewResult;

            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(aatfViewModel);
            clientCallAatf.MustHaveHappenedOnceExactly();
            mapperCall.MustHaveHappenedOnceExactly();
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            clientCallCountries.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsPost_ValidViewModel_ApiSendAndRedirectToDetails()
        {
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var viewModel = fixture.Build<AatfEditDetailsViewModel>().With(a => a.CompetentAuthoritiesList, competentAuthorities).Create();
            var request = fixture.Create<EditAatfDetails>();

            var aatfData = new AatfData()
            {
                Id = viewModel.Id,
                Organisation = new OrganisationData() { Id = Guid.NewGuid() }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfData.Id))).Returns(aatfData);

            var helper = A.Fake<UrlHelper>();
            controller.Url = helper;
            var url = fixture.Create<string>();

            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(Task.FromResult(competentAuthorities));
            var requestCall = A.CallTo(() => detailsRequestCreator.ViewModelToRequest(viewModel));
            requestCall.Returns(request);
            var helperCall = A.CallTo(() => helper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == viewModel.Id)));
            helperCall.Returns(url);

            var result = await controller.ManageAatfDetails(viewModel) as RedirectResult;

            result.Url.Should().Be(url);
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            requestCall.MustHaveHappenedOnceExactly();
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).MustHaveHappenedOnceExactly();
            helperCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsPost_InvalidViewModel_ApiShouldBeCalled()
        {
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            IList<CountryData> countries = fixture.CreateMany<CountryData>().ToList();
            var siteAddress = fixture.Build<AatfAddressData>().With(sa => sa.Countries, countries).Create();
            var viewModel = fixture.Build<AatfEditDetailsViewModel>().With(a => a.CompetentAuthoritiesList, competentAuthorities).With(a => a.SiteAddressData, siteAddress).Create();

            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(Task.FromResult(competentAuthorities));
            var clientCallCountries = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetCountries>.That.Matches(a => a.UKRegionsOnly == false)));
            clientCallCountries.Returns(Task.FromResult(countries));

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.ManageAatfDetails(viewModel) as ViewResult;

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumbService.InternalAatf.Should().Be(null);
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            clientCallCountries.MustHaveHappenedOnceExactly();

            result.ViewName.Should().Be("ManageAatfDetails");
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async void ManageAatfDetailsPost_ValidViewModel_CacheShouldBeInvalidated()
        {
            var viewModel = new AatfEditDetailsViewModel() { Id = Guid.NewGuid() };
            var helper = A.Fake<UrlHelper>();
            controller.Url = helper;
            var url = fixture.Create<string>();

            var aatfData = new AatfData()
            {
                Id = viewModel.Id,
                Organisation = new OrganisationData() { Id = Guid.NewGuid() }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfData.Id))).Returns(aatfData);

            var helperCall = A.CallTo(() => helper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == viewModel.Id)));
            helperCall.Returns(url);

            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A<GetUKCompetentAuthorities>._)).Returns(A.CollectionOfFake<UKCompetentAuthorityData>(1));

            await controller.ManageAatfDetails(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(aatfData.Organisation.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAeDetailsPost_ValidViewModel_ApiSendAndRedirectToDetails()
        {
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var viewModel = fixture.Build<AeEditDetailsViewModel>().With(a => a.CompetentAuthoritiesList, competentAuthorities).Create();
            var request = fixture.Create<EditAatfDetails>();

            var aatfData = new AatfData()
            {
                Id = viewModel.Id,
                Organisation = new OrganisationData() { Id = Guid.NewGuid() }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfData.Id))).Returns(aatfData);

            var helper = A.Fake<UrlHelper>();
            controller.Url = helper;
            var url = fixture.Create<string>();

            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(Task.FromResult(competentAuthorities));
            var requestCall = A.CallTo(() => detailsRequestCreator.ViewModelToRequest(viewModel));
            requestCall.Returns(request);
            var helperCall = A.CallTo(() => helper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == viewModel.Id)));
            helperCall.Returns(url);

            var result = await controller.ManageAeDetails(viewModel) as RedirectResult;

            result.Url.Should().Be(url);
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            requestCall.MustHaveHappenedOnceExactly();
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).MustHaveHappenedOnceExactly();
            helperCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAeDetailsPost_InvalidViewModel_ApiShouldBeCalled()
        {
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            IList<CountryData> countries = fixture.CreateMany<CountryData>().ToList();
            var siteAddress = fixture.Build<AatfAddressData>().With(sa => sa.Countries, countries).Create();
            var viewModel = fixture.Build<AeEditDetailsViewModel>().With(a => a.CompetentAuthoritiesList, competentAuthorities).With(a => a.SiteAddressData, siteAddress).Create();

            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(Task.FromResult(competentAuthorities));
            var clientCallCountries = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetCountries>.That.Matches(a => a.UKRegionsOnly == false)));
            clientCallCountries.Returns(Task.FromResult(countries));

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.ManageAeDetails(viewModel) as ViewResult;

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumbService.InternalAe.Should().Be(null);
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            clientCallCountries.MustHaveHappenedOnceExactly();

            result.ViewName.Should().Be("ManageAatfDetails");
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async void ManageAeDetailsPost_ValidViewModel_CacheShouldBeInvalidated()
        {
            var viewModel = new AeEditDetailsViewModel() { Id = Guid.NewGuid() };
            var helper = A.Fake<UrlHelper>();
            controller.Url = helper;
            var url = fixture.Create<string>();

            var aatfData = new AatfData()
            {
                Id = viewModel.Id,
                Organisation = new OrganisationData() { Id = Guid.NewGuid() }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfData.Id))).Returns(aatfData);

            var helperCall = A.CallTo(() => helper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == viewModel.Id)));
            helperCall.Returns(url);

            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A<GetUKCompetentAuthorities>._)).Returns(A.CollectionOfFake<UKCompetentAuthorityData>(1));

            await controller.ManageAeDetails(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(aatfData.Organisation.Id)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData("FacilityTypeEnumValues")]
        public async void ManageContactDetailsGet_GivenValidViewModel_BreadcrumbShouldBeSet(FacilityType type)
        {
            string activity = type == FacilityType.Aatf ? InternalUserActivity.ManageAatfs : InternalUserActivity.ManageAes;

            var aatfId = Guid.NewGuid();
            ContactDataAccessSetup(true);

            AatfData aatf = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);

            await controller.ManageContactDetails(aatfId, type);

            breadcrumbService.InternalActivity.Should().Be(activity);
            if (type == FacilityType.Aatf)
            {
                breadcrumbService.InternalAatf.Should().Be(aatf.Name);
            }
            else
            {
                breadcrumbService.InternalAe.Should().Be(aatf.Name);
            }
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenAction_DefaultViewShouldBeReturned()
        {
            ContactDataAccessSetup(true);
            var result = await controller.ManageContactDetails(A.Dummy<Guid>(), FacilityType.Aatf) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenAatf_ContactShouldBeRetrieved()
        {
            var aatfId = Guid.NewGuid();

            var result = await controller.ManageContactDetails(aatfId, FacilityType.Aatf);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfContact>.That.Matches(c => c.AatfId.Equals(aatfId)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenActionExecutes_CountriesShouldBeRetrieved()
        {
            ContactDataAccessSetup(true);
            var result = await controller.ManageContactDetails(A.Dummy<Guid>(), FacilityType.Aatf);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(c => c.UKRegionsOnly.Equals(false)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenActionExecutes_AatfShouldBeRetrieved()
        {
            ContactDataAccessSetup(true);
            var aatfId = Guid.NewGuid();
            var result = await controller.ManageContactDetails(aatfId, FacilityType.Aatf);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(c => c.AatfId == aatfId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenUnauthorizedAccess_HttpForbiddenReturned()
        {
            var result = await controller.ManageContactDetails(A.Dummy<Guid>(), FacilityType.Aatf);

            Assert.IsType<HttpForbiddenResult>(result);
        }

        [Fact]
        public async void ManageContactDetailsPost_OnSubmit_PageRedirectsToSiteList()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var aatfId = Guid.NewGuid();

            var viewModel = new AatfEditContactAddressViewModel
            {
                AatfId = aatfId,
            };

            httpContext.RouteData.Values.Add("id", aatfId);

            var helper = A.Fake<UrlHelper>();
            controller.Url = helper;
            var url = fixture.Create<string>();

            A.CallTo(() => helper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == viewModel.AatfId))).Returns(url);

            var result = await controller.ManageContactDetails(viewModel) as RedirectResult;

            result.Url.Should().Be($"{url}#contactDetails");
        }

        [Fact]
        public async void ManageContactDetailsPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new AatfEditContactAddressViewModel();
            var request = new EditAatfContact();

            A.CallTo(() => contactRequestCreator.ViewModelToRequest(model)).Returns(request);

            controller.Url = new UrlHelper(A.Fake<RequestContext>(), A.Fake<RouteCollection>());

            await controller.ManageContactDetails(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsPost_GivenInvalidViewModel_ApiShouldBeCalled()
        {
            var model = new AatfEditContactAddressViewModel() { ContactData = new AatfContactData() };
            controller.ModelState.AddModelError("error", "error");

            await controller.ManageContactDetails(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsPost_GivenInvalidViewModel_CountriesShouldBeAttached()
        {
            var model = new AatfEditContactAddressViewModel() { ContactData = new AatfContactData() };
            controller.ModelState.AddModelError("error", "error");

            var countryGuid = Guid.NewGuid();
            var countryName = "MyCountryName";
            var countryList = new List<CountryData>() { new CountryData() { Id = countryGuid, Name = countryName } };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countryList);

            var result = await controller.ManageContactDetails(model) as ViewResult;
            var viewModel = result.Model as AatfEditContactAddressViewModel;
            viewModel.ContactData.AddressData.Countries.Should().NotBeNull();
            viewModel.ContactData.AddressData.Countries.Count().Should().Be(1);
            viewModel.ContactData.AddressData.Countries.ElementAt(0).Id.Should().Be(countryGuid);
            viewModel.ContactData.AddressData.Countries.ElementAt(0).Name.Should().Be(countryName);
        }

        [Theory]
        [MemberData("FacilityTypeEnumValues")]
        public async void ManageContactDetailsPost_GivenInvalidViewModel_BreadcrumbShouldBeSet(FacilityType type)
        {
            var aatfId = Guid.NewGuid();
            var model = new AatfEditContactAddressViewModel() { AatfId = aatfId, ContactData = new AatfContactData(), FacilityType = type };
            controller.ModelState.AddModelError("error", "error");

            await controller.ManageContactDetails(model);

            if (type == FacilityType.Aatf)
            {
                breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
                breadcrumbService.InternalAatf.Should().Be(null);
            }
            else
            {
                breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAes);
                breadcrumbService.InternalAatf.Should().Be(null);
            }
        }

        [Fact]
        public void GenerateSharedAddress_GivenAddressData_LongAddressNameShouldBeCreatedCorrectly()
        {
            var siteAddress = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country"
            };
            var siteAddressLong = "Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country";

            var siteAddressWithoutAddress2 = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = null,
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country"
            };
            var siteAddressWithoutAddress2Long = "Site address 1<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country";

            var siteAddressWithoutCounty = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = null,
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country"
            };
            var siteAddressWithoutCountyLong = "Site address 1<br/>Site address 2<br/>Site town<br/>GU22 7UY<br/>Site country";

            var siteAddressWithoutPostcode = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = null,
                CountryId = Guid.NewGuid(),
                CountryName = "Site country"
            };
            var siteAddressWithoutPostcodeLong = "Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>Site country";

            var result = controller.GenerateSharedAddress(siteAddress);
            var resultWithoutAddress2 = controller.GenerateSharedAddress(siteAddressWithoutAddress2);
            var resultWithoutCounty = controller.GenerateSharedAddress(siteAddressWithoutCounty);
            var resultWithoutPostcode = controller.GenerateSharedAddress(siteAddressWithoutPostcode);

            result.Should().Be(siteAddressLong);
            resultWithoutAddress2.Should().Be(siteAddressWithoutAddress2Long);
            resultWithoutCounty.Should().Be(siteAddressWithoutCountyLong);
            resultWithoutPostcode.Should().Be(siteAddressWithoutPostcodeLong);
        }

        [Fact]
        public async void GetDelete_CheckAatfCanBeDeletedCalled_ViewModelCreatedAndViewReturned_CallToHandlerMustHaveBeenCalled()
        {
            CanAatfBeDeletedFlags canDelete = CanAatfBeDeletedFlags.HasActiveUsers | CanAatfBeDeletedFlags.OrganisationHasMoreAatfs;
            Guid aatfId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            FacilityType facilityType = FacilityType.Aatf;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CheckAatfCanBeDeleted>.That.Matches(a => a.AatfId == aatfId))).Returns(canDelete);

            ViewResult result = await controller.Delete(aatfId, organisationId, facilityType) as ViewResult;

            DeleteViewModel viewModel = result.Model as DeleteViewModel;

            Assert.Equal(aatfId, viewModel.AatfId);
            Assert.Equal(organisationId, viewModel.OrganisationId);
            Assert.Equal(facilityType, viewModel.FacilityType);
            Assert.Equal(canDelete, viewModel.CanDeleteFlags);

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Delete");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CheckAatfCanBeDeleted>.That.Matches(a => a.AatfId == aatfId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetDelete_EnsureBreadcrumbIsSet()
        {
            Guid aatfId = Guid.NewGuid();
            FacilityType facilityType = FacilityType.Aatf;

            AatfData aatfData = A.Dummy<AatfData>();
            aatfData.Name = "Name";
            Organisation organisation = A.Dummy<Organisation>();

            aatfData.Id = aatfId;

            A.CallTo(() => cache.FetchAatfData(organisation.Id, aatfId)).Returns(aatfData);

            await controller.Delete(aatfId, organisation.Id, facilityType);

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumbService.InternalAatf.Should().Be(aatfData.Name);
        }

        [Fact]
        public async void PostDelete_DeleteAnAatfHandlerIsCalled_ReturnsRedirectToManageAatf()
        {
            DeleteViewModel viewModel = new DeleteViewModel()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                FacilityType = FacilityType.Aatf,
                CanDeleteFlags = CanAatfBeDeletedFlags.HasActiveUsers
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<DeleteAnAatf>.That.Matches(a => a.AatfId == viewModel.AatfId)));

            RedirectToRouteResult result = await controller.Delete(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ManageAatfs");
            result.RouteValues["facilityType"].Should().Be(viewModel.FacilityType);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<DeleteAnAatf>.That.Matches(a => a.AatfId == viewModel.AatfId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        private void ContactDataAccessSetup(bool canEdit)
        {
            var contact = new AatfContactData()
            {
                CanEditContactDetails = canEdit
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfContact>._))
                .Returns(contact);
        }

        private void SetUpControllerContext(bool hasInternalAdminUserClaims)
        {
            HttpContextBase httpContextBase = A.Fake<HttpContextBase>();
            ClaimsPrincipal principal = new ClaimsPrincipal(httpContextBase.User);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(httpContextBase.User.Identity);

            if (hasInternalAdminUserClaims)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Claims.InternalAdmin));
            }
            principal.AddIdentity(claimsIdentity);

            A.CallTo(() => httpContextBase.User.Identity).Returns(claimsIdentity);

            var context = new ControllerContext(httpContextBase, new RouteData(), controller);
            controller.ControllerContext = context;
        }

        public static IEnumerable<object[]> FacilityTypeEnumValues()
        {
            foreach (var facilityType in Enum.GetValues(typeof(FacilityType)))
            {
                yield return new object[] { facilityType };
            }
        }
    }
}
