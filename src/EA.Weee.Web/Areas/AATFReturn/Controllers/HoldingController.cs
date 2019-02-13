namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Web.Mvc;
    using Web.Controllers.Base;

    public class HoldingController : ExternalSiteController
    {
        // GET: AatfReturn/Holding
        public ActionResult Index(Guid organisationId)
        {
            return View(organisationId);
        }
    }
}