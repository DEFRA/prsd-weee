namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
    using Core.Shared.Paging;
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
    public class OrganisationRegistrationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;

        public OrganisationRegistrationController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Type(Guid? organisationId = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisationExistsAndIncomplete =
                        await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExistsAndIncomplete(organisationId.Value));

                    if (!organisationExistsAndIncomplete)
                    {
                        throw new ArgumentException("No organisation found for supplied organisation Id with Incomplete status", "organisationId");
                    }

                    var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId.Value));

                    var model = new OrganisationTypeViewModel(organisation.OrganisationType, organisationId.Value);

                    return View("Type", model);
                }
            }
            return View(new OrganisationTypeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Type(OrganisationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organisationType =
                        model.OrganisationTypes.SelectedValue.GetValueFromDisplayName<OrganisationType>();

                if (model.OrganisationId != null)
                {
                    using (var client = apiClient())
                    {
                        var organisationExistsAndIncomplete =
                            await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExistsAndIncomplete(model.OrganisationId.Value));

                        if (!organisationExistsAndIncomplete)
                        {
                            throw new Exception("No organisation found for supplied organisation Id with Incomplete status");
                        }

                        switch (organisationType)
                        {
                            case OrganisationType.SoleTraderOrIndividual:
                                return RedirectToAction("SoleTraderDetails", "OrganisationRegistration", new { organisationId = model.OrganisationId.Value });
                            case OrganisationType.RegisteredCompany:
                                return RedirectToAction("RegisteredCompanyDetails", "OrganisationRegistration", new { organisationId = model.OrganisationId.Value });
                            case OrganisationType.Partnership:
                                return RedirectToAction("PartnershipDetails", "OrganisationRegistration", new { organisationId = model.OrganisationId.Value });
                        }
                    }
                }
                else
                {
                    switch (organisationType)
                    {
                        case OrganisationType.SoleTraderOrIndividual:
                            return RedirectToAction("SoleTraderDetails", "OrganisationRegistration");
                        case OrganisationType.RegisteredCompany:
                            return RedirectToAction("RegisteredCompanyDetails", "OrganisationRegistration");
                        case OrganisationType.Partnership:
                            return RedirectToAction("PartnershipDetails", "OrganisationRegistration");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderDetails(Guid? organisationId = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisationExistsAndIncomplete =
                        await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExistsAndIncomplete(organisationId.Value));

                    if (!organisationExistsAndIncomplete)
                    {
                        throw new ArgumentException("No organisation found for supplied organisation Id with Incomplete status", "organisationId");
                    }

                    var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId.Value));

                    if (organisation.OrganisationType == OrganisationType.SoleTraderOrIndividual)
                    {
                        var model = new SoleTraderDetailsViewModel
                        {
                            BusinessTradingName = organisation.TradingName,
                            OrganisationId = organisationId.Value
                        };

                        return View("SoleTraderDetails", model);
                    }
                    return View(new SoleTraderDetailsViewModel());
                }
            }
            return View(new SoleTraderDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SoleTraderDetails(SoleTraderDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrganisationId != null)
                {
                    return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                    {
                        tradingName = model.BusinessTradingName,
                        type = OrganisationType.SoleTraderOrIndividual,
                        organisationId = model.OrganisationId.Value
                    });
                }
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.SoleTraderOrIndividual
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> PartnershipDetails(Guid? organisationId = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisationExistsAndIncomplete =
                        await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExistsAndIncomplete(organisationId.Value));

                    if (!organisationExistsAndIncomplete)
                    {
                        throw new ArgumentException("No organisation found for supplied organisation Id with Incomplete status", "organisationId");
                    }

                    var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId.Value));
                    if (organisation.OrganisationType == OrganisationType.Partnership)
                    {
                        var model = new PartnershipDetailsViewModel
                        {
                            BusinessTradingName = organisation.TradingName,
                            OrganisationId = organisationId.Value
                        };

                        return View("PartnershipDetails", model);
                    }
                    return View(new PartnershipDetailsViewModel());
                }
            }
            return View(new PartnershipDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PartnershipDetails(PartnershipDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrganisationId != null)
                {
                    return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                    {
                        tradingName = model.BusinessTradingName,
                        type = OrganisationType.Partnership,
                        organisationId = model.OrganisationId.Value
                    });
                }
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.Partnership
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails(Guid? organisationId = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisationExistsAndIncomplete =
                        await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExistsAndIncomplete(organisationId.Value));

                    if (!organisationExistsAndIncomplete)
                    {
                        throw new ArgumentException("No organisation found for supplied organisation Id with Incomplete status", "organisationId");
                    }

                    var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId.Value));

                    if (organisation.OrganisationType == OrganisationType.RegisteredCompany)
                    {
                        var model = new RegisteredCompanyDetailsViewModel
                        {
                            BusinessTradingName = organisation.TradingName,
                            CompanyName = organisation.Name,
                            CompaniesRegistrationNumber = organisation.CompanyRegistrationNumber,
                            OrganisationId = organisationId.Value
                        };

                        return View("RegisteredCompanyDetails", model);
                    }
                    return View(new RegisteredCompanyDetailsViewModel());
                }
            }
            return View(new RegisteredCompanyDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisteredCompanyDetails(RegisteredCompanyDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrganisationId != null)
                {
                    return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                    {
                        name = model.CompanyName,
                        tradingName = model.BusinessTradingName,
                        companiesRegistrationNumber = model.CompaniesRegistrationNumber,
                        type = OrganisationType.RegisteredCompany,
                        organisationId = model.OrganisationId.Value
                    });
                }
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
            string companiesRegistrationNumber, OrganisationType type, Guid? organisationId = null, int page = 1)
        {
            var fallbackSelectOrganisationViewModel = BuildSelectOrganisationViewModel(name, tradingName,
                companiesRegistrationNumber, type, organisationId,
                new PagedList<OrganisationSearchData>());

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

                    var model = BuildSelectOrganisationViewModel(name, tradingName, companiesRegistrationNumber, type,
                        organisationId,
                        organisationSearchResultData.Results.ToPagedList(page - 1, OrganisationsPerPage,
                            organisationSearchResultData.TotalMatchingOrganisations));
                    return View(model);
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
            string companiesRegistrationNumber, OrganisationType type, Guid? organisationId,
            IPagedList<OrganisationSearchData> matchingOrganisations)
        {
            return new SelectOrganisationViewModel
            {
                Name = name,
                TradingName = tradingName,
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                Type = type,
                MatchingOrganisations = matchingOrganisations,
                OrganisationId = organisationId
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectOrganisation(SelectOrganisationViewModel model, string submitButton)
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
                    if (viewModel.OrganisationId != null)
                    {
                        var orgId = await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationTypeDetails(viewModel.OrganisationId.Value, viewModel.Type, viewModel.Name, viewModel.TradingName, viewModel.CompaniesRegistrationNumber));
                        return RedirectToAction("MainContactPerson", new { id = orgId });
                    }
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
                var organisationExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(id));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "id");
                }

                ContactPersonViewModel model;
                var contactPerson = await client.SendAsync(User.GetAccessToken(), new GetContactPersonByOrganisationId(id));
                if (contactPerson.HasContact)
                {
                    model = new ContactPersonViewModel(contactPerson);
                }
                else
                {
                    model = new ContactPersonViewModel { OrganisationId = id };
                }

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
                    catch (ApiBadRequestException ex)
                    {
                        this.HandleBadRequest(ex);

                        if (ModelState.IsValid)
                        {
                            throw;
                        }
                    }
                    catch (ApiException ex)
                    {
                        if (ex.ErrorData != null)
                        {
                            ModelState.AddModelError("Unable to save the address.", ex.Message);
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
                var model = await GetAddressViewModel(id, client, false, AddressType.OrganisationAddress);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganisationAddress(AddressViewModel viewModel)
        {
            viewModel.Address.Countries = await GetCountries(false);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                using (var client = apiClient())
                {
                    await AddAddressToOrganisation(viewModel, AddressType.OrganisationAddress, client);

                    var isUkAddress = await client.SendAsync(
                        User.GetAccessToken(),
                        new IsUkOrganisationAddress(viewModel.OrganisationId));

                    return
                        RedirectToAction(
                            isUkAddress ? "RegisteredOfficeAddressPrepopulate" : "RegisteredOfficeAddress",
                            "OrganisationRegistration", new { id = viewModel.OrganisationId });
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
            return View(viewModel);
        }

        [HttpGet]
        public async Task<ViewResult> RegisteredOfficeAddressPrepopulate(Guid id)
        {
            using (var client = apiClient())
            {
                return View(await GetAddressPrepopulateViewModel(id, client));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredOfficeAddressPrepopulate(AddressPrepopulateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ContactDetailsSameAs.Choices.SelectedValue == "No")
                {
                    return RedirectToAction("RegisteredOfficeAddress", new { id = viewModel.OrganisationId });
                }
                if (viewModel.ContactDetailsSameAs.Choices.SelectedValue == "Yes")
                {
                    using (var client = apiClient())
                    {
                        await
                            client.SendAsync(User.GetAccessToken(),
                                new CopyOrganisationAddressIntoRegisteredOffice(viewModel.OrganisationId));
                    }

                    return RedirectToAction("ReviewOrganisationDetails", new { id = viewModel.OrganisationId });
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredOfficeAddress(Guid id)
        {
            using (var client = apiClient())
            {
                var model = await GetAddressViewModel(id, client, true, AddressType.RegisteredOrPPBAddress);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredOfficeAddress(AddressViewModel viewModel)
        {
            viewModel.Address.Countries = await GetCountries(true);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                using (var client = apiClient())
                {
                    await AddAddressToOrganisation(viewModel, AddressType.RegisteredOrPPBAddress, client);
                    return RedirectToAction("ReviewOrganisationDetails", "OrganisationRegistration", new
                    {
                        id = viewModel.OrganisationId
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
            catch (ApiException ex)
            {
                if (ex.ErrorData != null)
                {
                    ModelState.AddModelError("Unable to save the address.", ex.Message);
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
                var organisationExists =
                   await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(id));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "id");
                }

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
                return View("ReviewOrganisationDetails", model);
            }
            try
            {
                using (var client = apiClient())
                {
                    await
                        client.SendAsync(User.GetAccessToken(),
                            new CompleteRegistration(model.OrganisationData.Id));
                }
                return RedirectToAction("RedirectProcess", "Account");
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);

                if (ModelState.IsValid)
                {
                    throw;
                }
            }
            return View("ReviewOrganisationDetails", model);
        }

        private async Task<AddressViewModel> GetAddressViewModel(Guid organisationId, IWeeeClient client, bool regionsOfUKOnly, AddressType addressType)
        {
            // Check the organisation Id is valid
            var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId));
            var model = new AddressViewModel
            {
                OrganisationId = organisationId,
                OrganisationType = organisation.OrganisationType,
            };

            if (addressType == AddressType.OrganisationAddress)
            {
                if (organisation.HasOrganisationAddress)
                {
                    model.Address = organisation.OrganisationAddress;
                }
            }
            else if (addressType == AddressType.RegisteredOrPPBAddress)
            {
                if (organisation.HasBusinessAddress)
                {
                    model.Address = organisation.BusinessAddress;
                }
            }
            else if (addressType == AddressType.ServiceOfNotice)
            {
                if (organisation.HasNotificationAddress)
                {
                    model.Address = organisation.NotificationAddress;
                }
            }

            model.Address.Countries = await GetCountries(regionsOfUKOnly);
            return model;
        }

        private async Task<AddressPrepopulateViewModel> GetAddressPrepopulateViewModel(Guid id, IWeeeClient client)
        {
            var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id));
            var model = new AddressPrepopulateViewModel
            {
                OrganisationId = id,
                OrganisationType = organisation.OrganisationType,
                ContactDetailsSameAs = new YesNoChoiceViewModel()
            };

            return model;
        }

        private async Task AddAddressToOrganisation(AddressViewModel model, AddressType type, IWeeeClient client)
        {
            var request = model.ToAddRequest(type);
            await client.SendAsync(User.GetAccessToken(), request);
        }

        private async Task<IEnumerable<CountryData>> GetCountries(bool regionsOfUKOnly)
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(regionsOfUKOnly));
            }
        }
    }
}
