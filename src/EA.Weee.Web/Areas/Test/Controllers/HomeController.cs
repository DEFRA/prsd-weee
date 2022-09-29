namespace EA.Weee.Web.Areas.Test.Controllers
{
    using EA.Weee.Web.Areas.Test.ViewModels;
    using System;
    using System.Web.Mvc;

    public class HomeController : TestControllerBase
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
                    case HomeViewModel.CreatePcsMemberXmlFile:
                        return RedirectToAction("SelectOrganisation", "CreatePcsMemberXmlFile");

                    case HomeViewModel.CreatePcsDataReturnXmlFile:
                        return RedirectToAction("SelectOrganisation", "CreatePcsDataReturnXmlFile");

                    case HomeViewModel.ManageCache:
                        return RedirectToAction("Index", "ManageCache");

                    case HomeViewModel.ManageSystemDate:
                        return RedirectToAction("Settings", "ManagePcsReturnsSubmissionWindow");

                    case HomeViewModel.CopyAndPaste:
                        return RedirectToAction("Index", "CopyValues");

                    default:
                        throw new NotSupportedException();
                }
            }

            return View(viewModel);
        }
    }
}