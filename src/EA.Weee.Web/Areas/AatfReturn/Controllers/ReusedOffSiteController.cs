namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;

    [ValidateReturnCreatedActionFilter]
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
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId)
        {
            var viewModel = new ReusedOffSiteViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = returnId,
                AatfId = aatfId,
                AatfName = (await cache.FetchAatfData(organisationId, aatfId)).Name
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
                    return AatfRedirect.ReusedOffSiteCreate(viewModel.ReturnId, viewModel.AatfId, viewModel.OrganisationId);
                }
                else
                {
                    return AatfRedirect.TaskList(viewModel.ReturnId);
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
            breadcrumb.OrganisationId = organisationId;
        }
    }
}