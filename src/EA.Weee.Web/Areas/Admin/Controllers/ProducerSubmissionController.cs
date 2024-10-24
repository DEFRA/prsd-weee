namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Authorization;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
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

        public ProducerSubmissionController(
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
    }
}