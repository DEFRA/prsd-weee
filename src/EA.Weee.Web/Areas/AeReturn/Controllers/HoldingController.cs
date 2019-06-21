namespace EA.Weee.Web.Areas.AeReturn.Controllers
{
    using System;
    using System.Web.Mvc;

    public class HoldingController : AeReturnBaseController
    {
        // GET: AatfReturn/Holding
        public ActionResult Index(Guid organisationId)
        {
            return View(organisationId);
        }
    }
}