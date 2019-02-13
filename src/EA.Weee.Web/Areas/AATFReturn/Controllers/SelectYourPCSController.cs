namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
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

    public class SelectYourPCSController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private List<SchemeData> schemeList;

        public SelectYourPCSController(Func<IWeeeClient> apiclient, BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.apiClient = apiclient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId)
        {
            var viewModel = new SelectYourPCSViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = returnId
            };

            using (var client = apiClient())
            {
                viewModel.SchemeList = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal());
            }
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectYourPCSViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    if (viewModel.SelectedSchemes == null)
                    {
                        viewModel.SelectedSchemes = new List<Guid>();
                    }

                    foreach (var scheme in viewModel.SelectedSchemes)
                    {
                        var returnSchemeRequest = new AddReturnScheme()
                        {
                            SchemeId = scheme,
                            ReturnId = viewModel.ReturnId
                        };

                        await client.SendAsync(User.GetAccessToken(), returnSchemeRequest);
                    }
                }
                return RedirectToAction("Index", "AatfTaskList",
                                            new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId });
            }
            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);
            return View(viewModel);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}