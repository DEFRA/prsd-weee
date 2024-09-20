﻿namespace EA.Weee.Web.Areas.Producer.Controllers
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
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
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
        private readonly IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryRequest> 
            addSignatoryRequestCreator;

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
            IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryRequest> addSignatoryRequestCreator)
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
            this.addSignatoryRequestCreator = addSignatoryRequestCreator;
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumbService.ExternalOrganisation = await weeeCache.FetchOrganisationName(organisationId);
            breadcrumbService.ExternalActivity = activity;
            breadcrumbService.OrganisationId = organisationId;
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
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
        [SmallProducerSubmissionContext]
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
        [SmallProducerSubmissionContext]
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
                AddressData organisationAddress;
                if (SmallProducerSubmissionData.CurrentSubmission.BusinessAddressData != null)
                {
                    organisationAddress = SmallProducerSubmissionData.CurrentSubmission.BusinessAddressData;
                }
                else
                {
                    organisationAddress = SmallProducerSubmissionData.OrganisationData.BusinessAddress;
                }
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
        [SmallProducerSubmissionContext]
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

            var countries = await GetCountries();

            model.Address.Countries = await GetCountries();

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
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
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> AppropriateSignatory()
        {
            var model =
                mapper.Map<SmallProducerSubmissionData, AppropriateSignatoryViewModel>(SmallProducerSubmissionData);

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View();
        }

        [HttpPost]
        [SmallProducerSubmissionContext]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AppropriateSignatory(AppropriateSignatoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = addSignatoryRequestCreator.ViewModelToRequest(model);

                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), request);
                }

                return RedirectToAction(nameof(ProducerController.TaskList),
                    typeof(ProducerController).GetControllerName());
            }

            await SetBreadcrumb(model.OrganisationId, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
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