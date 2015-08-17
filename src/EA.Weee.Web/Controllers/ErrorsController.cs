namespace EA.Weee.Web.Controllers
{
    using System.Web.Mvc;

    [AllowAnonymous]
    public class ErrorsController : Controller
    {
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
    }
}