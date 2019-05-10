namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Organisations;

    public class ReturnsController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ReturnsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMapper mapper)
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
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturns(organisationId));

                var viewModel = mapper.Map<ReturnsViewModel>(@return);

                viewModel.OrganisationName = (await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId))).OrganisationName;
                viewModel.OrganisationId = organisationId;

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReturnsViewModel model)
        {
            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new AddDefaultAatf() { OrganisationId = model.OrganisationId });

                var aatfReturnId = await client.SendAsync(User.GetAccessToken(), new AddReturn() { OrganisationId = model.OrganisationId });

                return AatfRedirect.SelectReportOptions(model.OrganisationId, aatfReturnId);
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