﻿namespace EA.Weee.Web.Controllers
{
    using Api.Client;
    using Base;
    using Core.Helpers;
    using Core.Organisations;
    using Core.Search;
    using Core.Shared;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Api.Client.Models;
    using EA.Weee.Api.Client.Models.AddressLookup;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
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
        private readonly int maxPartnersAllowed = 10;
        private readonly Func<ICompaniesHouseClient> companiesHouseClient;
        private readonly Func<IAddressLookupClient> addressLookupClient;
        private readonly ConfigurationService configurationService;

        public OrganisationRegistrationController(Func<IWeeeClient> apiClient,
            ISearcher<OrganisationSearchResult> organisationSearcher,
            ConfigurationService configurationService, IOrganisationTransactionService transactionService,
            IWeeeCache cache,
            Func<ICompaniesHouseClient> companiesHouseClient,
            Func<IAddressLookupClient> addressLookupClient)
        {
            this.apiClient = apiClient;
            this.organisationSearcher = organisationSearcher;
            this.configurationService = configurationService;
            this.transactionService = transactionService;
            this.cache = cache;
            this.companiesHouseClient = companiesHouseClient;
            this.addressLookupClient = addressLookupClient;
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
                return RedirectToAction(nameof(JoinOrganisation), new
                {
                    OrganisationId = viewModel.SelectedOrganisationId.Value,
                    SearchTerm = viewModel.SearchTerm
                });
            }

            return RedirectToAction("SearchResults", new { viewModel.SearchTerm });
        }

        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm)
        {
            DateTime currentDate;

            using (var client = this.apiClient())
            {
                currentDate = await client.SendAsync(this.User.GetAccessToken(), new GetApiUtcDate());
            }

            var results = await organisationSearcher.Search(searchTerm, maximumSearchResults, false);

            SearchResultsViewModel viewModel = new SearchResultsViewModel
            {
                SearchTerm = searchTerm,
                Results = results,
                ShowSmallProducerMessage = currentDate >= configurationService.CurrentConfiguration.SmallProducerFeatureEnabledFrom
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchResults(SearchResultsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Results =
                    await organisationSearcher.Search(viewModel.SearchTerm, maximumSearchResults, false);

                return View(viewModel);
            }

            return RedirectToAction("JoinOrganisation", new
            {
                OrganisationId = viewModel.SelectedOrganisationId.Value, viewModel.SearchTerm
            });
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="searchTerm"></param>
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

            IList<OrganisationSearchResult> searchResults =
                await organisationSearcher.Search(searchTerm, maximumSearchResults, true);

            return Json(searchResults, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> ContinueSmallProducerRegistration(Guid organisationId, string searchTerm, bool smallProducerFound)
        {
            var accessToken = User.GetAccessToken();

            // Handle the case where no producer was found first, before making unnecessary service calls
            if (!smallProducerFound)
            {
                await transactionService.DeleteOrganisationTransactionData(accessToken);

                await transactionService.ContinueMigratedProducerTransactionData(accessToken, organisationId);

                return RedirectToAction(nameof(TonnageType), new { searchTerm });
            }

            var existingTransaction = await transactionService.GetOrganisationTransactionData(accessToken);

            var continuedData = await transactionService.ContinueMigratedProducerTransactionData(accessToken, organisationId);

            // this means the user has been around the loop, completed all the data, found their organisation and are trying to re-join it.
            // as all data has been completed indicated by smallProducerFound being true, just complete the organisation.
            // we shouldn't get here if the organisation isn't an NPWD migrated one.
            if (existingTransaction != null && continuedData != null)
            {
                if (existingTransaction.AuthorisedRepresentative == YesNoType.Yes)
                {
                    var returnUrl = Url.Action(nameof(OrganisationDetails));
                    return RedirectToAction(nameof(RepresentingCompanyDetails), typeof(OrganisationRegistrationController).GetControllerName(), new { returnUrl });
                }

                await transactionService.CompleteTransaction(accessToken, continuedData.DirectRegistrantId);

                await cache.InvalidateOrganisationSearch();

                return RedirectToAction(nameof(RegistrationComplete), typeof(OrganisationRegistrationController).GetControllerName(), new { organisationId });
            }

            return RedirectToAction(nameof(TonnageType), new { searchTerm });
        }

        [HttpGet]
        public async Task<ActionResult> JoinOrganisation(Guid organisationId, string searchTerm = null, bool smallProducerFound = false)
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

                var activeUsers = await client.SendAsync(User.GetAccessToken(),
                    new GetActiveOrganisationUsers(organisationId));

                if (organisationData.NpwdMigrated && !organisationData.NpwdMigratedComplete)
                {
                    return await ContinueSmallProducerRegistration(organisationId, searchTerm, smallProducerFound);
                }

                if (existingAssociation != null)
                {
                    UserAlreadyAssociatedWithOrganisationViewModel viewModel =
                        new UserAlreadyAssociatedWithOrganisationViewModel()
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

                return RedirectToAction("JoinOrganisationConfirmation",
                    new { organisationId = viewModel.OrganisationId, activeUsers = viewModel.AnyActiveUsers });
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
                var routeValues = new { organisationType = model.SelectedValue };

                await transactionService.CaptureData(User.GetAccessToken(), model);

                var organisationType = model.SelectedValue.GetValueFromDisplayName<ExternalOrganisationType>();
                switch (organisationType)
                {
                    case ExternalOrganisationType.SoleTrader:
                        return RedirectToAction(nameof(SoleTraderDetails), typeof(OrganisationRegistrationController).GetControllerName(), routeValues);
                    case ExternalOrganisationType.Partnership:
                        return RedirectToAction(nameof(PartnerDetails), typeof(OrganisationRegistrationController).GetControllerName(), routeValues);
                    case ExternalOrganisationType.RegisteredCompany:
                        return RedirectToAction(nameof(OrganisationDetails), typeof(OrganisationRegistrationController).GetControllerName(), routeValues);
                }

                return RedirectToAction(nameof(OrganisationDetails), typeof(OrganisationRegistrationController).GetControllerName(), routeValues);
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
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            await transactionService.CaptureData(User.GetAccessToken(), model);

            var organisationId = await transactionService.CompleteTransaction(User.GetAccessToken(), existingTransaction.DirectRegistrantId);

            await cache.InvalidateOrganisationSearch();

            return RedirectToAction(nameof(RegistrationComplete), typeof(OrganisationRegistrationController).GetControllerName(), new { organisationId });
        }

        [HttpGet]
        public async Task<ActionResult> RepresentingCompanyDetails(string returnUrl)
        {
            RepresentingCompanyDetailsViewModel model = null;

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            model = existingTransaction?.RepresentingCompanyDetailsViewModel ??
                    new RepresentingCompanyDetailsViewModel();

            var countries = await GetCountries();
            model.Address.Countries = countries;

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> FindCompany(string companiesRegistrationNumber)
        {
            var result = await GetCompany(companiesRegistrationNumber);

            var orgModel = LookupOrganisationViewModel(result);

            return Json(orgModel, JsonRequestBehavior.AllowGet);
        }

        private static OrganisationViewModel LookupOrganisationViewModel(DefraCompaniesHouseApiModel result, OrganisationViewModel model = null)
        {
            var address1 = result.Organisation?.RegisteredOffice?.BuildingNumber ??
                           result.Organisation?.RegisteredOffice?.BuildingName ??
                           result.Organisation?.RegisteredOffice?.SubBuildingName;

            var address2 = result.Organisation?.RegisteredOffice?.Street;
            var countryId = UkCountry.GetIdByName(result.Organisation?.RegisteredOffice?.Country.Name);

            var orgModel = new OrganisationViewModel()
            {
                CompanyName = result.Organisation?.Name,
                CompaniesRegistrationNumber = result.Organisation?.RegistrationNumber,
                LookupFound = !result.HasError,
                Address = new ExternalAddressData
                {
                    Address1 = address1 ?? address2,
                    Address2 = address1 != null ? address2 : null,
                    TownOrCity = result.Organisation?.RegisteredOffice?.Town,
                    Postcode = result.Organisation?.RegisteredOffice?.Postcode,
                    CountyOrRegion = result.Organisation?.RegisteredOffice?.County ?? result.Organisation?.RegisteredOffice?.Locality,
                    CountryId = DetermineCountryId(countryId, model)
                },
            };
            return orgModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganisationDetails(OrganisationViewModel model)
        {
            if (model.Action == "Find Company")
            {
                var result = await GetCompany(model.CompaniesRegistrationNumber);

                var countries = await GetCountries();

                ModelState.Clear();

                model.Address.Countries = countries;

                if (result.HasError)
                {
                    model.LookupFound = false;

                    return View(model.CastToSpecificViewModel(model));
                }

                var orgModel = LookupOrganisationViewModel(result, model);
                orgModel.Address.Countries = countries;

                return View(model.CastToSpecificViewModel(orgModel));
            }

            var castedModel = model.CastToSpecificViewModel(model);
            var isValid = ValidationModel.ValidateModel(castedModel, ModelState);

            await ValidateProducerRegistrationNumber(model);

            if (!isValid || !ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;

                ModelState.ApplyCustomValidationSummaryOrdering(OrganisationViewModel.ValidationMessageDisplayOrder);

                return View(model.CastToSpecificViewModel(model));
            }

            await transactionService.CaptureData(User.GetAccessToken(), model);

            var existingOrgs = await GetExistingOrganisations(model);

            if (existingOrgs.Organisations.Any())
            {
                TempData["FoundOrganisations"] = existingOrgs;

                return RedirectToAction("OrganisationFound");
            }

            return await CheckAuthorisedRepresentitiveAndRedirect(null);
        }

        [HttpGet]
        public async Task<ActionResult> OrganisationDetails()
        {
            var accessToken = User.GetAccessToken();
            var existingTransaction = await transactionService.GetOrganisationTransactionData(accessToken);

            var model = MapToOrganisationViewModel(existingTransaction);
            await EnrichModelWithCountries(model);

            var specificViewModel = model.CastToSpecificViewModel(model);
            return View(specificViewModel);
        }

        private static OrganisationViewModel MapToOrganisationViewModel(OrganisationTransactionData existingTransaction)
        {
            if (existingTransaction == null)
            {
                return new OrganisationViewModel
                {
                    OrganisationType = ExternalOrganisationType.RegisteredCompany
                };
            }

            OrganisationViewModel model;
            if (existingTransaction.OrganisationViewModel != null)
            {
                model = existingTransaction.OrganisationViewModel;
                model.OrganisationType = existingTransaction.OrganisationType ?? ExternalOrganisationType.RegisteredCompany;
            }
            else
            {
                model = new OrganisationViewModel
                {
                    CompanyName = existingTransaction.SearchTerm,
                    OrganisationType = existingTransaction.OrganisationType ?? ExternalOrganisationType.RegisteredCompany
                };
            }

            model.HasAuthorisedRepresentitive = existingTransaction?.AuthorisedRepresentative == YesNoType.Yes;

            if (existingTransaction.PreviousRegistration == PreviouslyRegisteredProducerType.YesPreviousSchemeMember)
            {
                model.IsPreviousSchemeMember = true;
            }

            model.NpwdMigrated = existingTransaction.NpwdMigrated;

            return model;
        }

        private async Task EnrichModelWithCountries(OrganisationViewModel model)
        {
            if (model?.Address != null)
            {
                model.Address.Countries = await GetCountries();
            }
        }

        [HttpGet]
        public async Task<ActionResult> RepresentingCompanyRedirect(string returnUrl)
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            if (existingTransaction?.OrganisationType == null)
            {
                return RedirectToAction(nameof(Type), typeof(OrganisationRegistrationController).GetControllerName());
            }

            if (returnUrl is null)
            {
                return RedirectToAction(nameof(OrganisationDetails), typeof(OrganisationRegistrationController).GetControllerName());
            }

            return new RedirectResult(returnUrl);
        }

        private async Task<List<Core.Organisations.OrganisationData>> GetExistingByRegistrationNumber(OrganisationViewModel model)
        {
            using (var client = apiClient())
            {
                var res = await client
                    .SendAsync(User.GetAccessToken(), new OrganisationByRegistrationNumberValue(model.CompaniesRegistrationNumber));
                return res;
            }
        }

        private async Task<OrganisationExistsSearchResult> GetExistingOrganisations(OrganisationViewModel model)
        {
            if (!model.NpwdMigrated)
            {
                var existingOrganisations = new List<Core.Organisations.OrganisationData>();
                if (!string.IsNullOrWhiteSpace(model.CompaniesRegistrationNumber))
                {
                    existingOrganisations = await GetExistingByRegistrationNumber(model);
                }

                if (existingOrganisations.Any())
                {
                    return new OrganisationExistsSearchResult
                    {
                        Organisations = existingOrganisations.Select(existing => new OrganisationFoundViewModel
                        {
                            OrganisationName = existing.Name,
                            CompanyRegistrationNumber = existing.CompanyRegistrationNumber,
                            OrganisationId = existing.Id,
                            NpwdMigrated = existing.NpwdMigrated,
                            NpwdMigratedComplete = existing.NpwdMigratedComplete
                        }).ToList(),
                        FoundType = OrganisationFoundType.CompanyNumber
                    };
                }

                var nameSearch = await organisationSearcher.Search(model.CompanyName, maximumSearchResults, false);
                var organisationsMapped = nameSearch.Select(x => new OrganisationFoundViewModel
                {
                    OrganisationName = x.Name,
                    CompanyRegistrationNumber = x.CompanyRegistrationNumber,
                    OrganisationId = x.OrganisationId,
                    NpwdMigrated = x.NpwdMigrated,
                    NpwdMigratedComplete = x.NpwdMigratedComplete
                }).ToList();

                if (organisationsMapped.Any())
                {
                    return new OrganisationExistsSearchResult
                    {
                        Organisations = organisationsMapped,
                        FoundType = OrganisationFoundType.CompanyName
                    };
                }
            }
            return new OrganisationExistsSearchResult
            {
                Organisations = new List<OrganisationFoundViewModel>(),
                FoundType = OrganisationFoundType.NotFound
            };
        }

        [HttpGet]
        public ActionResult OrganisationFound()
        {
            TempData.Keep("FoundOrganisations");

            if (TempData["FoundOrganisations"] is OrganisationExistsSearchResult organisationsExistSearchResults)
            {
                var vm = new OrganisationsFoundViewModel
                {
                    OrganisationFoundViewModels = organisationsExistSearchResults.Organisations,
                    OrganisationFoundType = organisationsExistSearchResults.FoundType
                };

                return View(nameof(OrganisationFound), vm);
            }

            return View(new OrganisationsFoundViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrganisationFound(OrganisationsFoundViewModel orgsFoundViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(orgsFoundViewModel);
            }

            return RedirectToAction(nameof(JoinOrganisation), new
            {
                OrganisationId = orgsFoundViewModel.SelectedOrganisationId.Value,
                SmallProducerFound = true
            });
        }

        [HttpGet]
        public async Task<ActionResult> CheckAuthorisedRepresentitiveAndRedirect(string returnUrl)
        {
            var organisationTransactionData = await transactionService
                                                    .GetOrganisationTransactionData(User.GetAccessToken());

            if (organisationTransactionData.AuthorisedRepresentative == YesNoType.No)
            {
                var organisationId = await transactionService.CompleteTransaction(User.GetAccessToken(), organisationTransactionData.DirectRegistrantId);

                await cache.InvalidateOrganisationSearch();

                return RedirectToAction(nameof(RegistrationComplete), typeof(OrganisationRegistrationController).GetControllerName(), new { organisationId });
            }

            ViewBag.ReturnUrl = returnUrl;
            return RedirectToAction(nameof(RepresentingCompanyDetails),
                typeof(OrganisationRegistrationController).GetControllerName(), new { returnUrl });
        }

        private async Task<IList<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }
        }

        [HttpGet]
        public async Task<ViewResult> TonnageType(string searchTerm)
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            var viewModel = new TonnageTypeViewModel
            {
                SearchedText = !string.IsNullOrWhiteSpace(searchTerm) ? searchTerm : existingTransaction?.SearchTerm ?? " ",
                SelectedValue = existingTransaction?.TonnageType?.GetDisplayName() ?? string.Empty,
                NpwdMigrated = existingTransaction?.NpwdMigrated ?? false
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
                return RedirectToAction(nameof(FiveTonnesOrMore), typeof(OrganisationRegistrationController).GetControllerName());
            }
            if (tonnageTypeViewModel.NpwdMigrated)
            {
                return RedirectToAction(nameof(AuthorisedRepresentative),
                    typeof(OrganisationRegistrationController).GetControllerName());
            }

            return RedirectToAction(nameof(PreviousRegistration),
                typeof(OrganisationRegistrationController).GetControllerName());
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
        public async Task<ActionResult> PreviousRegistration(
            PreviousRegistrationViewModel previousRegistrationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(previousRegistrationViewModel);
            }

            await transactionService.CaptureData(User.GetAccessToken(), previousRegistrationViewModel);

            var previousRegistration = previousRegistrationViewModel.SelectedValue.GetValueFromDisplayName<PreviouslyRegisteredProducerType>();
            if (previousRegistration == PreviouslyRegisteredProducerType.YesPreviousSmallProducer)
            {
                return RedirectToAction(nameof(Search), typeof(OrganisationRegistrationController).GetControllerName());
            }

            return RedirectToAction(nameof(AuthorisedRepresentative),
                typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public async Task<ActionResult> AuthorisedRepresentative()
        {
            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            var selectedValue = string.Empty;
            var npwdMigrated = false;

            if (existingTransaction != null)
            {
                if (existingTransaction.AuthorisedRepresentative != null)
                {
                    selectedValue = existingTransaction.AuthorisedRepresentative.GetDisplayName();
                }

                npwdMigrated = existingTransaction.NpwdMigrated;
            }

            var viewModel = new AuthorisedRepresentativeViewModel
            {
                SelectedValue = selectedValue,
                NpwdMigrated = npwdMigrated
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorisedRepresentative(
            AuthorisedRepresentativeViewModel authorisedRepresentativeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(authorisedRepresentativeViewModel);
            }

            await transactionService.CaptureData(User.GetAccessToken(), authorisedRepresentativeViewModel);

            return RedirectToAction(nameof(ContactDetails),
                typeof(OrganisationRegistrationController).GetControllerName());
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

            model.HasAuthorisedRepresentitive = existingTransaction?.AuthorisedRepresentative == YesNoType.Yes;

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
        public ViewResult RegistrationComplete(Guid organisationId)
        {
            return View(organisationId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrationCompleteSubmit(Guid organisationId)
        {
            return RedirectToAction(nameof(Areas.Scheme.Controllers.HomeController.ChooseActivity), typeof(Areas.Scheme.Controllers.HomeController).GetControllerName(), new { area = nameof(Areas.Scheme), pcsId = organisationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PartnerDetails(PartnerViewModel model, string action, int? removeIndex)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.AllPartnerModels.Count >= maxPartnersAllowed)
            {
                ModelState.AddModelError("PartnerModels", $"A maximum of {maxPartnersAllowed} partners are allowed");

                return View(model);
            }

            if (action == PostActionConstant.PartnerPostAdd)
            {
                model.NotRequiredPartnerModels.Add(new NotRequiredPartnerModel());
                return View(model);
            }

            if (action == PostActionConstant.PartnerPostRemove && removeIndex.HasValue)
            {
                if (removeIndex.Value >= 0 && removeIndex.Value < model.NotRequiredPartnerModels.Count)
                {
                    model.NotRequiredPartnerModels.RemoveAt(removeIndex.Value);
                }
                ModelState.Clear();
                return View(model);
            }

            model.NotRequiredPartnerModels = model.NotRequiredPartnerModels.Where(x => x.FirstName != null).ToList();

            await transactionService.CaptureData(User.GetAccessToken(), model);

            return RedirectToAction(nameof(OrganisationDetails), typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public async Task<ActionResult> PartnerDetails()
        {
            var vm = new PartnerViewModel();

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            if (existingTransaction?.PartnerModels != null && existingTransaction.PartnerModels.Any())
            {
                vm.PartnerModels = existingTransaction.PartnerModels.Where(p => p.Order <= 1).ToList();
                vm.NotRequiredPartnerModels = new List<NotRequiredPartnerModel>(existingTransaction.PartnerModels.Where(p => p.Order > 1).Select(p1 => new NotRequiredPartnerModel()
                {
                    FirstName = p1.FirstName,
                    LastName = p1.LastName
                }));

                return View(vm);
            }

            vm.PartnerModels.Add(new AdditionalContactModel());
            vm.PartnerModels.Add(new AdditionalContactModel());

            return View(vm);
        }

        [HttpGet]
        public ActionResult PreviousPage(ExternalOrganisationType? orgType)
        {
            if (orgType == ExternalOrganisationType.Partnership)
            {
                return RedirectToAction(nameof(PartnerDetails));
            }
            if (orgType == ExternalOrganisationType.SoleTrader)
            {
                return RedirectToAction(nameof(SoleTraderDetails));
            }
            if (orgType == ExternalOrganisationType.RegisteredCompany)
            {
                return RedirectToAction(nameof(Type));
            }

            return RedirectToAction(nameof(Type));
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderDetails()
        {
            SoleTraderViewModel model = null;

            var existingTransaction = await transactionService.GetOrganisationTransactionData(User.GetAccessToken());

            model = existingTransaction?.SoleTraderViewModel ??
                    new SoleTraderViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SoleTraderDetails(SoleTraderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await transactionService.CaptureData(User.GetAccessToken(), model);

            return RedirectToAction(nameof(OrganisationDetails), typeof(OrganisationRegistrationController).GetControllerName());
        }

        [HttpGet]
        public async Task<ActionResult> GetAddresses(string postcode)
        {
            using (var client = addressLookupClient())
            {
                AddressLookupResponse addresses = await client.GetAddressesAsync(configurationService.CurrentConfiguration.AddressLookupReferencePath, postcode);

                return Json(addresses, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<IEnumerable<ValidationResult>> ValidateProducerRegistrationNumber(OrganisationViewModel model)
        {
            var results = new List<ValidationResult>();

            if (!model.NpwdMigrated)
            {
                if (model.IsPreviousSchemeMember && string.IsNullOrWhiteSpace(model.ProducerRegistrationNumber))
                {
                    results.Add(new ValidationResult("Enter a producer registration number", new[] { nameof(model.ProducerRegistrationNumber) }));
                    ModelState.AddModelError(nameof(model.ProducerRegistrationNumber), @"Enter a producer registration number");
                }
                else
                {
                    using (var client = apiClient())
                    {
                        if (!string.IsNullOrWhiteSpace(model.ProducerRegistrationNumber))
                        {
                            var exists = await client.SendAsync(User.GetAccessToken(), new ProducerRegistrationNumberRequest(model.ProducerRegistrationNumber));
                            if (!exists)
                            {
                                ModelState.AddModelError(nameof(OrganisationViewModel.ProducerRegistrationNumber), @"This producer registration number does not exist");
                            }
                        }
                    }
                }
            }

            return results;
        }

        private async Task<Api.Client.Models.DefraCompaniesHouseApiModel> GetCompany(string companiesRegistrationNumber)
        {
            using (var client = companiesHouseClient())
            {
                return await client.GetCompanyDetailsAsync(
                    configurationService.CurrentConfiguration.CompaniesHouseReferencePath,
                    companiesRegistrationNumber);
            }
        }

        private static Guid DetermineCountryId(Guid? countryId, OrganisationViewModel model)
        {
            if (countryId.HasValue && countryId.Value != Guid.Empty)
            {
                return countryId.Value;
            }

            if (model?.Address?.CountryId != null && model.Address.CountryId != Guid.Empty)
            {
                return model.Address.CountryId;
            }

            return Guid.Empty;
        }
    }
}
