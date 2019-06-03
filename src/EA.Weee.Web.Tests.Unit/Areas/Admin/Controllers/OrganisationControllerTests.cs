namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using AutoFixture;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
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

    public class OrganisationControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly OrganisationController controller;
        private readonly Fixture fixture;
        private readonly UrlHelper urlHelper;

        public OrganisationControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
            breadcrumb = A.Fake<BreadcrumbService>();
            fixture = new Fixture();
            urlHelper = A.Fake<UrlHelper>();

            controller = new OrganisationController(() => weeeClient, cache, breadcrumb) { Url = urlHelper };
        }

        [Fact]
        public void OrganisationControllerInheritsAdminController()
        {
            typeof(OrganisationController).BaseType.Name.Should().Be(typeof(AdminController).Name);
        }

        [Fact]
        public void OrganisationControllerMustHaveAuthorizeClaimsAttribute()
        {
            typeof(OrganisationController).Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public async void GetEditRegisteredCompanyOrganisationDetails_CanEditOrganisationIsTrue_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(new List<CountryData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    Name = "CompanyName",
                    CompanyRegistrationNumber = "123456789",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = true
                });

            var result = await controller.EditRegisteredCompanyOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be("EditRegisteredCompanyOrganisationDetails");
        }

        [Fact]
        public async void GetEditSoleTraderOrIndividualOrganisationDetails_CanEditOrganisationIsTrue_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(new List<CountryData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = true
                });

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be("EditSoleTraderOrIndividualOrganisationDetails");
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

            var result = await controller.EditRegisteredCompanyOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

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
        public async void GetEditSoleTraderOrIndividualOrganisationDetails_CanEditOrganisationIsFalse_ReturnsHttpForbiddenResult()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new Core.Shared.AddressData(),
                    CanEditOrganisation = false
                });

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            result.Should().NotBeNull();
            result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Fact]
        public async void PostEditSoleTraderOrIndividualOrganisationDetails_ModelIsInvalid_GetsCountriesAndReturnsDefaultView()
        {
            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            var viewModel = new EditSoleTraderOrIndividualOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new Core.Shared.AddressData(),
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(viewModel);

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
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_ModelIsValid_UpdatesDetailsAndRedirectsToSchemeOverview()
        {
            var viewModel = new EditSoleTraderOrIndividualOrganisationDetailsViewModel
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

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(viewModel);

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

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            var result = await controller.EditRegisteredCompanyOrganisationDetails(schemeId, organisationId, aatfId) as ViewResult;

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
            await controller.EditRegisteredCompanyOrganisationDetails(schemeId, A.Dummy<Guid>(), A.Dummy<Guid>());

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = organisation;
        }

        [Theory]
        [MemberData(nameof(Guids))]
        
        public async Task GetEditRegisteredCompanyOrganisationDetails_GivenSchemeIdAndAatfId_BreadCrumbShouldNotBeSet(Guid? schemeId, Guid? aatfId)
        {
            await controller.EditRegisteredCompanyOrganisationDetails(schemeId, A.Dummy<Guid>(), aatfId);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation.Should().BeEmpty();
        }

        [Fact]
        public async Task GetEditRegisteredCompanyOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatf);
            await controller.EditRegisteredCompanyOrganisationDetails(A.Dummy<Guid>(), organisationId, aatfId);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = aatf.Name;
        }

        [Fact]
        public async Task GetEditSoleTraderOrIndividualOrganisationDetails_CanEditOrganisation_ViewModelShouldBeReturned()
        {
            var organisationData = fixture.Build<OrganisationData>()
                .WithAutoProperties()
                .With(o => o.CanEditOrganisation, true)
                .Create();

            var countries = fixture.CreateMany<CountryData>().ToList();
            var organisationId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(schemeId, organisationId, aatfId) as ViewResult;

            var model = result.Model as EditSoleTraderOrIndividualOrganisationDetailsViewModel;

            model.OrganisationType.Should().Be(organisationData.OrganisationType);            
            model.BusinessTradingName.Should().Be(organisationData.TradingName);
            model.BusinessAddress.Should().Be(organisationData.BusinessAddress);
            model.BusinessAddress.Countries.Should().BeEquivalentTo(countries);
            model.SchemeId.Should().Be(schemeId);
            model.OrgId.Should().Be(organisationId);
            model.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public async Task GetEditEditSoleTraderOrIndividualOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var schemeId = Guid.NewGuid();
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(schemeId)).Returns(organisation);
            await controller.EditSoleTraderOrIndividualOrganisationDetails(schemeId, A.Dummy<Guid>(), A.Dummy<Guid>());

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = organisation;
        }

        [Theory]
        [MemberData(nameof(Guids))]
        public async Task GetEditEditSoleTraderOrIndividualOrganisationDetails_GivenSchemeIdAndAatfId_BreadCrumbShouldNotBeSet(Guid? schemeId, Guid? aatfId)
        {
            await controller.EditSoleTraderOrIndividualOrganisationDetails(null, A.Dummy<Guid>(), null);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation.Should().BeEmpty();
        }

        [Fact]
        public async Task GetEditEditSoleTraderOrIndividualOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatf);
            await controller.EditSoleTraderOrIndividualOrganisationDetails(A.Dummy<Guid>(), organisationId, aatfId);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = aatf.Name;
        }

        [Fact]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_ModelIsValid_UpdateOrganisationDetailsRequestSent()
        {
            var model = fixture.Build<EditSoleTraderOrIndividualOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditSoleTraderOrIndividualOrganisationDetails(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>.That.Matches(
                u => u.OrganisationData.Id.Equals(model.OrgId)
                     && u.OrganisationData.OrganisationType.Equals(model.OrganisationType)
                     && u.OrganisationData.TradingName.Equals(model.BusinessTradingName)
                     && u.OrganisationData.BusinessAddress.Equals(model.BusinessAddress)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditSoleTraderOrIndividualOrganisationDetailsViewModel>().WithAutoProperties().Create();
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(model.SchemeId.Value)).Returns(organisation);

            await controller.EditSoleTraderOrIndividualOrganisationDetails(model);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = organisation;
        }

        [Theory]
        [MemberData(nameof(Guids))]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_GivenSchemeIdAndAatfId_BreadCrumbShouldNotBeSet(Guid? schemeId, Guid? aatfId)
        {
            var model = fixture.Build<EditSoleTraderOrIndividualOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditSoleTraderOrIndividualOrganisationDetails(model);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation.Should().BeEmpty();
        }

        [Fact]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditSoleTraderOrIndividualOrganisationDetailsViewModel>().WithAutoProperties().Create();
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(model.OrgId, model.AatfId.Value)).Returns(aatf);

            await controller.EditSoleTraderOrIndividualOrganisationDetails(model);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = aatf.Name;
        }

        [Fact]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_GivenUpdateAndScheme_ShouldBeRedirectToSchemeOverview()
        {
            var model = fixture.Build<EditSoleTraderOrIndividualOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Create();

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(model) as RedirectToRouteResult;

            result.RouteValues["controller"].Should().Be("Scheme");
            result.RouteValues["action"].Should().Be("Overview");
            result.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
            result.RouteValues["schemeId"].Should().Be(model.SchemeId);
        }

        [Fact]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_GivenUpdateAndAatf_ShouldBeRedirectToAatfOrganisationDetails()
        {
            var model = fixture.Build<EditSoleTraderOrIndividualOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.SchemeId)
                .Create();

            var url = fixture.Create<string>();

            A.CallTo(() => urlHelper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == model.AatfId && o.GetPropertyValue<string>("controller") == "Aatf"))).Returns(url);

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(model) as RedirectResult;

            result.Url.Should().Be($"{url}#organisationDetails");
        }

        [Fact]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_GivenNoAatfOrSchemeId_ViewShouldBeReturned()
        {
            var model = fixture.Build<EditSoleTraderOrIndividualOrganisationDetailsViewModel>()
                .WithAutoProperties()
                .Without(c => c.AatfId)
                .Without(c => c.SchemeId)
                .Create();

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(model) as ViewResult;

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

        [Theory]
        [MemberData(nameof(Guids))]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenSchemeIdAndAatfId_BreadCrumbShouldNotBeSet(Guid? schemeId, Guid? aatfId)
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>().WithAutoProperties().Create();

            await controller.EditRegisteredCompanyOrganisationDetails(model);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation.Should().BeEmpty();
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenAatfId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>().WithAutoProperties().Create();
            var aatf = fixture.Build<AatfData>().WithAutoProperties().Create();

            A.CallTo(() => cache.FetchAatfData(model.OrgId, model.AatfId.Value)).Returns(aatf);

            await controller.EditRegisteredCompanyOrganisationDetails(model);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = aatf.Name;
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_GivenSchemeId_BreadCrumbShouldBeSet()
        {
            var model = fixture.Build<EditRegisteredCompanyOrganisationDetailsViewModel>().WithAutoProperties().Create();
            const string organisation = "organisation";

            A.CallTo(() => cache.FetchSchemeName(model.SchemeId.Value)).Returns(organisation);

            await controller.EditRegisteredCompanyOrganisationDetails(model);

            breadcrumb.InternalActivity = "Manage PCSs";
            breadcrumb.InternalOrganisation = organisation;
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
