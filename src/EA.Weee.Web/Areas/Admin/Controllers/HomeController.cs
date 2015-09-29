namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Base;
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
            if (ModelState.IsValid)
            {
                string choosenActivity = model.InternalUserActivityOptions.SelectedValue;
                switch (choosenActivity)
                {
                case InternalUserActivity.ManageUsers:
                    {
                        return RedirectToAction("ManageUsers", "User");
                    }

                case InternalUserActivity.ManageScheme:
                    {
                        return RedirectToAction("ManageSchemes", "Scheme");
                    }
                }
            }
            
            return View(model);
        }
    }
}