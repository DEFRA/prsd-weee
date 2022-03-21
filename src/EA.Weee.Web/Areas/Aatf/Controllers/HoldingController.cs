namespace EA.Weee.Web.Areas.AatfEvidence.Controllers
{
    using System;
    using System.Web.Mvc;

    public class HoldingController : AatfEvidenceBaseController
    {
        // GET: AatfReturn/Holding
        public ActionResult Index(Guid organisationId)
        {
            return View(organisationId);
        }
    }
}