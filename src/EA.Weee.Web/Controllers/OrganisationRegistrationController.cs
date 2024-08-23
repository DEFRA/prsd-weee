namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Services;
    using ViewModels.OrganisationRegistration;
    using Weee.Requests.Organisations;

    [Authorize]
    public class OrganisationRegistrationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly int maximumSearchResults;
        public OrganisationRegistrationController(Func<IWeeeClient> apiClient,
            ISearcher<OrganisationSearchResult> organisationSearcher,
            ConfigurationService configurationService)
        {
            this.apiClient = apiClient;
            this.organisationSearcher = organisationSearcher;

            maximumSearchResults = configurationService.CurrentConfiguration.MaximumOrganisationSearchResults;
        }

        [HttpGet]
        public async Task<ActionResult> Search()
        {
            IEnumerable<OrganisationUserData> organisations = await GetOrganisations();
            SearchViewModel model = new SearchViewModel
            {
                ShowPerformAnotherActivityLink = organisations.Any()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(SearchViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<OrganisationUserData> organisations = await GetOrganisations();
                viewModel.ShowPerformAnotherActivityLink = organisations.Any();

                return View(viewModel);
            }

            // Check to see if an organisation was selected.
            if (viewModel.SelectedOrganisationId != null)
            {
                return RedirectToAction("JoinOrganisation", new
                {
                    OrganisationId = viewModel.SelectedOrganisationId.Value
                });
            }

            return RedirectToAction("SearchResults", new { viewModel.SearchTerm });
        }

        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm)
        {
            SearchResultsViewModel viewModel = new SearchResultsViewModel
            {
                SearchTerm = searchTerm,
                Results = await organisationSearcher.Search(searchTerm, maximumSearchResults, false)
            };

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

            return RedirectToAction("JoinOrganisation", new
            {
                OrganisationId = viewModel.SelectedOrganisationId.Value
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

            IList<OrganisationSearchResult> searchResults = await organisationSearcher.Search(searchTerm, maximumSearchResults, true);

            return Json(searchResults, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public async Task<ViewResult> JoinOrganisation(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(
                    User.GetAccessToken(),
                    new GetPublicOrganisationInfo(organisationId));

                var existingAssociations = await client.SendAsync(
                     User.GetAccessToken(),
                     new GetUserOrganisationsByStatus(
                         new int[0],
                         new int[1] { (int)OrganisationStatus.Complete }));

                /* There should only ever be a single non-rejected association, but
                 * during development this wasn't enforced. Using FirstOrDefault
                 * instead of SingleOrDefault will avoid any issues being raised
                 * due to bad data.
                 */
                OrganisationUserData existingAssociation = existingAssociations
                    .Where(o => o.OrganisationId == organisationId)
                    .Where(o => o.UserStatus != UserStatus.Rejected)
                    .FirstOrDefault();

                var activeUsers = await client.SendAsync(User.GetAccessToken(), new GetActiveOrganisationUsers(organisationId));

                if (existingAssociation != null)
                {
                    UserAlreadyAssociatedWithOrganisationViewModel viewModel = new UserAlreadyAssociatedWithOrganisationViewModel()
                    {
                        OrganisationId = organisationId,
                        OrganisationName = organisationData.DisplayName,
                        Status = existingAssociation.UserStatus,
                        AnyActiveUsers = activeUsers.Any()
                    };

                    return View("UserAlreadyAssociatedWithOrganisation", viewModel);
                }
                else
                {
                    var model = new JoinOrganisationViewModel
                    {
                        OrganisationId = organisationId,
                        OrganisationName = organisationData.DisplayName,
                        AnyActiveUsers = activeUsers.Any()
                    };

                    return View(model);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> JoinOrganisation(JoinOrganisationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.SelectedValue == "No")
            {
                return RedirectToAction("Search", "OrganisationRegistration");
            }

            using (var client = apiClient())
            {
                try
                {
                    await client.SendAsync(User.GetAccessToken(), new JoinOrganisation(viewModel.OrganisationId));
                }
                catch (ApiException ex)
                {
                    if (ex.ErrorData != null)
                    {
                        ModelState.AddModelError(string.Empty, ex.ErrorData.ExceptionMessage);
                        return View(viewModel);
                    }
                    throw;
                }

                return RedirectToAction("JoinOrganisationConfirmation", new { organisationId = viewModel.OrganisationId, activeUsers = viewModel.AnyActiveUsers });
            }
        }

        [HttpGet]
        public async Task<ViewResult> JoinOrganisationConfirmation(Guid organisationId, bool activeUsers)
        {
            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(
                    User.GetAccessToken(),
                    new GetPublicOrganisationInfo(organisationId));

                var model = new JoinOrganisationConfirmationViewModel()
                {
                    OrganisationName = organisationData.DisplayName,
                    AnyActiveUsers = activeUsers
                };

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Type()
        {
            return View(new OrganisationTypeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Type(OrganisationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organisationType = model.SelectedValue.GetValueFromDisplayName<ExternalOrganisationType>();
                var routeValues = new { organisationType = model.SelectedValue };

                switch (organisationType)
                {
                    case ExternalOrganisationType.SoleTrader:
                        return RedirectToAction(nameof(SoleTraderDetails), "OrganisationRegistration", routeValues);
                    case ExternalOrganisationType.Partnership:
                        return RedirectToAction(nameof(PartnershipDetails), "OrganisationRegistration", routeValues);
                    case ExternalOrganisationType.RegisteredCompany:
                        return RedirectToAction(nameof(RegisteredCompanyDetails), "OrganisationRegistration", routeValues);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredCompanyDetails(
            ViewModels.OrganisationRegistration.Details.RegisteredCompanyDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails(string organisationType, string searchedText = null)
        {
            var countries = await GetCountries();

            var model = new ViewModels.OrganisationRegistration.Details.RegisteredCompanyDetailsViewModel
            {
                CompanyName = searchedText,
                OrganisationType = organisationType,
                Address = { Countries = countries }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> PartnershipDetails(string organisationType, string searchedText = null)
        {
            var countries = await GetCountries();

            var model = new PartnershipDetailsViewModel
            {
                BusinessTradingName = searchedText,
                OrganisationType = organisationType,
                Address = { Countries = countries }
            };

            return View(model);
        }

        private async Task<IList<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderDetails(string organisationType, string searchedText = null)
        {
            var countries = await GetCountries();

            var model = new SoleTraderDetailsViewModel
            {
                CompanyName = searchedText,
                OrganisationType = organisationType,
                Address = { Countries = countries }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> TonnageType(string searchTerm)
        {
            var viewModel = new TonnageTypeViewModel
            {
                SearchedText = searchTerm,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TonnageType(TonnageTypeViewModel tonnageTypeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(tonnageTypeViewModel);
            }

            return RedirectToAction("Index", "Holding");
        }

        private async Task<IEnumerable<OrganisationUserData>> GetOrganisations()
        {
            List<OrganisationUserData> organisations;

            using (var client = apiClient())
            {
                organisations = await
                 client.SendAsync(
                     User.GetAccessToken(),
                     new GetUserOrganisationsByStatus(new int[0], new int[1] { (int)OrganisationStatus.Complete }));
            }
            return organisations;
        }
    }
}
