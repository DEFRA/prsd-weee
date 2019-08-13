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
    using EA.Weee.Web.Areas.AatfReturn.Requests;
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
        private readonly IDeleteReturnDataRequestCreator requestCreator;

        public SelectReportOptionsNilController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IMap<ReturnDataToSelectReportOptionsNilViewModelMapTransfer, SelectReportOptionsNilViewModel> mapper,
            IDeleteReturnDataRequestCreator requestCreator)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
            this.requestCreator = requestCreator;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId)
        {
            using (var client = apiClient())
            {
                var returnData = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));
                var viewModel = mapper.Map(new ReturnDataToSelectReportOptionsNilViewModelMapTransfer()
                {
                    ReturnData = returnData,
                    OrganisationId = organisationId,
                    ReturnId = returnId
                });

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(returnData.Quarter, returnData.QuarterWindow));

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectReportOptionsNilViewModel viewModel)
        {
            using (var client = apiClient())
            {
                var request = requestCreator.ViewModelToRequest(viewModel);
                await client.SendAsync(User.GetAccessToken(), request);
                await client.SendAsync(User.GetAccessToken(), new SubmitReturn(viewModel.ReturnId, true));
            }
            return await Task.Run<ActionResult>(() => AatfRedirect.SubmittedReturn(viewModel.ReturnId));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, string quarter)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            breadcrumb.QuarterDisplayInfo = quarter;
        }
    }
}