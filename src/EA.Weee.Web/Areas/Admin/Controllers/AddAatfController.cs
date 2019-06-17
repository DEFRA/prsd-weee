namespace EA.Weee.Web.Areas.Admin.Controllers
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
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Type;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
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
        private const int maximumSearchResults = 5;
        private readonly BreadcrumbService breadcrumb;

        public AddAatfController(
            ISearcher<OrganisationSearchResult> organisationSearcher,
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache)
        {
            this.organisationSearcher = organisationSearcher;
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
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

                switch (organisationType)
                {
                    case OrganisationType.SoleTraderOrIndividual:
                    case OrganisationType.Partnership:
                        return RedirectToAction("SoleTraderOrPartnershipDetails", "AddAatf", new { organisationType = model.SelectedValue, facilityType = model.FacilityType, searchedText = model.SearchedText });
                    case OrganisationType.RegisteredCompany:
                        return RedirectToAction("RegisteredCompanyDetails", "AddAatf", new { organisationType = model.SelectedValue, facilityType = model.FacilityType, searchedText = model.SearchedText });
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderOrPartnershipDetails(string organisationType, FacilityType facilityType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            IList<CountryData> countries = await GetCountries();

            SoleTraderOrPartnershipDetailsViewModel model = new SoleTraderOrPartnershipDetailsViewModel
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
        public async Task<ActionResult> SoleTraderOrPartnershipDetails(SoleTraderOrPartnershipDetailsViewModel model)
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

                return RedirectToAction("Add", "AddAatf", new { organisationId = id, facilityType = model.FacilityType });
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
                viewModel.SiteAddressData.Countries = countries;
                viewModel.CompetentAuthoritiesList = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
                viewModel.PanAreaList = await client.SendAsync(User.GetAccessToken(), new GetPanAreas());
                viewModel.LocalAreaList = await client.SendAsync(User.GetAccessToken(), new GetLocalAreas());
                viewModel.SizeList = Enumeration.GetAll<AatfSize>();
                viewModel.StatusList = Enumeration.GetAll<AatfStatus>();
                viewModel.OrganisationName = organisation.OrganisationName;
            }

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

        private AatfData CreateFacilityData<T>(T viewModel)
            where T : AddFacilityViewModelBase
        {
            var data = new AatfData(
                Guid.NewGuid(),
                viewModel.Name,
                viewModel.ApprovalNumber,
                viewModel.ComplianceYear,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Id == viewModel.CompetentAuthorityId),
                Enumeration.FromValue<AatfStatus>(viewModel.StatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SizeValue),
                viewModel.ApprovalDate.GetValueOrDefault(),
                viewModel.PanAreaList.FirstOrDefault(p => p.Id == viewModel.PanAreaId),
                viewModel.LocalAreaList.FirstOrDefault(p => p.Id == viewModel.LocalAreaId)) {FacilityType = viewModel.FacilityType};

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