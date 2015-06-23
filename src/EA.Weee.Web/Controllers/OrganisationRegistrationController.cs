namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using Prsd.Core.Extensions;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using ViewModels.JoinOrganisation;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.Details;
    using ViewModels.OrganisationRegistration.Type;
    using ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Weee.Requests.Organisations.Create;
    using Weee.Requests.Organisations.Create.Base;
    using Weee.Requests.Shared;

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
        public async Task<ActionResult> SelectOrganisation(string name, string tradingName,
            string companiesRegistrationNumber, OrganisationType type, int page = 1)
        {
            var routeValues = new { name, tradingName, companiesRegistrationNumber, type };

            var fallbackPagingViewModel = new PagingViewModel("SelectOrganisation", "OrganisationRegistration",
                routeValues);
            var fallbackSelectOrganisationViewModel = BuildSelectOrganisationViewModel(name, tradingName,
                companiesRegistrationNumber, type,
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
                        await
                            client.SendAsync(User.GetAccessToken(),
                                new FindMatchingOrganisations(name ?? tradingName, page, OrganisationsPerPage));

                    var pagingViewModel =
                        PagingViewModel.FromValues(organisationSearchResultData.TotalMatchingOrganisations,
                            OrganisationsPerPage,
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

        private SelectOrganisationViewModel BuildSelectOrganisationViewModel(string name, string tradingName,
            string companiesRegistrationNumber, OrganisationType type,
            IList<OrganisationSearchData> matchingOrganisations, PagingViewModel pagingViewModel)
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
        public async Task<ActionResult> SelectOrganisation(SelectOrganisationViewModel model, string submitButton)
        {
            return RedirectToAction("JoinOrganisation", new { organisationId = Guid.Parse(submitButton) });
        }

        [HttpGet]
        public async Task<ViewResult> JoinOrganisation(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(organisationId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                return View(new JoinOrganisationViewModel { OrganisationToJoin = organisationId });
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

            using (var client = apiClient())
            {
                try
                {
                    await
                        client.SendAsync(
                            User.GetAccessToken(),
                            new JoinOrganisation(viewModel.OrganisationToJoin));
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

                return RedirectToAction("JoinOrganisationConfirmation");
            }
        }

        [HttpGet]
        public ViewResult JoinOrganisationConfirmation()
        {
            return View();
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

        private CreateOrganisationRequest MakeOrganisationCreationRequest(string name, string tradingName,
            string companiesRegistrationNumber, OrganisationType organisationType)
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
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(id));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "id");
                }
                var model = new ContactPersonViewModel { OrganisationId = id };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MainContactPerson(ContactPersonViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    try
                    {
                        await client.SendAsync(User.GetAccessToken(), viewModel.ToAddRequest());
                        return RedirectToAction("OrganisationAddress", "OrganisationRegistration", new
                        {
                            id = viewModel.OrganisationId
                        });
                    }
                    catch (ApiException ex)
                    {
                        if (ex.ErrorData != null)
                        {
                            ModelState.AddModelError(string.Empty, ex.ErrorData.ExceptionMessage);
                            return View(viewModel);
                        }
                    }

                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> OrganisationAddress(Guid id)
        {
            using (var client = apiClient())
            {
                var model = await GetAddressViewModel(id, client);
                await this.BindCountriesList(apiClient, User);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganisationAddress(AddressViewModel viewModel)
        {
            await this.BindCountriesList(apiClient, User);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                using (var client = apiClient())
                {
                    await AddAddressToOrganisation(viewModel, AddressType.OrganistionAddress, client);
                    return RedirectToAction("RegisteredOfficeAddress", "OrganisationRegistration", new
                    {
                        id = viewModel.OrganisationId
                    });
                }
            }
            catch (ApiException ex)
            {
                if (ex.ErrorData != null)
                {
                    ModelState.AddModelError(string.Empty, ex.ErrorData.ExceptionMessage);
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredOfficeAddress(Guid id)
        {
            using (var client = apiClient())
            {
                var model = await GetAddressViewModel(id, client);
                await this.BindUKRegionsList(client, User);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredOfficeAddress(AddressViewModel viewModel)
        {
            await this.BindUKRegionsList(apiClient, User);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                using (var client = apiClient())
                {
                    await AddAddressToOrganisation(viewModel, AddressType.RegisteredorPPBAddress, client);
                    return RedirectToAction("ReviewOrganisationDetails", "OrganisationRegistration", new
                    {
                        id = viewModel.OrganisationId
                    });
                }
            }
            catch (ApiException ex)
            {
                if (ex.ErrorData != null)
                {
                    ModelState.AddModelError(string.Empty, ex.ErrorData.ExceptionMessage);
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> ReviewOrganisationDetails(Guid id)
        {
            var model = new OrganisationSummaryViewModel();
            using (var client = apiClient())
            {
                var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id));
                model.OrganisationData = organisation;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmOrganisationDetails(OrganisationSummaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                using (var client = apiClient())
                {
                    await
                        client.SendAsync(User.GetAccessToken(),
                            new CompleteRegistration(model.OrganisationData.Id));
                }
                return RedirectToAction("Index", "Home");
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

        private async Task<AddressViewModel> GetAddressViewModel(Guid organisationId, IWeeeClient client)
        {
            var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId));// Check the organisation Id is valid
            var model = new AddressViewModel
            {
                OrganisationId = organisationId,
                OrganisationType = organisation.OrganisationType
            };
            return model;
        }

        private async Task AddAddressToOrganisation(AddressViewModel model, AddressType type, IWeeeClient client)
        {
            model.Address.Country = this.GetCountrybyId(model.Address.CountryId);
            var request = model.ToAddRequest(type);
            await client.SendAsync(User.GetAccessToken(), request);
        }
    }
}