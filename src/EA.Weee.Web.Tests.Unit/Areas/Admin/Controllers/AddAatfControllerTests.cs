namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Core.Search;
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

        public AddAatfControllerTests()
        {
            this.organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
        }

        [Fact]
        public void GetSearch_ReturnsView()
        {
            AddAatfController controller = new AddAatfController(organisationSearcher);

            ViewResult result = controller.Search() as ViewResult;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Search");
        }

        [Fact]
        public void PostSearch_InvalidViewModel_ReturnsView()
        {
            AddAatfController controller = new AddAatfController(organisationSearcher);

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
            AddAatfController controller = new AddAatfController(organisationSearcher);

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

            AddAatfController controller = new AddAatfController(organisationSearcher);

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

            AddAatfController controller = new AddAatfController(organisationSearcher);

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
        public async Task PostSearch_ValidViewModelSelectedOrganisation_RedirectsToAdminHolding()
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

            AddAatfController controller = new AddAatfController(organisationSearcher);

            SearchResultsViewModel viewModel = new SearchResultsViewModel()
            {
                Results = results,
                SearchTerm = searchTerm,
                SelectedOrganisationId = organisationId
            };

            RedirectToRouteResult result = await controller.SearchResults(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AdminHolding");
        }
    }
}
