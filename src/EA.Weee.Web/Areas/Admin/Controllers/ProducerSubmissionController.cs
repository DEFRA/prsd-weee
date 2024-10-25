namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.PaymentDetails;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Authorization;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ProducerSubmissionController : AdminController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;
        private readonly ISubmissionService submissionService;
        private readonly Func<IWeeeClient> apiClient;
        public ProducerSubmissionController(
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IMapper mapper,
            IMvcTemplateExecutor templateExecutor,
            IPdfDocumentProvider pdfDocumentProvider,
            ISubmissionService submissionService,
            Func<IWeeeClient> apiClient)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
            this.templateExecutor = templateExecutor;
            this.pdfDocumentProvider = pdfDocumentProvider;
            this.submissionService = submissionService;
            this.apiClient = apiClient;
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
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> AddPaymentDetails(Guid directProducerSubmissionId, string registrationNumber, int? year)
        {
            var model = new PaymentDetailsViewModel
            {
                DirectProducerSubmissionId = directProducerSubmissionId,
                RegistrationNumber = registrationNumber,
                Year = year
            };

            return View(model);
        }

        [HttpPost]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPaymentDetails(PaymentDetailsViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var details = await SendPaymentDetails(model);

            return RedirectToAction(nameof(OrganisationDetails), new { registrationNumber = details.RegistrationNumber, year = details.ComplianceYear });
        }

        [HttpGet]
        public async Task<ActionResult> RemoveSubmission()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ReturnProducerRegistration()
        {
            return View();
        }

        private ActionResult RedirectToOrganisationHasNoSubmissions()
        {
            return RedirectToAction("OrganisationHasNoSubmissions");
        }

        private async Task<ManualPaymentResult> SendPaymentDetails(PaymentDetailsViewModel model)
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(),
                    new AddPaymentDetails(model.PaymentMethod, model.PaymentReceivedDate, model.PaymentDetailsDescription, model.DirectProducerSubmissionId));
            }
        }
    }
}