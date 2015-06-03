namespace EA.Weee.Web.Controllers
{
    using EA.Weee.Web.ViewModels.Shared;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(actionName: "Home", controllerName: "Applicant");
            }

            return View("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LandingPage()
        {
            return View(new YesNoChoiceViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LandingPage(YesNoChoiceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var selectedOption = viewModel.Choices.SelectedValue;
            if (selectedOption.Equals("No"))
            {
                return RedirectToAction("CheckIsPcs", "NewUser");
            }

            return RedirectToAction("Login", "Account");
        }
    }
}