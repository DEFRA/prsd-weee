namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.DirectRegistrant;
    using Core.PaymentDetails;
    using EA.Weee.Core.Admin;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
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
        private readonly BreadcrumbService breadcrumbService;
        private readonly Func<IWeeeClient> apiClient;

        public ProducerSubmissionController(
            Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            ISubmissionService submissionService,
            BreadcrumbService breadcrumbService)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.submissionService = submissionService;
            this.breadcrumbService = breadcrumbService;
            this.apiClient = apiClient;
        }

        [AdminSmallProducerSubmissionContext(Order = 1)]
        [AdminSmallProducerSubmissionSubmittedContext(Order = 2)]
        [HttpGet]
        public async Task<ActionResult> Submissions(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.Submissions(year);

            model.RegistrationNumber = registrationNumber;

            model.IsAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);
            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext(Order = 1)]
        [AdminSmallProducerSubmissionSubmittedContext(Order = 2)]
        [HttpGet]
        public async Task<ActionResult> OrganisationDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.OrganisationDetails(year);

            model.IsAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);
            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext(Order = 1)]
        [AdminSmallProducerSubmissionSubmittedContext(Order = 2)]
        [HttpGet]
        public async Task<ActionResult> ContactDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.ContactDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/ContactDetails", model);
        }

        [AdminSmallProducerSubmissionContext(Order = 1)]
        [AdminSmallProducerSubmissionSubmittedContext(Order = 2)]
        [HttpGet]
        public async Task<ActionResult> ServiceOfNoticeDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.ServiceOfNoticeDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/ServiceOfNoticeDetails", model);
        }

        [AdminSmallProducerSubmissionContext(Order = 1)]
        [AdminSmallProducerSubmissionSubmittedContext(Order = 2)]
        [HttpGet]
        public async Task<ActionResult> RepresentedOrganisationDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(SmallProducerSubmissionData, true);

            var model = await submissionService.RepresentedOrganisationDetails(year);

            model.RegistrationNumber = registrationNumber;

            return View("Producer/ViewOrganisation/RepresentedOrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext(Order = 1)]
        [AdminSmallProducerSubmissionSubmittedContext(Order = 2)]
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
            SetBreadcrumb();

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
        [AdminSmallProducerSubmissionContext(Order = 1)]
        [HttpGet]
        public ActionResult RemoveSubmission(string registrationNumber, int year)
        {
            SetBreadcrumb();

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
            SetBreadcrumb();

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
            SetBreadcrumb();

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

        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [AdminSmallProducerSubmissionContext(Order = 1)]
        [HttpGet]
        public ActionResult ReturnProducerRegistration(string registrationNumber, int year, Guid directProducerSubmissionId)
        {
            SetBreadcrumb();

            var submission = SmallProducerSubmissionData.SubmissionHistory[year];
            var selectedValue = string.Empty;
            var model = new ConfirmReturnViewModel
            {
                SelectedValue = selectedValue,
                Producer = new ProducerDetailsScheme
                {
                    ComplianceYear = year,
                    ProducerName = SmallProducerSubmissionData.HasAuthorisedRepresentitive ? SmallProducerSubmissionData.AuthorisedRepresentitiveData.CompanyName : submission.CompanyName,
                    RegistrationNumber = registrationNumber,
                    RegisteredProducerId = submission.RegisteredProducerId
                },
                DirectProducerSubmissionId = directProducerSubmissionId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReturnProducerRegistration(ConfirmReturnViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedValue == "Yes")
                {
                    using (IWeeeClient client = apiClient())
                    {
                        await client.SendAsync(User.GetAccessToken(), new ReturnSmallProducerSubmission(viewModel.DirectProducerSubmissionId));
                    }

                    return RedirectToAction(nameof(Returned),
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
        public ActionResult Returned(string registrationNumber, string producerName, int year)
        {
            return View(new ReturnedViewModel
            {
                RegistrationNumber = registrationNumber,
                ProducerName = producerName,
                ComplianceYear = year
            });
        }

        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Returned(ReturnedViewModel model)
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
        public ActionResult OrganisationHasNoSubmissions(Guid organisationId, bool? fromRemoved = false)
        {
            SetBreadcrumb();

            var model = new OrganisationIdViewModel()
            {
                OrganisationId = organisationId,
                DisplayBack = fromRemoved == false
            };

            return View(model);
        }

        private ActionResult RedirectToOrganisationHasNoSubmissions(Guid organisationId)
        {
            return RedirectToAction("OrganisationHasNoSubmissions", new { organisationId, fromRemoved = true});
        }

        private async Task<ManualPaymentResult> SendPaymentDetails(PaymentDetailsViewModel model)
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(),
                    new AddPaymentDetails(model.PaymentMethod, model.PaymentReceivedDate, model.PaymentDetailsDescription, model.DirectProducerSubmissionId));
            }
        }

        private void SetBreadcrumb()
        {
            breadcrumbService.InternalActivity = InternalUserActivity.DirectRegistrantDetails;
        }
    }
}