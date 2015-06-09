namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Web.ApiClient;
    using EA.Prsd.Core.Web.Mvc.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Requests;
    using EA.Weee.Web.ViewModels.JoinOrganisation;
    using EA.Weee.Web.ViewModels.Organisation.Type;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details;

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
        public async Task<ViewResult> SelectOrganisation(string name, int page = 1)
        {
            if (string.IsNullOrEmpty(name))
            {
                return View(new SelectOrganisationViewModel());
            }

            using (var client = apiClient())
            {
                try
                {
                    const int OrganisationsPerPage = 4; // would rather bake this into the db query but not really feasible

                    var matchingOrganisations =
                        await client.SendAsync(User.GetAccessToken(), new FindMatchingOrganisations(name));

                    var totalPages = (int)Math.Ceiling(((double)matchingOrganisations.Count() / (double)OrganisationsPerPage));

                    var organisationsForThisPage =
                        matchingOrganisations.Skip((page - 1) * OrganisationsPerPage)
                            .Take(OrganisationsPerPage)
                            .ToList();

                    var previousPage = page - 1;
                    var nextPage = page + 1;
                    var startingAt = ((page - 1) * OrganisationsPerPage) + 1;

                    return
                        View(
                            new SelectOrganisationViewModel(name, organisationsForThisPage,
                                totalPages: totalPages,
                                previousPage: previousPage,
                                nextPage: nextPage,
                                startingAt: startingAt));
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return View(new SelectOrganisationViewModel
                    {
                        Name = name
                    });
                }
            }
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