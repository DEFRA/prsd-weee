namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
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

    public class ViewAATFContactDetailsListController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid orgId)
        {
            using (var client = apiClient())
            {
                var aatfs = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(orgId));

                if (aatfs.Count > 1)
                {
                    // Redirect to view contact details
                }

                var activities = new List<string>();

                foreach (var aatf in aatfs)
                {
                    activities.Add(aatf.Name + "(" + aatf.ApprovalNumber + ") - " + aatf.AatfStatus);
                }

                var model = new ViewAATFContactDetailsListViewModel(activities) { OrganisationId = orgId, AatfList = aatfs };

                await SetBreadcrumb(model.OrganisationId, null, false);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ViewAATFContactDetailsListViewModel model)
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