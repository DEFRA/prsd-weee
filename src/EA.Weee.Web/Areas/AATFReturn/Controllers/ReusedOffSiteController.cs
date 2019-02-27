namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public class ReusedOffSiteController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ReusedOffSiteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId)
        {
            var viewModel = new ReusedOffSiteViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = returnId
            };

            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReusedOffSiteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedValue == "Yes")
                {
                    return RedirectToAction("Index", "ReusedOffSiteCreateSite",
                                            new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId });
                }
                else
                {
                    return RedirectToAction("Index", "AatfTaskList",
                                            new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId });
                }
            }
            else
            {
                await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);
                return View(viewModel);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}