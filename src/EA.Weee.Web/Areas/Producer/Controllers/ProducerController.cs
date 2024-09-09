namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Controllers.Base;
    using System;
    using System.Web.Mvc;

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
        public ActionResult TaskList(Guid organisationId, Guid directRegistrantId)
        {
            return View();
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult Submissions(Guid organisationId, Guid directRegistrantId)
        {
            return View();
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult OrganisationDetails(Guid organisationId, Guid directRegistrantId)
        {
            return View();
        }
    }
}