namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Web.Mvc;
    using Filters;
    using Security;

    [AuthorizeClaims(Claims.CanAccessExternalArea)]
    public class HoldingController : Controller
    {
        // GET: AatfReturn/Holding
        public ActionResult Index(Guid organisationId)
        {
            return View(organisationId);
        }
    }
}