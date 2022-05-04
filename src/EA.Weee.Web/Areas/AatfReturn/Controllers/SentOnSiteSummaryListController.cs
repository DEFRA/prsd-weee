namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [ValidateReturnCreatedActionFilter]
    public class SentOnSiteSummaryListController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSentOnSummaryListViewModelMapTransfer, SentOnSiteSummaryListViewModel> mapper;

        public SentOnSiteSummaryListController(Func<IWeeeClient> apiClient, 
                                               BreadcrumbService breadcrumb, 
                                               IWeeeCache cache, 
                                               IMap<ReturnAndAatfToSentOnSummaryListViewModelMapTransfer, SentOnSiteSummaryListViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOn(aatfId, returnId, null));

                var model = mapper.Map(new ReturnAndAatfToSentOnSummaryListViewModelMapTransfer()
                {
                    WeeeSentOnDataItems = weeeSentOn,
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = organisationId,
                    AatfName = (await cache.FetchAatfData(organisationId, aatfId)).Name
                });

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                //Added Copy Previous Quater logic
                model.IsChkCopyPreviousQuarterVisible = IsChkCopyPreviousQuarterVisiable(weeeSentOn.Count);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SentOnSiteSummaryListViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOn(viewModel.AatfId, viewModel.ReturnId, null));

                    var model = mapper.Map(new ReturnAndAatfToSentOnSummaryListViewModelMapTransfer()
                    {
                        WeeeSentOnDataItems = weeeSentOn,
                        AatfId = viewModel.AatfId,
                        ReturnId = viewModel.ReturnId,
                        OrganisationId = viewModel.OrganisationId,
                        AatfName = (await cache.FetchAatfData(viewModel.OrganisationId, viewModel.AatfId)).Name
                    });

                    var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(viewModel.ReturnId, false));

                    await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, viewModel.AatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                    //Added Copy Previous Quater logic
                    model.IsChkCopyPreviousQuarterVisible = IsChkCopyPreviousQuarterVisiable(weeeSentOn.Count);

                    return View(model);
                }
            }
            else
            {
                using (var client = apiClient())
                {
                    var copyPreviousQuarterAatfDataViewModel = new CopyPreviousQuarterAatf()
                    {
                        AatfId = viewModel.AatfId,
                        OrganisationId = viewModel.OrganisationId,
                        ReturnId = viewModel.ReturnId
                    };
                    
                    var result = await client.SendAsync(User.GetAccessToken(), copyPreviousQuarterAatfDataViewModel);

                    return await Task.Run<ActionResult>(() =>
                                    RedirectToAction("Index", "SentOnSiteSummaryList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId, aatfId = viewModel.AatfId }));
                }
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, Guid aatfId, string quarter)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            var aatfInfo = await cache.FetchAatfData(organisationId, aatfId);
            breadcrumb.QuarterDisplayInfo = quarter;
            breadcrumb.AatfDisplayInfo = DisplayHelper.ReportingOnValue(aatfInfo.Name, aatfInfo.ApprovalNumber);
        }

        private bool IsChkCopyPreviousQuarterVisiable(int weeeSentOnListCount)
        {
            DateTime copyPreviousQuarterDataDisabledDate = Convert.ToDateTime(ConfigurationManager.AppSettings["Weee.CopyPreviousQuarterDataDisabledDate"]);
            DateTime utcCurrentDate = DateTime.UtcNow;

            if (utcCurrentDate > copyPreviousQuarterDataDisabledDate && weeeSentOnListCount == 0)
            {
                return true;
            }

            return false;
        }
    }
}