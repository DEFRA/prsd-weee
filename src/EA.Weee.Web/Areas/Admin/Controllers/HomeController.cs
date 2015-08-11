namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Web.Mvc;
    using Base;
    using ViewModels;

    public class HomeController : AdminController
    {
        [HttpGet]
        public ActionResult ChooseActivity()
        {
            var model = new InternalUserActivityViewModel();
            return View("ChooseActivity", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(InternalUserActivityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.InternalUserActivityOptions.SelectedValue == InternalUserActivity.ManageUsers)
            {
                return RedirectToAction("ManageUsers", "User");    
            }

            if (model.InternalUserActivityOptions.SelectedValue == InternalUserActivity.ManageScheme)
            {
                return RedirectToAction("ManageSchemes", "Scheme");
            }

            return View(model);
        }
    }
}