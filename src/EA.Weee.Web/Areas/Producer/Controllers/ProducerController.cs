namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ProducerController(BreadcrumbService breadcrumb, IWeeeCache cache, IMapper mapper)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            return View();
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> TaskList()
        {
            var submission = SmallProducerSubmissionData.CurrentSubmission;

            var model = new TaskListViewModel()
            {
                OrganisationId = SmallProducerSubmissionData.OrganisationData.Id,
                ProducerTaskModels = new List<ProducerTaskModel>
                {
                    new ProducerTaskModel 
                    { 
                        TaskLinkName = "Organisation details", 
                        Complete = submission.OrganisationDetailsComplete,
                        Action = nameof(ProducerSubmissionController.EditOrganisationDetails)
                    },
                    new ProducerTaskModel 
                    { 
                        TaskLinkName = "Contact details", 
                        Complete = submission.ContactDetailsComplete,
                        Action = nameof(ProducerSubmissionController.EditContactDetails)
                    },
                    new ProducerTaskModel 
                    {
                        TaskLinkName = "Service of notice", 
                        Complete = submission.ServiceOfNoticeComplete,
                        Action = nameof(ProducerSubmissionController.ServiceOfNotice)
                    },
                }
            };

            if (SmallProducerSubmissionData.HasAuthorisedRepresentitive)
            {
                model.ProducerTaskModels.Add(new ProducerTaskModel
                {
                    TaskLinkName = "Representing company details",
                    Complete = submission.RepresentingCompanyDetailsComplete,
                    Action = nameof(ProducerSubmissionController.EditRepresentedOrganisationDetails)
                });
            }

            model.ProducerTaskModels.Add(new ProducerTaskModel
            {
                TaskLinkName = "EEE details",
                Complete = submission.EEEDetailsComplete,
                Action = nameof(ProducerSubmissionController.EditEeeeData)
            });

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> CheckAnswers()
        {
            var model =
                mapper.Map<SmallProducerSubmissionData, CheckAnswersViewModel>(SmallProducerSubmissionData);

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpPost]
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> CheckAnswers(CheckAnswersViewModel model)
        {
            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return RedirectToAction("SubmitRegistration");
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult Submissions()
        {
            return View(SmallProducerSubmissionData.OrganisationData.Id);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult OrganisationDetails()
        {
            return View(SmallProducerSubmissionData.OrganisationData.Id);
        }

        [HttpGet]
        public ActionResult SubmitRegistration()
        {
            return View("SubmitRegistration");
        }
    }
}