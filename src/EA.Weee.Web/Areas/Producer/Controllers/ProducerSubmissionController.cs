namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.Mappings.ToRequest;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ProducerSubmissionController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly IMapper mapper;
        private readonly IRequestCreator<EditOrganisationDetailsViewModel, EditOrganisationDetailsRequest>
            editOrganisationDetailsRequestCreator;
        private readonly IRequestCreator<EditContactDetailsViewModel, EditContactDetailsRequest>
            editContactDetailsRequestCreator;
        private readonly IRequestCreator<RepresentingCompanyDetailsViewModel, RepresentedOrganisationDetailsRequest> editRepresentedOrganisationDetailsRequestCreator;
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IWeeeCache weeeCache;
        private readonly IRequestCreator<ServiceOfNoticeViewModel, ServiceOfNoticeRequest>
            serviceOfNoticeRequestCreator;
        private readonly IRequestCreator<EditEeeDataViewModel, EditEeeDataRequest> editEeeDataRequestCreator;
        private readonly IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryAndCompleteRequest>
            addSignatoryAndCompleteRequestCreator;
        private readonly IPaymentService paymentService;
        private readonly ConfigurationService configurationService;

        public ProducerSubmissionController(IMapper mapper,
            IRequestCreator<EditOrganisationDetailsViewModel, EditOrganisationDetailsRequest> editOrganisationDetailsRequestCreator,
            IRequestCreator<RepresentingCompanyDetailsViewModel, RepresentedOrganisationDetailsRequest> editRepresentedOrganisationDetailsRequestCreator,
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumbService,
            IWeeeCache weeeCache,
            IRequestCreator<EditContactDetailsViewModel, EditContactDetailsRequest>
                editContactDetailsRequestCreator,
            IRequestCreator<ServiceOfNoticeViewModel, ServiceOfNoticeRequest> serviceOfNoticeRequestCreator,
            IRequestCreator<EditEeeDataViewModel, EditEeeDataRequest> editEeeDataRequestCreator,
            IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryAndCompleteRequest> addSignatoryRequestCreator, 
            IPaymentService paymentService,
            ConfigurationService configuration)
        {
            this.mapper = mapper;
            this.editOrganisationDetailsRequestCreator = editOrganisationDetailsRequestCreator;
            this.editRepresentedOrganisationDetailsRequestCreator = editRepresentedOrganisationDetailsRequestCreator;
            this.apiClient = apiClient;
            this.breadcrumbService = breadcrumbService;
            this.weeeCache = weeeCache;
            this.editContactDetailsRequestCreator = editContactDetailsRequestCreator;
            this.serviceOfNoticeRequestCreator = serviceOfNoticeRequestCreator;
            this.editEeeDataRequestCreator = editEeeDataRequestCreator;
            this.addSignatoryAndCompleteRequestCreator = addSignatoryRequestCreator;
            this.paymentService = paymentService;
            this.configurationService = configuration;
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumbService.ExternalOrganisation = await weeeCache.FetchOrganisationName(organisationId);
            breadcrumbService.ExternalActivity = activity;
            breadcrumbService.OrganisationId = organisationId;
        }

        [HttpGet]
        [SmallProducerSubmissionContext(Order = 1)]
        [SmallProducerSubmissionSubmitted(Order = 2)]
        public async Task<ActionResult> EditOrganisationDetails(bool? redirectToCheckAnswers = false)
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                RedirectToCheckAnswers = redirectToCheckAnswers,
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, EditOrganisationDetailsViewModel>(source);

            var countries = await GetCountries();

            model.Organisation.Address.Countries = countries;

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditOrganisationDetails(EditOrganisationDetailsViewModel model)
        {
            var castedModel = model.Organisation.CastToSpecificViewModel(model.Organisation);
            var isValid = ValidationModel.ValidateModel(castedModel, ModelState, nameof(EditOrganisationDetailsViewModel.Organisation));

            if (ModelState.IsValid && isValid)
            {
                var request = editOrganisationDetailsRequestCreator.ViewModelToRequest(model);

                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), request);
                }

                if (model.RedirectToCheckAnswers == true)
                {
                    return RedirectToAction(nameof(ProducerController.CheckAnswers),
                    typeof(ProducerController).GetControllerName());
                }
                return RedirectToAction(nameof(ProducerController.TaskList),
                    typeof(ProducerController).GetControllerName());
            }

            await SetBreadcrumb(model.OrganisationId, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
            var countries = await GetCountries();

            model.Organisation.Address.Countries = countries;

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext(Order = 1)]
        [SmallProducerSubmissionSubmitted(Order = 2)]
        public async Task<ActionResult> EditEeeeData(bool? redirectToCheckAnswers = false)
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                RedirectToCheckAnswers = redirectToCheckAnswers,
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, EditEeeDataViewModel>(source);

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEeeeData(EditEeeDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = editEeeDataRequestCreator.ViewModelToRequest(model);

                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), request);
                }

                if (model.RedirectToCheckAnswers == true)
                {
                    return RedirectToAction(nameof(ProducerController.CheckAnswers),
                    typeof(ProducerController).GetControllerName());
                }
                return RedirectToAction(nameof(ProducerController.TaskList),
                    typeof(ProducerController).GetControllerName());
            }

            await SetBreadcrumb(model.OrganisationId, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext(Order = 1)]
        [SmallProducerSubmissionSubmitted(Order = 2)]
        public async Task<ActionResult> ServiceOfNotice(bool? sameAsOrganisationAddress, bool? redirectToCheckAnswers = false)
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                RedirectToCheckAnswers = redirectToCheckAnswers,
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>(source);

            var countries = await GetCountries();
            model.Address.Countries = countries;

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            model.SameAsOrganisationAddress = sameAsOrganisationAddress ?? false;

            if (model.SameAsOrganisationAddress)
            {
                var organisationAddress = SmallProducerSubmissionData.CurrentSubmission.BusinessAddressData ??
                                          SmallProducerSubmissionData.OrganisationData.BusinessAddress;
                model.Address = new ServiceOfNoticeAddressData
                {
                    Address1 = organisationAddress.Address1,
                    Address2 = organisationAddress.Address2,
                    TownOrCity = organisationAddress.TownOrCity,
                    Postcode = organisationAddress.Postcode,
                    CountyOrRegion = organisationAddress.CountyOrRegion,
                    Countries = countries,
                    CountryId = organisationAddress.CountryId,
                    Telephone = model.Address.Telephone,
                };
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ServiceOfNotice(ServiceOfNoticeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = serviceOfNoticeRequestCreator.ViewModelToRequest(model);

                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), request);
                }

                if (model.RedirectToCheckAnswers == true)
                {
                    return RedirectToAction(nameof(ProducerController.CheckAnswers),
                    typeof(ProducerController).GetControllerName());
                }
                return RedirectToAction(nameof(ProducerController.TaskList),
                    typeof(ProducerController).GetControllerName());
            }

            await SetBreadcrumb(model.OrganisationId, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            var countries = await GetCountries();
            model.Address.Countries = countries;

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext(Order = 1)]
        [SmallProducerSubmissionSubmitted(Order = 2)]
        public async Task<ActionResult> EditRepresentedOrganisationDetails(bool? redirectToCheckAnswers = false)
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                RedirectToCheckAnswers = redirectToCheckAnswers,
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(source);

            model.Address.Countries = await GetCountries();

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRepresentedOrganisationDetails(RepresentingCompanyDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = editRepresentedOrganisationDetailsRequestCreator.ViewModelToRequest(model);

                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), request);
                }

                if (model.RedirectToCheckAnswers == true)
                {
                    return RedirectToAction(nameof(ProducerController.CheckAnswers),
                    typeof(ProducerController).GetControllerName());
                }
                return RedirectToAction(nameof(ProducerController.TaskList), typeof(ProducerController).GetControllerName());
            }

            await SetBreadcrumb(model.OrganisationId, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            model.Address.Countries = await GetCountries();

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext(Order = 1)]
        [SmallProducerSubmissionSubmitted(Order = 2)]
        public async Task<ActionResult> EditContactDetails(bool? redirectToCheckAnswers = false)
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                RedirectToCheckAnswers = redirectToCheckAnswers,
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model =
                mapper.Map<SmallProducerSubmissionMapperData, EditContactDetailsViewModel>(source);

            var countries = await GetCountries();

            model.ContactDetails.AddressData.Countries = countries;

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditContactDetails(EditContactDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = editContactDetailsRequestCreator.ViewModelToRequest(model);

                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), request);
                }

                if (model.RedirectToCheckAnswers == true)
                {
                    return RedirectToAction(nameof(ProducerController.CheckAnswers),
                    typeof(ProducerController).GetControllerName());
                }
                return RedirectToAction(nameof(ProducerController.TaskList),
                    typeof(ProducerController).GetControllerName());
            }

            await SetBreadcrumb(model.OrganisationId, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
            var countries = await GetCountries();

            model.ContactDetails.AddressData.Countries = countries;

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionSubmitted(Order = 2)]
        [SmallProducerSubmissionContext(Order = 1)]
        public async Task<ActionResult> AppropriateSignatory()
        {
            var model = mapper.Map<SmallProducerSubmissionData, AppropriateSignatoryViewModel>(SmallProducerSubmissionData);

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> AppropriateSignatory(AppropriateSignatoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = addSignatoryAndCompleteRequestCreator.ViewModelToRequest(model);

                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), request);

                    await weeeCache.InvalidateOrganisationNameCache(model.OrganisationId);

                    await weeeCache.InvalidateOrganisationSearch();
                }

                return await RedirectToNextUrl();
            }

            await SetBreadcrumb(model.OrganisationId, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        private async Task<ActionResult> RedirectToNextUrl()
        {
            var existingPaymentInProgress = await paymentService.CheckInProgressPaymentAsync(User.GetAccessToken(),
                            SmallProducerSubmissionData.DirectRegistrantId);

            string nextUrl;
            if (existingPaymentInProgress == null)
            {
                var result = await paymentService.CreatePaymentAsync(SmallProducerSubmissionData.DirectRegistrantId,
                    User.GetEmailAddress(), User.GetAccessToken());

                nextUrl = result.Links.NextUrl.Href;
            }
            else
            {
                if (existingPaymentInProgress.State.Status == PaymentStatus.Success)
                {
                    return RedirectToAction(nameof(ProducerController.AlreadySubmittedAndPaid), typeof(ProducerController).GetControllerName());
                }

                nextUrl = existingPaymentInProgress.Links.NextUrl.Href;
            }

            if (paymentService.ValidateExternalUrl(nextUrl))
            {
                return Redirect(nextUrl);
            }

            throw new InvalidOperationException("Invalid payment next url");
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> PaymentSuccess(string reference)
        {
            var model = new PaymentResultModel()
            {
                PaymentReference = reference,
                OrganisationId = SmallProducerSubmissionData.OrganisationData.Id,
                ComplianceYear = SmallProducerSubmissionData.CurrentSubmission.ComplianceYear,
                TotalAmount = configurationService.CurrentConfiguration.GovUkPayAmountInPence / 100
            };

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public ActionResult PaymentFailure()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("PaymentFailure")]
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> RetryPayment()
        {
            return await RedirectToNextUrl();
        }

        public ActionResult BackToPrevious(bool? redirectToCheckAnswers)
        {
            if (redirectToCheckAnswers == true)
            {
                return RedirectToAction(nameof(ProducerController.CheckAnswers),
                    typeof(ProducerController).GetControllerName());
            }
            else
            {
                return RedirectToAction(nameof(ProducerController.TaskList),
                    typeof(ProducerController).GetControllerName());
            }
        }
        private async Task<IList<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }
        }
    }
}