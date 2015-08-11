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
    using ViewModels;
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
        public async Task<ActionResult> ManageUsers()
        {
            return View(new ManageUsersViewModel { Users = await GetUsers() });
        }

        [HttpPost]
        public async Task<ActionResult> ManageUsers(ManageUsersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new ManageUsersViewModel { Users = await GetUsers() });
            }

            return RedirectToAction("EditUser", "User", new { userId = model.SelectedUserId});
        }

        private async Task<List<UserSearchData>> GetUsers()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetAllUsers());
            }
        }

        [HttpGet]
        public ActionResult EditUser(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}