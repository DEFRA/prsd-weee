namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Type;
    using EA.Weee.Web.Filters;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;
    using AddressData = Core.Shared.AddressData;

    public class AddAatfControllerTests
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly IWeeeClient weeeClient;
        private readonly IList<CountryData> countries;

        public AddAatfControllerTests()
        {
            this.organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            this.weeeClient = A.Fake<IWeeeClient>();
            this.countries = A.Dummy<IList<CountryData>>();
        }

        [Fact]
        public void ControllerMustHaveAuthorizeClaimsAttribute()
        {
            typeof(AddAatfController).Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void GetSearch_ReturnsView()
        {
            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            ViewResult result = controller.Search() as ViewResult;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Search");
        }

        [Fact]
        public void PostSearch_InvalidViewModel_ReturnsView()
        {
            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            SearchViewModel viewModel = new SearchViewModel()
            {
                SearchTerm = "test"
            };

            controller.ModelState.AddModelError("error", "error");

            ViewResult result = controller.Search(viewModel) as ViewResult;
            SearchViewModel outputModel = result.Model as SearchViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Search");
            Assert.Equal(viewModel, outputModel);
        }

        [Fact]
        public void PostSearch_ValidViewModel_RedirectsToSearchResults()
        {
            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            SearchViewModel viewModel = new SearchViewModel()
            {
                SearchTerm = "test"
            };

            RedirectToRouteResult result = controller.Search(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("SearchResults");
            result.RouteValues["controller"].Should().Be("AddAatf");
            result.RouteValues["searchTerm"].Should().Be(viewModel.SearchTerm);
        }

        [Fact]
        public async Task GetSearch_SearchTermThatExists_RedirectsToSearchResultsWithResultsInViewModel()
        {
            Guid organisationId = Guid.NewGuid();
            string searchTerm = "civica";

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            SearchResultsViewModel viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId
            };

            ViewResult result = await controller.SearchResults(searchTerm) as ViewResult;
            SearchResultsViewModel outputModel = result.Model as SearchResultsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SearchResults");
            Assert.Equal(viewModel.SearchTerm, outputModel.SearchTerm);
            Assert.Equal(viewModel.Results, outputModel.Results);
        }

        [Fact]
        public async Task PostSearch_InValidViewModelSelectedOrganisation_ReturnsView()
        {
            Guid organisationId = Guid.NewGuid();
            string searchTerm = "civica";

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            controller.ModelState.AddModelError("error", "error");

            SearchResultsViewModel viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId
            };

            ViewResult result = await controller.SearchResults(viewModel) as ViewResult;
            SearchResultsViewModel outputModel = result.Model as SearchResultsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SearchResults");
            Assert.Equal(viewModel.SearchTerm, outputModel.SearchTerm);
            Assert.Equal(viewModel.Results, outputModel.Results);
        }

        [Fact]
        public async Task PostSearch_ValidViewModelSelectedOrganisation_RedirectsToAdd()
        {
            Guid organisationId = Guid.NewGuid();
            string searchTerm = "civica";

            IList<OrganisationSearchResult> results = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    Name = searchTerm,
                    OrganisationId = organisationId
                }
            };

            A.CallTo(() => organisationSearcher.Search(searchTerm, 5, false)).Returns(results);

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            SearchResultsViewModel viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId
            };

            RedirectToRouteResult result = await controller.SearchResults(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Add");
            result.RouteValues["controller"].Should().Be("AddAatf");
            Assert.Equal(organisationId, result.RouteValues["organisationId"]);
        }

        [Fact]
        public async Task AddGet_CreatesViewModel_ListsArePopulated()
        {
            AddAatfViewModel viewModel = CreateAddViewModel();

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            ViewResult result = await controller.Add(viewModel.OrganisationId) as ViewResult;

            AddAatfViewModel resultViewModel = result.Model as AddAatfViewModel;

            Assert.Equal(viewModel.SizeList, resultViewModel.SizeList);
            Assert.Equal(viewModel.StatusList, resultViewModel.StatusList);
            Assert.Equal(viewModel.ContactData.AddressData.Countries, resultViewModel.ContactData.AddressData.Countries);
            Assert.Equal(viewModel.SiteAddressData.Countries, resultViewModel.SiteAddressData.Countries);
            Assert.Equal(viewModel.OrganisationId, resultViewModel.OrganisationId);
        }

        [Fact]
        public async Task AddPost_ValidViewModel_ReturnsRedirect()
        {
            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            AddAatfViewModel viewModel = new AddAatfViewModel()
            {
                SelectedSizeValue = 1,
                SelectedStatusValue = 1
            };

            RedirectToRouteResult result = await controller.Add(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ManageAatfs");
            result.RouteValues["controller"].Should().Be("Aatf");
        }

        [Fact]
        public async Task AddPost_ValidViewModelRequestWithCorrectParametersCreated()
        {
            AddAatfViewModel viewModel = new AddAatfViewModel()
            {
                AatfName = "name",
                ApprovalNumber = "123",
                ApprovalDate = DateTime.Now,
                SiteAddressData = A.Fake<AatfAddressData>(),
                SelectedSizeValue = 1,
                SelectedStatusValue = 1,
                OrganisationId = Guid.NewGuid(),
                ContactData = A.Fake<AatfContactData>(),
                CompetentAuthoritiesList = A.Fake<List<UKCompetentAuthorityData>>(),
                CompetentAuthorityId = Guid.NewGuid()
            };

            AatfData aatfData = new AatfData(
                Guid.NewGuid(),
                viewModel.AatfName,
                viewModel.ApprovalNumber,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Id == viewModel.CompetentAuthorityId),
                Enumeration.FromValue<AatfStatus>(viewModel.SelectedStatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SelectedSizeValue),
                viewModel.ApprovalDate.GetValueOrDefault());

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            await controller.Add(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAatf>.That.Matches(
                p => p.OrganisationId == viewModel.OrganisationId
                && p.Aatf.Name == aatfData.Name
                && p.Aatf.ApprovalNumber == aatfData.ApprovalNumber
                && p.Aatf.CompetentAuthority == aatfData.CompetentAuthority
                && p.Aatf.AatfStatus == aatfData.AatfStatus
                && p.Aatf.SiteAddress == aatfData.SiteAddress
                && p.Aatf.Size == aatfData.Size
                && p.Aatf.ApprovalDate == aatfData.ApprovalDate
                && p.AatfContact == viewModel.ContactData))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task AddPost_InvalidViewModel_ReturnsViewWithViewModelPopulatedWithLists()
        {
            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);
            controller.ModelState.AddModelError("error", "error");

            AddAatfViewModel viewModel = CreateAddViewModel();

            ViewResult result = await controller.Add(viewModel) as ViewResult;
            AddAatfViewModel resultViewModel = result.Model as AddAatfViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Add");
            Assert.Equal(viewModel.SizeList, resultViewModel.SizeList);
            Assert.Equal(viewModel.StatusList, resultViewModel.StatusList);
            Assert.Equal(viewModel.ContactData.AddressData.Countries, resultViewModel.ContactData.AddressData.Countries);
            Assert.Equal(viewModel.SiteAddressData.Countries, resultViewModel.SiteAddressData.Countries);
            Assert.Equal(viewModel.OrganisationId, resultViewModel.OrganisationId);
        }

        [Fact]
        public void TypeGet_ReturnsViewWithViewModel_WithSearchText()
        {
            string searchText = "Company";

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            ViewResult result = controller.Type(searchText) as ViewResult;

            OrganisationTypeViewModel resultViewModel = result.Model as OrganisationTypeViewModel;

            Assert.Equal(searchText, resultViewModel.SearchedText);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Type");
        }

        [Fact]
        public void TypePost_ModelNotValid_ReturnsView()
        {
            OrganisationTypeViewModel viewModel = new OrganisationTypeViewModel()
            {
                SearchedText = "Company"
            };

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);
            controller.ModelState.AddModelError("error", "error");

            ViewResult result = controller.Type(viewModel) as ViewResult;
            OrganisationTypeViewModel resultViewModel = result.Model as OrganisationTypeViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Type");
            Assert.Equal(viewModel.SearchedText, resultViewModel.SearchedText);
            Assert.Equal(viewModel.PossibleValues, resultViewModel.PossibleValues);
        }

        [Theory]
        [InlineData("Sole trader or individual", "SoleTraderOrPartnershipDetails")]
        [InlineData("Partnership", "SoleTraderOrPartnershipDetails")]
        [InlineData("Registered company", "RegisteredCompanyDetails")]
        public void TypePost_ValidViewModel_ReturnsCorrectRedirect(string selectedValue, string action)
        {
            OrganisationTypeViewModel viewModel = new OrganisationTypeViewModel()
            {
                SearchedText = "Company",
                SelectedValue = selectedValue
            };

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);
            RedirectToRouteResult result = controller.Type(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be("AddAatf");
            result.RouteValues["organisationType"].Should().Be(viewModel.SelectedValue);
            result.RouteValues["searchedText"].Should().Be(viewModel.SearchedText);
        }

        [Fact]
        public async Task SoleTraderOrPartnershipDetailsGet_ReturnsViewWithViewModelPopulated()
        {
            string searchText = "Company";
            string organisationType = "Sole trader or individual";

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            ViewResult result = await controller.SoleTraderOrPartnershipDetails(organisationType, searchText) as ViewResult;

            SoleTraderOrPartnershipDetailsViewModel resultViewModel = result.Model as SoleTraderOrPartnershipDetailsViewModel;

            Assert.Equal(searchText, resultViewModel.BusinessTradingName);
            Assert.Equal(organisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
        }

        [Fact]
        public async Task SoleTraderOrPartnershipDetailsPost_ModelNotValid_ReturnsView()
        {
            SoleTraderOrPartnershipDetailsViewModel viewModel = new SoleTraderOrPartnershipDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual"
            };
            viewModel.Address.Countries = countries;

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);
            controller.ModelState.AddModelError("error", "error");

            ViewResult result = await controller.SoleTraderOrPartnershipDetails(viewModel) as ViewResult;
            SoleTraderOrPartnershipDetailsViewModel resultViewModel = result.Model as SoleTraderOrPartnershipDetailsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
            Assert.Equal(viewModel.BusinessTradingName, resultViewModel.BusinessTradingName);
            Assert.Equal(viewModel.OrganisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);
        }

        [Fact]
        public async Task SoleTraderOrPartnershipDetailsPost_ValidViewModel_ReturnsCorrectRedirect()
        {
            SoleTraderOrPartnershipDetailsViewModel viewModel = new SoleTraderOrPartnershipDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual"
            };

            viewModel.Address.Countries = countries;

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);
            RedirectToRouteResult result = await controller.SoleTraderOrPartnershipDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrganisationConfirmation");
            result.RouteValues["controller"].Should().Be("AddAatf");

            result.RouteValues["organisationId"].Should().NotBe(null);
            result.RouteValues["organisationName"].Should().Be(viewModel.BusinessTradingName);
        }

        [Fact]
        public async Task SoleTraderOrPartnershipDetailsPost_ValidViewModel_RequestWithCorrectParametersCreated()
        {
            SoleTraderOrPartnershipDetailsViewModel viewModel = new SoleTraderOrPartnershipDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Sole trader or individual",
                Address = A.Dummy<AddressData>()
            };

            viewModel.Address.Countries = countries;

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            await controller.SoleTraderOrPartnershipDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateOrganisationAdmin>.That.Matches(
                p => p.BusinessName == viewModel.BusinessTradingName
                && p.Address == viewModel.Address
                && p.OrganisationType == viewModel.OrganisationType.GetValueFromDisplayName<OrganisationType>()))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task RegisteredCompanyDetailsGet_ReturnsViewWithViewModelPopulated()
        {
            string searchText = "Company";
            string organisationType = "Registered company";

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);

            ViewResult result = await controller.RegisteredCompanyDetails(organisationType, searchText) as ViewResult;

            RegisteredCompanyDetailsViewModel resultViewModel = result.Model as RegisteredCompanyDetailsViewModel;

            Assert.Equal(searchText, resultViewModel.CompanyName);
            Assert.Equal(organisationType, resultViewModel.OrganisationType);
            Assert.Equal(countries, resultViewModel.Address.Countries);

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "RegisteredCompanyDetails");
        }

        [Fact]
        public async Task RegisteredCompanyDetailsPost_ModelNotValid_ReturnsView()
        {
            RegisteredCompanyDetailsViewModel viewModel = new RegisteredCompanyDetailsViewModel()
            {
                BusinessTradingName = "Company",
                OrganisationType = "Registered company",
                CompaniesRegistrationNumber = "1234567",
                CompanyName = "Name"
            };
            viewModel.Address.Countries = countries;

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);
            controller.ModelState.AddModelError("error", "error");

            ViewResult result = await controller.RegisteredCompanyDetails(viewModel) as ViewResult;
            RegisteredCompanyDetailsViewModel resultViewModel = result.Model as RegisteredCompanyDetailsViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
            Assert.Equal(viewModel.BusinessTradingName, resultViewModel.BusinessTradingName);
            Assert.Equal(viewModel.OrganisationType, resultViewModel.OrganisationType);
            Assert.Equal(viewModel.CompanyName, resultViewModel.CompanyName);
            Assert.Equal(viewModel.CompaniesRegistrationNumber, resultViewModel.CompaniesRegistrationNumber);
            Assert.Equal(countries, resultViewModel.Address.Countries);
        }

        [Fact]
        public async Task RegisteredCompanyDetailsPost_ValidViewModel_ReturnsCorrectRedirect()
        {
            RegisteredCompanyDetailsViewModel viewModel = new RegisteredCompanyDetailsViewModel()
            {
                BusinessTradingName = "name",
                OrganisationType = "Registered company",
                CompaniesRegistrationNumber = "1234567",
                CompanyName = "Name"
            };

            viewModel.Address.Countries = countries;

            AddAatfController controller = new AddAatfController(organisationSearcher, () => weeeClient);
            RedirectToRouteResult result = await controller.RegisteredCompanyDetails(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrganisationConfirmation");
            result.RouteValues["controller"].Should().Be("AddAatf");

            result.RouteValues["organisationId"].Should().NotBe(null);
            result.RouteValues["organisationName"].Should().Be(viewModel.CompanyName);
        }

        private AddAatfViewModel CreateAddViewModel()
        {
            IList<UKCompetentAuthorityData> competentAuthoritiesList = A.Dummy<IList<UKCompetentAuthorityData>>();
            IEnumerable<AatfSize> sizeList = Enumeration.GetAll<AatfSize>();
            IEnumerable<AatfStatus> statusList = Enumeration.GetAll<AatfStatus>();

            AddAatfViewModel viewModel = new AddAatfViewModel()
            {
                SiteAddressData = new AatfAddressData()
                {
                    Countries = countries
                },
                ContactData = new AatfContactData()
                {
                    AddressData = new AatfContactAddressData()
                    {
                        Countries = countries
                    }
                },
                SizeList = sizeList,
                StatusList = statusList,
                OrganisationId = Guid.NewGuid()
            };

            return viewModel;
        }
    }
}
