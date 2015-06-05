namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using Prsd.Core.Extensions;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Requests.Organisations;
    using Requests.Shared;
    using ViewModels.Organisation;
    using ViewModels.Organisation.Type;
    using ViewModels.OrganisationRegistration.Details;

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
                        return RedirectToAction("SoleTraderDetails");
                    case OrganisationTypeEnum.RegisteredCompany:
                        return RedirectToAction("RegisteredCompanyDetails");
                    case OrganisationTypeEnum.Partnership:
                        return RedirectToAction("PartnershipDetails");
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
                // TODO: Save details 
                return RedirectToAction("Search", "OrganisationRegistration"); // TODO: Change this to correct address
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
                // TODO: Save details 
                return RedirectToAction("Search", "OrganisationRegistration"); // TODO: Change this to correct address
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
            // TODO: Validate company registration number
            if (ModelState.IsValid)
            {
                // TODO: Save details 
                return RedirectToAction("Search", "OrganisationRegistration"); // TODO: Change this to correct address
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> MainContactPerson(Guid id)
        {
            using (var client = apiClient())
            {
                //TODO: Get organisation id from organisation record
                //var response = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id));
                var model = new OrganisationContactPersonViewModel { OrganisationId = new Guid() };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MainContactPerson(OrganisationContactPersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    try
                    {
                        //TODO: Save details
                        //var response = await client.SendAsync(User.GetAccessToken(),
                        //    new AddContactPersonToOrganisation
                        //    {
                        //        OrganisationId = model.OrganisationId,
                        //        MainContactPerson = model.MainContactPerson
                        //    });

                        return RedirectToAction("ContactDetails", "OrganisationRegistration"); //TODO: change this to correct address
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
        public async Task<ActionResult> OrganisationContactDetails(Guid id)
        {
            using (var client = apiClient())
            {
                //TODO: Get organisation id from organisation record
                //var response = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(id));
                var model = new OrganisationContactDetailsViewModel { OrganisationId = id };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganisationContactDetails(OrganisationContactDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    try
                    {
                        //TODO: Save details
                        var address = new AddressData
                        {
                            Address1 = model.Address1,
                            Address2 = model.Address2,
                            Country = model.Country,
                            CountyOrRegion = model.CountyOrRegion,
                            Email = model.Email,
                            PostalCode = model.Postcode,
                            Telephone = model.Phone,
                            TownOrCity = model.TownOrCity
                        };

                        var response = await client.SendAsync(User.GetAccessToken(),
                            new AddOrganisationContactDetails
                            {
                                OrganisationId = model.OrganisationId,
                                OrganisationContactAddress = address
                            });

                        return RedirectToAction("OrganisationRegisteredOfficePrePopulate", "OrganisationRegistration"); //TODO: change this to correct address
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
    }
}