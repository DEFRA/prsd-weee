namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Shared.Paging;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;
    using Weee.Requests.Admin;

    public class UserController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private const int DefaultPageSize = 25;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;

        public UserController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        // GET: Admin/User
        [HttpGet]
        public async Task<ActionResult> ManageUsers(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            using (var client = apiClient())
            {
                await SetBreadcrumb(null);

                try
                {
                    var usersSearchResultData = await client.SendAsync(User.GetAccessToken(), new FindMatchingUsers(page, DefaultPageSize));
                    ManageUsersViewModel model = new ManageUsersViewModel();
                    model.Users = usersSearchResultData.Results.ToPagedList(page - 1, DefaultPageSize, usersSearchResultData.UsersCount);
                    return View(model);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageUsers(ManageUsersViewModel model, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var usersSearchResultData = await client.SendAsync(User.GetAccessToken(), new FindMatchingUsers(page, DefaultPageSize));
                    model.Users = usersSearchResultData.Results.ToPagedList(page - 1, DefaultPageSize, usersSearchResultData.UsersCount);
                    await SetBreadcrumb(null);
                    return View(model);
                }
            }
            return RedirectToAction("EditUser", "User", new { userId = model.SelectedUserId });
        }

        [HttpGet]
        public async Task<ActionResult> EditUser(Guid userId)
        {
            await SetBreadcrumb(userId);
            throw new NotImplementedException();
        }

        private async Task SetBreadcrumb(Guid? userId)
        {
            breadcrumb.InternalActivity = "Manage users";

            if (userId.HasValue)
            {
                breadcrumb.User = await cache.FetchUserName(userId.Value);
            }
        }
    }
}