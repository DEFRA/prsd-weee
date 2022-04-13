namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Web.Mvc;
    using AatfEvidence.Controllers;

    public class HoldingController : AatfEvidenceBaseController
    {
        // GET: AatfReturn/Holding
        public ActionResult Index(Guid organisationId)
        {
            return View(organisationId);
        }
    }
}