﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Infrastructure;
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
        private readonly IProducerSearcher producerSearcher;
        private readonly Func<IWeeeClient> apiClient;
        private const int maximumSearchResults = 10;

        public ProducersController(BreadcrumbService breadcrumb, IProducerSearcher producerSearcher, Func<IWeeeClient> apiClient)
        {
            this.breadcrumb = breadcrumb;
            this.producerSearcher = producerSearcher;
            this.apiClient = apiClient;
        }

        /// <summary>
        /// This method is used by both JS and non-JS users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Search()
        {
            await SetBreadcrumb();
            return View();
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
                return View(viewModel);
            }

            // Check to see if a registration number was selected.
            if (!string.IsNullOrEmpty(viewModel.SelectedRegistrationNumber))
            {
                return RedirectToAction("Details", new
                {
                    RegistrationNumber = viewModel.SelectedRegistrationNumber
                });
            }

            return RedirectToAction("SearchResults", new { viewModel.SearchTerm });
        }

        /// <summary>
        /// This method is used by users who are not using the auto-complete.
        /// It loads the search results page for a specified search term.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm)
        {
            await SetBreadcrumb();

            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = searchTerm;
            viewModel.Results = await producerSearcher.Search(searchTerm, maximumSearchResults);

            return View(viewModel);
        }

        /// <summary>
        /// This method is used to select an item from the search results page.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchResults(SearchResultsViewModel viewModel)
        {
            await SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                viewModel.Results = await producerSearcher.Search(viewModel.SearchTerm, maximumSearchResults);

                return View(viewModel);
            }

            return RedirectToAction("Details", new
            {
                RegistrationNumber = viewModel.SelectedRegistrationNumber
            });
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> FetchSearchResultsJson(string searchTerm)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            IList<ProducerSearchResult> searchResults = await producerSearcher.Search(searchTerm, maximumSearchResults);

            return Json(searchResults, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string registrationNumber)
        {
            await SetBreadcrumb();

            ProducerDetails producerDetails;
            using (IWeeeClient client = apiClient())
            {
                GetProducerDetails request = new GetProducerDetails()
                {
                    RegistrationNumber = registrationNumber
                };

                producerDetails = await client.SendAsync(User.GetAccessToken(), request);
            }

            DetailsViewModel viewModel = new DetailsViewModel();
            viewModel.Details = producerDetails;

            return View(viewModel);
        }

        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "View producer information";

            await Task.Yield();
        }
    }
}