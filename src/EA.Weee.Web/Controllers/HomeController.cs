namespace EA.Weee.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;
    using ViewModels.Shared;

    public class HomeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
              //TODO : Aunthenticated user home page to perfrom different activities
                return RedirectToAction("Type", "OrganisationRegistration");
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

        [AllowAnonymous]
        public ActionResult _WeeeTitle()
        {
            ViewBag.Name = User.Identity.Name;

            return PartialView();
        }
    }
}