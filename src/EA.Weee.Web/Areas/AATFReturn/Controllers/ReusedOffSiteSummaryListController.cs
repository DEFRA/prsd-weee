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
            var returnViewModel = new ReturnViewModel();
            using (var client = apiClient())
            {
                var sites = await client.SendAsync(User.GetAccessToken(), new GetAatfSite(aatfId, returnId));
                viewModel = mapper.Map<ReusedOffSiteSummaryListViewModel>(sites);

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));
                returnViewModel = mapper.Map<ReturnViewModel>(@return);

                if (returnViewModel.AatfsData != null && returnViewModel.AatfsData.Select(a => a.Aatf.Id).Contains(aatfId))
                {
                    viewModel.B2bTotal = returnViewModel.AatfsData.Where(a => a.Aatf.Id == aatfId).FirstOrDefault().WeeeReused.B2B;
                    viewModel.B2cTotal = returnViewModel.AatfsData.Where(a => a.Aatf.Id == aatfId).FirstOrDefault().WeeeReused.B2C;
                }
                else
                {
                    viewModel.B2bTotal = "-";
                    viewModel.B2cTotal = "-";
                }
            }

            viewModel.OrganisationId = organisationId;
            viewModel.ReturnId = returnId;
            viewModel.AatfId = aatfId;

            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReusedOffSiteSummaryListViewModel viewModel)
        {
            return await Task.Run<ActionResult>(() =>
                RedirectToAction("Index", "AatfTaskList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}