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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Shared;
    using ViewModels;
    using ViewModels.User;
    using Weee.Requests.Admin;
    using Weee.Requests.Users;
    using GetUserData = Weee.Requests.Admin.GetUserData;

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
            return RedirectToAction("EditUser", "User", new { orgUserId = model.SelectedUserId });
        }

        [HttpGet]
        public async Task<ActionResult> EditUser(Guid orgUserId)
        {
            using (var client = apiClient())
            {
                var editUserData = await client.SendAsync(User.GetAccessToken(), new GetUserData(orgUserId));
                var model = new EditUserViewModel(editUserData);
                model.UserStatusSelectList = FilterUserStatus(model.UserStatus, model.UserStatusSelectList);
                await SetBreadcrumb(new Guid(model.UserId));
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            await SetBreadcrumb(new Guid(model.UserId));

            if (!ModelState.IsValid)
            {
                model.UserStatusSelectList = FilterUserStatus(model.UserStatus, model.UserStatusSelectList);
                return View(model);
            }

            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new UpdateUser(model.UserId, model.FirstName, model.LastName));

                if (model.IsCompetentAuthorityUser)
                {
                    await client.SendAsync(User.GetAccessToken(), new UpdateCompetentAuthorityUserStatus(model.Id, model.UserStatus));
                }
                else
        {
                    await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationUserStatus(model.Id, model.UserStatus));
                }
            }

            return RedirectToAction("ManageUsers", "User");
        }

        private async Task SetBreadcrumb(Guid? userId)
        {
            breadcrumb.InternalActivity = "Manage users";

            if (userId.HasValue)
            {
                breadcrumb.InternalUser = await cache.FetchUserName(userId.Value);
            }
        }

        private IEnumerable<SelectListItem> FilterUserStatus(UserStatus userStatus, IEnumerable<SelectListItem> userStatusSelectList)
        {
            if (userStatus == UserStatus.Active)
            {
                return userStatusSelectList.Where(item => item.Text == UserStatus.Active.ToString() || item.Text == UserStatus.Inactive.ToString());
            }
            if (userStatus == UserStatus.Pending)
            {
                return userStatusSelectList.Where(item => item.Text == UserStatus.Pending.ToString() || item.Text == UserStatus.Active.ToString() || item.Text == UserStatus.Rejected.ToString());
            }
            if (userStatus == UserStatus.Inactive)
            {
                return userStatusSelectList.Where(item => item.Text == UserStatus.Inactive.ToString() || item.Text == UserStatus.Active.ToString());
            }
            if (userStatus == UserStatus.Rejected)
            {
                return userStatusSelectList.Where(item => item.Text == UserStatus.Rejected.ToString() || item.Text == UserStatus.Active.ToString());
            }

            return userStatusSelectList;
        }
    }
}