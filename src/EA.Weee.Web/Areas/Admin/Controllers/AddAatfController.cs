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
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
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
        private const int maximumSearchResults = 5;

        public AddAatfController(ISearcher<OrganisationSearchResult> organisationSearcher, Func<IWeeeClient> apiClient)
        {
            this.organisationSearcher = organisationSearcher;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("SearchResults", "AddAatf", new { viewModel.SearchTerm });
        }

        [HttpGet]
        public async Task<ActionResult> SearchResults(string searchTerm)
        {
            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = searchTerm;
            viewModel.Results = await organisationSearcher.Search(searchTerm, maximumSearchResults, false);

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

            return RedirectToAction("Add", "AddAatf", new { organisationId = viewModel.SelectedOrganisationId });
        }

        [HttpGet]
        public async Task<ActionResult> Add(Guid organisationId)
        {
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

                return RedirectToAction("ManageAatfs", "Aatf");
            }
        }

        [HttpGet]
        public ActionResult Type(string searchedText)
        {
            return View(new OrganisationTypeViewModel(searchedText));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Type(OrganisationTypeViewModel model)
        {
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

                return RedirectToAction("OrganisationConfirmation", "AddAatf", new { organisationId = id, organisationName = model.BusinessTradingName });
            }
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails(string organisationType, string searchedText = null)
        {
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

                return RedirectToAction("OrganisationConfirmation", "AddAatf", new { organisationId = id, organisationName = model.CompanyName });
            }
        }

        [HttpGet]
        public ActionResult OrganisationConfirmation(Guid organisationId, string organisationName)
        {
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
                viewModel.AatfName,
                viewModel.ApprovalNumber,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Id == viewModel.CompetentAuthorityId),
                Enumeration.FromValue<AatfStatus>(viewModel.SelectedStatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SelectedSizeValue),
                viewModel.ApprovalDate.GetValueOrDefault());
        }
    }
}