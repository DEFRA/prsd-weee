namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Thinktecture.IdentityModel.Client;
    using ViewModels.Account;
    using ViewModels.OrganisationRegistration;

    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAuthenticationManager authenticationManager;
        private readonly Func<IOAuthClient> oauthClient;
        private readonly Func<IWeeeClient> apiClient;

        public AccountController(Func<IOAuthClient> oauthClient, Func<IWeeeClient> apiClient, IAuthenticationManager authenticationManager)
        {
            this.oauthClient = oauthClient;
            this.apiClient = apiClient;
            this.authenticationManager = authenticationManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await oauthClient().GetAccessTokenAsync(model.Email, model.Password);
            if (response.AccessToken != null)
            {
                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe },
                   response.GenerateUserIdentity());
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, ParseLoginError(response.Error));

            return View(model);
        }

        private string ParseLoginError(string error)
        {
            switch (error)
            {
                case OAuth2Constants.Errors.AccessDenied:
                    return "Access denied";
                case OAuth2Constants.Errors.InvalidGrant:
                    return "Invalid credentials";
                case OAuth2Constants.Errors.Error:
                case OAuth2Constants.Errors.InvalidClient:
                case OAuth2Constants.Errors.InvalidRequest:
                case OAuth2Constants.Errors.InvalidScope:
                case OAuth2Constants.Errors.UnauthorizedClient:
                case OAuth2Constants.Errors.UnsupportedGrantType:
                case OAuth2Constants.Errors.UnsupportedResponseType:
                default:
                    return "Internal error";
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("RedirectProcess");
        }

        [HttpGet]
        public async Task<ActionResult> RedirectProcess()
        {
            using (var client = apiClient())
            {
                var approvedOrganisationUsers = await
                     client.SendAsync(
                         User.GetAccessToken(),
                         new GetOrganisationsByUserId(User.GetUserId(), new[] { (int)OrganisationUserStatus.Approved }));

                var pendingOrganisationUsers = await
                     client.SendAsync(
                         User.GetAccessToken(),
                         new GetOrganisationsByUserId(User.GetUserId(), new[] { (int)OrganisationUserStatus.Pending, (int)OrganisationUserStatus.Refused, (int)OrganisationUserStatus.Inactive }));

                if (approvedOrganisationUsers.Count >= 1)
                {
                    return RedirectToAction("ChooseActivity", "PCS", new { id = approvedOrganisationUsers.First().OrganisationId });
                }
                else if (pendingOrganisationUsers.Count >= 1)
                {   
                    return RedirectToAction("HoldingMessageForPending", "OrganisationRegistration");
                }
                else
                {
                    return RedirectToAction("Type", "OrganisationRegistration");
                }
            }
        }
    }
}