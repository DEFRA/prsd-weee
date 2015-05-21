namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Requests.Notification;
    using Requests.Registration;
    using ViewModels.NotificationApplication;
    using ViewModels.Shared;

    [Authorize]
    public class ImporterController : Controller
    {
        private readonly Func<IIwsClient> apiClient;

        public ImporterController(Func<IIwsClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Add(Guid id)
        {
            var model = new ImporterViewModel();
            var address = new AddressViewModel { Countries = await GetCountries() };
            var business = new BusinessViewModel();

            model.NotificationId = id;
            model.AddressDetails = address;
            model.BusinessViewModel = business;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(ImporterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AddressDetails.Countries = await GetCountries();
                return View(model);
            }

            var importerData = new CreateImporter
            {
                NotificationId = model.NotificationId,

                Name = model.BusinessViewModel.Name,
                Type = model.BusinessViewModel.EntityType,
                RegistrationNumber = model.BusinessViewModel.CompaniesHouseRegistrationNumber ??
                    (model.BusinessViewModel.SoleTraderRegistrationNumber ?? model.BusinessViewModel.PartnershipRegistrationNumber),
                AdditionalRegistrationNumber = model.BusinessViewModel.AdditionalRegistrationNumber,

                Building = model.AddressDetails.Building,
                Address1 = model.AddressDetails.Address1,
                Address2 = model.AddressDetails.Address2,
                TownOrCity = model.AddressDetails.TownOrCity,
                County = model.AddressDetails.County,
                PostalCode = model.AddressDetails.Postcode,
                CountryId = model.AddressDetails.CountryId,

                FirstName = model.ContactDetails.FirstName,
                LastName = model.ContactDetails.LastName,
                Phone = model.ContactDetails.Telephone,
                Fax = model.ContactDetails.Fax,
                Email = model.ContactDetails.Email,
            };

            using (var client = apiClient())
            {
                try
                {
                    var response = await client.SendAsync(User.GetAccessToken(), importerData);

                    return RedirectToAction("Add", "Facility", new { id = model.NotificationId });
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                }

                model.AddressDetails.Countries = await GetCountries(client);
                return View(model);
            }
        }

        private async Task<IEnumerable<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(new GetCountries());
            }
        }

        private async Task<IEnumerable<CountryData>> GetCountries(IIwsClient iwsClient)
        {
            return await iwsClient.SendAsync(new GetCountries());
        }
    }
}