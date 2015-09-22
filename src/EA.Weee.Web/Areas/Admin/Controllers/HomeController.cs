namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Shared;
    using Infrastructure;
    using Services;
    using ViewModels;
    using Weee.Requests.Admin;
   
    public class HomeController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public HomeController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumbService)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumbService;
        }
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChooseActivity()
        {
            var model = new InternalUserActivityViewModel();
            return View("ChooseActivity", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseActivity(InternalUserActivityViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var status = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));
                    string choosenActivity = model.InternalUserActivityOptions.SelectedValue;
                    if (status == UserStatus.Active)
                    {
                        switch (choosenActivity)
                        {
                            case InternalUserActivity.ManageUsers:
                                {
                                    return RedirectToAction("ManageUsers", "User");
                                }

                            case InternalUserActivity.ManageScheme:
                                {
                                    return RedirectToAction("ManageSchemes", "Scheme");
                                }
                        }
                    }
                    else
                    {
                        return RedirectToAction("InternalUserAuthorisationRequired", "Home", new {activity = choosenActivity});
                    }
                }
            }
            
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> InternalUserAuthorisationRequired(string activity)
        {
            SetBreadcrumb(activity);
            using (var client = apiClient())
            {
               // var userId = GetUserId();
                var status = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                if (status == UserStatus.Active)
                {
                    switch (activity)
                    {
                        case InternalUserActivity.ManageUsers:
                            {
                                return RedirectToAction("ManageUsers", "User");
                            }

                        case InternalUserActivity.ManageScheme:
                            {
                                return RedirectToAction("ManageSchemes", "Scheme");
                            }
                    }
                }

                InternalUserAuthorizationRequiredViewModel model = new InternalUserAuthorizationRequiredViewModel() { Status = status };

                return View(model);
            }
        }

        private async Task SetBreadcrumb(string activity)
        {
            breadcrumb.InternalActivity = activity;
        }
    }
}