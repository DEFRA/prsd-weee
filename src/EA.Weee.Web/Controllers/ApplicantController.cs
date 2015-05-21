namespace EA.Weee.Web.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class ApplicantController : Controller
    {
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }
    }
}