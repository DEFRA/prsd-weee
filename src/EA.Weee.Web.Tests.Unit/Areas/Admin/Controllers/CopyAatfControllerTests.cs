namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Type;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Infrastructure;
    using Xunit;
    using AddressData = Core.Shared.AddressData;
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
        public async Task CopyGet_Always_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            var aatf = fixture.Create<AatfData>();
            aatf.FacilityType = facilityType;
            aatf.CanEdit = true;

            var aatfViewModel = fixture.Create<CopyAatfViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);

            A.CallTo(() => mapper.Map<CopyAatfViewModel>(aatf)).Returns(aatfViewModel);

            await controller.CopyAatfDetails(aatf.Id);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        [Theory]
        [InlineData(FacilityType.Ae, "Copy AE for new compliance year")]
        public async Task CopyGetAe_Always_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            var aatf = fixture.Create<AatfData>();
            aatf.FacilityType = facilityType;
            aatf.CanEdit = true;

            var aatfViewModel = fixture.Create<CopyAeViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>._)).Returns(aatf);

            A.CallTo(() => mapper.Map<CopyAeViewModel>(aatf)).Returns(aatfViewModel);

            await controller.CopyAatfDetails(aatf.Id);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
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
