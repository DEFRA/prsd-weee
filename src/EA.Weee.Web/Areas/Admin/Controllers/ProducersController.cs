﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Home;

    public class ProducersController : AdminController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly ISearcher<ProducerSearchResult> producerSearcher;
        private readonly ISearcher<SmallProducerSearchResult> smallProducerSearcher;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly int maximumSearchResults;

        public ProducersController(BreadcrumbService breadcrumb,
            ISearcher<ProducerSearchResult> producerSearcher,
            Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            ConfigurationService configurationService, ISearcher<SmallProducerSearchResult> smallProducerSearcher)
        {
            this.breadcrumb = breadcrumb;
            this.producerSearcher = producerSearcher;
            this.apiClient = apiClient;
            this.cache = cache;
            this.smallProducerSearcher = smallProducerSearcher;

            maximumSearchResults = configurationService.CurrentConfiguration.MaximumProducerOrganisationSearchResults;
        }

        /// <summary>
        /// This method is used by both JS and non-JS users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Search(SearchTypeEnum searchType)
        {
            await SetBreadcrumb(searchType);
            return View(new SearchViewModel() { SearchType = searchType });
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
            await SetBreadcrumb(viewModel.SearchType);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Check to see if a registration number was selected.
            if (!string.IsNullOrEmpty(viewModel.SelectedRegistrationNumber))
            {
                return RedirectBySearchType(viewModel.SearchType, viewModel.SelectedRegistrationNumber);
            }

            return RedirectToAction("SearchResults", new { viewModel.SearchTerm, searchType = viewModel.SearchType });
        }

        /// <summary>
        /// This method is used by users who are not using the auto-complete.
        /// It loads the search results page for a specified search term.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm, SearchTypeEnum searchType)
        {
            await SetBreadcrumb(searchType);

            SearchResultsViewModel viewModel = new SearchResultsViewModel
            {
                SearchTerm = searchTerm,
                SearchType = searchType
            };

            var resultsList = await GetSearchResults(searchTerm, searchType);

            viewModel.Results = resultsList;

            return View(viewModel);
        }

        private async Task<List<RegisteredProducerSearchResult>> GetSearchResults(string searchTerm, SearchTypeEnum searchType)
        {
            var resultsList = new List<RegisteredProducerSearchResult>();

            if (searchType == SearchTypeEnum.SmallProducer)
            {
                var smallProducerResults = await smallProducerSearcher.Search(searchTerm, maximumSearchResults, false);
                var res = smallProducerResults.ToList().ConvertAll<RegisteredProducerSearchResult>(x => x);

                resultsList.AddRange(res);
            }
            else
            {
                var producerResults = await producerSearcher.Search(searchTerm, maximumSearchResults, false);
                var res = producerResults.ToList().ConvertAll<RegisteredProducerSearchResult>(x => x);

                resultsList.AddRange(res);
            }

            return resultsList;
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
            await SetBreadcrumb(viewModel.SearchType);

            if (!ModelState.IsValid)
            {
                var resultsList = await GetSearchResults(viewModel.SearchTerm, viewModel.SearchType);

                viewModel.Results = resultsList;

                return View(viewModel);
            }

            return RedirectBySearchType(viewModel.SearchType, viewModel.SelectedRegistrationNumber);
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> FetchSearchResultsJson(string searchTerm, SearchTypeEnum searchType)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var resultsList = await GetSearchResults(searchTerm, searchType);

            return Json(resultsList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used by both JS and non-JS users.
        /// </summary>
        /// <param name="registrationNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Details(string registrationNumber)
        {
            using (IWeeeClient client = apiClient())
            {
                await SetBreadcrumb();

                var allYears = await client.SendAsync(User.GetAccessToken(), new GetProducerComplianceYear { RegistrationNumber = registrationNumber });
                var latestYear = allYears.First();

                DetailsViewModel viewModel = new DetailsViewModel();
                viewModel.RegistrationNumber = registrationNumber;
                viewModel.ComplianceYears = new SelectList(allYears);
                viewModel.SelectedYear = latestYear;

                return View(viewModel);
            }
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="registrationNumber"></param>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FetchDetails(string registrationNumber, int complianceYear)
        {
            if (Request != null &&
                !Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using (IWeeeClient client = apiClient())
            {
                await SetBreadcrumb();

                GetProducerDetails request = new GetProducerDetails()
                {
                    RegistrationNumber = registrationNumber,
                    ComplianceYear = complianceYear
                };

                ProducerDetails producerDetails = await client.SendAsync(User.GetAccessToken(), request);

                return PartialView("_detailsResults", producerDetails);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadProducerAmendmentsCsv(string registrationNumber)
        {
            using (IWeeeClient client = apiClient())
            {
                var producerAmendmentsCsvData = await client.SendAsync(User.GetAccessToken(),
                    new GetProducerAmendmentsHistoryCSV(registrationNumber));

                byte[] data = new UTF8Encoding().GetBytes(producerAmendmentsCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(producerAmendmentsCsvData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmRemoval(Guid registeredProducerId)
        {
            await SetBreadcrumb();

            ProducerDetailsScheme producerDetailsScheme = await FetchProducerDetailsScheme(registeredProducerId);

            if (!producerDetailsScheme.CanRemoveProducer)
            {
                return new HttpForbiddenResult();
            }

            return View(new ConfirmRemovalViewModel
            {
                Producer = producerDetailsScheme
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmRemoval(Guid registeredProducerId, ConfirmRemovalViewModel model)
        {
            await SetBreadcrumb();

            ProducerDetailsScheme producerDetailsScheme = await FetchProducerDetailsScheme(registeredProducerId);

            if (!ModelState.IsValid)
            {
                return View(new ConfirmRemovalViewModel
                {
                    Producer = producerDetailsScheme
                });
            }

            if (model.SelectedValue == "Yes")
            {
                RemoveProducerResult result;
                using (IWeeeClient client = apiClient())
                {
                    result = await client.SendAsync(User.GetAccessToken(), new RemoveProducer(registeredProducerId));
                }

                if (result.InvalidateProducerSearchCache)
                {
                    await cache.InvalidateProducerSearch();
                }

                return RedirectToAction("Removed",
                    new
                    {
                        producerDetailsScheme.RegistrationNumber,
                        producerDetailsScheme.ComplianceYear,
                        producerDetailsScheme.SchemeName
                    });
            }
            else
            {
                return RedirectToAction("Details",
                    new
                    {
                        producerDetailsScheme.RegistrationNumber
                    });
            }
        }

        private async Task<ProducerDetailsScheme> FetchProducerDetailsScheme(Guid registeredProducerId)
        {
            using (IWeeeClient client = apiClient())
            {
                GetProducerDetailsByRegisteredProducerId request = new GetProducerDetailsByRegisteredProducerId(registeredProducerId);
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Removed(string registrationNumber, int complianceYear, string schemeName)
        {
            await SetBreadcrumb();
            return View(new RemovedViewModel
            {
                RegistrationNumber = registrationNumber,
                ComplianceYear = complianceYear,
                SchemeName = schemeName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Removed(RemovedViewModel model)
        {
            using (IWeeeClient client = apiClient())
            {
                var isAssociatedWithAnotherScheme = await client.SendAsync(User.GetAccessToken(),
                    new IsProducerRegisteredForComplianceYear(model.RegistrationNumber, model.ComplianceYear));

                if (isAssociatedWithAnotherScheme)
                {
                    return RedirectToAction("Details", new { model.RegistrationNumber });
                }

                return RedirectToAction("Search", new { searchType = SearchTypeEnum.Producer });
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadProducerEeeDataHistoryCsv(string registrationNumber)
        {
            using (IWeeeClient client = apiClient())
            {
                var producerEeeDataHistoryCsv = await client.SendAsync(User.GetAccessToken(),
                    new GetProducerEeeDataHistoryCsv(registrationNumber));

                byte[] data = new UTF8Encoding().GetBytes(producerEeeDataHistoryCsv.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(producerEeeDataHistoryCsv.FileName));
            }
        }

        private async Task SetBreadcrumb(SearchTypeEnum? searchType = null)
        {
            breadcrumb.InternalActivity = searchType == SearchTypeEnum.SmallProducer ? InternalUserActivity.DirectRegistrantDetails : InternalUserActivity.ProducerDetails;

            await Task.Yield();
        }

        private ActionResult RedirectBySearchType(SearchTypeEnum searchType, string registrationNumber)
        {
            if (searchType == SearchTypeEnum.SmallProducer)
            {
                return RedirectToAction(nameof(ProducerSubmissionController.Submissions), typeof(ProducerSubmissionController).GetControllerName(), new
                {
                    RegistrationNumber = registrationNumber
                });
            }

            return RedirectToAction(nameof(Details), typeof(ProducersController).GetControllerName(), new
            {
                RegistrationNumber = registrationNumber
            });
        }
    }
}