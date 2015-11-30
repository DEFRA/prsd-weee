namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
    using EA.Weee.Core.Search;
    using Infrastructure;
    using Prsd.Core.Extensions;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.Details;
    using ViewModels.OrganisationRegistration.Type;
    using ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Weee.Requests.Organisations.Create;
    using Weee.Requests.Shared;

    [Authorize]
    public class OrganisationRegistrationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private const int maximumSearchResults = 5;

        public OrganisationRegistrationController(Func<IWeeeClient> apiClient, ISearcher<OrganisationSearchResult> organisationSearcher)
        {
            this.apiClient = apiClient;
            this.organisationSearcher = organisationSearcher;
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
                return RedirectToAction("JoinOrganisation", new
                {
                    OrganisationId = viewModel.SelectedOrganisationId.Value
                });
            }

            return RedirectToAction("SearchResults", new { viewModel.SearchTerm });
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

            return RedirectToAction("JoinOrganisation", new
            {
                OrganisationId = viewModel.SelectedOrganisationId.Value
            });
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
        public async Task<ActionResult> Type(string searchedText, Guid? organisationId = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisation = await GetOrganisation(organisationId, client);
                    var model = new OrganisationTypeViewModel(organisationId.Value);
                    return View("Type", model);
                }
            }

            return View(new OrganisationTypeViewModel(searchedText));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Type(OrganisationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organisationType =
                        model.SelectedValue.GetValueFromDisplayName<OrganisationType>();

                if (model.OrganisationId != null)
                {
                    using (var client = apiClient())
                    {
                        await GetOrganisation(model.OrganisationId.Value, client);

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
                            return RedirectToAction("SoleTraderDetails", new { searchedText = model.SearchedText });
                        case OrganisationType.RegisteredCompany:
                            return RedirectToAction("RegisteredCompanyDetails", new { searchedText = model.SearchedText });
                        case OrganisationType.Partnership:
                            return RedirectToAction("PartnershipDetails", new { searchedText = model.SearchedText });
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SoleTraderDetails(Guid? organisationId = null, string searchedText = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisation = await GetOrganisation(organisationId, client);

                    if (organisation.OrganisationType == OrganisationType.SoleTraderOrIndividual)
                    {
                        var model = new SoleTraderDetailsViewModel
                        {
                            BusinessTradingName = organisation.TradingName,
                            OrganisationId = organisationId.Value
                        };

                        return View("SoleTraderDetails", model);
                    }
                    return View(new SoleTraderDetailsViewModel { BusinessTradingName = searchedText });
                }
            }
            return View(new SoleTraderDetailsViewModel { BusinessTradingName = searchedText });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SoleTraderDetails(SoleTraderDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                if (model.OrganisationId != null)
                {
                    // update orgnisation details
                    UpdateOrganisationTypeDetails updateRequest = new UpdateOrganisationTypeDetails(
                        model.OrganisationId.Value,
                        OrganisationType.SoleTraderOrIndividual,
                        String.Empty,
                        model.BusinessTradingName,
                        String.Empty);

                    Guid organisationId = await client.SendAsync(User.GetAccessToken(), updateRequest);
                    return RedirectToAction("MainContactPerson", new { organisationId });
                }

                CreateSoleTraderRequest request = new CreateSoleTraderRequest
                {
                    TradingName = model.BusinessTradingName
                };
                //create the organisation only if does not exist
                Guid orgId = await client.SendAsync(User.GetAccessToken(), request);
                return RedirectToAction("MainContactPerson", new { organisationId = orgId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> PartnershipDetails(Guid? organisationId = null, string searchedText = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisation = await GetOrganisation(organisationId, client);
                    if (organisation.OrganisationType == OrganisationType.Partnership)
                    {
                        var model = new PartnershipDetailsViewModel
                        {
                            BusinessTradingName = organisation.TradingName,
                            OrganisationId = organisationId.Value
                        };

                        return View("PartnershipDetails", model);
                    }
                    return View(new PartnershipDetailsViewModel { BusinessTradingName = searchedText });
                }
            }
            return View(new PartnershipDetailsViewModel { BusinessTradingName = searchedText });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PartnershipDetails(PartnershipDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                if (model.OrganisationId != null)
                {
                    // update orgnisation details
                    UpdateOrganisationTypeDetails updateRequest = new UpdateOrganisationTypeDetails(
                        model.OrganisationId.Value,
                        OrganisationType.Partnership,
                        string.Empty,
                        model.BusinessTradingName,
                        String.Empty);

                    Guid organisationId = await client.SendAsync(User.GetAccessToken(), updateRequest);
                    return RedirectToAction("MainContactPerson", new { organisationId });
                }

                CreatePartnershipRequest request = new CreatePartnershipRequest
                {
                    TradingName = model.BusinessTradingName
                };
                //create the organisation only if does not exist
                Guid orgId = await client.SendAsync(User.GetAccessToken(), request);
                return RedirectToAction("MainContactPerson", new { organisationId = orgId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> RegisteredCompanyDetails(Guid? organisationId = null, string searchedText = null)
        {
            if (organisationId != null)
            {
                using (var client = apiClient())
                {
                    var organisation = await GetOrganisation(organisationId, client);

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
                    return View(new RegisteredCompanyDetailsViewModel { CompanyName = searchedText });
                }
            }
            return View(new RegisteredCompanyDetailsViewModel { CompanyName = searchedText });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredCompanyDetails(RegisteredCompanyDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (var client = apiClient())
            {
                if (model.OrganisationId != null)
                {
                    // update orgnisation details
                    UpdateOrganisationTypeDetails updateRequest = new UpdateOrganisationTypeDetails(
                        model.OrganisationId.Value,
                        OrganisationType.RegisteredCompany,
                        model.CompanyName,
                        model.BusinessTradingName,
                        model.CompaniesRegistrationNumber);

                    Guid organisationId = await client.SendAsync(User.GetAccessToken(), updateRequest);
                    return RedirectToAction("MainContactPerson", new { organisationId });
                }

                CreateRegisteredCompanyRequest request = new CreateRegisteredCompanyRequest
                {
                    BusinessName = model.CompanyName,
                    CompanyRegistrationNumber = model.CompaniesRegistrationNumber,
                    TradingName = model.BusinessTradingName
                };
                //create the organisation only if does not exist
                Guid orgId = await client.SendAsync(User.GetAccessToken(), request);
                return RedirectToAction("MainContactPerson", new { organisationId = orgId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> JoinOrganisation(Guid organisationId)
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

                if (existingAssociation != null)
                {
                    UserAlreadyAssociatedWithOrganisationViewModel viewModel = new UserAlreadyAssociatedWithOrganisationViewModel()
                    {
                        OrganisationId = organisationId,
                        OrganisationName = organisationData.DisplayName,
                        Status = existingAssociation.UserStatus
                    };

                    return View("UserAlreadyAssociatedWithOrganisation", viewModel);
                }
                else
                {
                    var model = new JoinOrganisationViewModel
                    {
                        OrganisationId = organisationId,
                        OrganisationName = organisationData.DisplayName
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

                    return
                        RedirectToAction("RegisteredOfficeAddressPrepopulate",
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
                if (viewModel.SelectedValue == "No")
                {
                    return RedirectToAction("RegisteredOfficeAddress", new { viewModel.OrganisationId });
                }
                if (viewModel.SelectedValue == "Yes")
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
                var model = await GetAddressViewModel(organisationId, client, false, AddressType.RegisteredOrPPBAddress);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisteredOfficeAddress(AddressViewModel viewModel)
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

                return RedirectToAction("Confirmation", new
                {
                    organisationName = model.OrganisationData.OrganisationType == OrganisationType.RegisteredCompany ? model.OrganisationData.Name : model.OrganisationData.TradingName
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

            return await ReviewOrganisationDetails(organisationId);
        }

        [HttpGet]
        public ActionResult Confirmation(string organisationName)
        {
            return View((object)organisationName);
        }

        [HttpGet]
        public ActionResult CreateGuidance(string searchedText)
        {
            return View((object)searchedText);
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

        private async Task<OrganisationData> GetOrganisation(Guid? organisationId, IWeeeClient client)
        {
            var organisationExistsAndIncomplete =
                await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExistsAndIncomplete(organisationId.Value));

            if (!organisationExistsAndIncomplete)
            {
                throw new ArgumentException("No organisation found for supplied organisation Id with Incomplete status",
                    "organisationId");
            }

            var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId.Value));
            return organisation;
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
    }
}
