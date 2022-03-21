namespace EA.Weee.Web.Areas.AatfEvidence.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfEvidence.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfEvidence.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class SelectYourAatfController : AatfEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<AatfDataToSelectYourAatfViewModelMapTransfer, SelectYourAatfViewModel> mapper;

        public SelectYourAatfController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client, IMap<AatfDataToSelectYourAatfViewModelMapTransfer, SelectYourAatfViewModel> mapper)
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

                var model = mapper.Map(new AatfDataToSelectYourAatfViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = organisationId, FacilityType = facilityType });

                if (model.AatfList.Count == 1)
                {
                    return RedirectToAction("Index", "ContactDetails", new { organisationId, aatfId = model.AatfList[0].Id, facilityType });
                }

                await SetBreadcrumb(model.OrganisationId, string.Format(BreadCrumbConstant.AatfEvidence, facilityType.ToDisplayString()));

                return View(model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}