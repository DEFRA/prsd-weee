namespace EA.Weee.Web.Controllers
{
    using EA.Weee.Web.Services;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;
    using ViewModels.Shared;

    public class HomeController : Controller
    {
        private readonly BreadcrumbService breadcrumb;

        public HomeController(BreadcrumbService breadcrumb)
        {
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("SelectOrganisation");
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
        [ChildActionOnly]
        public ActionResult _WeeeTitle()
        {
            TitleViewModel model = new TitleViewModel();
            model.User = User;
            model.Breadcrumb = breadcrumb;

            return PartialView(model);
        }

        [AllowAnonymous]
        public ActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }
    }
}