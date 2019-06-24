namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Api.Client;
    using Constant;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ReturnsSummaryController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public ReturnsSummaryController(Func<IWeeeClient> apiClient,
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
        public virtual async Task<ActionResult> Index(Guid returnId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, true));

                var viewModel = mapper.Map<ReturnViewModel>(@return);

                viewModel.OrganisationId = @return.OrganisationData.Id;

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn);

                return View("Index", viewModel);
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
