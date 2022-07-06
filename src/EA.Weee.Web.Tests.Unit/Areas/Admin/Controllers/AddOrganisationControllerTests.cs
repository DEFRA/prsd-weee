namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Api.Client;
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Organisations;
    using Core.Search;
    using Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Extensions;
    using Services;
    using Services.Caching;
    using TestHelpers;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Web.Areas.Admin.ViewModels.AddOrganisation;
    using Web.Areas.Admin.ViewModels.AddOrganisation.Details;
    using Web.Areas.Admin.ViewModels.AddOrganisation.Type;
    using Web.Areas.Admin.ViewModels.Home;
    using Weee.Requests.Admin;
    using Xunit;
    using AddressData = Core.Shared.AddressData;

    public class AddOrganisationControllerTests
    {
        private readonly AddOrganisationController controller;
        private readonly Fixture fixture;
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly IWeeeClient weeeClient;
        private readonly IList<CountryData> countries;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IWeeeCache cache;

        public AddOrganisationControllerTests()
        {
            fixture = new Fixture();
            organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            weeeClient = A.Fake<IWeeeClient>();
            countries = A.Dummy<IList<CountryData>>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            var configurationService = A.Fake<ConfigurationService>();

            A.CallTo(() => configurationService.CurrentConfiguration.MaximumAatfOrganisationSearchResults).Returns(5);

            controller = new AddOrganisationController(organisationSearcher, () => weeeClient, breadcrumbService, cache, configurationService);
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(AddOrganisationController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public void GetSearch_ReturnsView()
        {
            var entityType = fixture.Create<EntityType>();
            var result = controller.Search(entityType) as ViewResult;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Search");
        }

        [Fact]
        public void PostSearch_InvalidViewModel_ReturnsView()
        {
            var viewModel = new SearchViewModel()
            {
                SearchTerm = "test"
            };

            controller.ModelState.AddModelError("error", "error");

            var result = controller.Search(viewModel) as ViewResult;
            var outputModel = result.Model as SearchViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Search");
            Assert.Equal(viewModel, outputModel);
        }

        [Theory]
        [InlineData(EntityType.Aatf)]
        [InlineData(EntityType.Ae)]
        [InlineData(EntityType.Pcs)]
        public void PostSearch_ValidViewModelAndAatf_RedirectsToSearchResults(EntityType entityType)
        {
            var viewModel = new SearchViewModel()
            {
                SearchTerm = "test",
                EntityType = entityType
            };

            var result = controller.Search(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("SearchResults");
            result.RouteValues["controller"].Should().Be("AddOrganisation");
            result.RouteValues["searchTerm"].Should().Be(viewModel.SearchTerm);
            result.RouteValues["entityType"].Should().Be(viewModel.EntityType);
        }

        [Fact]
        public void TypeGet_ReturnsViewWithViewModel_WithSearchText()
        {
            const string searchText = "Company";
            var entityType = fixture.Create<EntityType>();

            var result = controller.Type(searchText, entityType) as ViewResult;

            var resultViewModel = result.Model as OrganisationTypeViewModel;

            Assert.Equal(searchText, resultViewModel.SearchedText);
            Assert.Equal(entityType, resultViewModel.EntityType);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Type");
        }

        [Fact]
        public void TypePost_ModelNotValid_ReturnsView()
        {
            var viewModel = new OrganisationTypeViewModel()
            {
                SearchedText = "Company"
            };

            controller.ModelState.AddModelError("error", "error");

            var result = controller.Type(viewModel) as ViewResult;
            var resultViewModel = result.Model as OrganisationTypeViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Type");
            Assert.Equal(viewModel.SearchedText, resultViewModel.SearchedText);
            Assert.Equal(viewModel.PossibleValues, resultViewModel.PossibleValues);
        }

        [Theory]
        [InlineData("Sole trader or individual", "SoleTraderDetails")]
        [InlineData("Partnership", "PartnershipDetails")]
        [InlineData("Registered company", "RegisteredCompanyDetails")]
        public void TypePost_ValidViewModel_ReturnsCorrectRedirect(string selectedValue, string action)
        {
            var viewModel = new OrganisationTypeViewModel()
            {
                SearchedText = "Company",
                SelectedValue = selectedValue,
                EntityType = fixture.Create<EntityType>()
            };

            var result = controller.Type(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be("AddOrganisation");
            result.RouteValues["organisationType"].Should().Be(viewModel.SelectedValue);
            result.RouteValues["searchedText"].Should().Be(viewModel.SearchedText);
            result.RouteValues["entityType"].Should().Be(viewModel.EntityType);
        }

        [Fact]
        public async Task PartnershipDetailsGet_ReturnsViewWithViewModelPopulated()
        {
            const string searchText = "Company";
            const string organisationType = "Sole trader or individual";
            var entityType = fixture.Create<EntityType>();

            var result = await controller.PartnershipDetails(organisationType, entityType, searchText) as ViewResult;

            var resultViewModel = result.Model as PartnershipDetailsViewModel;

            Assert.Equal(searchText, resultViewModel.BusinessTradingName);
            Assert.Equal(organisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);
            Assert.Equal(entityType, resultViewModel.EntityType);

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
        }

        [Fact]
        public async Task PartnershipDetailsPost_ModelNotValid_ReturnsView()
        {
            var viewModel = new PartnershipDetailsViewModel
            {
                BusinessTradingName = "Company", OrganisationType = "Sole trader or individual", Address = {Countries = countries}
            };

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.PartnershipDetails(viewModel) as ViewResult;
            var resultViewModel = result.Model as PartnershipDetailsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
            Assert.Equal(viewModel.BusinessTradingName, resultViewModel.BusinessTradingName);
            Assert.Equal(viewModel.OrganisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);
        }

        [Theory]
        [InlineData(EntityType.Aatf, "AddAatf", "Add")]
        [InlineData(EntityType.Ae, "AddAatf", "Add")]
        [InlineData(EntityType.Pcs, "Scheme", "AddScheme")]
        public async Task PartnershipDetailsPost_ValidViewModel_ReturnsCorrectRedirect(EntityType type, string expectedController, string expectedAction)
        {
            var viewModel = new PartnershipDetailsViewModel
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual",
                EntityType = type,
                Address = {Countries = countries}
            };

            var result = await controller.PartnershipDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(expectedAction);
            result.RouteValues["controller"].Should().Be(expectedController);

            result.RouteValues["organisationId"].Should().NotBe(null);

            if (type != EntityType.Pcs)
            {
                result.RouteValues["facilityType"].Should().Be(viewModel.EntityType);
            }
        }

        [Fact]
        public async Task PartnershipDetailsPost_ValidViewModel_RequestWithCorrectParametersCreated()
        {
            var viewModel = new PartnershipDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual",
                Address = A.Dummy<AddressData>()
            };

            viewModel.Address.Countries = countries;

            await controller.PartnershipDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateOrganisationAdmin>.That.Matches(
                p => p.BusinessName == viewModel.BusinessTradingName
                && p.Address == viewModel.Address
                && p.OrganisationType == viewModel.OrganisationType.GetValueFromDisplayName<OrganisationType>()))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task SoleTraderDetailsGet_ReturnsViewWithViewModelPopulated()
        {
            const string searchText = "Company";
            const string organisationType = "Sole trader or individual";
            var entityType = fixture.Create<EntityType>();

            var result = await controller.SoleTraderDetails(organisationType, entityType, searchText) as ViewResult;

            var resultViewModel = result.Model as SoleTraderDetailsViewModel;

            Assert.Equal(searchText, resultViewModel.CompanyName);
            Assert.Equal(organisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);
            Assert.Equal(entityType, resultViewModel.EntityType);

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
        }

        [Fact]
        public async Task SoleTraderDetailsPost_ModelNotValid_ReturnsView()
        {
            var viewModel = new SoleTraderDetailsViewModel
            {
                BusinessTradingName = "Company", OrganisationType = "Sole trader or individual", Address = {Countries = countries}
            };

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.SoleTraderDetails(viewModel) as ViewResult;
            var resultViewModel = result.Model as SoleTraderDetailsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
            Assert.Equal(viewModel.BusinessTradingName, resultViewModel.BusinessTradingName);
            Assert.Equal(viewModel.OrganisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);
        }

        [Theory]
        [InlineData(EntityType.Aatf, "AddAatf", "Add")]
        [InlineData(EntityType.Ae, "AddAatf", "Add")]
        [InlineData(EntityType.Pcs, "Scheme", "AddScheme")]
        public async Task SoleTraderDetailsPost_ValidViewModel_ReturnsCorrectRedirect(EntityType type, string expectedController, string expectedAction)
        {
            var viewModel = new SoleTraderDetailsViewModel
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual",
                EntityType = type,
                Address = {Countries = countries}
            };

            var result = await controller.SoleTraderDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(expectedAction);
            result.RouteValues["controller"].Should().Be(expectedController);

            result.RouteValues["organisationId"].Should().NotBe(null);

            if (type != EntityType.Pcs)
            {
                result.RouteValues["facilityType"].Should().Be(viewModel.EntityType);
            }
        }

        [Fact]
        public async Task SoleTraderDetailsPost_ValidViewModel_RequestWithCorrectParametersCreated()
        {
            var viewModel = new SoleTraderDetailsViewModel()
            {
                CompanyName = "Company",
                OrganisationType = "Sole trader or individual",
                Address = A.Dummy<AddressData>()
            };

            viewModel.Address.Countries = countries;

            await controller.SoleTraderDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateOrganisationAdmin>.That.Matches(
                p => p.BusinessName == viewModel.CompanyName
                && p.Address == viewModel.Address
                && p.OrganisationType == viewModel.OrganisationType.GetValueFromDisplayName<OrganisationType>()))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task RegisteredCompanyDetailsGet_ReturnsViewWithViewModelPopulated()
        {
            const string searchText = "Company";
            const string organisationType = "Registered company";
            var entityType = fixture.Create<EntityType>();

            var result = await controller.RegisteredCompanyDetails(organisationType, entityType, searchText) as ViewResult;

            var resultViewModel = result.Model as RegisteredCompanyDetailsViewModel;

            Assert.Equal(searchText, resultViewModel.CompanyName);
            Assert.Equal(organisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);
            Assert.Equal(entityType, resultViewModel.EntityType);

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "RegisteredCompanyDetails");
        }

        [Fact]
        public async Task RegisteredCompanyDetailsPost_InvalidateCacheMustBeRun()
        {
            var viewModel = new RegisteredCompanyDetailsViewModel
            {
                BusinessTradingName = "Company",
                OrganisationType = "Registered company",
                CompaniesRegistrationNumber = "1234567",
                CompanyName = "Name",
                Address = {Countries = countries}
            };

            await controller.RegisteredCompanyDetails(viewModel);

            A.CallTo(() => cache.InvalidateOrganisationSearch()).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task RegisteredCompanyDetailsPost_ModelNotValid_ReturnsView()
        {
            var viewModel = new RegisteredCompanyDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Registered company",
                CompaniesRegistrationNumber = "1234567",
                CompanyName = "Name"
            };
            viewModel.Address.Countries = countries;

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.RegisteredCompanyDetails(viewModel) as ViewResult;
            var resultViewModel = result.Model as RegisteredCompanyDetailsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
            Assert.Equal(viewModel.BusinessTradingName, resultViewModel.BusinessTradingName);
            Assert.Equal(viewModel.OrganisationType, resultViewModel.OrganisationType);
            Assert.Equal(viewModel.CompanyName, resultViewModel.CompanyName);
            Assert.Equal(viewModel.CompaniesRegistrationNumber, resultViewModel.CompaniesRegistrationNumber);
            Assert.Equal(countries, resultViewModel.Address.Countries);
        }

        [Theory]
        [InlineData(EntityType.Aatf, "AddAatf", "Add")]
        [InlineData(EntityType.Ae, "AddAatf", "Add")]
        [InlineData(EntityType.Pcs, "Scheme", "AddScheme")]
        public async Task RegisteredCompanyDetailsPost_ValidViewModel_ReturnsCorrectRedirect(EntityType type, string expectedController, string expectedAction)
        {
            var viewModel = new RegisteredCompanyDetailsViewModel()
            {
                BusinessTradingName = "name",
                OrganisationType = "Registered company",
                CompaniesRegistrationNumber = "1234567",
                CompanyName = "Name",
                EntityType = type
            };

            viewModel.Address.Countries = countries;

            var result = await controller.RegisteredCompanyDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(expectedAction);
            result.RouteValues["controller"].Should().Be(expectedController);

            result.RouteValues["organisationId"].Should().NotBe(null);

            if (type != EntityType.Pcs)
            {
                result.RouteValues["facilityType"].Should().Be(viewModel.EntityType);
            }
        }

        [Theory]
        [InlineData(EntityType.Aatf, "Manage AATFs")]
        [InlineData(EntityType.Ae, "Manage AEs")]
        [InlineData(EntityType.Pcs, "Manage PCSs")]
        public void SearchGet_Always_SetsInternalBreadcrumb(EntityType entityType, string expectedBreadcrumb)
        {
            controller.Search(entityType);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        [Theory]
        [InlineData(EntityType.Aatf, "Manage AATFs")]
        [InlineData(EntityType.Ae, "Manage AEs")]
        [InlineData(EntityType.Pcs, "Manage PCSs")]
        public void SearchPost_Always_SetsInternalBreadcrumb(EntityType entityType, string expectedBreadcrumb)
        {
            var viewModel = fixture.Build<SearchViewModel>().With(m => m.EntityType, entityType).Create();
            controller.Search(viewModel);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        [Theory]
        [InlineData(EntityType.Aatf, "Manage AATFs")]
        [InlineData(EntityType.Ae, "Manage AEs")]
        [InlineData(EntityType.Pcs, "Manage PCSs")]
        public async Task SearchResultsGet_Always_SetsInternalBreadcrumb(EntityType entityType, string expectedBreadcrumb)
        {
            await controller.SearchResults("test", entityType);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        [Theory]
        [InlineData(EntityType.Aatf, "Manage AATFs")]
        [InlineData(EntityType.Ae, "Manage AEs")]
        [InlineData(EntityType.Pcs, "Manage PCSs")]
        public async Task SearchResultsPost_Always_SetsInternalBreadcrumb(EntityType entityType, string expectedBreadcrumb)
        {
            var viewModel = fixture.Build<SearchResultsViewModel>().With(m => m.EntityType, entityType).Create();

            viewModel.SelectedOrganisationId = viewModel.Results.FirstOrDefault().OrganisationId;

            await controller.SearchResults(viewModel);

            Assert.Equal(expectedBreadcrumb, breadcrumbService.InternalActivity);
        }

        [Fact]
        public void TypeGet_Always_SetsInternalBreadcrumb()
        {
            controller.Type("test", fixture.Create<EntityType>());

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public void TypePost_Always_SetsInternalBreadcrumb()
        {
            var viewModel = new OrganisationTypeViewModel()
            {
                SearchedText = "Company",
                SelectedValue = "Partnership"
            };

            controller.Type(viewModel);

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task PartnershipDetailsGet_Always_SetsInternalBreadcrumb()
        {
            await controller.PartnershipDetails("test", fixture.Create<EntityType>());

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task PartnershipDetailsPost_Always_SetsInternalBreadcrumb()
        {
            var viewModel = new PartnershipDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual",
                Address = A.Dummy<AddressData>()
            };

            await controller.PartnershipDetails(viewModel);

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task SoleTraderDetailsGet_Always_SetsInternalBreadcrumb()
        {
            await controller.SoleTraderDetails("test", fixture.Create<EntityType>());

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task SoleTraderDetailsPost_Always_SetsInternalBreadcrumb()
        {
            var viewModel = new SoleTraderDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual",
                Address = A.Dummy<AddressData>()
            };

            await controller.SoleTraderDetails(viewModel);

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task RegisteredCompanyDetailsGet_Always_SetsInternalBreadcrumb()
        {
            await controller.RegisteredCompanyDetails("test", fixture.Create<EntityType>());

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task RegisteredCompanyDetailsPost_Always_SetsInternalBreadcrumb()
        {
            var viewModel = new RegisteredCompanyDetailsViewModel
            {
                BusinessTradingName = "name",
                OrganisationType = "Registered company",
                CompaniesRegistrationNumber = "1234567",
                CompanyName = "Name",
                Address = {Countries = countries}
            };

            await controller.RegisteredCompanyDetails(viewModel);

            Assert.Equal("Add new organisation", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async Task FetchSearchResultsJson_GivenNotAnAjaxRequest_InvalidOperationExceptionExpected()
        {
            SetupControllerRequest();

            var exception = await Record.ExceptionAsync(() => controller.FetchSearchResultsJson(A.Dummy<string>()));

            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task FetchSearchResultsJson_GivenNotValidModel_EmptyJsonExpected()
        {
            SetupControllerAjaxRequest();

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.FetchSearchResultsJson(A.Dummy<string>()) as JsonResult;

            result.JsonRequestBehavior.Should().Be(JsonRequestBehavior.AllowGet);
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task FetchSearchResultsJson_GivenValidRequest_JsonResultExpected()
        {
            const string query = "query";

            SetupControllerAjaxRequest();
            var address = new AddressData()
            {
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountryName = "England"
            };

            var organisationResult = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult() { Name = "name", OrganisationId = Guid.NewGuid(), Address = address }
            };

            A.CallTo(() => organisationSearcher.Search(query, A<int>._, true)).Returns(organisationResult);

            var jsonResult = await controller.FetchSearchResultsJson(query) as JsonResult;

            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<OrganisationSearchResult>>(serializer.Serialize(jsonResult.Data));

            result.Count().Should().Be(1);
            result.ElementAt(0).Should().BeEquivalentTo(organisationResult.ElementAt(0));
        }

        [Fact]
        public async Task FetchSearchResultsJson_GivenValidRequest_JsonResultExpectedShouldNotContainBalancingScheme()
        {
            const string query = "query";

            SetupControllerAjaxRequest();
            var address = new AddressData()
            {
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountryName = "England"
            };

            var organisationId = Guid.NewGuid();
            var balancingSchemeId = Guid.NewGuid();

            var organisationResult = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult() { Name = "name", OrganisationId = organisationId, Address = address },
                new OrganisationSearchResult() { Name = "name", OrganisationId = balancingSchemeId, Address = address, IsBalancingScheme = true }
            };

            A.CallTo(() => organisationSearcher.Search(query, A<int>._, true)).Returns(organisationResult);

            var jsonResult = await controller.FetchSearchResultsJson(query) as JsonResult;

            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<OrganisationSearchResult>>(serializer.Serialize(jsonResult.Data));

            result.Count().Should().Be(1);
            result.Should().Contain(r => r.OrganisationId == organisationId);
            result.Should().NotContain(r => r.OrganisationId == balancingSchemeId);
        }

        [Fact]
        public async Task GetSearch_SearchTermThatExists_RedirectsToSearchResultsWithResultsInViewModel()
        {
            var organisationId = Guid.NewGuid();
            const string searchTerm = "civica";
            var entityType = fixture.Create<EntityType>();

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            var viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId
            };

            var result = await controller.SearchResults(searchTerm, entityType) as ViewResult;
            var outputModel = result.Model as SearchResultsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SearchResults");
            Assert.Equal(viewModel.SearchTerm, outputModel.SearchTerm);
            Assert.Equal(viewModel.Results, outputModel.Results);
        }

        [Fact]
        public async Task GetSearch_SearchTermReturnsBalancingScheme_RedirectsToSearchResultsWithResultsInViewModelWithoutBalancingScheme()
        {
            var organisationId = Guid.NewGuid();
            var balancingSchemeOrganisationId = Guid.NewGuid();

            var entityType = fixture.Create<EntityType>();

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = organisationId
                },
                new OrganisationSearchResult()
                {
                    OrganisationId = balancingSchemeOrganisationId,
                    IsBalancingScheme = true
                }
            };

            A.CallTo(() => organisationSearcher.Search(A<string>._, 5, A<bool>._)).Returns(results);

            var result = await controller.SearchResults(A.Dummy<string>(), entityType) as ViewResult;

            var outputModel = result.Model as SearchResultsViewModel;
            outputModel.Results.Should().NotContain(r => r.OrganisationId == balancingSchemeOrganisationId);
            outputModel.Results.Should().Contain(r => r.OrganisationId == organisationId);
        }

        [Fact]
        public async Task PostSearch_GivenInvalidModelAndSearchTermReturnsBalancingScheme_ResultsInViewModelWithoutBalancingScheme()
        {
            var organisationId = Guid.NewGuid();
            var balancingSchemeOrganisationId = Guid.NewGuid();

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = organisationId
                },
                new OrganisationSearchResult()
                {
                    OrganisationId = balancingSchemeOrganisationId,
                    IsBalancingScheme = true
                }
            };

            var model = new SearchResultsViewModel();
            A.CallTo(() => organisationSearcher.Search(A<string>._, 5, A<bool>._)).Returns(results);

            controller.ModelState.AddModelError("error", "error");
            var result = await controller.SearchResults(model) as ViewResult;

            var outputModel = result.Model as SearchResultsViewModel;
            outputModel.Results.Should().NotContain(r => r.OrganisationId == balancingSchemeOrganisationId);
            outputModel.Results.Should().Contain(r => r.OrganisationId == organisationId);
        }

        [Fact]
        public async Task PostSearch_InValidViewModelSelectedOrganisation_ReturnsView()
        {
            var organisationId = Guid.NewGuid();
            const string searchTerm = "civica";

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            controller.ModelState.AddModelError("error", "error");

            var viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId
            };

            var result = await controller.SearchResults(viewModel) as ViewResult;
            var outputModel = result.Model as SearchResultsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SearchResults");
            Assert.Equal(viewModel.SearchTerm, outputModel.SearchTerm);
            Assert.Equal(viewModel.Results, outputModel.Results);
        }

        [Fact]
        public async Task PostSearchForAatf_ValidViewModelSelectedOrganisation_RedirectsToAdd()
        {
            var organisationId = Guid.NewGuid();
            var searchTerm = "civica";

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            var viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId,
                EntityType = EntityType.Aatf
            };

            var result = await controller.SearchResults(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Add");
            result.RouteValues["controller"].Should().Be("AddAatf");
            Assert.Equal(organisationId, result.RouteValues["organisationId"]);
        }

        [Fact]
        public async Task PostSearchForPcs_ValidViewModelSelectedOrganisation_RedirectsToAdd()
        {
            var organisationId = Guid.NewGuid();
            var searchTerm = "civica";

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            var viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId,
                EntityType = EntityType.Pcs
            };

            var result = await controller.SearchResults(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("AddScheme");
            result.RouteValues["controller"].Should().Be("Scheme");
            Assert.Equal(organisationId, result.RouteValues["organisationId"]);
        }

        [Fact]
        public async Task PostSearchForPcs_ValidViewModelSelectedOrganisationAlreadyHasPcs_RedirectsToOrgAlreadyHasPcs()
        {
            var organisationId = Guid.NewGuid();
            var searchTerm = "civica";

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId,
                    PcsCount = 1
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            var viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId,
                EntityType = EntityType.Pcs
            };

            var result = await controller.SearchResults(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrgAlreadyHasScheme");
            result.RouteValues["controller"].Should().Be("AddOrganisation");
            Assert.Equal(searchTerm, result.RouteValues["searchTerm"]);
        }

        [Fact]
        public void OrgAlreadyHasSchemeGet_BreadCrumbShouldBeSet()
        {
            controller.OrgAlreadyHasScheme(fixture.Create<string>());

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageScheme);
        }

        [Fact]
        public void OrgAlreadyHasSchemeGet_ViewModelShouldBeReturned()
        {
            var search = fixture.Create<string>();

            var result = controller.OrgAlreadyHasScheme(search) as ViewResult;

            var model = result.Model as OrgAlreadyHasSchemeViewModel;
            model.SearchTerm.Should().Be(search);
        }

        [Fact]
        public void OrgAlreadyHasSchemeGet_DefaultViewShouldBeReturned()
        {
            var result = controller.OrgAlreadyHasScheme(fixture.Create<string>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        private void SetupControllerAjaxRequest()
        {
            var mocker = new HttpContextMocker();
            mocker.AttachToController(controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => mocker.HttpContextBase.Request).Returns(request);
            A.CallTo(() => request["X-Requested-With"]).Returns("XMLHttpRequest");
        }

        private void SetupControllerRequest()
        {
            var mocker = new HttpContextMocker();
            mocker.AttachToController(controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => mocker.HttpContextBase.Request).Returns(request);
        }
    }
}
