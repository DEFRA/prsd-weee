namespace EA.Weee.Web.Controllers
{
    using Api.Client;
    using Base;
    using Core.Helpers;
    using Core.Organisations;
    using Core.Search;
    using Core.Shared;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.Type;
    using Weee.Requests.Organisations;

    [Authorize]
    public class OrganisationRegistrationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly int maximumSearchResults;
        private readonly IOrganisationTransactionService transactionService;
        private readonly IWeeeCache cache;

        public OrganisationRegistrationController(Func<IWeeeClient> apiClient,
            ISearcher<OrganisationSearchResult> organisationSearcher,
            ConfigurationService configurationService, IOrganisationTransactionService transactionService, IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.organisationSearcher = organisationSearcher;
            this.transactionService = transactionService;
            this.cache = cache;

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
        public async Task<ActionResult> RegisterSmallProducer(string searchTerm)
        {
            // temporary clear down any existing Organisation transactions for the user before we know about
            // continuing a partially completed registration
            await transactionService.DeleteOrganisationTransactionData(User.GetAccessToken());

            return RedirectToAction(nameof(TonnageType), new { searchTerm });
        }

        [HttpGet]
        public async Task<ActionResult> Type()
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            var selectedValue = string.Empty;
            if (existingTransaction?.OrganisationType != null)
            {
                selectedValue = existingTransaction.OrganisationType.GetDisplayName();
            }

            return View(new OrganisationTypeViewModel() { SelectedValue = selectedValue });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Type(OrganisationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organisationType = model.SelectedValue.GetValueFromDisplayName<ExternalOrganisationType>();
                var routeValues = new { organisationType = model.SelectedValue };

                await transactionService.CaptureData(User.GetAccessToken(), model);

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
        public async Task<ActionResult> RepresentingCompanyDetails(RepresentingCompanyDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;

                return View(model);
            }

            return await CheckRepresentingCompanyDetailsAndRedirect(model);
        }

        private async Task<ActionResult> CheckRepresentingCompanyDetailsAndRedirect(RepresentingCompanyDetailsViewModel model)
        {
            await transactionService.CaptureData(User.GetAccessToken(), model);

            await transactionService.CompleteTransaction(User.GetAccessToken());
            await cache.InvalidateOrganisationSearch();

            return RedirectToAction(nameof(RegistrationComplete), typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public async Task<ActionResult> RepresentingCompanyDetails()
        {
            RepresentingCompanyDetailsViewModel model = null;

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            model = existingTransaction?.RepresentingCompanyDetailsViewModel ?? new RepresentingCompanyDetailsViewModel();

            var countries = await GetCountries();
            model.Address.Countries = countries;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredCompanyDetails(RegisteredCompanyDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;

                ModelState.ApplyCustomValidationSummaryOrdering(OrganisationViewModel.ValidationMessageDisplayOrder);

                return View(model);
            }

            await transactionService.CaptureData(User.GetAccessToken(), model);

            return await CheckAuthorisedRepresentitiveAndRedirect();
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails()
        {
            RegisteredCompanyDetailsViewModel model = null;

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            if (existingTransaction?.RegisteredCompanyDetailsViewModel != null)
            {
                model = existingTransaction.RegisteredCompanyDetailsViewModel;
            }
            else
            {
                model = new RegisteredCompanyDetailsViewModel
                {
                    CompanyName = existingTransaction?.SearchTerm
                };
            }

            var countries = await GetCountries();
            model.Address.Countries = countries;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> PartnershipDetails()
        {
            PartnershipDetailsViewModel model = null;

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            if (existingTransaction?.PartnershipDetailsViewModel != null)
            {
                model = existingTransaction.PartnershipDetailsViewModel;
            }
            else
            {
                model = new PartnershipDetailsViewModel
                {
                    CompanyName = existingTransaction?.SearchTerm
                };
            }

            var countries = await GetCountries();
            model.Address.Countries = countries;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> RepresentingCompanyRedirect()
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            if (existingTransaction?.OrganisationType == null)
            {
                return RedirectToAction(nameof(Type), typeof(OrganisationRegistrationController).GetControllerName());
            }

            switch (existingTransaction.OrganisationType)
            {
                case ExternalOrganisationType.RegisteredCompany:
                    return RedirectToAction(nameof(RegisteredCompanyDetails), typeof(OrganisationRegistrationController).GetControllerName());
                case ExternalOrganisationType.Partnership:
                    return RedirectToAction(nameof(PartnershipDetails), typeof(OrganisationRegistrationController).GetControllerName());
                case ExternalOrganisationType.SoleTrader:
                    return RedirectToAction(nameof(SoleTraderDetails), typeof(OrganisationRegistrationController).GetControllerName());
                default:
                    return RedirectToAction(nameof(Type), typeof(OrganisationRegistrationController).GetControllerName());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PartnershipDetails(PartnershipDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;

                ModelState.ApplyCustomValidationSummaryOrdering(OrganisationViewModel.ValidationMessageDisplayOrder);

                return View(model);
            }

            await transactionService.CaptureData(User.GetAccessToken(), model);

            return await CheckAuthorisedRepresentitiveAndRedirect();
        }

        private async Task<ActionResult> CheckAuthorisedRepresentitiveAndRedirect()
        {
            var organisationTransactionData = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());
            if (organisationTransactionData != null &&
                organisationTransactionData.AuthorisedRepresentative == YesNoType.No)
            {
                await transactionService.CompleteTransaction(User.GetAccessToken());
                await cache.InvalidateOrganisationSearch();

                return RedirectToAction(nameof(RegistrationComplete), typeof(OrganisationRegistrationController).GetControllerName());
            }

            return RedirectToAction(nameof(RepresentingCompanyDetails), typeof(OrganisationRegistrationController).GetControllerName());
        }

        private async Task<IList<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderDetails()
        {
            SoleTraderDetailsViewModel model = null;

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            if (existingTransaction?.SoleTraderDetailsViewModel != null)
            {
                model = existingTransaction.SoleTraderDetailsViewModel;
            }
            else
            {
                model = new SoleTraderDetailsViewModel
                {
                    CompanyName = existingTransaction?.SearchTerm,
                };
            }

            var countries = await GetCountries();
            model.Address.Countries = countries;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SoleTraderDetails(SoleTraderDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;

                ModelState.ApplyCustomValidationSummaryOrdering(OrganisationViewModel.ValidationMessageDisplayOrder);

                return View(model);
            }

            await transactionService.CaptureData(User.GetAccessToken(), model);

            return await CheckAuthorisedRepresentitiveAndRedirect();
        }

        [HttpGet]
        public async Task<ViewResult> TonnageType(string searchTerm)
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            var selectedValue = string.Empty;
            var selectedSearch = searchTerm;
            if (existingTransaction != null)
            {
                if (existingTransaction.TonnageType.HasValue)
                {
                    selectedValue = existingTransaction.TonnageType.GetDisplayName();
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    selectedSearch = existingTransaction.SearchTerm;
                }
            }
            
            var viewModel = new TonnageTypeViewModel
            {
                SearchedText = selectedSearch,
                SelectedValue = selectedValue
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

            var tonnageType = tonnageTypeViewModel.SelectedValue.GetValueFromDisplayName<TonnageType>();

            await transactionService.CaptureData(User.GetAccessToken(), tonnageTypeViewModel);

            if (tonnageType == Core.Organisations.TonnageType.FiveTonnesOrMore)
            {
                return RedirectToAction("FiveTonnesOrMore", "OrganisationRegistration");
            }

            return RedirectToAction(nameof(PreviousRegistration), typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public ViewResult FiveTonnesOrMore()
        {
            return View();
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

        [HttpGet]
        public async Task<ViewResult> PreviousRegistration()
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            var selectedValue = string.Empty;
            var searchTerm = string.Empty;
            if (existingTransaction?.PreviousRegistration != null)
            {
                selectedValue = existingTransaction.PreviousRegistration.GetDisplayName();
            }

            var viewModel = new PreviousRegistrationViewModel
            {
                SelectedValue = selectedValue
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PreviousRegistration(PreviousRegistrationViewModel previousRegistrationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(previousRegistrationViewModel);
            }

            await transactionService.CaptureData(User.GetAccessToken(), previousRegistrationViewModel);

            var previousRegistration = previousRegistrationViewModel.SelectedValue.GetValueFromDisplayName<YesNoType>();
            if (previousRegistration == YesNoType.Yes)
            {
                return RedirectToAction("Search", "OrganisationRegistration");
            }

            return RedirectToAction(nameof(AuthorisedRepresentative), typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public async Task<ActionResult> AuthorisedRepresentative()
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            var selectedValue = string.Empty;
            if (existingTransaction?.AuthorisedRepresentative != null)
            {
                selectedValue = existingTransaction.AuthorisedRepresentative.GetDisplayName();
            }

            var viewModel = new AuthorisedRepresentativeViewModel
            {
                SelectedValue = selectedValue,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorisedRepresentative(AuthorisedRepresentativeViewModel authorisedRepresentativeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(authorisedRepresentativeViewModel);
            }

            await transactionService.CaptureData(User.GetAccessToken(), authorisedRepresentativeViewModel);

            return RedirectToAction(nameof(ContactDetails), typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public async Task<ActionResult> ContactDetails()
        {
            ContactDetailsViewModel model = null;

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            if (existingTransaction?.ContactDetailsViewModel != null)
            {
                model = existingTransaction.ContactDetailsViewModel;
            }
            else
            {
                model = new ContactDetailsViewModel();
            }

            var countries = await GetCountries();
            model.AddressData.Countries = countries;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactDetails(ContactDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.AddressData.Countries = countries;

                ModelState.ApplyCustomValidationSummaryOrdering(OrganisationViewModel.ValidationMessageDisplayOrder);

                return View(model);
            }

            await transactionService.CaptureData(User.GetAccessToken(), model);

            return RedirectToAction(nameof(Type), typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public ViewResult RegistrationComplete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckAndCompleteApplication()
        {
            return RedirectToAction("Index", typeof(HoldingController).GetControllerName());
        }
    }
}
