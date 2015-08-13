namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Base;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Admin;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Requests.Admin;

    public class UserController : AdminController
    {
          private readonly Func<IWeeeClient> apiClient;

          public UserController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        // GET: Admin/User
        [HttpGet]
        public async Task<ActionResult> ManageUsers(int page = 1)
        {
            PagingViewModel fallbackPagingViewModel = new PagingViewModel("User", "ManageUsers");
            ManageUsersViewModel fallbackManageUsersViewModel = BuildManageUsersViewModel(new List<UserSearchData>(),
                fallbackPagingViewModel);
            using (var client = apiClient())
            {
                try
                {
                    const int usersPerPage = 2;

                    var usersSearchResultData = await client.SendAsync(User.GetAccessToken(), new FindMatchingUsers(page, usersPerPage));

                    PagingViewModel pagingViewModel =
                        PagingViewModel.FromValues(usersSearchResultData.UsersCount, usersPerPage, page,
                            "ManageUsers", "User");

                    return View(BuildManageUsersViewModel(usersSearchResultData.Results, pagingViewModel));
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return View(fallbackManageUsersViewModel);
                }
            }
        }

        private ManageUsersViewModel BuildManageUsersViewModel(IList<UserSearchData> matchingUsers, PagingViewModel pagingViewModel)
        {
            return new ManageUsersViewModel
            {
                Users = matchingUsers,
                UsersPagingViewModel = pagingViewModel
            };
        }
        [HttpPost]
        public async Task<ActionResult> ManageUsers(ManageUsersViewModel model, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    const int usersPerPage = 2;
                    var usersSearchResultData = await client.SendAsync(User.GetAccessToken(), new FindMatchingUsers(page, usersPerPage));
                   
                    var pagingViewModel = PagingViewModel.FromValues(usersSearchResultData.UsersCount, 2, 1, "ManageUsers", "User");
                    
                    return View(BuildManageUsersViewModel(usersSearchResultData.Results, pagingViewModel));
                }
            }
            return RedirectToAction("EditUser", "User", new { userId = model.SelectedUserId});
        }

        [HttpGet]
        public ActionResult EditUser(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}