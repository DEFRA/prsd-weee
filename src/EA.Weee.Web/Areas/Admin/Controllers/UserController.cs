namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Base;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared.Paging;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using ViewModels;
    using Weee.Requests.Admin;

    public class UserController : AdminController
    {
          private readonly Func<IWeeeClient> apiClient;
          private const int DefaultPageSize = 25;
          public UserController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
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
                    return View();
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