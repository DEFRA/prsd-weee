namespace EA.Weee.Web.Controllers
{
    using Authorization;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AllowAnonymous]
    public class ErrorsController : Controller
    {
        private readonly IWeeeAuthorization weeeAuthorization;

        public ErrorsController(IWeeeAuthorization weeeAuthorization)
        {
            this.weeeAuthorization = weeeAuthorization;
        }

        [HttpGet]
        public ActionResult NotFound()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InternalError()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ErrorRedirect()
        {
            var authState = await weeeAuthorization.GetAuthorizationState();

            if (!authState.IsLoggedIn)
            {
                return RedirectToAction("SignIn", "Account", new { area = string.Empty });
            }

            return authState.DefaultLoginAction;
        }
    }
}