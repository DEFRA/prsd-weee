﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Type;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public class AddAatfController : AdminController
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly int maximumSearchResults;
        private readonly IFacilityViewModelBaseValidatorWrapper validationWrapper;

        public AddAatfController(
            ISearcher<OrganisationSearchResult> organisationSearcher,
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            ConfigurationService configurationService,
            IFacilityViewModelBaseValidatorWrapper validationWrapper)
        {
            this.organisationSearcher = organisationSearcher;
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.validationWrapper = validationWrapper;

            maximumSearchResults = configurationService.CurrentConfiguration.MaximumAatfOrganisationSearchResults;
        }

        [HttpGet]
        public ActionResult Search(FacilityType facilityType)
        {
            SetBreadcrumb(facilityType);
            return View(new SearchViewModel { FacilityType = facilityType });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel viewModel)
        {
            SetBreadcrumb(viewModel.FacilityType);
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("SearchResults", "AddAatf", new { viewModel.SearchTerm, viewModel.FacilityType });
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
            
            IList<OrganisationSearchResult> searchResults = await organisationSearcher.Search(searchTerm, maximumSearchResults, true);

            return Json(searchResults, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm, FacilityType facilityType)
        {
            SetBreadcrumb(facilityType);

            var viewModel = new SearchResultsViewModel
            {
                SearchTerm = searchTerm,
                FacilityType = facilityType,
                Results = await organisationSearcher.Search(searchTerm, maximumSearchResults, false)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchResults(SearchResultsViewModel viewModel)
        {
            SetBreadcrumb(viewModel.FacilityType);

            if (!ModelState.IsValid)
            {
                viewModel.Results = await organisationSearcher.Search(viewModel.SearchTerm, maximumSearchResults, false);

                return View(viewModel);
            }

            return RedirectToAction("Add", "AddAatf", new { organisationId = viewModel.SelectedOrganisationId, facilityType = viewModel.FacilityType });
        }

        [HttpGet]
        public async Task<ActionResult> Add(Guid organisationId, FacilityType facilityType)
        {
            SetBreadcrumb(facilityType);

            switch (facilityType)
            {
                case FacilityType.Aatf:
                    return View(await PopulateAndReturnViewModel(new AddAatfViewModel(), organisationId));
                case FacilityType.Ae:
                    return View(await PopulateAndReturnViewModel(new AddAeViewModel(), organisationId));
                default:
                    throw new ArgumentOutOfRangeException(nameof(facilityType));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAatf(AddAatfViewModel viewModel)
        {
            return await AddFacility(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAe(AddAeViewModel viewModel)
        {
            return await AddFacility(viewModel);
        }

        [HttpGet]
        public ActionResult Type(string searchedText, FacilityType facilityType)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            return View(new OrganisationTypeViewModel(searchedText, facilityType));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Type(OrganisationTypeViewModel model)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            if (ModelState.IsValid)
            {
                var organisationType = model.SelectedValue.GetValueFromDisplayName<OrganisationType>();
                var routeValues = new { organisationType = model.SelectedValue, facilityType = model.FacilityType, searchedText = model.SearchedText };

                switch (organisationType)
                {
                    case OrganisationType.SoleTraderOrIndividual:
                        return RedirectToAction(nameof(SoleTraderDetails), "AddAatf", routeValues);
                    case OrganisationType.Partnership:
                        return RedirectToAction(nameof(PartnershipDetails), "AddAatf", routeValues);
                    case OrganisationType.RegisteredCompany:
                        return RedirectToAction(nameof(RegisteredCompanyDetails), "AddAatf", routeValues);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> PartnershipDetails(string organisationType, FacilityType facilityType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            IList<CountryData> countries = await GetCountries();

            var model = new PartnershipDetailsViewModel
            {
                BusinessTradingName = searchedText,
                OrganisationType = organisationType,
                FacilityType = facilityType
            };

            model.Address.Countries = countries;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PartnershipDetails(PartnershipDetailsViewModel model)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            if (!ModelState.IsValid)
            {
                IList<CountryData> countries = await GetCountries();

                model.Address.Countries = countries;
                return View(model);
            }

            using (var client = apiClient())
            {
                CreateOrganisationAdmin request = new CreateOrganisationAdmin()
                {
                    Address = model.Address,
                    BusinessName = model.BusinessTradingName,
                    OrganisationType = model.OrganisationType.GetValueFromDisplayName<OrganisationType>()
                };

                Guid id = await client.SendAsync(User.GetAccessToken(), request);

                return RedirectToAction(nameof(Add), "AddAatf", new { organisationId = id, facilityType = model.FacilityType });
            }
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderDetails(string organisationType, FacilityType facilityType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            var countries = await GetCountries();

            var model = new SoleTraderDetailsViewModel
            {
                CompanyName = searchedText,
                OrganisationType = organisationType,
                FacilityType = facilityType
            };

            model.Address.Countries = countries;

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

                return RedirectToAction(nameof(Add), "AddAatf", new { organisationId = id, facilityType = model.FacilityType });
            }
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails(string organisationType, FacilityType facilityType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            IList<CountryData> countries = await GetCountries();

            RegisteredCompanyDetailsViewModel model = new RegisteredCompanyDetailsViewModel()
            {
                CompanyName = searchedText,
                OrganisationType = organisationType,
                FacilityType = facilityType
            };

            model.Address.Countries = countries;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredCompanyDetails(RegisteredCompanyDetailsViewModel model)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            if (!ModelState.IsValid)
            {
                IList<CountryData> countries = await GetCountries();

                model.Address.Countries = countries;

                return View(model);
            }

            using (var client = apiClient())
            {
                CreateOrganisationAdmin request = new CreateOrganisationAdmin()
                {
                    Address = model.Address,
                    BusinessName = model.CompanyName,
                    OrganisationType = model.OrganisationType.GetValueFromDisplayName<OrganisationType>(),
                    RegistrationNumber = model.CompaniesRegistrationNumber,
                    TradingName = model.BusinessTradingName
                };

                Guid id = await client.SendAsync(User.GetAccessToken(), request);

                await cache.InvalidateOrganisationSearch();

                return RedirectToAction("Add", "AddAatf", new { organisationId = id, facilityType = model.FacilityType });
            }
        }

        private async Task<T> PopulateAndReturnViewModel<T>(T viewModel, Guid organisationId)
            where T : AddFacilityViewModelBase
        {
            viewModel.OrganisationId = organisationId;
            return await PopulateViewModelLists(viewModel);
        }

        private async Task<T> PopulateViewModelLists<T>(T viewModel)
            where T : AddFacilityViewModelBase
        {
            using (var client = apiClient())
            {
                var countries = await GetCountries();
                var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(viewModel.OrganisationId));

                viewModel.ContactData.AddressData.Countries = countries;
                viewModel.OrganisationName = organisation.OrganisationName;
                viewModel = await PopulateFacilityViewModelLists(viewModel, countries, client, User.GetAccessToken());
            }

            return viewModel;
        }

        public static async Task<T> PopulateFacilityViewModelLists<T>(T viewModel, IList<CountryData> countries, IWeeeClient client, string accessToken)
           where T : FacilityViewModelBase
        {          
                viewModel.SiteAddressData.Countries = countries;
                viewModel.CompetentAuthoritiesList = await client.SendAsync(accessToken, new GetUKCompetentAuthorities());
                viewModel.PanAreaList = await client.SendAsync(accessToken, new GetPanAreas());
                viewModel.LocalAreaList = await client.SendAsync(accessToken, new GetLocalAreas());
                viewModel.SizeList = Enumeration.GetAll<AatfSize>();
                viewModel.StatusList = Enumeration.GetAll<AatfStatus>();
          
            return viewModel;
        }

        private async Task<IList<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }
        }

        private async Task<ActionResult> AddFacility(AddFacilityViewModelBase viewModel)
        {
            PreventSiteAddressNameValidationErrors();
            SetBreadcrumb(viewModel.FacilityType);
            viewModel = await PopulateViewModelLists(viewModel);

            if (!ModelState.IsValid)
            {
                if (!viewModel.ModelValidated)
                {
                    ModelState.RunCustomValidation(viewModel);
                }

                ModelState.ApplyCustomValidationSummaryOrdering(AddFacilityViewModelBase.ValidationMessageDisplayOrder);
                return View(nameof(Add), viewModel);
            }

            using (var client = apiClient())
            {
                var result = await validationWrapper.Validate(User.GetAccessToken(), viewModel);

                if (!result.IsValid)
                {
                    ModelState.AddModelError("ApprovalNumber", Constants.ApprovalNumberExistsError);
                    return View(nameof(Add), viewModel);
                }
                
                var request = new AddAatf()
                {
                    Aatf = CreateFacilityData(viewModel),
                    AatfContact = viewModel.ContactData,
                    OrganisationId = viewModel.OrganisationId,
                };

                await client.SendAsync(User.GetAccessToken(), request);

                await cache.InvalidateAatfCache(request.OrganisationId);

                await client.SendAsync(User.GetAccessToken(), new CompleteOrganisationAdmin() { OrganisationId = viewModel.OrganisationId });
                await cache.InvalidateOrganisationSearch();

                return RedirectToAction("ManageAatfs", "Aatf", new { viewModel.FacilityType });
            }
        }

        public static AatfData CreateFacilityData<T>(T viewModel)
            where T : FacilityViewModelBase
        {
            var data = new AatfData(
                Guid.NewGuid(),
                viewModel.Name,
                viewModel.ApprovalNumber,
                viewModel.ComplianceYear,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Abbreviation == viewModel.CompetentAuthorityId),
                Enumeration.FromValue<AatfStatus>(viewModel.StatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SizeValue),
                viewModel.ApprovalDate.GetValueOrDefault(),
                viewModel.PanAreaList.FirstOrDefault(p => p.Id == viewModel.PanAreaId),
                viewModel.LocalAreaList.FirstOrDefault(p => p.Id == viewModel.LocalAreaId))
            { FacilityType = viewModel.FacilityType };

            return data;
        }

        private void SetBreadcrumb(FacilityType type)
        {
            switch (type)
            {
                case FacilityType.Aatf:
                    SetBreadcrumb(InternalUserActivity.CreateAatf);
                    break;
                case FacilityType.Ae:
                    SetBreadcrumb(InternalUserActivity.CreateAe);
                    break;
                default:
                    break;
            }
        }

        private void SetBreadcrumb(string activity)
        {
            breadcrumb.InternalActivity = activity;
        }
    }
}