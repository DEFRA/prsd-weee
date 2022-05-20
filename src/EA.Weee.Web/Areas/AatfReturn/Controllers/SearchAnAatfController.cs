namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
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
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [ValidateReturnCreatedActionFilter]
    public class SearchAnAatfController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSearchAnAatfViewModelMapTransfer, SearchAnAatfViewModel> mapper;

        public SearchAnAatfController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMap<ReturnAndAatfToSearchAnAatfViewModelMapTransfer, SearchAnAatfViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId, Guid? weeeSentOnId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));
                WeeeSentOnData weeeSentOn = null;

                if (weeeSentOnId != null)
                {
                    weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOnById(weeeSentOnId.Value));
                }
                var viewModel = mapper.Map(new ReturnAndAatfToSearchAnAatfViewModelMapTransfer()
                {
                    Return = @return,
                    AatfId = aatfId,
                    WeeeSentOnData = weeeSentOn
                });

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SearchAnAatfViewModel searchAnAatfViewModel)
        {
            if (ModelState.IsValid)
            {
                return await Task.Run<ActionResult>(() =>
                        RedirectToAction("Index", "SearchedAatfResultList", new { area = "AatfReturn", organisationId = searchAnAatfViewModel.OrganisationId, returnId = searchAnAatfViewModel.ReturnId, aatfId = searchAnAatfViewModel.AatfId, selectedAatfId = searchAnAatfViewModel.SelectedAatfId, selectedAatfName = searchAnAatfViewModel.SearchTerm }));
            }
            else
            {
                using (var client = apiClient())
                {
                    var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(searchAnAatfViewModel.ReturnId, false));
                    WeeeSentOnData weeeSentOn = null;

                    if (searchAnAatfViewModel.WeeeSentOnId != null)
                    {
                        weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOnById(searchAnAatfViewModel.WeeeSentOnId.Value));
                    }

                    var viewModel = mapper.Map(new ReturnAndAatfToSearchAnAatfViewModelMapTransfer()
                    {
                        Return = @return,
                        AatfId = searchAnAatfViewModel.AatfId,
                        WeeeSentOnData = weeeSentOn
                    });

                    await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, searchAnAatfViewModel.AatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                    TempData["currentQuarter"] = @return.Quarter;
                    TempData["currentQuarterWindow"] = @return.QuarterWindow;

                    return View(searchAnAatfViewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SearchAatf(string searchTerm, Guid aatfId)
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using (var client = apiClient())
            {
                var returnData = await client.SendAsync(User.GetAccessToken(), new GetSearchAatfAddress(searchTerm, aatfId));
                return Json(returnData, JsonRequestBehavior.AllowGet);
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
    }
}