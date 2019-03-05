namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public class ReusedOffSiteSummaryListController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ReusedOffSiteSummaryListController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId)
        {
            var viewModel = new ReusedOffSiteSummaryListViewModel();

            using (var client = apiClient())
            {
                var sites = await client.SendAsync(User.GetAccessToken(), new GetAatfSite(aatfId, returnId));
                viewModel = mapper.Map<ReusedOffSiteSummaryListViewModel>(sites);

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));
                var mappedReturn = mapper.Map<ReturnViewModel>(@return);

                viewModel.B2bTotal = mappedReturn.AatfsData.First().WeeeReused.B2B;
                viewModel.B2cTotal = mappedReturn.AatfsData.First().WeeeReused.B2C;
            }

            viewModel.OrganisationId = organisationId;
            viewModel.ReturnId = returnId;
            viewModel.AatfId = aatfId;

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