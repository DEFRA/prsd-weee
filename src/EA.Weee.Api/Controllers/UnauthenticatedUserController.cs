namespace EA.Weee.Api.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Client.Entities;
    using DataAccess.Identity;
    using Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Prsd.Core.Domain;

    [RoutePrefix("api/UnauthenticatedUser")]
    public class UnauthenticatedUserController : ApiController
    {
        private readonly ApplicationUserManager userManager;
        private readonly IUserContext userContext;

        public UnauthenticatedUserController(ApplicationUserManager userManager, IUserContext userContext)
        {
            this.userManager = userManager;
            this.userContext = userContext;
        }

        // POST api/UnauthenticatedUser/CreateUser
        [AllowAnonymous]
        [Route("CreateUser")]
        public async Task<IHttpActionResult> CreateUser(UserCreationData model)
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

            foreach (var claim in model.Claims)
            {
                user.Claims.Add(new IdentityUserClaim
                {
                    ClaimType = ClaimTypes.AuthenticationMethod, 
                    ClaimValue = claim, 
                    UserId = user.Id
                });
            }

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(user.Id);
        }

        [HttpGet]
        [Route("GetUserAccountActivationToken")]
        public async Task<string> GetUserAccountActivationToken()
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(userContext.UserId.ToString());

            return token;
        }

        [HttpPost]
        [Route("ActivateUserAccount")]
        public async Task<IHttpActionResult> ActivateUserAccount(ActivatedUserAccountData model)
        {
            var result = await userManager.ConfirmEmailAsync(model.Id.ToString(), model.Code);

            return Ok(result.Succeeded);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(PasswordResetData model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
            await userManager.ResetPasswordAsync(model.UserId.ToString(), model.Token, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(true);
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