namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Prsd.Core.Web.OAuth;
    using Requests.NewUser;
    using Requests.Organisations;
    using ViewModels.JoinOrganisation;
    using ViewModels.NewUser;

    [Authorize]
    public class NewUserController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        //private readonly IAuthenticationManager authenticationManager;
        //private readonly Func<IOAuthClient> oauthClient;

        public NewUserController(Func<IOAuthClient> oauthClient, Func<IWeeeClient> apiClient,
            IAuthenticationManager authenticationManager)
        {
            //this.oauthClient = oauthClient;
            this.apiClient = apiClient;
            //this.authenticationManager = authenticationManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult UserCreation()
        {
            return View(new UserCreationViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserCreation(UserCreationViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var userCreationData = new UserCreationData
                    {
                        Email = model.Email,
                        FirstName = model.Name,
                        Surname = model.Surname,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword
                    };

                    try
                    {
                        var response = await client.NewUser.CreateUserAsync(userCreationData);

                        //var signInResponse = await oauthClient().GetAccessTokenAsync(model.Email, model.Password);
                        //authenticationManager.SignIn(signInResponse.GenerateUserIdentity());

                        return RedirectToAction("Confirm", "NewUser");
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

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> SelectOrganisation(string organisationName)
        {
            var model = new SelectOrganisationViewModel
            {
                Name = organisationName
            };

            if (string.IsNullOrEmpty(organisationName))
            {
                model.Organisations = null;
            }
            else
            {
                using (var client = apiClient())
                {
                    var response =
                        await client.SendAsync(User.GetAccessToken(), new FindMatchingOrganisations(organisationName));

                    model.Organisations = response;
                }
            }

            return View(model);
        }
    }
}