namespace EA.Weee.Api.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Client.Entities;
    using DataAccess.Identity;
    using EA.Weee.Core;
    using EA.Weee.Email;
    using Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Prsd.Core.Domain;
    using Security;

    [RoutePrefix("api/UnauthenticatedUser")]
    public class UnauthenticatedUserController : ApiController
    {
        private readonly ApplicationUserManager userManager;
        private readonly IUserContext userContext;
        private readonly IWeeeEmailService emailService;

        public UnauthenticatedUserController(
            ApplicationUserManager userManager,
            IUserContext userContext,
            IWeeeEmailService emailService)
        {
            this.userManager = userManager;
            this.userContext = userContext;
            this.emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CreateInternalUser")]
        public async Task<IHttpActionResult> CreateInternalUser(InternalUserCreationData model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email, 
                Email = model.Email, 
                FirstName = model.FirstName, 
                Surname = model.Surname, 
            };

            user.Claims.Add(new IdentityUserClaim
            {
                ClaimType = ClaimTypes.AuthenticationMethod, 
                ClaimValue = Claims.CanAccessInternalArea, 
                UserId = user.Id
            });

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            await SendActivationEmail(user.Id, user.Email, model.ActivationBaseUrl);

            return Ok(user.Id);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CreateExternalUser")]
        public async Task<IHttpActionResult> CreateExternalUser(ExternalUserCreationData model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname,
            };

            user.Claims.Add(new IdentityUserClaim
            {
                ClaimType = ClaimTypes.AuthenticationMethod,
                ClaimValue = Claims.CanAccessExternalArea,
                UserId = user.Id
            });

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            await SendActivationEmail(user.Id, user.Email, model.ActivationBaseUrl);

            return Ok(user.Id);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ActivateUserAccount")]
        public async Task<IHttpActionResult> ActivateUserAccount(ActivatedUserAccountData model)
        {
            var result = await userManager.ConfirmEmailAsync(model.Id.ToString(), model.Code);

            return Ok(result.Succeeded);
        }

        [HttpGet]
        [Route("GetUserAccountActivationToken")]
        public async Task<string> GetUserAccountActivationToken()
        {
            string userId = userContext.UserId.ToString();

            return await userManager.GenerateEmailConfirmationTokenAsync(userId);
        }

        [HttpPost]
        [Route("ResendActivationEmail")]
        public async Task<IHttpActionResult> ResendActivationEmail(ResendActivationEmailRequest model)
        {
            string userId = userContext.UserId.ToString();
            
            string emailAddress = await userManager.GetEmailAsync(userId);

            bool emailSent = await SendActivationEmail(userId, emailAddress, model.ActivationBaseUrl);
            
            return Ok(emailSent);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResendActivationEmailByUserId")]
        public async Task<IHttpActionResult> ResendActivationEmailByUserId(ResendActivationEmailByUserIdRequest model)
        {
            var user = await userManager.FindByEmailAsync(model.EmailAddress);
            if (user == null || user.Id != model.UserId)
            {
                return Ok(false);
            }
            else
            {
                await SendActivationEmail(model.UserId, model.EmailAddress, model.ActivationBaseUrl);
                return Ok(true);
            }
        }

        private async Task<bool> SendActivationEmail(string userId, string emailAddress, string activationBaseUrl)
        {
            string activationToken = await userManager.GenerateEmailConfirmationTokenAsync(userId);

            var uriBuilder = new UriBuilder(activationBaseUrl);
            uriBuilder.Path += "/" + userId;
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["code"] = activationToken;
            uriBuilder.Query = parameters.ToString();
            string activationUrl = uriBuilder.Uri.ToString();

            return await emailService.SendActivateUserAccount(emailAddress, activationUrl);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(PasswordResetData model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = null;

            try
            {
                result = await userManager.ResetPasswordAsync(model.UserId.ToString(), model.Token, model.Password);
                
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }
            catch (InvalidOperationException)
            {
                // Because an invalid token or an invalid password does not throw an error on reset,
                // we can say the only other parameter (user Id) is invalid.
                ModelState.AddModelError(string.Empty, "User not recognised");
                return BadRequest(ModelState);
            }

            return Ok(result.Succeeded);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPasswordRequest")]
        public async Task<IHttpActionResult> ResetPasswordRequest(PasswordResetRequest model)
        {
            var result = new PasswordResetRequestResult();

            var user = await userManager.FindByEmailAsync(model.EmailAddress);
            if (user != null)
            {
                result.ValidEmail = true;
                result.PasswordResetToken = await userManager.GeneratePasswordResetTokenAsync(user.Id);

                model.Route.UserID = user.Id;
                model.Route.Token = result.PasswordResetToken;
                string passwordResetUrl = model.Route.GenerateUrl();

                await emailService.SendPasswordResetRequest(model.EmailAddress, passwordResetUrl);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("IsPasswordResetTokenValid")]
        public async Task<IHttpActionResult> IsPasswordResetTokenValid(PasswordResetData model)
        {
            string userId = model.UserId.ToString();

            bool result = await userManager.VerifyUserTokenAsync(userId, "ResetPassword", model.Token);
            
            return Ok(result);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        //map the password error to password field
                        if (error.StartsWith("Password"))
                        {
                            ModelState.AddModelError("Password", error);
                        }
                        //We are using the email address as the username so this avoids duplicate validation error message
                        if (!error.StartsWith("Name"))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}