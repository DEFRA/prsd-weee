namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System.Web.Mvc;
    using AatfReturn.ViewModels;
    using Core.AatfReturn;
    using Core.Shared;
    using EA.Weee.Core.Helpers;

    public class CopyValuesController : Controller
    {
        private readonly IPasteProcessor pasteProcesser;
        private readonly ICategoryValueTotalCalculator calculator;

        public CopyValuesController(IPasteProcessor pasteProcesser, ICategoryValueTotalCalculator calculator)
        {
            this.pasteProcesser = pasteProcesser;
            this.calculator = calculator;
        }

        // GET: Test/CopyValues
        public ActionResult Index()
        {
            var pastValues = TempData["pasteValues"] as ObligatedCategoryValues;

            if (pastValues != null)
            {
                var model = new ObligatedViewModel(pastValues, calculator);

                return View(model);
            }

            return View(new ObligatedViewModel(new ObligatedCategoryValues(), calculator));
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