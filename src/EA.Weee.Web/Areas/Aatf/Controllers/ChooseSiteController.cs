namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AatfEvidence.Controllers;
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using EA.Weee.Web.Extensions;
    using Infrastructure;
    using Mappings.ToViewModel;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Weee.Requests.AatfReturn;

    public class ChooseSiteController : AatfEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ChooseSiteController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client, IMapper mapper)
        {
            this.apiClient = client;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId)
        {
            using (var client = apiClient())
            {
                FacilityType facilityType = FacilityType.Aatf;
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(organisationId));

                var model = mapper.Map<SelectYourAatfViewModel>(new AatfDataToSelectYourAatfViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = organisationId, FacilityType = facilityType });

                if (model.AatfList.Count == 1)
                {
                    // TODO: Update with appropriate Manage evidence page once created
                    return RedirectToAction("Index", "Holding", new { organisationId });
                }

                await SetBreadcrumb(model.OrganisationId, string.Format(BreadCrumbConstant.AatfEvidence, facilityType.ToDisplayString()));

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectYourAatfViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Update with appropriate Manage evidence page once created
                return RedirectToAction("Index", "Holding", new { organisationId = model.OrganisationId });
            }

            using (var client = apiClient())
            {
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(model.OrganisationId));

                model = mapper.Map<SelectYourAatfViewModel>(new AatfDataToSelectYourAatfViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = model.OrganisationId, FacilityType = model.FacilityType});
            }

            if (!model.ModelValidated)
            {
                ModelState.RunCustomValidation(model);
            }

            await SetBreadcrumb(model.OrganisationId, string.Format(AatfAction.ManageAatfContactDetails, model.FacilityType.ToDisplayString()));

            return View(model);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}