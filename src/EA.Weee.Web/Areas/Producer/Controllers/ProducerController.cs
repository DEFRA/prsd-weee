namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using iText.Kernel.XMP.Options;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ProducerController(
            BreadcrumbService breadcrumb, 
            IWeeeCache cache,
            IMapper mapper)
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
        public ActionResult CheckAnswers()
        {
            return View("CheckAnswers");
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult Submissions()
        {
            return View(SmallProducerSubmissionData.OrganisationData.Id);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> OrganisationDetails()
        {
            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.ViewOrganisation);

            return ViewOrganisation(this.SmallProducerSubmissionData);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> RouteTab(OrganisationDetailsDisplayOption option)
        {
            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.ViewOrganisation);

            if (option == OrganisationDetailsDisplayOption.OrganisationDetails)
            {
                return ViewOrganisation(this.SmallProducerSubmissionData);
            }
            if (option == OrganisationDetailsDisplayOption.ContactDetails)
            {
                return ContactDetails(this.SmallProducerSubmissionData);
            }
            if (option == OrganisationDetailsDisplayOption.ServiceOfNoticeDetails)
            {
                return ServiceOfNoticeDetails(this.SmallProducerSubmissionData);
            }
            if (option == OrganisationDetailsDisplayOption.RepresentedOrganisationDetails)
            {
                return RepresentedOrganisationDetails(this.SmallProducerSubmissionData);
            }

            return await OrganisationDetails();
        }

        public ActionResult ViewOrganisation(SmallProducerSubmissionData submissionsData)
        {
            var organisationVM = mapper.Map<SmallProducerSubmissionData, OrganisationViewModel>(SmallProducerSubmissionData);

            var vm = new OrganisationDetailsTabsViewModel
            {
                ActiveOption = OrganisationDetailsDisplayOption.OrganisationDetails,
                OrganisationViewModel = organisationVM,
                SmallProducerSubmissionData = submissionsData,
            };

            return View("ViewOrganisation/OrganisationDetails", vm);
        }

        public ActionResult ContactDetails(SmallProducerSubmissionData submissionsData)
        {
            var vm = new OrganisationDetailsTabsViewModel
            {
                ActiveOption = OrganisationDetailsDisplayOption.ContactDetails,
                SmallProducerSubmissionData = submissionsData
            };

            return View("ViewOrganisation/ContactDetails", vm);
        }

        public ActionResult ServiceOfNoticeDetails(SmallProducerSubmissionData submissionsData)
        {
            var vm = new OrganisationDetailsTabsViewModel
            {
                ActiveOption = OrganisationDetailsDisplayOption.ServiceOfNoticeDetails,
                SmallProducerSubmissionData = submissionsData
            };

            return View("ViewOrganisation/ServiceOfNoticeDetails", vm);
        }

        public ActionResult RepresentedOrganisationDetails(SmallProducerSubmissionData submissionsData)
        {
            var vm = new OrganisationDetailsTabsViewModel
            {
                ActiveOption = OrganisationDetailsDisplayOption.RepresentedOrganisationDetails,
                SmallProducerSubmissionData = submissionsData
            };

            return View("ViewOrganisation/RepresentedOrganisationDetails", vm);
        }
    }
}