namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Core;
    using Filters;

    [AuthorizeClaims(Claims.CanAccessInternalArea)]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}