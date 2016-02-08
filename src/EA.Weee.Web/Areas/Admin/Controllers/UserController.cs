namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Security;
    using Core.Shared;
    using Core.Shared.Paging;
    using EA.Weee.Core.Admin;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
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

        /// <summary>
        /// Get a list of organisation-users and authority-users with optional paging and ordering.
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Index(FindMatchingUsers.OrderBy orderBy = FindMatchingUsers.OrderBy.FullNameAscending, int page = 1)
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

        /// <summary>
        /// Select a user from the list.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ManageUsersViewModel model, FindMatchingUsers.OrderBy orderBy = FindMatchingUsers.OrderBy.FullNameAscending, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                return await Index(orderBy, page);
            }

            return RedirectToAction("View", new { id = model.SelectedUserId });
        }

        /// <summary>
        /// Get a page where the details of an organisation-user or authority-user can be viewed.
        /// </summary>
        /// <param name="id">
        /// For internal users, this is the CompetentAuthorityUserId.
        /// For external users, this is the OrganisationUserId.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> View(Guid id)
        {
            ManageUserData editUserData;
            using (var client = apiClient())
            {
                editUserData = await client.SendAsync(User.GetAccessToken(), new GetUserData(id));
            }

            EditUserViewModel model = new EditUserViewModel(editUserData);

            SetBreadcrumb();
            return View(model);
        }

        /// <summary>
        /// Get a page where the details of an organisation-user or authority-user can be edited.
        /// </summary>
        /// <param name="id">
        /// For internal users, this is the CompetentAuthorityUserId.
        /// For external users, this is the OrganisationUserId.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            ManageUserData editUserData;
            List<Role> roles;
            using (var client = apiClient())
            {
                editUserData = await client.SendAsync(User.GetAccessToken(), new GetUserData(id));
                roles = await client.SendAsync(User.GetAccessToken(), new GetRoles());
            }

            EditUserViewModel model = new EditUserViewModel(editUserData);
            model.UserStatusSelectList = FilterUserStatus(model.UserStatus, model.UserStatusSelectList);
            model.UserRoleSelectList = new SelectList(roles, "Name", "Description");

            SetBreadcrumb();
            return View(model);
        }

        /// <summary>
        /// Make changes to the details of an organisation-user or authority-user.
        /// </summary>
        /// <param name="id">
        /// For internal users, this is the CompetentAuthorityUserId.
        /// For external users, this is the OrganisationUserId.
        /// </param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                List<Role> roles;
                using (var client = apiClient())
                {
                    roles = await client.SendAsync(User.GetAccessToken(), new GetRoles());
                }

                model.UserStatusSelectList = FilterUserStatus(model.UserStatus, model.UserStatusSelectList);
                model.UserRoleSelectList = new SelectList(roles, "Name", "Description");

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
                        await client.SendAsync(User.GetAccessToken(), new UpdateCompetentAuthorityUserRoleAndStatus(id, model.UserStatus, model.Role.Name));
                    }
                }
                else
                {
                    await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationUserStatus(id, model.UserStatus));
                }
            }

            return RedirectToAction("View", new { id });
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