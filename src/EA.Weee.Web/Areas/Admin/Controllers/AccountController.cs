﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Api.Client.Entities;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using ViewModels;

    public class AccountController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public AccountController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View(new InternalUserCreationViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InternalUserCreationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userCreationData = new UserCreationData
            {
                Email = model.Email,
                FirstName = model.Name,
                Surname = model.Surname,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Roles = new[]
                {
                    UserRole.InternalUser
                }
            };

            try
            {
                using (var client = apiClient())
                {
                    await client.NewUser.CreateUserAsync(userCreationData);               
                }

                return RedirectToAction("UserAccountActivationRequired", "Account", new { area = string.Empty });
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);

                if (ModelState.IsValid)
                {
                    throw;
                }
            }

            return View(model);
        }
    }
}