namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using AatfReturn.ViewModels;
    using Core.AatfReturn;
    using Core.Shared;
    using Web.ViewModels.Shared;

    public class CopyValuesController : Controller
    {
        private readonly IPasteProcesser pasteProcesser;

        public CopyValuesController(IPasteProcesser pasteProcesser)
        {
            this.pasteProcesser = pasteProcesser;
        }

        // GET: Test/CopyValues
        public ActionResult Index()
        {
            var pastValues = TempData["pasteValues"] as ObligatedCategoryValues;

            if (pastValues != null)
            {
                var model = new ObligatedViewModel(pastValues);

                return View(model);
            }

            return View(new ObligatedViewModel(new ObligatedCategoryValues()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NonObligatedValuesViewModel values)
        {
            return View(values);
        }

        public ActionResult Paste(string returnUrl)
        {
            return View("Paste", null, returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Paste(string pasteValues, string returnUrl)
        {
            TempData["pasteValues"] = pasteProcesser.BuildModel(pasteValues);

            return Redirect(returnUrl);
        }
    }
}