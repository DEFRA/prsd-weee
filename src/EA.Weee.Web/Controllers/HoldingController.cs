namespace EA.Weee.Web.Controllers
{
    using System.Web.Mvc;
    using Filters;
    using Security;

    [AuthorizeClaims(Claims.CanAccessExternalArea)]
    public class HoldingController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}