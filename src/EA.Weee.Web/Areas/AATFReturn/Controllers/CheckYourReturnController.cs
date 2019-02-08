namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Organisations;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;

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
                var orgResult = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId));

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var mappedView = mapper.Map<ReturnViewModel>(@return);

                mappedView.OrganisationName = orgResult.Name;

                return View("Index", mappedView);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReturnViewModel viewModel)
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
