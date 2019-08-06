namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using EA.Weee.Api.Client;
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

    public class ViewAatfContactDetails : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ViewAatfContactDetails(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client)
        {
            this.apiClient = client;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                var model = new ViewAatfContactDetailsViewModel() { OrganisationId = organisationId, AatfId = aatfId };

                await SetBreadcrumb(model.OrganisationId, null, false);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ViewAatfContactDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "ViewAATFContactDetailsList", new { });
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