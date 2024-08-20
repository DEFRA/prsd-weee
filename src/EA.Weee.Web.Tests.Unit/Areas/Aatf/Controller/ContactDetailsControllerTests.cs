namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoFixture;

    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Requests;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared.Aatf;
    using EA.Weee.Web.ViewModels.Shared.Aatf.Mapping;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class ContactDetailsControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly IEditAatfContactRequestCreator contactRequestCreator;
        private readonly ContactDetailsController controller;
        private readonly Fixture fixture;

        public ContactDetailsControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMapper>();
            this.contactRequestCreator = A.Fake<IEditAatfContactRequestCreator>();
            this.fixture = new Fixture();

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(this.fixture.Create<AatfData>());

            this.controller = new ContactDetailsController(this.cache, this.breadcrumb, () => this.apiClient, this.mapper, this.contactRequestCreator);
        }

        [Fact]
        public void HomeControllerInheritsExternalSiteController()
        {
            typeof(ContactDetailsController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "AATF")]
        [InlineData(FacilityType.Ae, "AE")]
        public async Task IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet(FacilityType facilityType, string expected)
        {
            var organisationName = "Organisation";
            var aatfDataExternal = this.fixture.Build<AatfData>().With(a => a.FacilityType, facilityType).Create();

            A.CallTo(() => this.cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);
            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfDataExternal);

            await this.controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), facilityType);

            this.breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            this.breadcrumb.ExternalAatf.Should().BeEquivalentTo(aatfDataExternal);
            this.breadcrumb.ExternalActivity.Should().Be($"Manage {expected} contact details");
        }

        [Fact]
        public async Task IndexGet_GivenActionParameters_ApiShouldBeCalled()
        {
            var aatfId = this.fixture.Create<Guid>();

            await this.controller.Index(A.Dummy<Guid>(), aatfId, this.fixture.Create<FacilityType>());

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == aatfId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenActionParameters_HomeViewModelShouldBeBuiltAsync()
        {
            var organisationId = this.fixture.Create<Guid>();
            var aatfId = this.fixture.Create<Guid>();
            var facilityType = this.fixture.Create<FacilityType>();

            var result = await this.controller.Index(organisationId, aatfId, facilityType) as ViewResult;

            var model = result.Model as ContactDetailsViewModel;

            model.FacilityType.Should().Be(facilityType);
            model.OrganisationId.Should().Be(organisationId);
            model.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public async Task EditGet_GivenId_AatfDataShouldBeRetrieved()
        {
            var id = this.fixture.Create<Guid>();

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(this.fixture.Create<AatfData>());

            await this.controller.Edit(id);

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == id)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditGet_GivenId_CountriesShouldBeRetrieved()
        {
            await this.controller.Edit(A.Dummy<Guid>());

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditGet_GivenId_CurrentDateShouldBeRetrieved()
        {
            await this.controller.Edit(A.Dummy<Guid>());

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditGet_GivenIdAatfDataCountriesAndCurrentDate_ViewModelShouldBeMapped()
        {
            var aatf = this.fixture.Create<AatfData>();
            var countries = this.fixture.CreateMany<CountryData>().ToList();
            var currentDate = this.fixture.Create<DateTime>();

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatf);
            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);
            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            await this.controller.Edit(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<AatfEditContactAddressViewModel>(A<AatfEditContactTransfer>.That.Matches(e => e.AatfData == aatf && e.Countries == countries && e.CurrentDate == currentDate))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditGet_GivenMappedViewModel_ModelShouldBeReturned()
        {
            var model = this.fixture.Create<AatfEditContactAddressViewModel>();

            A.CallTo(() => mapper.Map<AatfEditContactAddressViewModel>(A<AatfEditContactTransfer>._)).Returns(model);

            var result = await this.controller.Edit(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task EditGet_GivenMappedViewModel_DefaultViewShouldBeReturned()
        {
            var model = this.fixture.Create<AatfEditContactAddressViewModel>();

            A.CallTo(() => mapper.Map<AatfEditContactAddressViewModel>(A<AatfEditContactTransfer>._)).Returns(model);

            var result = await this.controller.Edit(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task EditGet_GivenAatf_BreadCrumbShouldBeSet()
        {
            var aatf = this.fixture.Create<AatfData>();
            var organisationName = this.fixture.Create<string>();

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatf);
            A.CallTo(() => this.cache.FetchOrganisationName(aatf.Organisation.Id)).Returns(organisationName);

            await this.controller.Edit(A.Dummy<Guid>());

            this.breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            this.breadcrumb.ExternalAatf.Should().Be(aatf);
            this.breadcrumb.ExternalActivity.Should().Be(string.Format(AatfAction.ManageAatfContactDetails, aatf.FacilityType.ToDisplayString()));
            this.breadcrumb.OrganisationId.Should().Be(aatf.Organisation.Id);
        }

        [Fact]
        public async Task EditPost_GivenModel_BreadCrumbShouldBeSet()
        {
            var model = this.fixture.Create<AatfEditContactAddressViewModel>();
            var organisationName = this.fixture.Create<string>();

            A.CallTo(() => this.cache.FetchOrganisationName(model.AatfData.Organisation.Id)).Returns(organisationName);

            await this.controller.Edit(model);

            this.breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            this.breadcrumb.ExternalAatf.Should().Be(model.AatfData);
            this.breadcrumb.ExternalActivity.Should().Be(string.Format(AatfAction.ManageAatfContactDetails, model.AatfData.FacilityType.ToDisplayString()));
            this.breadcrumb.OrganisationId.Should().Be(model.AatfData.Organisation.Id);
        }

        [Fact]
        public async Task EditPost_GivenInvalidModel_BreadCrumbShouldBeSet()
        {
            var model = this.fixture.Create<AatfEditContactAddressViewModel>();
            var organisationName = this.fixture.Create<string>();

            A.CallTo(() => this.cache.FetchOrganisationName(model.AatfData.Organisation.Id)).Returns(organisationName);

            this.controller.ModelState.AddModelError("error", "error");

            await this.controller.Edit(model);

            this.breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            this.breadcrumb.ExternalAatf.Should().Be(model.AatfData);
            this.breadcrumb.ExternalActivity.Should().Be(string.Format(AatfAction.ManageAatfContactDetails, model.AatfData.FacilityType.ToDisplayString()));
            this.breadcrumb.OrganisationId.Should().Be(model.AatfData.Organisation.Id);
        }

        [Fact]
        public async Task EditPost_GivenInvalidModel_ModelCountriesShouldBeSet()
        {
            var countries = this.fixture.CreateMany<CountryData>().ToList();

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            this.controller.ModelState.AddModelError("error", "error");

            var result = await this.controller.Edit(this.fixture.Create<AatfEditContactAddressViewModel>()) as ViewResult;

            var model = result.Model as AatfEditContactAddressViewModel;
            model.ContactData.AddressData.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public async Task EditPost_GivenValidViewModel_RequestShouldBeCreated()
        {
            var model = this.fixture.Create<AatfEditContactAddressViewModel>();

            await this.controller.Edit(model);

            A.CallTo(() => this.contactRequestCreator.ViewModelToRequest(model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditPost_GivenRequest_ApiShouldBeCalledWithRequest()
        {
            var request = this.fixture.Build<EditAatfContact>().With(r => r.SendNotification, true).Create();

            A.CallTo(() => this.contactRequestCreator.ViewModelToRequest(A<AatfEditContactAddressViewModel>._)).Returns(request);

            await this.controller.Edit(this.fixture.Create<AatfEditContactAddressViewModel>());

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditPost_GivenValidViewModelAndRequestSent_ShouldRedirectToIndexAction()
        {
            var model = this.fixture.Create<AatfEditContactAddressViewModel>();
            var result = await this.controller.Edit(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            result.RouteValues["aatfId"].Should().Be(model.Id);
            result.RouteValues["facilityType"].Should().Be(model.AatfData.FacilityType);
        }
    }
}
