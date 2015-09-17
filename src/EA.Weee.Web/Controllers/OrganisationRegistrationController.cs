namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
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
        private const string NoSearchAnotherOrganisation = "No - search for another organisation";

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
                                return RedirectToAction("SoleTraderDetails", new { organisationId = model.OrganisationId.Value });
                            case OrganisationType.RegisteredCompany:
                                return RedirectToAction("RegisteredCompanyDetails", new { organisationId = model.OrganisationId.Value });
                            case OrganisationType.Partnership:
                                return RedirectToAction("PartnershipDetails", new { organisationId = model.OrganisationId.Value });
                        }
                    }
                }
                else
                {
                    switch (organisationType)
                    {
                        case OrganisationType.SoleTraderOrIndividual:
                            return RedirectToAction("SoleTraderDetails");
                        case OrganisationType.RegisteredCompany:
                            return RedirectToAction("RegisteredCompanyDetails");
                        case OrganisationType.Partnership:
                            return RedirectToAction("PartnershipDetails");
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.OrganisationId != null)
            {
                return RedirectToAction("SelectOrganisation", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.SoleTraderOrIndividual,
                    organisationId = model.OrganisationId.Value
                });
            }
            else
            {
                return RedirectToAction("SelectOrganisation", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.SoleTraderOrIndividual
                });
            }
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.OrganisationId != null)
            {
                return RedirectToAction("SelectOrganisation", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.Partnership,
                    organisationId = model.OrganisationId.Value
                });
            }
            else
            {
                return RedirectToAction("SelectOrganisation", new
                {
                    tradingName = model.BusinessTradingName,
                    type = OrganisationType.Partnership
                });
            }
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.OrganisationId != null)
            {
                return RedirectToAction("SelectOrganisation", new
                {
                    name = model.CompanyName,
                    tradingName = model.BusinessTradingName,
                    companiesRegistrationNumber = model.CompaniesRegistrationNumber,
                    type = OrganisationType.RegisteredCompany,
                    organisationId = model.OrganisationId.Value
                });
            }
            else
            {
                return RedirectToAction("SelectOrganisation", new
                {
                    name = model.CompanyName,
                    tradingName = model.BusinessTradingName,
                    companiesRegistrationNumber = model.CompaniesRegistrationNumber,
                    type = OrganisationType.RegisteredCompany
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> SelectOrganisation(string name, string tradingName,
            string companiesRegistrationNumber, OrganisationType type, Guid? organisationId = null)
        {
            var selectOrganisationViewModel = BuildSelectOrganisationViewModel(name, tradingName,
                companiesRegistrationNumber, type, organisationId,
                new SelectOrganisationRadioButtons());

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(tradingName))
            {
                ModelState.AddModelError(string.Empty, "No name or trading name supplied, unable to perform search");
                return View(selectOrganisationViewModel);
            }

            using (var client = apiClient())
            {
                try
                {
                    var organisationSearchResultData =
                        await
                            client.SendAsync(User.GetAccessToken(),
                                new FindMatchingOrganisations(name ?? tradingName));

                    if (organisationSearchResultData.TotalMatchingOrganisations == 0)
                    {
                        return RedirectToAction("NotFoundOrganisation", new
                        {
                            name,
                            tradingName,
                            companiesRegistrationNumber,
                            type,
                            organisationId,
                        });
                    }

                    var orgsKeyValuePairs =
                            organisationSearchResultData.Results.ToList().Select(
                            o => new KeyValuePair<string, string>(o.DisplayName, o.Id.ToString()));

                    orgsKeyValuePairs = orgsKeyValuePairs.Concat(new[]
                    {
                        new KeyValuePair<string, string>(SelectOrganisationAction.CreateNewOrg, SelectOrganisationAction.CreateNewOrg), 
                        new KeyValuePair<string, string>(SelectOrganisationAction.TryAnotherSearch, SelectOrganisationAction.TryAnotherSearch)
                    });

                    var orgRadioButtons = new SelectOrganisationRadioButtons(orgsKeyValuePairs);

                    var model = BuildSelectOrganisationViewModel(name, tradingName, companiesRegistrationNumber,
                        type, organisationId, orgRadioButtons);

                    return View(model);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return View(selectOrganisationViewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectOrganisation(SelectOrganisationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                if (model.Organisations.SelectedValue == SelectOrganisationAction.TryAnotherSearch)
                {
                    if (model.OrganisationId != null)
                    {
                        return RedirectToAction("Type", "OrganisationRegistration", new { model.OrganisationId });
                    }
                    return RedirectToAction("Type", "OrganisationRegistration");
                }

                if (model.Organisations.SelectedValue == SelectOrganisationAction.CreateNewOrg)
                {
                    if (model.OrganisationId != null)
                    {
                        UpdateOrganisationTypeDetails request = new UpdateOrganisationTypeDetails(
                            model.OrganisationId.Value,
                            model.Type,
                            model.Name,
                            model.TradingName,
                            model.CompaniesRegistrationNumber);

                        Guid organisationId = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectToAction("MainContactPerson", new { organisationId });
                    }
                    else
                    {
                        CreateOrganisationRequest request = MakeOrganisationCreationRequest(
                            model.Name,
                            model.TradingName,
                            model.CompaniesRegistrationNumber,
                            model.Type);

                        Guid organisationId = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectToAction("MainContactPerson", new { organisationId });
                    }
                }
            }
            var selectedOrgId = new Guid(model.Organisations.SelectedValue);
            return RedirectToAction("JoinOrganisation", "OrganisationRegistration",
                new { OrganisationID = selectedOrgId });
        }

        [HttpGet]
        public ActionResult NotFoundOrganisation(string name, string tradingName,
                            string companiesRegistrationNumber,
                            OrganisationType type, Guid? organisationId = null)
        {
            var model = new NotFoundOrganisationViewModel
            {
                SearchedText = name ?? tradingName,
                Name = name,
                TradingName = tradingName,
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                Type = type,
                OrganisationId = organisationId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NotFoundOrganisation(NotFoundOrganisationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (var client = apiClient())
            {
                if (model.ActivityOptions.SelectedValue == NotFoundOrganisationAction.TryAnotherSearch)
                {
                    if (model.OrganisationId != null)
                    {
                        return RedirectToAction("Type", "OrganisationRegistration", new { model.OrganisationId });
                    }
                    return RedirectToAction("Type", "OrganisationRegistration");
                }

                if (model.ActivityOptions.SelectedValue == NotFoundOrganisationAction.CreateNewOrg)
                {
                    if (model.OrganisationId != null)
                    {
                        UpdateOrganisationTypeDetails request = new UpdateOrganisationTypeDetails(
                            model.OrganisationId.Value,
                            model.Type,
                            model.Name,
                            model.TradingName,
                            model.CompaniesRegistrationNumber);

                        Guid organisationId = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectToAction("MainContactPerson", new { organisationId });
                    }
                    else
                    {
                        var request = MakeOrganisationCreationRequest(
                            model.Name,
                            model.TradingName,
                            model.CompaniesRegistrationNumber,
                            model.Type);

                        var organisationId = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectToAction("MainContactPerson", new { organisationId });
                    }
                }
            }
            return View(model);
        }

        private SelectOrganisationViewModel BuildSelectOrganisationViewModel(string name, string tradingName,
            string companiesRegistrationNumber, OrganisationType type, Guid? organisationId,
            SelectOrganisationRadioButtons organisationRadioButtons)
        {
            return new SelectOrganisationViewModel
            {
                Name = name,
                TradingName = tradingName,
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                SearchedText = name ?? tradingName,
                Type = type,
                OrganisationId = organisationId,
                Organisations = organisationRadioButtons
            };
        }

        [HttpGet]
        public async Task<ViewResult> JoinOrganisation(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(
                    User.GetAccessToken(),
                    new GetPublicOrganisationInfo(organisationId));

                var collection = new List<string> { "Yes - join " + organisationData.DisplayName, NoSearchAnotherOrganisation };
                var model = new JoinOrganisationViewModel
                {
                    OrganisationId = organisationId,
                    JoinOrganisationOptions = new RadioButtonStringCollectionViewModel
                    {
                        PossibleValues = collection
                    }
                };
                return View(model);
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

            if (viewModel.JoinOrganisationOptions.SelectedValue == NoSearchAnotherOrganisation)
            {
                return RedirectToAction("Type", "OrganisationRegistration");
            }

            using (var client = apiClient())
            {
                try
                {
                    await
                        client.SendAsync(
                            User.GetAccessToken(),
                            new JoinOrganisation(viewModel.OrganisationId));
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

                return RedirectToAction("JoinOrganisationConfirmation", new { organisationId = viewModel.OrganisationId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> JoinOrganisationConfirmation(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(
                    User.GetAccessToken(),
                    new GetPublicOrganisationInfo(organisationId));

                var model = new JoinOrganisationConfirmationViewModel()
                {
                    OrganisationName = organisationData.DisplayName
                };

                return View(model);
            }
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
                        UpdateOrganisationTypeDetails request = new UpdateOrganisationTypeDetails(
                            viewModel.OrganisationId.Value,
                            viewModel.Type,
                            viewModel.Name,
                            viewModel.TradingName,
                            viewModel.CompaniesRegistrationNumber);

                        Guid organisationId = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectToAction("MainContactPerson", new { organisationId });
                    }
                    else
                    {
                        CreateOrganisationRequest request = MakeOrganisationCreationRequest(
                            viewModel.Name,
                            viewModel.TradingName,
                            viewModel.CompaniesRegistrationNumber,
                            viewModel.Type);

                        Guid organisationId = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectToAction("MainContactPerson", new { organisationId });
                    }
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
        public async Task<ActionResult> MainContactPerson(Guid organisationId)
        {
            using (var client = apiClient())
            {
                /* RP: Check with the API to see if this is a valid organisation
                 * It would be annoying for a user to fill out a form only to get an error at the end, 
                 * when this could be avoided by checking the validity of the ID before the page loads */
                var organisationExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(organisationId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                ContactPersonViewModel model;
                var contactPerson = await client.SendAsync(User.GetAccessToken(), new GetContactPersonByOrganisationId(organisationId));
                if (contactPerson.HasContact)
                {
                    model = new ContactPersonViewModel(contactPerson);
                }
                else
                {
                    model = new ContactPersonViewModel { OrganisationId = organisationId };
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
                        return RedirectToAction("OrganisationAddress", new
                        {
                            viewModel.OrganisationId
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
        public async Task<ActionResult> OrganisationAddress(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var model = await GetAddressViewModel(organisationId, client, false, AddressType.OrganisationAddress);

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
                            new { viewModel.OrganisationId });
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
        public async Task<ViewResult> RegisteredOfficeAddressPrepopulate(Guid organisationId)
        {
            using (var client = apiClient())
            {
                return View(await GetAddressPrepopulateViewModel(organisationId, client));
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
                    return RedirectToAction("RegisteredOfficeAddress", new { viewModel.OrganisationId });
                }
                if (viewModel.ContactDetailsSameAs.Choices.SelectedValue == "Yes")
                {
                    using (var client = apiClient())
                    {
                        await
                            client.SendAsync(User.GetAccessToken(),
                                new CopyOrganisationAddressIntoRegisteredOffice(viewModel.OrganisationId));
                    }

                    return RedirectToAction("ReviewOrganisationDetails", new { viewModel.OrganisationId });
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredOfficeAddress(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var model = await GetAddressViewModel(organisationId, client, true, AddressType.RegisteredOrPPBAddress);
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
                    return RedirectToAction("ReviewOrganisationDetails", new
                    {
                        viewModel.OrganisationId
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
        public async Task<ActionResult> ReviewOrganisationDetails(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var organisationExists =
                   await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(organisationId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                OrganisationData organisationData = await client.SendAsync(
                    User.GetAccessToken(),
                    new GetOrganisationInfo(organisationId));

                var model = new OrganisationSummaryViewModel()
                {
                    OrganisationData = organisationData,
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmOrganisationDetails(OrganisationSummaryViewModel model, Guid organisationId)
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
                            new CompleteRegistration(organisationId));
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

            return await ReviewOrganisationDetails(organisationId);
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

        private async Task<AddressPrepopulateViewModel> GetAddressPrepopulateViewModel(Guid organisationId, IWeeeClient client)
        {
            var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId));
            var model = new AddressPrepopulateViewModel
            {
                OrganisationId = organisationId,
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
