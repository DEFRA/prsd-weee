namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Shared;
    using Core.Shared.Paging;
    using EA.Weee.Core.Admin;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
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
        public async Task<ActionResult> ManageUsers(FindMatchingUsers.OrderBy orderBy = FindMatchingUsers.OrderBy.FullNameAscending, int page = 1)
        {   
            SetBreadcrumb();

            if (page < 1)
            {
                page = 1;
            }

            using (var client = apiClient())
            {
                FindMatchingUsers query = new FindMatchingUsers(page, DefaultPageSize, orderBy);
                
                UserSearchDataResult usersSearchResultData = await client.SendAsync(User.GetAccessToken(), query);
                
                ManageUsersViewModel model = new ManageUsersViewModel();
                
                model.Users = usersSearchResultData.Results.ToPagedList(page - 1, DefaultPageSize, usersSearchResultData.UsersCount);
                model.OrderBy = orderBy;

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageUsers(ManageUsersViewModel model, FindMatchingUsers.OrderBy orderBy = FindMatchingUsers.OrderBy.FullNameAscending, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                return await ManageUsers(orderBy, page);
            }

            return RedirectToAction("EditUser", "User", new { orgUserId = model.SelectedUserId });
        }

        [HttpGet]
        public async Task<ActionResult> EditUser(Guid? orgUserId)
        {
            if (orgUserId.HasValue)
            {
                using (var client = apiClient())
                {
                    var editUserData = await client.SendAsync(User.GetAccessToken(), new GetUserData(orgUserId.Value));
                    var model = new EditUserViewModel(editUserData);
                    model.UserStatusSelectList = FilterUserStatus(model.UserStatus, model.UserStatusSelectList);
                    Guid userId = new Guid(editUserData.UserId);
                    SetBreadcrumb();
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("ManageUsers");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UserStatusSelectList = FilterUserStatus(model.UserStatus, model.UserStatusSelectList);
                Guid userId = new Guid(model.UserId);
                SetBreadcrumb();
                return View(model);
            }

            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new UpdateUser(model.UserId, model.FirstName, model.LastName));

                if (model.IsCompetentAuthorityUser)
                {
                    if (User.GetUserId() != model.UserId)
                    {
                        await client.SendAsync(User.GetAccessToken(), new UpdateCompetentAuthorityUserStatus(model.Id, model.UserStatus));
                    }
                }
                else
                {
                    await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationUserStatus(model.Id, model.UserStatus));
                }
            }

            return RedirectToAction("ManageUsers", "User");
        }

        private void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "Manage users";
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