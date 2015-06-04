namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Web.ApiClient;
    using EA.Prsd.Core.Web.Extensions;
    using EA.Prsd.Core.Web.Mvc.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.JoinOrganisation;
    using EA.Weee.Web.ViewModels.Organisation.Type;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details;

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
        public async Task<ViewResult> Search(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return View(new SelectOrganisationViewModel());
            }

            using (var client = apiClient())
            {
                try
                {
                    var matchingOrganisations = await client.SendAsync(
                        User.GetAccessToken(),
                        new FindMatchingOrganisations(name));

                    return View(new SelectOrganisationViewModel(name, matchingOrganisations));
                }
                catch (Exception ex)
                {
                    throw;
                    //this.HandleBadRequest(ex);
                    //if (ModelState.IsValid)
                    //{
                    //    throw;
                    //}
                }

                return View(new SelectOrganisationViewModel(name));
            }
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

                        return RedirectToAction("ContactDetails", "Organisation"); //TODO: change this to correct address
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