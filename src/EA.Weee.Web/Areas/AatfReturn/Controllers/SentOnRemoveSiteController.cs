namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateReturnCreatedActionFilter]
    public class SentOnRemoveSiteController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel> mapper;

        public SentOnRemoveSiteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId, Guid weeeSentOnId)
        {
            using (var client = apiClient())
            {
                var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOnById(weeeSentOnId));

                if (weeeSentOn == null)
                {
                    return AatfRedirect.SentOnSummaryList(organisationId, returnId, aatfId);
                }

                var viewModel = mapper.Map(new ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
                {
                    ReturnId = returnId,
                    AatfId = aatfId,
                    OrganisationId = organisationId,
                    WeeeSentOn = weeeSentOn
                });

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.FormatQuarter(@return.Quarter, @return.QuarterWindow));

                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SentOnRemoveSiteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    if (viewModel.SelectedValue == "Yes")
                    {
                        var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new RemoveWeeeSentOn(viewModel.WeeeSentOnId));
                    }
                    return AatfRedirect.SentOnSummaryList(viewModel.OrganisationId, viewModel.ReturnId, viewModel.AatfId);
                }
            }
            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, viewModel.AatfId, DisplayHelper.FormatQuarter(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));
            return View(viewModel);
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
    }
}