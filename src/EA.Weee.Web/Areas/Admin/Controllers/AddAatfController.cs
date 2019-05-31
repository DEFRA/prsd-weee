namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Organisations.Create;
    using EA.Weee.Requests.Organisations.Create.Base;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Type;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)]
    public class AddAatfController : AdminController
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private const int maximumSearchResults = 5;
        private readonly BreadcrumbService breadcrumb;

        public AddAatfController(ISearcher<OrganisationSearchResult> organisationSearcher, Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.organisationSearcher = organisationSearcher;
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public ActionResult Search()
        {
            SetBreadcrumb(InternalUserActivity.CreateAatf);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel viewModel)
        {
            SetBreadcrumb(InternalUserActivity.CreateAatf);
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("SearchResults", "AddAatf", new { viewModel.SearchTerm });
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
        public async Task<ActionResult> SearchResults(string searchTerm)
        {
            SetBreadcrumb(InternalUserActivity.CreateAatf);

            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = searchTerm;
            viewModel.Results = await organisationSearcher.Search(searchTerm, maximumSearchResults, false);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchResults(SearchResultsViewModel viewModel)
        {
            SetBreadcrumb(InternalUserActivity.CreateAatf);

            if (!ModelState.IsValid)
            {
                viewModel.Results = await organisationSearcher.Search(viewModel.SearchTerm, maximumSearchResults, false);

                return View(viewModel);
            }

            return RedirectToAction("Add", "AddAatf", new { organisationId = viewModel.SelectedOrganisationId });
        }

        [HttpGet]
        public async Task<ActionResult> Add(Guid organisationId)
        {
            SetBreadcrumb(InternalUserActivity.CreateAatf);

            AddAatfViewModel viewModel = new AddAatfViewModel()
            {
                OrganisationId = organisationId
            };

            viewModel = await PopulateViewModelLists(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddAatfViewModel viewModel)
        {
            SetBreadcrumb(InternalUserActivity.CreateAatf);

            viewModel = await PopulateViewModelLists(viewModel);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            using (var client = apiClient())
            {
                AddAatf request = new AddAatf()
                {
                    Aatf = CreateAatfData(viewModel),
                    AatfContact = viewModel.ContactData,
                    OrganisationId = viewModel.OrganisationId
                };

                await client.SendAsync(User.GetAccessToken(), request);

                await cache.InvalidateAatfCache(request.OrganisationId);

                return RedirectToAction("ManageAatfs", "Aatf", new { type = "AATF" });
            }
        }

        [HttpGet]
        public ActionResult Type(string searchedText)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            return View(new OrganisationTypeViewModel(searchedText));
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
                        return RedirectToAction("SoleTraderOrPartnershipDetails", "AddAatf", new { organisationType = model.SelectedValue, searchedText = model.SearchedText });
                    case OrganisationType.RegisteredCompany:
                        return RedirectToAction("RegisteredCompanyDetails", "AddAatf", new { organisationType = model.SelectedValue, searchedText = model.SearchedText });
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderOrPartnershipDetails(string organisationType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            IList<CountryData> countries = await GetCountries();

            SoleTraderOrPartnershipDetailsViewModel model = new SoleTraderOrPartnershipDetailsViewModel
            {
                BusinessTradingName = searchedText,
                OrganisationType = organisationType
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

                await cache.InvalidateOrganisationSearch();

                return RedirectToAction("OrganisationConfirmation", "AddAatf", new { organisationId = id, organisationName = model.BusinessTradingName });
            }
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails(string organisationType, string searchedText = null)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            IList<CountryData> countries = await GetCountries();

            RegisteredCompanyDetailsViewModel model = new RegisteredCompanyDetailsViewModel()
            {
                CompanyName = searchedText,
                OrganisationType = organisationType
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

                return RedirectToAction("OrganisationConfirmation", "AddAatf", new { organisationId = id, organisationName = model.CompanyName });
            }
        }

        [HttpGet]
        public ActionResult OrganisationConfirmation(Guid organisationId, string organisationName)
        {
            SetBreadcrumb(InternalUserActivity.CreateOrganisation);

            OrganisationConfirmationViewModel model = new OrganisationConfirmationViewModel()
            {
                OrganisationId = organisationId,
                OrganisationName = organisationName
            };

            return View(model);
        }

        private async Task<AddAatfViewModel> PopulateViewModelLists(AddAatfViewModel viewModel)
        {
            using (var client = apiClient())
            {
                IList<CountryData> countries = await GetCountries();
                OrganisationData organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(viewModel.OrganisationId));

                viewModel.ContactData.AddressData.Countries = countries;
                viewModel.SiteAddressData.Countries = countries;
                viewModel.CompetentAuthoritiesList = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
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

        private AatfData CreateAatfData(AddAatfViewModel viewModel)
        {
            return new AatfData(
                Guid.NewGuid(),
                viewModel.Name,
                viewModel.ApprovalNumber,
                viewModel.SelectedComplianceYear,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Id == viewModel.CompetentAuthorityId),
                Enumeration.FromValue<AatfStatus>(viewModel.SelectedStatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SelectedSizeValue),
                viewModel.ApprovalDate.GetValueOrDefault());
        }

        private void SetBreadcrumb(string activity)
        {
            breadcrumb.InternalActivity = activity;
        }
    }
}