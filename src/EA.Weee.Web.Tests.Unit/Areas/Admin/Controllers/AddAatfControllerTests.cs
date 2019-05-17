namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class AddAatfControllerTests
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly IWeeeClient weeeClient;

        public AddAatfControllerTests()
        {
            this.organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            this.weeeClient = A.Fake<IWeeeClient>();
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

            ViewResult result = await controller.SearchResults(searchTerm) as ViewResult;
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

        private AddAatfViewModel CreateAddViewModel()
        {
            IList<UKCompetentAuthorityData> competentAuthoritiesList = A.Dummy<IList<UKCompetentAuthorityData>>();
            IList<CountryData> countries = A.Dummy<IList<CountryData>>();
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
