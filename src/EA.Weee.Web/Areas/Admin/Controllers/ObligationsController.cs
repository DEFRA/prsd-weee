namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using System.Web.Mvc;

    public class ObligationsController : ObligationsBaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}