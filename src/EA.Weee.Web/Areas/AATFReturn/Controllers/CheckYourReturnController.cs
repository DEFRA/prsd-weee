namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Organisations;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Web.OAuth;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.AatfReturn.NonObligated;

    public class CheckYourReturnController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public CheckYourReturnController(Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb, 
            IMapper mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid organisationId)
        { 
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var mappedView = mapper.Map<ReturnViewModel>(@return);

                return View("Index", mappedView);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(CheckYourReturnViewModel viewModel)
        {
            return await Task.Run<ActionResult>(() => 
                RedirectToAction("Index", "SubmittedReturn", new { area  = "AatfReturn", organisationId = RouteData.Values["organisationId"], returnId = RouteData.Values["returnId"] }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}
