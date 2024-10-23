﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Admin;

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

            return View("Producer/ViewOrganisation/OrganisationDetails", model);
        }

        [AdminSmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> OrganisationDetails(string registrationNumber, int? year = null)
        {
            submissionService.WithSubmissionData(this.SmallProducerSubmissionData, true);

            var model = await submissionService.OrganisationDetails(year);

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
        public async Task<ActionResult> RemoveSubmission(Guid registeredProducerId)
        {
            ProducerDetailsScheme producerDetailsScheme = await FetchProducerDetailsScheme(registeredProducerId);

            if (!producerDetailsScheme.CanRemoveProducer)
            {
                return new HttpForbiddenResult();
            }

            return View(new ConfirmRemovalViewModel
            {
                Producer = producerDetailsScheme
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveSubmission(ConfirmRemovalViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                if (viewModel.SelectedValue == "Yes")
                {
                    //soft remove
                    return RedirectToAction(nameof(ProducerSubmissionController.Removed),
                        new { viewModel.Producer.RegistrationNumber });
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

        [HttpGet]
        public async Task<ActionResult> Removed(string registrationNumber)
        {
            return View(new RemovedViewModel
            {
                RegistrationNumber = registrationNumber
            });
        }

        private ActionResult RedirectToOrganisationHasNoSubmissions()
        {
            return RedirectToAction("OrganisationHasNoSubmissions");
        }

        private async Task<ProducerDetailsScheme> FetchProducerDetailsScheme(Guid registeredProducerId)
        {
            using (IWeeeClient client = apiClient())
            {
                GetProducerDetailsByRegisteredProducerId request = new GetProducerDetailsByRegisteredProducerId(registeredProducerId);
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }
    }
}