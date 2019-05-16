namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class AddAatfController : AdminController
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly Func<IWeeeClient> apiClient;
        private const int maximumSearchResults = 5;

        public AddAatfController(ISearcher<OrganisationSearchResult> organisationSearcher, Func<IWeeeClient> apiClient)
        {
            this.organisationSearcher = organisationSearcher;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("SearchResults", "AddAatf", new { viewModel.SearchTerm });
        }

        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm)
        {
            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = searchTerm;
            viewModel.Results = await organisationSearcher.Search(searchTerm, maximumSearchResults, false);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchResults(SearchResultsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Results = await organisationSearcher.Search(viewModel.SearchTerm, maximumSearchResults, false);

                return View(viewModel);
            }

            return RedirectToAction("Index", "AdminHolding");
        }

        [HttpGet]
        public async Task<ActionResult> Add()
        {
            AddAatfViewModel viewModel = new AddAatfViewModel();

            viewModel = await PopulateViewModelLists(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddAatfViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await PopulateViewModelLists(viewModel);

                return View(viewModel);
            }

            return RedirectToAction("Index", "AdminHolding");
        }

        private async Task<AddAatfViewModel> PopulateViewModelLists(AddAatfViewModel viewModel)
        {
            using (var client = apiClient())
            {
                IList<CountryData> countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                viewModel.ContactData.AddressData.Countries = countries;
                viewModel.SiteAddressData.Countries = countries;
                viewModel.CompetentAuthoritiesList = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
                viewModel.SizeList = Enumeration.GetAll<AatfSize>();
                viewModel.StatusList = Enumeration.GetAll<AatfStatus>();
            }

            return viewModel;
        }
    }
}