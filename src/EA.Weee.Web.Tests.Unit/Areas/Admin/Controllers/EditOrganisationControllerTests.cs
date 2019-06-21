namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using AutoFixture;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class EditOrganisationControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly EditOrganisationController controller;
        private readonly Fixture fixture;
        private readonly UrlHelper urlHelper;

        public EditOrganisationControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
            breadcrumb = A.Fake<BreadcrumbService>();
            fixture = new Fixture();
            urlHelper = A.Fake<UrlHelper>();

            controller = new EditOrganisationController(() => weeeClient, cache, breadcrumb) { Url = urlHelper };
        }

        [Fact]
        public void OrganisationControllerInheritsAdminController()
        {
            typeof(EditOrganisationController).BaseType.Name.Should().Be(typeof(AdminController).Name);
        }

        [Fact]
        public void OrganisationControllerMustHaveAuthorizeClaimsAttribute()
        {
            typeof(EditOrganisationController).Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public async void GetEditRegisteredCompanyOrganisationDetails_CanEditOrganisationIsTrue_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(new List<CountryData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetInternalOrganisation>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    Name = "CompanyName",
                    CompanyRegistrationNumber = "123456789",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = true
                });

            var result = await controller.EditRegisteredCompanyOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FacilityType.Aatf);

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EditRegisteredCompanyOrganisationDetailsViewModel>();
        }

        [Fact]
        public async void EditSoleTraderOrganisationDetails_CanEditOrganisationIsTrue_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(new List<CountryData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetInternalOrganisation>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = true
                });

            var result = await controller.EditSoleTraderOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FacilityType.Aatf);

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EditSoleTraderOrganisationDetailsViewModel>();
        }

        [Fact]
        public async void EditPartnershipOrganisationDetails_CanEditOrganisationIsTrue_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(new List<CountryData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetInternalOrganisation>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = true
                });

            var result = await controller.EditPartnershipOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FacilityType.Aatf);

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EditPartnershipOrganisationDetailsViewModel>();
        }

        [Fact]
        public async void GetEditRegisteredCompanyOrganisationDetails_CanEditOrganisationIsFalse_ReturnsHttpForbiddenResult()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    Name = "CompanyName",
                    CompanyRegistrationNumber = "123456789",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = false
                });

            var result = await controller.EditRegisteredCompanyOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FacilityType.Aatf);

            result.Should().NotBeNull();
            result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Fact]
        public async void PostEditRegisteredCompanyOrganisationDetails_ModelIsInvalid_GetsCountriesAndReturnsDefaultView()
        {
            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            var viewModel = new EditRegisteredCompanyOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new Core.Shared.AddressData(),
                CompanyName = "CompanyName",
                CompaniesRegistrationNumber = "123456789",
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            var result = await controller.EditRegisteredCompanyOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            countries.Should().BeSameAs(viewModel.BusinessAddress.Countries);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult)result;

            viewResult.ViewName.Should().BeEmpty();
            viewModel.Should().BeSameAs(viewResult.Model);
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_ModelIsValid_UpdatesDetailsAndRedirectsToSchemeOverview()
        {
            var viewModel = new EditRegisteredCompanyOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new Core.Shared.AddressData(),
                CompanyName = "CompanyName",
                CompaniesRegistrationNumber = "123456789",
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .Returns(true);
            
            new HttpContextMocker().AttachToController(controller);

            var result = await controller.EditRegisteredCompanyOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToRouteResult>();

            var redirectResult = (RedirectToRouteResult)result;
            redirectResult.RouteValues["Action"].Should().Be("Overview");
            redirectResult.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
        }

        [Fact]
        public async void EditPartnershipOrganisationDetails_CanEditOrganisationIsFalse_ReturnsHttpForbiddenResult()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = false
                });

            var result = await controller.EditPartnershipOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FacilityType.Aatf);

            result.Should().NotBeNull();
            result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Fact]
        public async void EditPartnershipOrganisationDetails_ModelIsInvalid_GetsCountriesAndReturnsDefaultView()
        {
            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            var viewModel = new EditPartnershipOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new Core.Shared.AddressData(),
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            var result = await controller.EditPartnershipOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            countries.Should().BeSameAs(viewModel.BusinessAddress.Countries);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult)result;

            viewResult.ViewName.Should().BeEmpty();
            viewResult.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task EditPartnershipOrganisationDetails_ModelIsValid_UpdatesDetailsAndRedirectsToSchemeOverview()
        {
            var viewModel = new EditPartnershipOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new Core.Shared.AddressData(),
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .Returns(true);
            
            new HttpContextMocker().AttachToController(controller);

            var result = await controller.EditPartnershipOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToRouteResult>();

            var redirectResult = (RedirectToRouteResult)result;
            redirectResult.RouteValues["Action"].Should().Be("Overview");
            redirectResult.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
        }

        [Fact]
        public async void EditSoleTraderOrganisationDetails_CanEditOrganisationIsFalse_ReturnsHttpForbiddenResult()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = false
                });

            var result = await controller.EditSoleTraderOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FacilityType.Aatf);

            result.Should().NotBeNull();
            result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Fact]
        public async void EditSoleTraderOrganisationDetails_ModelIsInvalid_GetsCountriesAndReturnsDefaultView()
        {
            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            var viewModel = new EditSoleTraderOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new Core.Shared.AddressData(),
                CompanyName = "CompanyName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            var result = await controller.EditSoleTraderOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            countries.Should().BeSameAs(viewModel.BusinessAddress.Countries);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult)result;

            viewResult.ViewName.Should().BeEmpty();
            viewResult.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task EditSoleTraderOrganisationDetails_ModelIsValid_UpdatesDetailsAndRedirectsToSchemeOverview()
        {
            var viewModel = new EditSoleTraderOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new Core.Shared.AddressData(),
                CompanyName = "CompanyName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .Returns(true);

            new HttpContextMocker().AttachToController(controller);

            var result = await controller.EditSoleTraderOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToRouteResult>();

            var redirectResult = (RedirectToRouteResult)result;
            redirectResult.RouteValues["Action"].Should().Be("Overview");
            redirectResult.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
        }

        [Fact]
        public async Task GetEditRegisteredCompanyOrganisationDetails_CanEditOrganisation_ViewModelShouldBeReturned()
        {
            var organisationData = fixture.Build<OrganisationData>()
                .WithAutoProperties()
                .With(o => o.CanEditOrganisation, true)
                .Create();
            var countries = fixture.CreateMany<CountryData>().ToList();
            var organisationId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetInternalOrganisation>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            var result = await controller.EditRegisteredCompanyOrganisationDetails(schemeId, organisationId, aatfId, FacilityType.Aatf) as ViewResult;

            var model = result.Model as EditRegisteredCompanyOrganisationDetailsViewModel;

            model.OrganisationType.Should().Be(organisationData.OrganisationType);
            model.CompanyName.Should().Be(organisationData.OrganisationName);
            model.BusinessTradingName.Should().Be(organisationData.TradingName);
            model.CompaniesRegistrationNumber.Should().Be(organisationData.CompanyRegistrationNumber);
            model.BusinessAddress.Should().Be(organisationData.BusinessAddress);
            model.BusinessAddress.Countries.Should().BeEquivalentTo(countries);
            model.SchemeId.Should().Be(schemeId);
            model.OrgId.Should().Be(organisationId);
            model.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public async Task GetEditRegisteredCompanyOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var schemeId = Guid.NewGuid();
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(schemeId)).Returns(organisation);
            await controller.EditRegisteredCompanyOrganisationDetails(schemeId, A.Dummy<Guid>(), A.Dummy<Guid>(), FacilityType.Aatf);

            breadcrumb.InternalActivity = InternalUserActivity.ManageScheme;
            breadcrumb.InternalOrganisation = organisation;
        }

        [Fact]
        public async Task GetEditRegisteredCompanyOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatf);
            await controller.EditRegisteredCompanyOrganisationDetails(A.Dummy<Guid>(), organisationId, aatfId, FacilityType.Aatf);

            breadcrumb.InternalActivity = InternalUserActivity.ManageAatfs;
            breadcrumb.InternalOrganisation = aatf.Name;
        }

        [Fact]
        public async Task EditPartnershipOrganisationDetails_CanEditOrganisation_ViewModelShouldBeReturned()
        {
            var organisationData = fixture.Build<OrganisationData>()
                .WithAutoProperties()
                .With(o => o.CanEditOrganisation, true)
                .Create();

            var countries = fixture.CreateMany<CountryData>().ToList();
            var organisationId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetInternalOrganisation>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            var result = await controller.EditPartnershipOrganisationDetails(schemeId, organisationId, aatfId, FacilityType.Aatf) as ViewResult;

            var model = result.Model as EditPartnershipOrganisationDetailsViewModel;

            model.OrganisationType.Should().Be(organisationData.OrganisationType);            
            model.BusinessTradingName.Should().Be(organisationData.TradingName);
            model.BusinessAddress.Should().Be(organisationData.BusinessAddress);
            model.BusinessAddress.Countries.Should().BeEquivalentTo(countries);
            model.SchemeId.Should().Be(schemeId);
            model.OrgId.Should().Be(organisationId);
            model.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public async Task EditPartnershipOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var schemeId = Guid.NewGuid();
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(schemeId)).Returns(organisation);
            await controller.EditPartnershipOrganisationDetails(schemeId, A.Dummy<Guid>(), null, FacilityType.Aatf);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageScheme);
            breadcrumb.InternalScheme.Should().Be(organisation);
        }

        [Fact]
        public async Task EditPartnershipOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatf);
            await controller.EditPartnershipOrganisationDetails(null, organisationId, aatfId, FacilityType.Aatf);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumb.InternalAatf.Should().Be(aatf.Name);
        }

        [Fact]
        public async Task PostEditPartnershipOrganisationDetails_OrganisationUpdated_SearchCacheShouldBeInvalidated()
        {
            var model = fixture.Build<EditPartnershipOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditPartnershipOrganisationDetails(model);

            A.CallTo(() => cache.InvalidateOrganisationSearch()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PostEditPartnershipOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditPartnershipOrganisationDetailsViewModel>().WithAutoProperties().Create();
            model.AatfId = null;
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(model.SchemeId.Value)).Returns(organisation);

            await controller.EditPartnershipOrganisationDetails(model);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageScheme);
            breadcrumb.InternalScheme.Should().Be(organisation);
        }

        [Fact]
        public async Task PostEditPartnershipOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditPartnershipOrganisationDetailsViewModel>().WithAutoProperties().Create();
            model.SchemeId = null;

            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(model.OrgId, model.AatfId.Value)).Returns(aatf);

            await controller.EditPartnershipOrganisationDetails(model);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumb.InternalAatf.Should().Be(aatf.Name);
        }

        [Fact]
        public async Task PostEditPartnershipOrganisationDetails_GivenUpdateAndScheme_ShouldBeRedirectToSchemeOverview()
        {
            var model = fixture.Build<EditPartnershipOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Create();

            var result = await controller.EditPartnershipOrganisationDetails(model) as RedirectToRouteResult;

            result.RouteValues["controller"].Should().Be("Scheme");
            result.RouteValues["action"].Should().Be("Overview");
            result.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
            result.RouteValues["schemeId"].Should().Be(model.SchemeId);
        }

        [Fact]
        public async Task PostEditPartnershipOrganisationDetails_GivenUpdateAndAatf_ShouldBeRedirectToAatfOrganisationDetails()
        {
            var model = fixture.Build<EditPartnershipOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.SchemeId)
                .Create();

            var url = fixture.Create<string>();

            A.CallTo(() => urlHelper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == model.AatfId && o.GetPropertyValue<string>("controller") == "Aatf"))).Returns(url);

            var result = await controller.EditPartnershipOrganisationDetails(model) as RedirectResult;

            result.Url.Should().Be($"{url}#organisationDetails");
        }

        [Fact]
        public async Task PostEditPartnershipOrganisationDetails_GivenNoAatfOrSchemeId_ViewShouldBeReturned()
        {
            var model = fixture.Build<EditPartnershipOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.AatfId)
                .Without(c => c.SchemeId)
                .Create();

            var result = await controller.EditPartnershipOrganisationDetails(model) as ViewResult;

            result.ViewData.Should().BeEmpty();
        }

        [Fact]
        public async Task EditSoleTraderOrganisationDetails_CanEditOrganisation_ViewModelShouldBeReturned()
        {
            var organisationData = fixture.Build<OrganisationData>()
                .WithAutoProperties()
                .With(o => o.CanEditOrganisation, true)
                .Create();

            var countries = fixture.CreateMany<CountryData>().ToList();
            var organisationId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetInternalOrganisation>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            var result = await controller.EditSoleTraderOrganisationDetails(schemeId, organisationId, aatfId, FacilityType.Aatf) as ViewResult;

            var model = result.Model as EditSoleTraderOrganisationDetailsViewModel;

            model.OrganisationType.Should().Be(organisationData.OrganisationType);
            model.BusinessTradingName.Should().Be(organisationData.TradingName);
            model.BusinessAddress.Should().Be(organisationData.BusinessAddress);
            model.BusinessAddress.Countries.Should().BeEquivalentTo(countries);
            model.SchemeId.Should().Be(schemeId);
            model.OrgId.Should().Be(organisationId);
            model.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public async Task EditSoleTraderOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var schemeId = Guid.NewGuid();
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(schemeId)).Returns(organisation);
            await controller.EditSoleTraderOrganisationDetails(schemeId, A.Dummy<Guid>(), null, FacilityType.Aatf);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageScheme);
            breadcrumb.InternalScheme.Should().Be(organisation);
        }

        [Fact]
        public async Task EditSoleTraderOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatf);
            await controller.EditSoleTraderOrganisationDetails(null, organisationId, aatfId, FacilityType.Aatf);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumb.InternalAatf.Should().Be(aatf.Name);
        }

        [Fact]
        public async Task PostEditSoleTraderOrganisationDetails_OrganisationUpdated_SearchCacheShouldBeInvalidated()
        {
            var model = fixture.Build<EditSoleTraderOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditSoleTraderOrganisationDetails(model);

            A.CallTo(() => cache.InvalidateOrganisationSearch()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PostEditSoleTraderOrganisationDetails_ModelIsValid_UpdateOrganisationDetailsRequestSent()
        {
            var model = fixture.Build<EditSoleTraderOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditSoleTraderOrganisationDetails(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>.That.Matches(
                u => u.OrganisationData.Id.Equals(model.OrgId)
                     && u.OrganisationData.OrganisationType.Equals(model.OrganisationType)
                     && u.OrganisationData.TradingName.Equals(model.BusinessTradingName)
                     && u.OrganisationData.BusinessAddress.Equals(model.BusinessAddress)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PostEditSoleTraderOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditSoleTraderOrganisationDetailsViewModel>().WithAutoProperties().Create();
            model.AatfId = null;
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(model.SchemeId.Value)).Returns(organisation);

            await controller.EditSoleTraderOrganisationDetails(model);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageScheme);
            breadcrumb.InternalScheme.Should().Be(organisation);
        }

        [Fact]
        public async Task PostEditSoleTraderOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditSoleTraderOrganisationDetailsViewModel>().WithAutoProperties().Create();
            model.SchemeId = null;
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(model.OrgId, model.AatfId.Value)).Returns(aatf);

            await controller.EditSoleTraderOrganisationDetails(model);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumb.InternalAatf.Should().Be(aatf.Name);
        }

        [Fact]
        public async Task PostEditSoleTraderOrganisationDetails_GivenUpdateAndScheme_ShouldBeRedirectToSchemeOverview()
        {
            var model = fixture.Build<EditSoleTraderOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Create();

            var result = await controller.EditSoleTraderOrganisationDetails(model) as RedirectToRouteResult;

            result.RouteValues["controller"].Should().Be("Scheme");
            result.RouteValues["action"].Should().Be("Overview");
            result.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
            result.RouteValues["schemeId"].Should().Be(model.SchemeId);
        }

        [Fact]
        public async Task PostEditSoleTraderOrganisationDetails_GivenUpdateAndAatf_ShouldBeRedirectToAatfOrganisationDetails()
        {
            var model = fixture.Build<EditSoleTraderOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.SchemeId)
                .Create();

            var url = fixture.Create<string>();

            A.CallTo(() => urlHelper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == model.AatfId && o.GetPropertyValue<string>("controller") == "Aatf"))).Returns(url);

            var result = await controller.EditSoleTraderOrganisationDetails(model) as RedirectResult;

            result.Url.Should().Be($"{url}#organisationDetails");
        }

        [Fact]
        public async Task PostEditSoleTraderOrganisationDetails_GivenNoAatfOrSchemeId_ViewShouldBeReturned()
        {
            var model = fixture.Build<EditSoleTraderOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.AatfId)
                .Without(c => c.SchemeId)
                .Create();

            var result = await controller.EditSoleTraderOrganisationDetails(model) as ViewResult;

            result.ViewData.Should().BeEmpty();
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenUpdateAndScheme_ShouldBeRedirectToSchemeOverview()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>()
                .Without(c => c.AatfId)
                .WithAutoProperties().Create();

            var result = await controller.EditRegisteredCompanyOrganisationDetails(model) as RedirectToRouteResult;

            result.RouteValues["controller"].Should().Be("Scheme");
            result.RouteValues["action"].Should().Be("Overview");
            result.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
            result.RouteValues["schemeId"].Should().Be(model.SchemeId);
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenUpdateAndAatf_ShouldBeRedirectToAatfOrganisationDetails()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.SchemeId)
                .Create();

            var url = fixture.Create<string>();

            A.CallTo(() => urlHelper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == model.AatfId && o.GetPropertyValue<string>("controller") == "Aatf"))).Returns(url);

            var result = await controller.EditRegisteredCompanyOrganisationDetails(model) as RedirectResult;

            result.Url.Should().Be($"{url}#organisationDetails");
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenNoAatfOrSchemeId_ViewShouldBeReturned()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.AatfId)
                .Without(c => c.SchemeId)
                .Create();

            var result = await controller.EditRegisteredCompanyOrganisationDetails(model) as ViewResult;

            result.ViewData.Should().BeEmpty();
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>().WithAutoProperties().Create();
            model.SchemeId = null;
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(model.OrgId, model.AatfId.Value)).Returns(aatf);

            await controller.EditRegisteredCompanyOrganisationDetails(model);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            breadcrumb.InternalAatf.Should().Be(aatf.Name);
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>().WithAutoProperties().Create();
            model.AatfId = null;
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(model.SchemeId.Value)).Returns(organisation);

            await controller.EditRegisteredCompanyOrganisationDetails(model);

            breadcrumb.InternalActivity.Should().Be(InternalUserActivity.ManageScheme);
            breadcrumb.InternalScheme.Should().Be(organisation);
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_ModelIsValid_UpdateOrganisationDetailsRequestSent()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditRegisteredCompanyOrganisationDetails(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>.That.Matches(
                u => u.OrganisationData.Id.Equals(model.OrgId)
                     && u.OrganisationData.OrganisationType.Equals(model.OrganisationType)
                     && u.OrganisationData.CompanyRegistrationNumber.Equals(model.CompaniesRegistrationNumber)
                     && u.OrganisationData.Name.Equals(model.CompanyName)
                     && u.OrganisationData.TradingName.Equals(model.BusinessTradingName)
                     && u.OrganisationData.BusinessAddress.Equals(model.BusinessAddress)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_OrganisationUpdated_SearchCacheShouldBeInvalidated()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditRegisteredCompanyOrganisationDetails(model);

            A.CallTo(() => cache.InvalidateOrganisationSearch()).MustHaveHappenedOnceExactly();
        }

        public static IEnumerable<object> Guids
        {
            get
            {
                yield return new object[] { Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { null, null };
            }
        }
    }
}
