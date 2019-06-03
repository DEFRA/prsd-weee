namespace EA.Weee.Web.Areas.AeReturn.Controllers
{
    using AutoMapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class AeReturnsController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public AeReturnsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId)
        {
            using (var client = apiClient())
            {
                /*var @return = await client.SendAsync(User.GetAccessToken(), new GetReturns(organisationId));

                var viewModel = mapper.Map<ReturnsViewModel>(@return);

                viewModel.OrganisationName = (await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId))).OrganisationName;
                viewModel.OrganisationId = organisationId;

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AeReturn);

                return View(viewModel);*/
                return View();
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