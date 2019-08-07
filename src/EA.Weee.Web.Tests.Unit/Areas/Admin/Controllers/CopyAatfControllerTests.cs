namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
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
        private readonly CopyAatfController controller;

        public CopyAatfControllerTests()
        {
            fixture = new Fixture();
            mapper = A.Fake<IMapper>();
            weeeClient = A.Fake<IWeeeClient>();
            countries = A.Dummy<IList<CountryData>>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();

            controller = new CopyAatfController(() => weeeClient, breadcrumbService, mapper, cache);
        }

        [Fact]
        public void ControllerMustHaveAuthorizeClaimsAttribute()
        {
            typeof(CopyAatfController).Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "Copy AATF for new compliance year")]
        public async Task CopyGet_CanEdit_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create(); 
            aatf.FacilityType = facilityType;

            var aatfViewModel = fixture.Create<CopyAatfViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(aatfViewModel);

            await controller.CopyAatfDetails(aatf.Id);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(FacilityType.Ae, "Copy AE for new compliance year")]
        public async Task CopyGetAe_CanEdit_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create();
            aatf.FacilityType = facilityType;

            var aatfViewModel = fixture.Create<CopyAeViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(aatfViewModel);

            await controller.CopyAatfDetails(aatf.Id);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
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
            clientCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CopyAatfPost_ValidViewModel_ReturnsRedirect()
        {
            Guid orgId = Guid.NewGuid();
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var viewModel = CreateCopyAatfViewModel();

            viewModel.StatusValue = 1;
            viewModel.SizeValue = 1;
            viewModel.ComplianceYear = 2019;
            viewModel.Name = "Name";
            viewModel.ApprovalNumber = "WEE/AB1234CD/ATF";

            var request = fixture.Create<CopyAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatf.Id))).Returns(aatf);

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(viewModel);

            var result = await controller.CopyAatfDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ManageAatfs");
            result.RouteValues["controller"].Should().Be("Aatf");
        }

        [Fact]
        public async Task CopyAatfPost_ValidViewModel_CacheShouldBeInvalidated()
        {
            Guid orgId = Guid.NewGuid();
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var viewModel = CreateCopyAatfViewModel();

            viewModel.StatusValue = 1;
            viewModel.SizeValue = 1;
            viewModel.ComplianceYear = 2019;
            viewModel.Name = "Name";
            viewModel.ApprovalNumber = "WEE/AB1234CD/ATF";

            var request = fixture.Create<CopyAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).Returns(true);

            var result = await controller.CopyAatfDetails(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CopyAePost_ValidViewModel_ReturnsRedirect()
        {
            Guid orgId = Guid.NewGuid();
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var viewModel = CreateCopyAeViewModel();

            viewModel.StatusValue = 1;
            viewModel.SizeValue = 1;
            viewModel.ComplianceYear = 2019;
            viewModel.Name = "Name";

            var request = fixture.Create<CopyAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatf.Id))).Returns(aatf);

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(viewModel);

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

            var request = fixture.Create<CopyAatf>();

            var aatf = new AatfData()
            {
                Id = viewModel.Id,

                Organisation = new OrganisationData() { Id = viewModel.OrganisationId }
            };

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).Returns(true);

            var result = await controller.CopyAeDetails(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
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
            return viewModel;
        }
    }
}
