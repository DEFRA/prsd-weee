namespace EA.Weee.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Client;
    using Client.Entities;
    using DataAccess.Identity;
    using Identity;
    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/NewUser")]
    public class NewUserController : ApiController
    {
        private readonly ApplicationUserManager userManager;

        public NewUserController(ApplicationUserManager userManager)
        {
            this.userManager = userManager;
        }

        // POST api/NewUser/CreateUser
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
                Surname = model.Surname
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(model.Email);
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