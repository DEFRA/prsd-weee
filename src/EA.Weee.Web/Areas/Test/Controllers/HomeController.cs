namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System;
    using System.Web.Mvc;
    using EA.Weee.Web.Areas.Test.ViewModels;

    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HomeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                switch (viewModel.SelectedValue)
                {
                    case HomeViewModel.OptionGeneratePcsXmlFile:
                        return RedirectToAction("SelectOrganisation", "GeneratePcsXml");

                    case HomeViewModel.ManageCache:
                        return RedirectToAction("Index", "ManageCache");

                    default:
                        throw new NotSupportedException();
                }
            }

            return View(viewModel);
        }
    }
}