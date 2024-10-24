namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Web.Filters;

    public class ProducerSubmissionController : AdminController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly BreadcrumbService breadcrumb;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;
        private readonly ISubmissionService submissionService;

        public ProducerSubmissionController(
            BreadcrumbService breadcrumb,
            Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            IMapper mapper,
            IMvcTemplateExecutor templateExecutor,
            IPdfDocumentProvider pdfDocumentProvider,
            ISubmissionService submissionService)
        {
            this.breadcrumb = breadcrumb;
            this.apiClient = apiClient;
            this.cache = cache;
            this.mapper = mapper;
            this.templateExecutor = templateExecutor;
            this.pdfDocumentProvider = pdfDocumentProvider;
            this.submissionService = submissionService;
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> Submissions(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(this.SmallProducerSubmissionData, true);

            var model = await submissionService.Submissions(year);

            model.RegistrationNumber = registrationNumber;

            model.IsAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);
            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> OrganisationDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(this.SmallProducerSubmissionData, true);

            var model = await submissionService.OrganisationDetails(year);

            model.IsAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);
            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ContactDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(this.SmallProducerSubmissionData, true);

            var model = await submissionService.ContactDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/ContactDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ServiceOfNoticeDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(this.SmallProducerSubmissionData, true);

            var model = await submissionService.ServiceOfNoticeDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/ServiceOfNoticeDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> RepresentedOrganisationDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(this.SmallProducerSubmissionData, true);

            var model = await submissionService.RepresentedOrganisationDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/RepresentedOrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> TotalEEEDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(this.SmallProducerSubmissionData, true);

            var model = await submissionService.TotalEEEDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/TotalEEEDetails", model);
        }

        [HttpGet]
        public async Task<ActionResult> AddPaymentDetails()
        {
            return View();
        }

        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult RemoveSubmission(string registrationNumber, int year)
        {
            var submission = this.SmallProducerSubmissionData.SubmissionHistory[year];
            var selectedValue = string.Empty;
            var model = new ConfirmRemovalViewModel
            {
                SelectedValue = selectedValue,
                Producer = new ProducerDetailsScheme
                {
                    ComplianceYear = submission.ComplianceYear,
                    ProducerName = submission.CompanyName,
                    RegistrationNumber = registrationNumber,
                    RegisteredProducerId = submission.RegisteredProducerId
                }
            };

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveSubmission(ConfirmRemovalViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                if (viewModel.SelectedValue == "Yes")
                {
                    RemoveSmallProducerResult result;
                    using (IWeeeClient client = apiClient())
                    {
                        result = await client.SendAsync(User.GetAccessToken(), new RemoveSmallProducer(viewModel.Producer.RegisteredProducerId));
                    }

                    if (result.InvalidateProducerSearchCache)
                    {
                        await cache.InvalidateProducerSearch();
                    }

                    return RedirectToAction(nameof(ProducerSubmissionController.Removed),
                        new { registrationNumber = viewModel.Producer.RegistrationNumber, producerName = viewModel.Producer.ProducerName, year = viewModel.Producer.ComplianceYear });
                }
                else
                {
                    return RedirectToAction(nameof(ProducerSubmissionController.Submissions),
                        new { viewModel.Producer.RegistrationNumber });
                }
            }
            else
            {
                return this.View(viewModel);
            }
        }

        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [HttpGet]
        public ActionResult Removed(string registrationNumber, string producerName, int year)
        {
            return View(new RemovedViewModel
            {
                RegistrationNumber = registrationNumber,
                ProducerName = producerName,
                ComplianceYear = year
            });
        }

        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Removed(RemovedViewModel model)
        {
            using (IWeeeClient client = apiClient())
            {
                this.SmallProducerSubmissionData = await client.SendAsync(User.GetAccessToken(), new GetSmallProducerSubmissionByRegistrationNumber(model.RegistrationNumber));
                
                if (SmallProducerSubmissionData.AnySubmissionSubmitted)
                {
                    return RedirectToAction(nameof(ProducerSubmissionController.Submissions),
                        new { model.RegistrationNumber });
                }

                return RedirectToOrganisationHasNoSubmissions();
            }
        }

        private ActionResult RedirectToOrganisationHasNoSubmissions()
        {
            return RedirectToAction("OrganisationHasNoSubmissions");
        }
    }
}