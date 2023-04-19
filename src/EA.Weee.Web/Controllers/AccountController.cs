namespace EA.Weee.Web.Controllers
{
    using Api.Client;
    using Api.Client.Entities;
    using Authorization;
    using EA.Weee.Core.Routing;
    using EA.Weee.Web.Controllers.Base;
    using Extensions;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Prsd.Core.Web.OAuth;
    using ViewModels.Account;

    [Authorize]
    public class AccountController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly Func<IOAuthClientCredentialClient> apiClientCredential;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IExternalRouteService externalRouteService;

        public AccountController(Func<IWeeeClient> apiClient,
            IWeeeAuthorization weeeAuthorization,
            IExternalRouteService externalRouteService, Func<IOAuthClientCredentialClient> apiClientCredential)
        {
            this.apiClient = apiClient;
            this.weeeAuthorization = weeeAuthorization;
            this.externalRouteService = externalRouteService;
            this.apiClientCredential = apiClientCredential;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SignIn(string returnUrl)
        {
            var authorizationState = await weeeAuthorization.GetAuthorizationState();

            if (authorizationState.IsLoggedIn)
            {
                return this.LoginRedirect(authorizationState.DefaultLoginAction, returnUrl);
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await weeeAuthorization.SignIn(model.Email, model.Password, model.RememberMe);

                if (loginResult.Successful)
                {
                    return this.LoginRedirect(loginResult.DefaultLoginAction, model.ReturnUrl);
                }

                ModelState.AddModelError(string.Empty, loginResult.ErrorMessage);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Refresh()
        {
            var loginResult = await weeeAuthorization.RefreshAuthentication();

            return RedirectToAction("SignIn");
        }

        // POST: /Account/SignOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            weeeAuthorization.SignOut();

            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public ActionResult UserAccountActivationRequired()
        {
            ViewBag.UserEmailAddress = User.GetEmailAddress();

            return View("AccountActivationRequired");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserAccountActivationRequired(FormCollection model)
        {
            using (var client = apiClient())
            {
                string accessToken = User.GetAccessToken();

                string activationBaseUrl = externalRouteService.ActivateExternalUserAccountUrl;

                await client.User.ResendActivationEmail(accessToken, activationBaseUrl);
            }

            return View("AccountActivationRequested");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserAccountActivationFailed(Guid id, AccountActivationRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AccountActivationFailed", model);
            }

            using (var client = apiClient())
            {
                string activationBaseUrl = externalRouteService.ActivateExternalUserAccountUrl;

                var result = await client.User.ResendActivationEmailByUserId(id.ToString(), model.Email, activationBaseUrl, User.GetAccessToken());

                if (!result)
                {
                    ModelState.AddModelError("Email", "The email address does not match the address for your account.");
                    return View("AccountActivationFailed", model);
                }
                else
                {
                    ViewBag.Email = model.Email;
                    return View("AccountActivationRequested");
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> ActivateUserAccount(Guid id, string code)
        {
            using (var client = apiClient())
            {
                bool result = await client.User.ActivateUserAccountEmailAsync(new ActivatedUserAccountData { Id = id, Code = code }, User.GetAccessToken());

                if (!result)
                {
                    return View("AccountActivationFailed");
                }
                else
                {
                    return View("AccountActivated");
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(Guid id, string token)
        {
            using (var client = apiClient())
            {
                var passwordResetData = new PasswordResetData
                {
                    Password = string.Empty,
                    Token = token,
                    UserId = id
                };

                var response = await apiClientCredential().GetClientCredentialsAsync();

                bool result = await client.User.IsPasswordResetTokenValidAsync(passwordResetData, response.AccessToken);

                return View(!result ? "ResetPasswordExpired" : "ResetPassword");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(Guid id, string token, ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                var passwordResetData = new PasswordResetData
                {
                    Password = model.Password,
                    Token = token,
                    UserId = id
                };

                try
                {
                    var response = await apiClientCredential().GetClientCredentialsAsync();

                    bool result = await client.User.ResetPasswordAsync(passwordResetData, response.AccessToken);
                    return View(!result ? "ResetPasswordExpired" : "ResetPasswordComplete");
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }

                    foreach (var modelState in ViewData.ModelState.Values.ToList())
                    {
                        List<int> errorsToRemoveIndex = new List<int>();
                        for (var i = modelState.Errors.Count - 1; i >= 0; i--)
                        {
                            if (modelState.Errors[i].ErrorMessage.Contains("Passwords") && modelState.Value == null)
                            {
                                errorsToRemoveIndex.Add(i);
                            }
                        }
                        foreach (int index in errorsToRemoveIndex)
                        {
                            modelState.Errors.RemoveAt(index);
                        }
                    }
                }
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordRequest()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPasswordRequest(ResetPasswordRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var access = await apiClientCredential().GetClientCredentialsAsync();
            
            using (var client = apiClient())
            {
                ResetPasswordRoute route = externalRouteService.ExternalUserResetPasswordRoute;
                PasswordResetRequest apiModel = new PasswordResetRequest(model.Email, route);

                var result = await client.User.ResetPasswordRequestAsync(apiModel, access.AccessToken);

                if (!result.ValidEmail)
                {
                    ModelState.AddModelError("Email", "Email address not recognised.");
                    return View(model);
                }
                else
                {
                    ViewBag.Email = model.Email;
                    return View("ResetPasswordInstruction");
                }
            }
        }
    }
}