namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Core.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class ProducersController : AdminController
    {
        /// <summary>
        /// This method is used by both JS and non-JS users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search()
        {
            return View("Search", new SearchViewModel());
        }

        /// <summary>
        /// This method is used by non-JS users to retrieve search results.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel viewModel)
        {
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

            resultsViewModel.Results = FetchSearchResults(viewModel.SearchTerm);

            return View("SearchResults", resultsViewModel);
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FetchSearchResults(SearchViewModel viewModel)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null);
            }

            IList<ProducerSearchResult> searchResults = FetchSearchResults(viewModel.SearchTerm);

            return Json(searchResults);
        }

        [HttpGet]
        public ActionResult Details(string registrationNumber, int complianceYear)
        {
            // TODO: Data access

            return View((object)registrationNumber);
        }

        private IList<ProducerSearchResult> FetchSearchResults(string searchTerm)
        {
            // TODO: Data access

            return new List<ProducerSearchResult>()
            {
                new ProducerSearchResult() { RegistrationNumber = "WEE/AA1234AA", Name = "Graham's waste disposal company", ComplianceYear = 2011 },
                new ProducerSearchResult() { RegistrationNumber = "WEE/AE4776DJ", Name = "Waste not want not", ComplianceYear = 2012 },
                new ProducerSearchResult() { RegistrationNumber = "WEE/ED1254EK", Name = "Waste of time", ComplianceYear = 2013 },
                new ProducerSearchResult() { RegistrationNumber = "WEE/HB4434FD", Name = "More waste less speed", ComplianceYear = 2014 },
                new ProducerSearchResult() { RegistrationNumber = "WEE/NJ4234RE", Name = "Cut, copy and waste", ComplianceYear = 2015 },
                new ProducerSearchResult() { RegistrationNumber = "WEE/DE1244HW", Name = "Wasted on a Sunday morning", ComplianceYear = 2016 },
                new ProducerSearchResult() { RegistrationNumber = "WEE/EE1254CQ", Name = "Fancy waistcoat company", ComplianceYear = 2017 },
                new ProducerSearchResult() { RegistrationNumber = "WEE/GJ5495AQ", Name = "This producer has a really long name that will need to be handled nicely on narrow browsers", ComplianceYear = 2018 },
            }
            .Where(i => i.RegistrationNumber.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) || i.Name.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()))
            .OrderBy(i => i.RegistrationNumber)
            .Take(10)
            .ToList();
        }
    }
}