namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Shared;

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
            var pastValues = TempData["pasteValues"];

            if (pastValues != null)
            {
                return View(pastValues);
            }

            return View(new CategoryValues());
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