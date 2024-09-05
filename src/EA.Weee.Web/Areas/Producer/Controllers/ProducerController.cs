namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Weee.Core;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Controllers.Base;
    using System.Web.Mvc;
    using EA.Weee.Core.DirectRegistrant;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;

        public ActionResult Index()
        {
            return View();
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult TaskList()
        {
            return View();
        }
    }
}