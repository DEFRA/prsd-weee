namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Web.Mvc;

    public class HoldingController : AatfReturnBaseController
    {
        // GET: AatfReturn/Holding
        public ActionResult Index(Guid organisationId)
        {
            return View(organisationId);
        }
    }
}