namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using FluentValidation.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Infrastructure;
    using Xunit;

    public class CopyAatfControllerTests
    {
        private readonly Fixture fixture;
        private readonly IMapper mapper;
        private readonly IWeeeClient weeeClient;
        private readonly IList<CountryData> countries;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IWeeeCache cache;
        private readonly IFacilityViewModelBaseValidatorWrapper validationWrapper;
        private readonly CopyAatfController controller;

        public CopyAatfControllerTests()
        {
            fixture = new Fixture();
            mapper = A.Fake<IMapper>();
            weeeClient = A.Fake<IWeeeClient>();
            countries = A.Dummy<IList<CountryData>>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            validationWrapper = A.Fake<IFacilityViewModelBaseValidatorWrapper>();

            controller = new CopyAatfController(() => weeeClient, breadcrumbService, mapper, cache, validationWrapper);
        }

        [Fact]
        public void CopyAatfController_ShouldInheritFromAdminController()
        {
            typeof(CopyAatfController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public void ControllerMustHaveAuthorizeClaimsAttribute()
        {
            typeof(CopyAatfController).Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "Manage AATFs")]
        public async Task CopyGet_CanEdit_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create();
            aatf.FacilityType = facilityType;

            var aatfViewModel = fixture.Create<CopyAatfViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(aatfViewModel);

            await controller.CopyAatfDetails(aatf.Id);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(FacilityType.Ae, "Manage AEs")]
        public async Task CopyGetAe_CanEdit_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create();
            aatf.FacilityType = facilityType;

            var aatfViewModel = fixture.Create<CopyAeViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(aatfViewModel);

            await controller.CopyAatfDetails(aatf.Id);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task CopyGetAatf_CanEdit_ViewModelShouldBeReturned()
        {
            var aatf = fixture.Build<AatfData>()
                .With(a => a.CanEdit, true)
                .Create();
            aatf.FacilityType = FacilityType.Aatf;
            var aatfViewModel = fixture.Create<CopyAatfViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(aatfViewModel);

            var result = await controller.CopyAatfDetails(aatf.Id) as ViewResult;

            result.ViewName.Should().Be("Copy");
            result.Model.Should().Be(aatfViewModel);
        }

        [Fact]
        public async Task CopyGetAatf_CanEdit_MapperAndApiShouldBeCalled()
        {
            var aatf = fixture.Build<AatfData>()
                .With(a => a.CanEdit, true)
                .Create();
            aatf.FacilityType = FacilityType.Aatf;
            var aatfViewModel = fixture.Create<CopyAatfViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(aatfViewModel);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            await controller.CopyAatfDetails(aatf.Id);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CopyGetAe_CanEdit_ViewModelShouldBeReturned()
        {
            var aatf = fixture.Build<AatfData>()
                .With(a => a.CanEdit, true)
                .Create();
            aatf.FacilityType = FacilityType.Ae;
            var aatfViewModel = fixture.Create<CopyAeViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(aatfViewModel);

            var result = await controller.CopyAatfDetails(aatf.Id) as ViewResult;

            result.ViewName.Should().Be("Copy");
            result.Model.Should().Be(aatfViewModel);
        }

        [Fact]
        public async Task CopyGetAe_CanEdit_MapperAndApiShouldBeCalled()
        {
            var aatf = fixture.Build<AatfData>()
                .With(a => a.CanEdit, true)
                .Create();
            aatf.FacilityType = FacilityType.Ae;
            var aatfViewModel = fixture.Create<CopyAeViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(aatfViewModel);

            await controller.CopyAatfDetails(aatf.Id);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CopyAatfGet_CreatesViewModel_ListsArePopulated()
        {
            var aatfData = fixture.Create<AatfData>();
            aatfData.CanEdit = true;
            CopyAatfViewModel viewModel = CreateCopyAatfViewModel();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatfData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatfData)).Returns(viewModel);

            ViewResult result = await controller.CopyAatfDetails(viewModel.Id) as ViewResult;

            CopyAatfViewModel resultViewModel = result.Model as CopyAatfViewModel;

            Assert.Equal(viewModel.SizeList, resultViewModel.SizeList);
            Assert.Equal(viewModel.StatusList, resultViewModel.StatusList);
            Assert.Equal(viewModel.ContactData.AddressData.Countries, resultViewModel.ContactData.AddressData.Countries);
            Assert.Equal(viewModel.SiteAddressData.Countries, resultViewModel.SiteAddressData.Countries);
            Assert.Equal(viewModel.OrganisationId, resultViewModel.OrganisationId);
        }

        [Fact]
        public async Task CopyAeGet_CreatesViewModel_ListsArePopulated()
        {
            var aatfData = fixture.Create<AatfData>();
            aatfData.FacilityType = FacilityType.Ae;
            aatfData.CanEdit = true;
            CopyAeViewModel viewModel = CreateCopyAeViewModel();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatfData);
            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatfData)).Returns(viewModel);

            ViewResult result = await controller.CopyAatfDetails(viewModel.Id) as ViewResult;

            CopyAeViewModel resultViewModel = result.Model as CopyAeViewModel;

            Assert.Equal(viewModel.SizeList, resultViewModel.SizeList);
            Assert.Equal(viewModel.StatusList, resultViewModel.StatusList);
            Assert.Equal(viewModel.ContactData.AddressData.Countries, resultViewModel.ContactData.AddressData.Countries);
            Assert.Equal(viewModel.SiteAddressData.Countries, resultViewModel.SiteAddressData.Countries);
            Assert.Equal(viewModel.OrganisationId, resultViewModel.OrganisationId);
        }

        [Fact]
        public async void CopyAatfDetailsGet_CanNotEdit_ReturnsForbiddenResult()
        {
            var id = fixture.Create<Guid>();
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, false).Create();

            var clientCall = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfById>.That.Matches(a => a.AatfId == id)));
            clientCall.Returns(aatf);

            var result = await controller.CopyAatfDetails(id);

            Assert.IsType<HttpForbiddenResult>(result);
        }

        [Fact]
        public async Task CopyAatfPost_ValidViewModel_ReturnsRedirect()
        {
            Guid orgId = Guid.NewGuid();
            var viewModel = CreateCopyAatfViewModel();

            viewModel.StatusValue = 1;
            viewModel.SizeValue = 1;
            viewModel.ComplianceYear = 2019;
            viewModel.Name = "Name";
            viewModel.ApprovalNumber = "WEE/AB1234CD/ATF";

            var request = fixture.Create<AddAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(viewModel);

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.ValidateByYear(A<string>._, viewModel, viewModel.ComplianceYear)).Returns(validationResult);

            var result = await controller.CopyAatfDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ManageAatfs");
            result.RouteValues["controller"].Should().Be("Aatf");
        }

        [Fact]
        public async Task CopyAatfPost_ValidViewModel_CacheShouldBeInvalidated()
        {
            Guid orgId = Guid.NewGuid();
            var viewModel = CreateCopyAatfViewModel();

            viewModel.StatusValue = 1;
            viewModel.SizeValue = 1;
            viewModel.ComplianceYear = 2019;
            viewModel.Name = "Name";
            viewModel.ApprovalNumber = "WEE/AB1234CD/ATF";

            var request = fixture.Create<AddAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).Returns(true);

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.ValidateByYear(A<string>._, viewModel, viewModel.ComplianceYear)).Returns(validationResult);

            var result = await controller.CopyAatfDetails(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => cache.InvalidateAatfDataForOrganisationDataCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CopyAePost_ValidViewModel_ReturnsRedirect()
        {
            Guid orgId = Guid.NewGuid();
            var viewModel = CreateCopyAeViewModel();

            viewModel.StatusValue = 1;
            viewModel.SizeValue = 1;
            viewModel.ComplianceYear = 2019;
            viewModel.Name = "Name";

            var request = fixture.Create<AddAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(viewModel);

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.ValidateByYear(A<string>._, viewModel, viewModel.ComplianceYear)).Returns(validationResult);

            var result = await controller.CopyAeDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ManageAatfs");
            result.RouteValues["controller"].Should().Be("Aatf");
        }

        [Fact]
        public async Task CopyAePost_ValidViewModel_CacheShouldBeInvalidated()
        {
            Guid orgId = Guid.NewGuid();
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var viewModel = CreateCopyAeViewModel();

            viewModel.StatusValue = 1;
            viewModel.SizeValue = 1;
            viewModel.ComplianceYear = 2019;
            viewModel.Name = "Name";

            var request = fixture.Create<AddAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).Returns(true);

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.ValidateByYear(A<string>._, viewModel, viewModel.ComplianceYear)).Returns(validationResult);

            var result = await controller.CopyAeDetails(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => cache.InvalidateAatfDataForOrganisationDataCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void CopyAatfDetailsPost_InvalidViewModel_ApiShouldBeCalled()
        {
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            IList<CountryData> countries = fixture.CreateMany<CountryData>().ToList();
            var siteAddress = fixture.Build<AatfAddressData>().With(sa => sa.Countries, countries).Create();
            var viewModel = fixture.Build<CopyAatfViewModel>().Create();
            var request = fixture.Create<AddAatf>();

            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(Task.FromResult(competentAuthorities));

            var clientCallCountries = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetCountries>.That.Matches(a => a.UKRegionsOnly == false)));
            clientCallCountries.Returns(Task.FromResult(countries));

            controller.ModelState.AddModelError("error", "error");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            var result = await controller.CopyAatfDetails(viewModel) as ViewResult;

            clientCallAuthorities.MustHaveHappenedOnceExactly();
            clientCallCountries.MustHaveHappenedOnceExactly();

            result.ViewName.Should().Be("Copy");
            result.Model.Should().Be(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAatf>.That.Matches(
              p => p.AatfId == viewModel.AatfId))).MustNotHaveHappened();
        }

        [Fact]
        public async Task CopyAatfPost_ValidViewModel_CorrectAatfIdShouldBeSet()
        {
            var viewModel = new CopyAatfViewModel()
            {
                Name = "name",
                ApprovalNumber = "123",
                ApprovalDate = DateTime.Now,
                SiteAddressData = A.Fake<AatfAddressData>(),
                SizeValue = 1,
                StatusValue = 1,
                OrganisationId = Guid.NewGuid(),
                ContactData = A.Fake<AatfContactData>(),
                CompetentAuthoritiesList = A.Fake<List<UKCompetentAuthorityData>>(),
                CompetentAuthorityId = Guid.NewGuid().ToString(),
                PanAreaList = A.Fake<List<PanAreaData>>(),
                PanAreaId = Guid.NewGuid(),
                LocalAreaList = A.Fake<List<LocalAreaData>>(),
                LocalAreaId = Guid.NewGuid(),
                ComplianceYear = (Int16)2019,
                AatfId = Guid.NewGuid()
            };

            var aatf = new AatfData(
                Guid.NewGuid(),
                viewModel.Name,
                viewModel.ApprovalNumber,
                viewModel.ComplianceYear,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Id == Guid.Parse(viewModel.CompetentAuthorityId)),
                Enumeration.FromValue<AatfStatus>(viewModel.StatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SizeValue),
                viewModel.ApprovalDate.GetValueOrDefault(),
                viewModel.PanAreaList.FirstOrDefault(p => p.Id == viewModel.PanAreaId),
                viewModel.LocalAreaList.FirstOrDefault(p => p.Id == viewModel.LocalAreaId));
            aatf.Contact = viewModel.ContactData;

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(viewModel);

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.ValidateByYear(A<string>._, viewModel, viewModel.ComplianceYear)).Returns(validationResult);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            var result = await controller.CopyAatfDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAatf>.That.Matches(
               p => p.OrganisationId == viewModel.OrganisationId
               && p.AatfId == viewModel.AatfId
               && p.Aatf.Name == aatf.Name
               && p.Aatf.ApprovalNumber == aatf.ApprovalNumber
               && p.Aatf.CompetentAuthority == aatf.CompetentAuthority
               && p.Aatf.PanAreaData == aatf.PanAreaData
               && p.Aatf.LocalAreaData == aatf.LocalAreaData
               && p.Aatf.AatfStatus == aatf.AatfStatus
               && p.Aatf.SiteAddress == aatf.SiteAddress
               && p.Aatf.Size == aatf.Size
               && p.Aatf.ApprovalDate == aatf.ApprovalDate
               && p.AatfContact == viewModel.ContactData))).MustHaveHappened(1, Times.Exactly);
        }

        private CopyAatfViewModel CreateCopyAatfViewModel()
        {
            return CreateCopyFacilityViewModel(new CopyAatfViewModel());
        }

        private CopyAeViewModel CreateCopyAeViewModel()
        {
            return CreateCopyFacilityViewModel(new CopyAeViewModel());
        }

        private T CreateCopyFacilityViewModel<T>(T viewModel)
            where T : CopyFacilityViewModelBase
        {
            var sizeList = Enumeration.GetAll<AatfSize>();
            var statusList = Enumeration.GetAll<AatfStatus>();

            viewModel.ContactData = new AatfContactData
            {
                AddressData = new AatfContactAddressData
                {
                    Countries = countries
                }
            };

            viewModel.SiteAddressData = new AatfAddressData { Countries = countries };
            viewModel.SizeList = sizeList;
            viewModel.StatusList = statusList;
            viewModel.OrganisationId = Guid.NewGuid();
            viewModel.Id = Guid.NewGuid();
            viewModel.AatfId = Guid.NewGuid();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));
            return viewModel;
        }
    }
}
