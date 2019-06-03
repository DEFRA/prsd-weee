namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateOrganisationActionFilter]
    [ValidateReturnCreatedActionFilter]
    public class SelectReportOptionsNilController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnDataToSelectReportOptionsNilViewModelMapTransfer, SelectReportOptionsNilViewModel> mapper;

        public SelectReportOptionsNilController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IMap<ReturnDataToSelectReportOptionsNilViewModelMapTransfer, SelectReportOptionsNilViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId)
        {
            using (var client = apiClient())
            {
                var returnData = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));
                var viewModel = mapper.Map(new ReturnDataToSelectReportOptionsNilViewModelMapTransfer()
                {
                    ReturnData = returnData,
                    OrganisationId = organisationId,
                    ReturnId = returnId
                });

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectReportOptionsNilViewModel viewModel)
        {
            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new SubmitReturn(viewModel.ReturnId));
            }
            return await Task.Run<ActionResult>(() => AatfRedirect.SubmittedReturn(viewModel.ReturnId));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}