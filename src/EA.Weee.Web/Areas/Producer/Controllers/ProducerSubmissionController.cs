namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Weee.Core;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Controllers.Base;
    using System.Web.Mvc;

    [AuthorizeDirectRegistrantClaims(WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerSubmissionController : ExternalSiteController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}