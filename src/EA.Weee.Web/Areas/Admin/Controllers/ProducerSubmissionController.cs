namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using System.Web.Mvc;

    public class ProducerSubmissionController : AdminController
    {
        public ActionResult Submissions()
        {
            return View();
        }
    }
}