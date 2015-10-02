namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    public class ProducersController : AdminController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ProducersController(BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        /// <summary>
        /// This method is used by both JS and non-JS users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Search()
        {
            await SetBreadcrumb();
            return View("Search", new SearchViewModel());
        }

        /// <summary>
        /// This method is used by non-JS users to retrieve search results.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(SearchViewModel viewModel)
        {
            await SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                return View("Search", viewModel);
            }

            // Check to see if a registration number and compliance year were selected.
            if (!string.IsNullOrEmpty(viewModel.SelectedRegistrationNumber) && viewModel.SelectedComplianceYear != null)
            {
                // Make sure that the search term wasn't changed since a value was selected.
                if (viewModel.SearchTerm.Contains(viewModel.SelectedRegistrationNumber))
                {
                    return RedirectToAction("Details", new
                    {
                        RegistrationNumber = viewModel.SelectedRegistrationNumber,
                        ComplianceYear = viewModel.SelectedComplianceYear
                    });
                }
            }

            SearchResultsViewModel resultsViewModel = new SearchResultsViewModel();

            resultsViewModel.Results = await FetchSearchResults(viewModel.SearchTerm);

            return View("SearchResults", resultsViewModel);
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> FetchSearchResults(SearchViewModel viewModel)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null);
            }

            IList<ProducerSearchResult> searchResults = await FetchSearchResults(viewModel.SearchTerm);

            return Json(searchResults);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string registrationNumber, int complianceYear)
        {
            await SetBreadcrumb();

            // TODO: Data access

            return View((object)registrationNumber);
        }

        private async Task<IList<ProducerSearchResult>> FetchSearchResults(string searchTerm)
        {
            var list = await cache.FetchProducerSearchResultList();
            
            return list
                .Where(i => i.RegistrationNumber.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) || i.Name.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()))
                .OrderBy(i => i.RegistrationNumber)
                .Take(10)
                .ToList();
        }

        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "View producer information";

            await Task.Yield();
        }
    }
}