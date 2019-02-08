namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
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
        public virtual async Task<ActionResult> Index(Guid organisationId)
        {
            var viewModel = new SelectYourPCSViewModel();

            using (var client = apiClient())
            {
                var schemeRequest = new GetSchemes();
                schemeList = await client.SendAsync(User.GetAccessToken(), schemeRequest);
                viewModel.SchemeList = schemeList;
            }
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

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