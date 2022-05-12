namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using System.Web.Mvc;

    public class AdminHoldingController : AdminController
    {
        // GET: Admin/Holding page
        public ActionResult Index()
        {
            return View();
        }
    }
}