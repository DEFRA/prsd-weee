namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Core.Search;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class AddAatfController : AdminController
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private const int maximumSearchResults = 5;

        public AddAatfController(ISearcher<OrganisationSearchResult> organisationSearcher)
        {
            this.organisationSearcher = organisationSearcher;
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
    }
}