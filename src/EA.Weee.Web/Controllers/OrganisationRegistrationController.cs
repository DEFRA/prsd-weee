namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Infrastructure;
    using Prsd.Core.Extensions;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Requests;
    using ViewModels.Organisation.Type;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.Details;

    [Authorize]
    public class OrganisationRegistrationController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly ISoleTraderDetailsRequestCreator soleTraderDetailsRequestCreator;

        public OrganisationRegistrationController(Func<IWeeeClient> apiClient, ISoleTraderDetailsRequestCreator soleTraderDetailsRequestCreator)
        {
            this.apiClient = apiClient;
            this.soleTraderDetailsRequestCreator = soleTraderDetailsRequestCreator;
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
                // TODO: Temp data needs to be handled by the organisation search after redirect
                TempData[typeof(SoleTraderDetailsViewModel).Name] = model;
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    name = model.BusinessTradingName
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
                // TODO: Temp data needs to be handled by the organisation search after redirect
                TempData[typeof(PartnershipDetailsViewModel).Name] = model;
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    name = model.BusinessTradingName
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
                // TODO: Temp data needs to be handled by the organisation search after redirect
                TempData[typeof(PartnershipDetailsViewModel).Name] = model;
                return RedirectToAction("SelectOrganisation", "OrganisationRegistration", new
                {
                    name = model.CompanyName
                });
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
                var model = new OrganisationContactPersonViewModel { OrganisationId = id };
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
                        var response = await client.SendAsync(User.GetAccessToken(), model.ToAddRequest());
                        return RedirectToAction("ContactDetails", "OrganisationRegistration"); 
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