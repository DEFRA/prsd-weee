namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using FluentValidation.Results;
    using Web.Areas.Admin.Controllers.Base;
    using Web.Infrastructure;
    using Xunit;

    public class AddAatfControllerTests
    {
        private readonly Fixture fixture;
        private readonly IWeeeClient weeeClient;
        private readonly IList<CountryData> countries;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IWeeeCache cache;
        private readonly IFacilityViewModelBaseValidatorWrapper validationWrapper;
        private readonly AddAatfController controller;

        public AddAatfControllerTests()
        {
            fixture = new Fixture();
            weeeClient = A.Fake<IWeeeClient>();
            countries = A.Dummy<IList<CountryData>>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            validationWrapper = A.Fake<IFacilityViewModelBaseValidatorWrapper>();

            controller = new AddAatfController(() => weeeClient, breadcrumbService, cache, validationWrapper);
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(AddAatfController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public void ControllerMustHaveAuthorizeClaimsAttribute()
        {
            typeof(AddAatfController).Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public async Task AddGet_CreatesViewModel_ListsArePopulated()
        {
            var facilityType = fixture.Create<FacilityType>();
            AddAatfViewModel viewModel = CreateAddAatfViewModel();

            ViewResult result = await controller.Add(viewModel.OrganisationId, facilityType) as ViewResult;

            AddAatfViewModel resultViewModel = result.Model as AddAatfViewModel;

            Assert.Equal(viewModel.SizeList, resultViewModel.SizeList);
            Assert.Equal(viewModel.StatusList, resultViewModel.StatusList);
            Assert.Equal(viewModel.ContactData.AddressData.Countries, resultViewModel.ContactData.AddressData.Countries);
            Assert.Equal(viewModel.SiteAddressData.Countries, resultViewModel.SiteAddressData.Countries);
            Assert.Equal(viewModel.OrganisationId, resultViewModel.OrganisationId);
        }

        [Fact]
        public async Task AddAatfPost_ValidViewModel_ReturnsRedirect()
        {
            AddAatfViewModel viewModel = new AddAatfViewModel()
            {
                SizeValue = 1,
                StatusValue = 1
            };

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.Validate(A<string>._, viewModel)).Returns(validationResult);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            RedirectToRouteResult result = await controller.AddAatf(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ManageAatfs");
            result.RouteValues["controller"].Should().Be("Aatf");
        }

        [Fact]
        public async Task AddAatfPost_ValidViewModelRequestWithCorrectParametersCreated()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            AddAatfViewModel viewModel = new AddAatfViewModel()
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
                ComplianceYear = (Int16)2019
            };

            AatfData aatfData = new AatfData(
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

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.Validate(A<string>._, viewModel)).Returns(validationResult);

            await controller.AddAatf(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAatf>.That.Matches(
                p => p.OrganisationId == viewModel.OrganisationId
                && p.Aatf.Name == aatfData.Name
                && p.Aatf.ApprovalNumber == aatfData.ApprovalNumber
                && p.Aatf.CompetentAuthority == aatfData.CompetentAuthority
                && p.Aatf.PanAreaData == aatfData.PanAreaData
                && p.Aatf.LocalAreaData == aatfData.LocalAreaData
                && p.Aatf.AatfStatus == aatfData.AatfStatus
                && p.Aatf.SiteAddress == aatfData.SiteAddress
                && p.Aatf.Size == aatfData.Size
                && p.Aatf.ApprovalDate == aatfData.ApprovalDate
                && p.AatfContact == viewModel.ContactData))).MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CompleteOrganisationAdmin>.That.Matches(
                 p => p.OrganisationId == viewModel.OrganisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task AddAatfPost_InvalidViewModel_ReturnsViewWithViewModelPopulatedWithLists()
        {
            controller.ModelState.AddModelError("error", "error");

            AddAatfViewModel viewModel = CreateAddAatfViewModel();

            ViewResult result = await controller.AddAatf(viewModel) as ViewResult;
            AddAatfViewModel resultViewModel = result.Model as AddAatfViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Add");
            Assert.Equal(viewModel.SizeList, resultViewModel.SizeList);
            Assert.Equal(viewModel.StatusList, resultViewModel.StatusList);
            Assert.Equal(viewModel.ContactData.AddressData.Countries, resultViewModel.ContactData.AddressData.Countries);
            Assert.Equal(viewModel.SiteAddressData.Countries, resultViewModel.SiteAddressData.Countries);
            Assert.Equal(viewModel.OrganisationId, resultViewModel.OrganisationId);
        }

        [Fact]
        public async Task AddAatfPost_ApprovalNumberExists_ReturnsViewWithViewModelAndErrorMessage()
        {
            AddAatfViewModel viewModel = CreateAddAatfViewModel();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CheckApprovalNumberIsUnique>.That.Matches(
                 p => p.ApprovalNumber == viewModel.ApprovalNumber))).Returns(true);

            ViewResult result = await controller.AddAatf(viewModel) as ViewResult;
            AddAatfViewModel resultViewModel = result.Model as AddAatfViewModel;

            IEnumerable<ModelError> allErrors = controller.ModelState.Values.SelectMany(v => v.Errors);

            ModelError error = allErrors.FirstOrDefault(p => p.ErrorMessage == Constants.ApprovalNumberExistsError);
            Assert.NotNull(error);
        }

        [Fact]
        public async Task AddAePost_ApprovalNumberExists_ReturnsViewWithViewModelAndErrorMessage()
        {
            AddAeViewModel viewModel = CreateAddAeViewModel();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CheckApprovalNumberIsUnique>.That.Matches(
                 p => p.ApprovalNumber == viewModel.ApprovalNumber))).Returns(true);

            ViewResult result = await controller.AddAe(viewModel) as ViewResult;
            AddAatfViewModel resultViewModel = result.Model as AddAatfViewModel;

            IEnumerable<ModelError> allErrors = controller.ModelState.Values.SelectMany(v => v.Errors);

            ModelError error = allErrors.FirstOrDefault(p => p.ErrorMessage == Constants.ApprovalNumberExistsError);
            Assert.NotNull(error);
        }

        [Fact]
        public async Task AddAatfPost_ValidViewModel_CacheShouldBeInvalidated()
        {
            AddAatfViewModel viewModel = new AddAatfViewModel()
            {
                SizeValue = 1,
                StatusValue = 1,
                OrganisationId = Guid.NewGuid()
            };

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.Validate(A<string>._, viewModel)).Returns(validationResult);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            await controller.AddAatf(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => cache.InvalidateAatfDataForOrganisationDataCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => cache.InvalidateOrganisationSearch()).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task AddAePost_ValidViewModel_ReturnsRedirect()
        {
            var viewModel = new AddAeViewModel()
            {
                SizeValue = 1,
                StatusValue = 1
            };

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.Validate(A<string>._, viewModel)).Returns(validationResult);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            var result = await controller.AddAe(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ManageAatfs");
            result.RouteValues["controller"].Should().Be("Aatf");
        }

        [Fact]
        public async Task AddAePost_ValidViewModelRequestWithCorrectParametersCreated()
        {
            var viewModel = new AddAeViewModel()
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
                ComplianceYear = (Int16)2019
            };

            var aatfData = new AatfData(
                Guid.NewGuid(),
                viewModel.Name,
                viewModel.ApprovalNumber,
                viewModel.ComplianceYear,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Id == Guid.Parse(viewModel.CompetentAuthorityId)),
                Enumeration.FromValue<AatfStatus>(viewModel.StatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SizeValue),
                viewModel.ApprovalDate.GetValueOrDefault());

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.Validate(A<string>._, viewModel)).Returns(validationResult);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            await controller.AddAe(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAatf>.That.Matches(
                p => p.OrganisationId == viewModel.OrganisationId
                && p.Aatf.Name == aatfData.Name
                && p.Aatf.ApprovalNumber == aatfData.ApprovalNumber
                && p.Aatf.CompetentAuthority == aatfData.CompetentAuthority
                && p.Aatf.AatfStatus == aatfData.AatfStatus
                && p.Aatf.SiteAddress == aatfData.SiteAddress
                && p.Aatf.Size == aatfData.Size
                && p.Aatf.ApprovalDate == aatfData.ApprovalDate
                && p.AatfContact == viewModel.ContactData))).MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CompleteOrganisationAdmin>.That.Matches(
                p => p.OrganisationId == viewModel.OrganisationId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task AddAePost_InvalidViewModel_ReturnsViewWithViewModelPopulatedWithLists()
        {
            controller.ModelState.AddModelError("error", "error");

            var viewModel = CreateAddAeViewModel();

            var result = await controller.AddAe(viewModel) as ViewResult;
            var resultViewModel = result.Model as AddAeViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Add");
            Assert.Equal(viewModel.SizeList, resultViewModel.SizeList);
            Assert.Equal(viewModel.StatusList, resultViewModel.StatusList);
            Assert.Equal(viewModel.ContactData.AddressData.Countries, resultViewModel.ContactData.AddressData.Countries);
            Assert.Equal(viewModel.SiteAddressData.Countries, resultViewModel.SiteAddressData.Countries);
            Assert.Equal(viewModel.OrganisationId, resultViewModel.OrganisationId);
        }

        [Fact]
        public async Task AddAePost_ValidViewModel_CacheShouldBeInvalidated()
        {
            var viewModel = new AddAeViewModel()
            {
                SizeValue = 1,
                StatusValue = 1,
                OrganisationId = Guid.NewGuid()
            };

            var validationResult = new ValidationResult();

            A.CallTo(() => validationWrapper.Validate(A<string>._, viewModel)).Returns(validationResult);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            await controller.AddAe(viewModel);

            A.CallTo(() => cache.InvalidateAatfCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => cache.InvalidateOrganisationSearch()).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => cache.InvalidateAatfDataForOrganisationDataCache(viewModel.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "Manage AATFs")]
        [InlineData(FacilityType.Ae, "Manage AEs")]
        public async Task AddGet_Always_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            await controller.Add(Guid.NewGuid(), facilityType);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "Manage AATFs")]
        [InlineData(FacilityType.Ae, "Manage AEs")]
        public async Task AddAatfPost_Always_SetsInternalBreadcrumb(FacilityType facilityType, string expectedBreadcrumb)
        {
            AddAatfViewModel viewModel = new AddAatfViewModel()
            {
                SizeValue = 1,
                StatusValue = 1,
                FacilityType = facilityType
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));

            await controller.AddAatf(viewModel);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        private AddAatfViewModel CreateAddAatfViewModel()
        {
            return CreateAddFacilityViewModel(new AddAatfViewModel());
        }

        private AddAeViewModel CreateAddAeViewModel()
        {
            return CreateAddFacilityViewModel(new AddAeViewModel());
        }

        private T CreateAddFacilityViewModel<T>(T viewModel)
            where T : AddFacilityViewModelBase
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

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(new DateTime(2019, 1, 1));
            return viewModel;
        }
    }
}
