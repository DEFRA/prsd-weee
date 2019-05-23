namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateOrganisationActionFilter]
    [ValidateReturnCreatedActionFilter]
    public class SelectYourPcsController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IAddReturnSchemeRequestCreator requestCreator;
        private readonly IWeeeCache cache;

        public SelectYourPcsController(Func<IWeeeClient> apiclient, BreadcrumbService breadcrumb, IWeeeCache cache, IAddReturnSchemeRequestCreator requestCreator)
        {
            this.apiClient = apiclient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.requestCreator = requestCreator;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId)
        {
            using (var client = apiClient())
            {
                var viewModel = new SelectYourPcsViewModel
                {
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    SchemeList = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal())
                };

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectYourPcsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var requests = requestCreator.ViewModelToRequest(viewModel);

                    foreach (var request in requests)
                    {
                        await client.SendAsync(User.GetAccessToken(), request);
                    }
                }

                return AatfRedirect.TaskList(viewModel.ReturnId);
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