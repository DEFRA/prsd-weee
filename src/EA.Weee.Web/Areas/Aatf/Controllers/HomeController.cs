namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfReturn;

    public class HomeController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel> mapper;

        public HomeController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client, IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel> mapper)
        {
            this.apiClient = client;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, FacilityType facilityType)
        {
            using (var client = apiClient())
            {
                var allAatfsAndAes = await cache.FetchAatfDataForOrganisationData(organisationId);

                var model = mapper.Map(new AatfDataToHomeViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = organisationId, FacilityType = facilityType });

                if (model.AatfList.Count == 1)
                {
                    return RedirectToAction("Index", "ContactDetails", new { organisationId, aatfId = model.AatfList[0].Id, facilityType });
                }

                await SetBreadcrumb(model.OrganisationId, string.Format(AatfAction.ManageAatfContactDetails, facilityType.ToDisplayString()));
               
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
               return RedirectToAction("Index", "ContactDetails", new { area = "Aatf", organisationId = model.OrganisationId, aatfId = model.SelectedId, facilityType = model.FacilityType });
            }

            using (var client = apiClient())
            {
                var allAatfsAndAes = await cache.FetchAatfDataForOrganisationData(model.OrganisationId);

                model = mapper.Map(new AatfDataToHomeViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = model.OrganisationId, FacilityType = model.FacilityType });
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