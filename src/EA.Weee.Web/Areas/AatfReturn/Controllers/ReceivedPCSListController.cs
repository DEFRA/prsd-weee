namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Attributes;
    using Constant;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;

    [ValidateReturnCreatedActionFilter]
    public class ReceivedPcsListController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel> mapper;

        public ReceivedPcsListController(Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb,
            IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid returnId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                var schemeList = await client.SendAsync(User.GetAccessToken(), new GetReturnScheme(returnId));

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var viewModel = mapper.Map(new ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer()
                {
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = schemeList.OrganisationData.Id,
                    AatfName = (await cache.FetchAatfData(schemeList.OrganisationData.Id, aatfId)).Name,
                    ReturnData = @return,
                    SchemeDataItems = schemeList.SchemeDataItems.ToList()
                });

                await SetBreadcrumb(schemeList.OrganisationData.Id, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.FormatQuarter(@return.Quarter, @return.QuarterWindow));
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReceivedPcsListViewModel viewModel)
        {
            return await Task.Run(() => RedirectToAction("Index", "AatfTaskList", new { area = "AatfReturn", returnId = viewModel.ReturnId, organisationid = viewModel.OrganisationId }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, Guid aatfId, string quarter)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            var aatfInfo = await cache.FetchAatfData(organisationId, aatfId);
            breadcrumb.AatfDisplayInfo = DisplayHelper.ReportingOnValue(aatfInfo.Name, aatfInfo.ApprovalNumber, quarter);
        }
    }
}