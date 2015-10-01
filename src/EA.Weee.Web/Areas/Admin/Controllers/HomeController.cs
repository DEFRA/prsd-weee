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

            switch (model.InternalUserActivityOptions.SelectedValue)
            {
                case InternalUserActivity.ManageUsers:
                    return RedirectToAction("ManageUsers", "User");

                case InternalUserActivity.ManageScheme:
                    return RedirectToAction("ManageSchemes", "Scheme");

                case InternalUserActivity.ViewProducerInformation:
                    return RedirectToAction("Search", "Producers");

                default:
                    throw new NotSupportedException();
            }
        }
    }
}