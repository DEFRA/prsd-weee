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

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(string placeholder)
        {
            return View();
        }

        [HttpGet]
        public ActionResult DownloadTemplate()
        {

            return View();
        }
    }
}