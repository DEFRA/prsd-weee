namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Requests.Base;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerSubmissionController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;

        private readonly IMap<SmallProducerSubmissionData, EditOrganisationDetailsViewModel>
            editOrganisationDetailsMapper;

        private readonly IRequestCreator<EditOrganisationDetailsViewModel, EditProducerSubmissionAddressRequest>
            editOrganisationDetailsRequestCreator;

        public ProducerSubmissionController(IMap<SmallProducerSubmissionData, EditOrganisationDetailsViewModel> editOrganisationDetailsMapper, IRequestCreator<EditOrganisationDetailsViewModel, EditProducerSubmissionAddressRequest> editOrganisationDetailsRequestCreator)
        {
            this.editOrganisationDetailsMapper = editOrganisationDetailsMapper;
            this.editOrganisationDetailsRequestCreator = editOrganisationDetailsRequestCreator;
        }

        [HttpGet]
        public ActionResult EditOrganisationDetails()
        {
            var model = editOrganisationDetailsMapper.Map(SmallProducerSubmissionData);

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
    }
}