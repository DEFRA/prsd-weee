namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Web.Mvc;
    using AATF_Return.ViewModels;

    public class NonObligatedController : Controller
    {
        [HttpGet]
        public ActionResult Index(Guid organisationId)
        {
            var viewModel = new IndexViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexViewModel viewModel)
        {
            return View(viewModel);
        }
    }
}