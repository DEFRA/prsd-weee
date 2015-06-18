namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Web.ApiClient;
    using EA.Prsd.Core.Web.Mvc.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Organisations.Create;
    using EA.Weee.Requests.Organisations.Create.Base;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Requests;
    using EA.Weee.Web.ViewModels.JoinOrganisation;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
    using EA.Weee.Web.ViewModels.Shared;

    [Authorize]
    public class OrganisationRegistrationController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public OrganisationRegistrationController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult Type()
        {
            return View(new OrganisationTypeViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Type(OrganisationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organisationType =
                    model.OrganisationTypes.SelectedValue.GetValueFromDisplayName<OrganisationTypeEnum>();

                switch (organisationType)
                {
                    case OrganisationTypeEnum.SoleTrader:
                        return RedirectToAction("SoleTraderDetails", "OrganisationRegistration");
                    case OrganisationTypeEnum.RegisteredCompany:
                        return RedirectToAction("RegisteredCompanyDetails", "OrganisationRegistration");
                    case OrganisationTypeEnum.Partnership:
                        return RedirectToAction("PartnershipDetails", "OrganisationRegistration");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult SoleTraderDetails()
        {
            return View(new SoleTraderDetailsViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SoleTraderDetails(SoleTraderDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.SoleTraderOrIndividual
                });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult PartnershipDetails()
        {
            return View(new PartnershipDetailsViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PartnershipDetails(PartnershipDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.Partnership
                });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult RegisteredCompanyDetails()
        {
            return View(new RegisteredCompanyDetailsViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RegisteredCompanyDetails(RegisteredCompanyDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    name = model.CompanyName,
                    tradingName = model.BusinessTradingName,
                    companiesRegistrationNumber = model.CompaniesRegistrationNumber,
                    type = OrganisationType.RegisteredCompany
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SelectOrganisation(string name, string tradingName, string companiesRegistrationNumber, OrganisationType type, int page = 1)
        {
            var routeValues = new { name = name, tradingName = tradingName, companiesRegistrationNumber = companiesRegistrationNumber, type = type };

            var fallbackPagingViewModel = new PagingViewModel("SelectOrganisation", "OrganisationRegistration", routeValues);
            var fallbackSelectOrganisationViewModel = BuildSelectOrganisationViewModel(name, tradingName, companiesRegistrationNumber, type,
                                new List<OrganisationSearchData>(), fallbackPagingViewModel);

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(tradingName))
            {
                ModelState.AddModelError(string.Empty, "No name or trading name supplied, unable to perform search");
                return View(fallbackSelectOrganisationViewModel);
            }

            using (var client = apiClient())
            {
                try
                {
                    const int OrganisationsPerPage = 4;
                    // would rather bake this into the db query but not really feasible

                    var organisationSearchResultData =
                        await client.SendAsync(User.GetAccessToken(), new FindMatchingOrganisations(name ?? tradingName, page, OrganisationsPerPage));

                    var pagingViewModel = PagingViewModel.FromValues(organisationSearchResultData.TotalMatchingOrganisations, OrganisationsPerPage,
                        page, "SelectOrganisation", "OrganisationRegistration", routeValues);

                    return View(BuildSelectOrganisationViewModel(name, tradingName, companiesRegistrationNumber, type,
                                    organisationSearchResultData.Results, pagingViewModel));
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return View(fallbackSelectOrganisationViewModel);
                }
            }
        }

        private SelectOrganisationViewModel BuildSelectOrganisationViewModel(string name, string tradingName, string companiesRegistrationNumber, OrganisationType type, IList<OrganisationSearchData> matchingOrganisations, PagingViewModel pagingViewModel)
        {
            return new SelectOrganisationViewModel
            {
                Name = name,
                TradingName = tradingName,
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                Type = type,
                MatchingOrganisations = matchingOrganisations,
                PagingViewModel = pagingViewModel
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectOrganisation(SelectOrganisationViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrganisation(SelectOrganisationViewModel viewModel)
        {
            using (var client = apiClient())
            {
                try
                {
                    var command = MakeOrganisationCreationRequest(
                        viewModel.Name,
                        viewModel.TradingName,
                        viewModel.CompaniesRegistrationNumber,
                        viewModel.Type);
                    var organisationId = await client.SendAsync(User.GetAccessToken(), command);
                    return RedirectToAction("MainContactPerson", new { id = organisationId });
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }

                    return RedirectToAction("Type"); // where ought this really go? redirect will eat the model errors!
                }
            }
        }

        private CreateOrganisationRequest MakeOrganisationCreationRequest(string name, string tradingName, string companiesRegistrationNumber, OrganisationType organisationType)
        {
            switch (organisationType)
            {
                case OrganisationType.RegisteredCompany:

                    return new CreateRegisteredCompanyRequest
                    {
                        BusinessName = name,
                        CompanyRegistrationNumber = companiesRegistrationNumber,
                        TradingName = tradingName
                    };

                case OrganisationType.SoleTraderOrIndividual:

                    return new CreateSoleTraderRequest
                    {
                        TradingName = tradingName
                    };

                case OrganisationType.Partnership:

                    return new CreatePartnershipRequest
                    {
                        TradingName = tradingName
                    };

                default:

                    throw new InvalidEnumArgumentException("organisationType");
            }
        }

        [HttpGet]
        public async Task<ActionResult> MainContactPerson(Guid id)
        {
            using (var client = apiClient())
            {
                /* RP: Check with the API to see if this is a valid organisation
                 * It would be annoying for a user to fill out a form only to get an error at the end, 
                 * when this could be avoided by checking the validity of the ID before the page loads */
                await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id));

                var model = new ContactPersonViewModel { OrganisationId = id };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MainContactPerson(ContactPersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    try
                    {
                        var response = await client.SendAsync(User.GetAccessToken(), model.ToAddRequest());
                        return RedirectToAction("OrganisationAddress", "OrganisationRegistration", new
                        {
                            id = model.OrganisationId
                        });
                    }
                    catch (ApiBadRequestException ex)
                    {
                        this.HandleBadRequest(ex);

                        if (ModelState.IsValid)
                        {
                            throw;
                        }
                    }

                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> OrganisationAddress(Guid id)
        {
            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id)); // Check the organisation Id is valid
                var model = new AddressViewModel
                {
                    OrganisationId = id
                };

                await this.BindUKCompetentAuthorityRegionsList(client, User);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganisationAddress(AddressViewModel model)
        {
            await this.BindUKCompetentAuthorityRegionsList(apiClient, User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var client = apiClient())
                {
                    var type = AddressType.OrganistionAddress;

                    model.Address.Country = this.GetUKRegionById(model.Address.CountryId);
                    var request = model.ToAddRequest(type);
                    var response = await client.SendAsync(User.GetAccessToken(), request);
                    return RedirectToAction("RegisteredOfficeAddress", "OrganisationRegistration", new
                    {
                        id = model.OrganisationId
                    });
                }
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);

                if (ModelState.IsValid)
                {
                    throw;
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredOfficeAddress(Guid id)
        {
            using (var client = apiClient())
            {
                var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id));
                var model = new AddressViewModel
                {
                    OrganisationId = id, 
                    OrganisationType = organisation.OrganisationType,
                };
                    
                await this.BindUKCompetentAuthorityRegionsList(client, User);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredOfficeAddress(AddressViewModel model)
        {
            await this.BindUKCompetentAuthorityRegionsList(apiClient, User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var client = apiClient())
                {
                    var type = AddressType.RegisteredorPPBAddress;
                    model.Address.Country = this.GetUKRegionById(model.Address.CountryId);
                    var request = model.ToAddRequest(type);
                    var response = await client.SendAsync(User.GetAccessToken(), request);
                    return RedirectToAction("ReviewOrganisationDetails", "OrganisationRegistration", new
                    {
                        id = model.OrganisationId
                    });
                }
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);

                if (ModelState.IsValid)
                {
                    throw;
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ReviewOrganisationDetails(Guid id)
        {
            var model = new OrganisationSummaryViewModel { OrganisationId = id };
            using (var client = apiClient())
            {
                try
                {
                    var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id));
                    model.OrganisationData = organisation;
                    return View(model);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                }
                return View(model);
            }
        }
    }
}
