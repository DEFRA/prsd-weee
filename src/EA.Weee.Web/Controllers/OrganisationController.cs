namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Web.ViewModels.Organisation;

    public class OrganisationController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult OrganisationContactDetails()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult OrganisationContactDetails(OrganisationContactDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {   
                return RedirectToAction("OrganisationRegisteredOfficePrePopulate");
            }
            return View(viewModel);
        }
    }
}