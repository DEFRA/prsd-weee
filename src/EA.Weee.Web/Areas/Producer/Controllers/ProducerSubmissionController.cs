namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
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
        private readonly IRequestCreator<EditOrganisationDetailsViewModel, EditProducerSubmissionAddressRequest>
            editOrganisationDetailsRequestCreator;
        private readonly IRequestCreator<ServiceOfNoticeViewModel, ServiceOfNoticeRequest>
            serviceOfNoticeRequestCreator;
        private readonly Func<IWeeeClient> apiClient;

        public ProducerSubmissionController(IMapper mapper, 
            IRequestCreator<EditOrganisationDetailsViewModel, EditProducerSubmissionAddressRequest> editOrganisationDetailsRequestCreator,
            IRequestCreator<ServiceOfNoticeViewModel, ServiceOfNoticeRequest> serviceOfNoticeRequestCreator,
            Func<IWeeeClient> apiClient)
        {
            this.mapper = mapper;
            this.editOrganisationDetailsRequestCreator = editOrganisationDetailsRequestCreator;
            this.serviceOfNoticeRequestCreator = serviceOfNoticeRequestCreator;
            this.apiClient = apiClient;
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public ActionResult EditOrganisationDetails()
        {
            var model = mapper.Map<SmallProducerSubmissionData, EditOrganisationDetailsViewModel>(SmallProducerSubmissionData);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrganisationDetails(EditOrganisationDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = editOrganisationDetailsRequestCreator.ViewModelToRequest(model);

                // send the request
            }

            return View(model);
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> ServiceOfNotice()
        {
            var model = mapper.Map<SmallProducerSubmissionData, ServiceOfNoticeViewModel>(SmallProducerSubmissionData);

            if (model.Address == null)
            {
                model.Address = new AddressPostcodeRequiredData();
            }

            var countries = await GetCountries();
            model.Address.Countries = countries;

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

                return RedirectToAction(nameof(ProducerController.TaskList),
                    typeof(ProducerController).GetControllerName());
            }

            return View(model);
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