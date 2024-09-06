namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using System.Collections.Generic;
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
        public ActionResult TaskList()
        {
            var submission = SmallProducerSubmissionData.CurrentSubmission;

            var model = new TaskListViewModel()
            {
                ProducerTaskModels = new List<ProducerTaskModel>
                {
                    new ProducerTaskModel { TaskLinkName = "Organisation details", Complete = submission.OrganizationDetailsComplete },
                    new ProducerTaskModel { TaskLinkName = "Contact details", Complete = submission.ContactDetailsComplete },
                    new ProducerTaskModel { TaskLinkName = "Service of notice", Complete = submission.ServiceOfNoticeComplete },
                    new ProducerTaskModel { TaskLinkName = "Representing company details", Complete = submission.RepresentingCompanyDetailsComplete },
                    new ProducerTaskModel { TaskLinkName = "EEE details", Complete = submission.EEEDetailsComplete }
                }
            };

            return View(model);
        }

        [SmallProducerSubmissionContext]
        [HttpPost]
        public ActionResult TaskList()//check answers?
        {
            return View();
        }
    }
}