namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Base;

    public class HomeController : AdminController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}