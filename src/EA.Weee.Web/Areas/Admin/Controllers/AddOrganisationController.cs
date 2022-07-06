namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Search;
    using Core.Shared;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation;
    using Filters;
    using Infrastructure;
    using Security;
    using Services;
    using Services.Caching;
    using ViewModels.AddOrganisation.Details;
    using ViewModels.AddOrganisation.Type;
    using ViewModels.Home;
    using Weee.Requests.Admin;
    using Weee.Requests.Shared;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public class AddOrganisationController : AdminController
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly int maximumSearchResults;

        public AddOrganisationController(
            ISearcher<OrganisationSearchResult> organisationSearcher,
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            ConfigurationService configurationService)
        {
            this.organisationSearcher = organisationSearcher;
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;

            maximumSearchResults = configurationService.CurrentConfiguration.MaximumAatfOrganisationSearchResults;
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
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var searchResults = await GetSearchResults(searchTerm, true);

            return Json(searchResults, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm, EntityType entityType)
        {
            SetBreadcrumb(entityType);

            var viewModel = new SearchResultsViewModel
            {
                SearchTerm = searchTerm,
                EntityType = entityType,
                Results = await GetSearchResults(searchTerm, false)
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Search(EntityType entityType)
        {
            SetBreadcrumb(entityType);
            return View(new SearchViewModel { EntityType = entityType });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel viewModel)
        {
            SetBreadcrumb(viewModel.EntityType);
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("SearchResults", "AddOrganisation", new { viewModel.SearchTerm, entityType = viewModel.EntityType });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchResults(SearchResultsViewModel viewModel)
        {
            SetBreadcrumb(viewModel.EntityType);

            if (!ModelState.IsValid)
            {
                viewModel.Results = await GetSearchResults(viewModel.SearchTerm, false);

                return View(viewModel);
            }

            if (viewModel.EntityType == EntityType.Pcs)
            {
                var selectedOrg = viewModel.Results.FirstOrDefault(p => p.OrganisationId == viewModel.SelectedOrganisationId);

                if (selectedOrg.PcsCount == 0)
                {
                    return RedirectToAction("AddScheme", "Scheme", new { organisationId = viewModel.SelectedOrganisationId });
                }

                return RedirectToAction("OrgAlreadyHasScheme", "AddOrganisation", new {searchTerm = viewModel.SearchTerm});
            }

            return RedirectToAction("Add", "AddAatf", new { organisationId = viewModel.SelectedOrganisationId, facilityType = viewModel.EntityType });
        }

        [HttpGet]
        public ActionResult OrgAlreadyHasScheme(string searchTerm)
        {
            SetBreadcrumb(InternalUserActivity.ManageScheme);

            OrgAlreadyHasSchemeViewModel model = new OrgAlreadyHasSchemeViewModel()
            {
                SearchTerm = searchTerm
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Type(string searchedText, EntityType entityType)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            return View(new OrganisationTypeViewModel(searchedText, entityType));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Type(OrganisationTypeViewModel model)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            if (ModelState.IsValid)
            {
                var organisationType = model.SelectedValue.GetValueFromDisplayName<OrganisationType>();
                var routeValues = new { organisationType = model.SelectedValue, entityType = model.EntityType, searchedText = model.SearchedText };

                switch (organisationType)
                {
                    case OrganisationType.SoleTraderOrIndividual:
                        return RedirectToAction(nameof(SoleTraderDetails), "AddOrganisation", routeValues);
                    case OrganisationType.Partnership:
                        return RedirectToAction(nameof(PartnershipDetails), "AddOrganisation", routeValues);
                    case OrganisationType.RegisteredCompany:
                        return RedirectToAction(nameof(RegisteredCompanyDetails), "AddOrganisation", routeValues);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> PartnershipDetails(string organisationType, EntityType entityType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            var countries = await GetCountries();

            var model = new PartnershipDetailsViewModel
            {
                BusinessTradingName = searchedText,
                OrganisationType = organisationType,
                EntityType = entityType,
                Address = { Countries = countries }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PartnershipDetails(PartnershipDetailsViewModel model)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;
                return View(model);
            }

            using (var client = apiClient())
            {
                var request = new CreateOrganisationAdmin()
                {
                    Address = model.Address,
                    BusinessName = model.BusinessTradingName,
                    OrganisationType = model.OrganisationType.GetValueFromDisplayName<OrganisationType>()
                };

                var id = await client.SendAsync(User.GetAccessToken(), request);

                return RedirectToAddAction(id, model.EntityType);
            }
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderDetails(string organisationType, EntityType entityType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            var countries = await GetCountries();

            var model = new SoleTraderDetailsViewModel
            {
                CompanyName = searchedText,
                OrganisationType = organisationType,
                EntityType = entityType,
                Address = { Countries = countries }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SoleTraderDetails(SoleTraderDetailsViewModel model)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;
                return View(model);
            }

            using (var client = apiClient())
            {
                var request = new CreateOrganisationAdmin()
                {
                    Address = model.Address,
                    BusinessName = model.CompanyName,
                    TradingName = model.BusinessTradingName,
                    OrganisationType = model.OrganisationType.GetValueFromDisplayName<OrganisationType>()
                };

                var id = await client.SendAsync(User.GetAccessToken(), request);

                return RedirectToAddAction(id, model.EntityType);
            }
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails(string organisationType, EntityType entityType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            var countries = await GetCountries();

            var model = new RegisteredCompanyDetailsViewModel
            {
                CompanyName = searchedText,
                OrganisationType = organisationType,
                EntityType = entityType,
                Address = { Countries = countries }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredCompanyDetails(RegisteredCompanyDetailsViewModel model)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            if (!ModelState.IsValid)
            {
                var countries = await GetCountries();

                model.Address.Countries = countries;

                return View(model);
            }

            using (var client = apiClient())
            {
                var request = new CreateOrganisationAdmin()
                {
                    Address = model.Address,
                    BusinessName = model.CompanyName,
                    OrganisationType = model.OrganisationType.GetValueFromDisplayName<OrganisationType>(),
                    RegistrationNumber = model.CompaniesRegistrationNumber,
                    TradingName = model.BusinessTradingName
                };

                var id = await client.SendAsync(User.GetAccessToken(), request);

                await cache.InvalidateOrganisationSearch();

                return RedirectToAddAction(id, model.EntityType);
            }
        }

        private ActionResult RedirectToAddAction(Guid organisationId, EntityType entityType)
        {
            if (entityType == EntityType.Pcs)
            {
                return RedirectToAction("AddScheme", "Scheme", new {organisationId});
            }

            return RedirectToAction("Add", "AddAatf", new { organisationId, FacilityType = entityType });
        }

        private async Task<IList<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }
        }

        private void SetBreadcrumb(EntityType type)
        {
            switch (type)
            {
                case EntityType.Aatf:
                    SetBreadcrumb(InternalUserActivity.ManageAatfs);
                    break;
                case EntityType.Ae:
                    SetBreadcrumb(InternalUserActivity.ManageAes);
                    break;
                case EntityType.Pcs:
                    SetBreadcrumb(InternalUserActivity.ManageScheme);
                    break;
                default:
                    break;
            }
        }

        private void SetBreadcrumb(string activity)
        {
            breadcrumb.InternalActivity = activity;
        }

        private async Task<List<OrganisationSearchResult>> GetSearchResults(string searchTerm, bool asYouType)
        {
            var results = await organisationSearcher.Search(searchTerm, maximumSearchResults, asYouType);

            return results.Where(r => !r.IsBalancingScheme).ToList();
        }
    }
}