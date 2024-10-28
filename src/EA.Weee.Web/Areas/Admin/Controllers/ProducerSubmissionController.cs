namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.DirectRegistrant;
    using Core.PaymentDetails;
    using EA.Weee.Core.Admin;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Web.Areas.Producer.Filters;
    using Filters;
    using Infrastructure;
    using Security;
    using Services.Caching;
    using Services.SubmissionService;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Producers;

    public class ProducerSubmissionController : AdminController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly IWeeeCache cache;
        private readonly ISubmissionService submissionService;
        private readonly Func<IWeeeClient> apiClient;

        public ProducerSubmissionController(
            Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            ISubmissionService submissionService)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.submissionService = submissionService;
            this.apiClient = apiClient;
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> Submissions(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.Submissions(year);

            model.RegistrationNumber = registrationNumber;

            model.IsAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);
            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> OrganisationDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.OrganisationDetails(year);

            model.IsAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);
            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ContactDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.ContactDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/ContactDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ServiceOfNoticeDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.ServiceOfNoticeDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/ServiceOfNoticeDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> RepresentedOrganisationDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.RepresentedOrganisationDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/RepresentedOrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> TotalEEEDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.TotalEEEDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/TotalEEEDetails", model);
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public ActionResult AddPaymentDetails(Guid directProducerSubmissionId, string registrationNumber, int? year)
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

        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public ActionResult RemoveSubmission(string registrationNumber, int year)
        {
            var submission = SmallProducerSubmissionData.SubmissionHistory[year];
            var selectedValue = string.Empty;
            var model = new ConfirmRemovalViewModel
            {
                SelectedValue = selectedValue,
                Producer = new ProducerDetailsScheme
                {
                    ComplianceYear = year,
                    ProducerName = SmallProducerSubmissionData.HasAuthorisedRepresentitive ? SmallProducerSubmissionData.AuthorisedRepresentitiveData.CompanyName : submission.CompanyName,
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
            if (ModelState.IsValid)
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

                    return RedirectToAction(nameof(Removed),
                        new { registrationNumber = viewModel.Producer.RegistrationNumber, producerName = viewModel.Producer.ProducerName, year = viewModel.Producer.ComplianceYear });
                }
                else
                {
                    return RedirectToAction(nameof(Submissions),
                        new { viewModel.Producer.RegistrationNumber });
                }
            }
            else
            {
                return View(viewModel);
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
            using (var client = apiClient())
            {
                SmallProducerSubmissionData = await client.SendAsync(User.GetAccessToken(), new GetSmallProducerSubmissionByRegistrationNumber(model.RegistrationNumber));
                
                if (SmallProducerSubmissionData.AnySubmissions)
                {
                    return RedirectToAction(nameof(Submissions),
                        new { model.RegistrationNumber });
                }

                return RedirectToOrganisationHasNoSubmissions(SmallProducerSubmissionData.OrganisationData.Id);
            }
        }

        [HttpGet]
        public ActionResult OrganisationHasNoSubmissions(Guid organisationId)
        {
            var model = new OrganisationIdViewModel()
            {
                OrganisationId = organisationId
            };

            return View(model);
        }

        private ActionResult RedirectToOrganisationHasNoSubmissions(Guid organisationId)
        {
            return RedirectToAction("OrganisationHasNoSubmissions", new { organisationId });
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