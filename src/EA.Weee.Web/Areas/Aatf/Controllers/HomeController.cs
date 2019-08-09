namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    public class HomeController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public HomeController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client)
        {
            this.apiClient = client;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, bool isAE)
        {
            using (var client = apiClient())
            {
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(organisationId));
                var selectedAatfsOrAes = new List<AatfData>();

                if (isAE)
                {
                    foreach (var aatf in allAatfsAndAes)
                    {
                        if (aatf.FacilityType == Core.AatfReturn.FacilityType.Ae)
                        {
                            selectedAatfsOrAes.Add(aatf);
                        }
                    }
                }
                else
                {
                    foreach (var aatf in allAatfsAndAes)
                    {
                        if (aatf.FacilityType == Core.AatfReturn.FacilityType.Aatf)
                        {
                            selectedAatfsOrAes.Add(aatf);
                        }
                    }
                }

                if (selectedAatfsOrAes.Count == 1)
                {
                    return RedirectToAction("Index", "ViewAatfContactDetails", new { organisationId = organisationId, aatfId = selectedAatfsOrAes[0].Id, isAE = isAE });
                }

                foreach (var aatf in selectedAatfsOrAes)
                {
                    aatf.AatfContactDetailsName = aatf.Name + " (" + aatf.ApprovalNumber + ") - " + aatf.AatfStatus;
                }

                var model = new HomeViewModel() { OrganisationId = organisationId, AatfList = selectedAatfsOrAes, IsAE = isAE };

                await SetBreadcrumb(model.OrganisationId, null, false);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "ViewAatfContactDetails", new { area = "Aatf", organisationId = model.OrganisationId, aatfId = model.SelectedAatfId, isAE = model.IsAE});
            }

            await SetBreadcrumb(model.OrganisationId, null, false);

            return View(model);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, bool setScheme = true)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            if (setScheme)
            {
                breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
            }
        }
    }
}