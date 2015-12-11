namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Shared;
    using Infrastructure;
    using ViewModels.Home;
    using Weee.Requests.Admin;

    public class HomeController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;

        public HomeController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        // GET: Admin/Home
        public async Task<ActionResult> Index()
        {
            using (var client = apiClient())
            {
                var userStatus = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                switch (userStatus)
                {
                    case UserStatus.Active:
                        return RedirectToAction("ChooseActivity", "Home");
                    case UserStatus.Inactive:
                    case UserStatus.Pending:
                    case UserStatus.Rejected:
                        return RedirectToAction("InternalUserAuthorisationRequired", "Account", new { userStatus });
                    default:
                        throw new NotSupportedException(
                            string.Format("Cannot determine result for user with status '{0}'", userStatus));
                }
            }
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.SelectedValue)
            {
                case InternalUserActivity.ManageUsers:
                    return RedirectToAction("ManageUsers", "User");

                case InternalUserActivity.ManageScheme:
                    return RedirectToAction("ManageSchemes", "Scheme");

                case InternalUserActivity.ProducerDetails:
                    return RedirectToAction("Search", "Producers");

                case InternalUserActivity.SubmissionsHistory:
                    return RedirectToAction("SubmissionsHistory", "Submissions");

                case InternalUserActivity.ViewReports:
                    return RedirectToAction("ChooseReport", "Reports");

                default:
                    throw new NotSupportedException();
            }
        }
    }
}