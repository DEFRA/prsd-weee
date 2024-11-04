namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ProducerController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;
        private readonly ISubmissionService submissionService;

        public ProducerController(
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IMapper mapper,
            IMvcTemplateExecutor templateExecutor,
            IPdfDocumentProvider pdfDocumentProvider,
            ISubmissionService submissionService)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
            this.templateExecutor = templateExecutor;
            this.pdfDocumentProvider = pdfDocumentProvider;
            this.submissionService = submissionService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public ActionResult AlreadySubmittedAndPaid()
        {
            var model = new AlreadySubmittedAndPaidViewModel()
            {
                OrganisationId = SmallProducerSubmissionData.OrganisationData.Id,
                ComplianceYear = SmallProducerSubmissionData.CurrentSubmission.ComplianceYear
            };

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public ActionResult OrganisationHasNoSubmissions()
        {
            var model = new AlreadySubmittedAndPaidViewModel()
            {
                OrganisationId = SmallProducerSubmissionData.OrganisationData.Id,
                ComplianceYear = SmallProducerSubmissionData.CurrentSubmission.ComplianceYear
            };

            SetHistoricBreadcrumb();

            return View(model);
        }

        [SmallProducerSubmissionContext(Order = 1)]
        [SmallProducerSubmissionSubmitted(Order = 2)]
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
                    TaskLinkName = "Represented organisation details",
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
        [SmallProducerSubmissionSubmitted(Order = 2)]
        [SmallProducerSubmissionContext(Order = 1)]
        public async Task<ActionResult> CheckAnswers()
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>(source);

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> Submissions(int? year = null)
        {
            if (SmallProducerSubmissionData.AnySubmissionsToDisplay == false && year.HasValue)
            {
                return RedirectToOrganisationHasNoSubmissions();
            }

            submissionService.WithSubmissionData(this.SmallProducerSubmissionData);

            var model = await submissionService.Submissions(year);

            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> OrganisationDetails(int? year = null)
        {
            if (SmallProducerSubmissionData.AnySubmissionsToDisplay == false && year.HasValue)
            {
                return RedirectToOrganisationHasNoSubmissions();
            }

            submissionService.WithSubmissionData(this.SmallProducerSubmissionData);

            var model = await submissionService.OrganisationDetails(year);

            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ContactDetails(int? year = null)
        {
            if (SmallProducerSubmissionData.AnySubmissionsToDisplay == false && year.HasValue)
            {
                return RedirectToOrganisationHasNoSubmissions();
            }

            submissionService.WithSubmissionData(this.SmallProducerSubmissionData);

            var model = await submissionService.ContactDetails(year);
            
            return View("Producer/ViewOrganisation/ContactDetails", model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ServiceOfNoticeDetails(int? year = null)
        {
            if (SmallProducerSubmissionData.AnySubmissionsToDisplay == false && year.HasValue)
            {
                return RedirectToOrganisationHasNoSubmissions();
            }

            submissionService.WithSubmissionData(this.SmallProducerSubmissionData);

            var model = await submissionService.ServiceOfNoticeDetails(year);
          
            return View("Producer/ViewOrganisation/ServiceOfNoticeDetails", model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> RepresentedOrganisationDetails(int? year = null)
        {
            if (SmallProducerSubmissionData.AnySubmissionsToDisplay == false && year.HasValue)
            {
                return RedirectToOrganisationHasNoSubmissions();
            }

            submissionService.WithSubmissionData(this.SmallProducerSubmissionData);

            var model = await submissionService.RepresentedOrganisationDetails(year);
          
            return View("Producer/ViewOrganisation/RepresentedOrganisationDetails", model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> TotalEEEDetails(int? year = null)
        {
            if (SmallProducerSubmissionData.AnySubmissionsToDisplay == false && year.HasValue)
            {
                return RedirectToOrganisationHasNoSubmissions();
            }

            submissionService.WithSubmissionData(this.SmallProducerSubmissionData);

            var model = await submissionService.TotalEEEDetails(year);

            return View("Producer/ViewOrganisation/TotalEEEDetails", model);
        }

        [HttpGet]
        public ActionResult SubmitRegistration()
        {
            return View("SubmitRegistration");
        }

        [HttpGet]
        public ActionResult RegistrationSubmissions()
        {
            return View("SubmitRegistration");
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public ActionResult DownloadSubmission()
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>(source);

            model.IsPdfDownload = true;

            var content = templateExecutor.RenderRazorView(ControllerContext, "DownloadSubmission", model);

            var pdf = pdfDocumentProvider.GeneratePdfFromHtml(content);

            var timestamp = SystemTime.Now;
            var fileName = $"producer_submission{timestamp.ToString(DateTimeConstants.SubmissionTimestamp)}.pdf";

            return File(pdf, "application/pdf", fileName);
        }

        private Task SetHistoricBreadcrumb() => SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.HistoricProducerRegistrationSubmission);

        private ActionResult RedirectToOrganisationHasNoSubmissions()
        {
            return RedirectToAction("OrganisationHasNoSubmissions");
        }
    }
}