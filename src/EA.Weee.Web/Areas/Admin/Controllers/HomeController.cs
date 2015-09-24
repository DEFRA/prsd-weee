namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Base;
    using System;
    using System.Web.Mvc;
    using ViewModels.Home;

    public class HomeController : AdminController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

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