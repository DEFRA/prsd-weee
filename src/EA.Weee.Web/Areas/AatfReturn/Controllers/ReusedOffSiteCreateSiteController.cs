namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateReturnCreatedActionFilter]
    public class ReusedOffSiteCreateSiteController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IObligatedReusedSiteRequestCreator requestCreator;
        private readonly IMap<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer, ReusedOffSiteCreateSiteViewModel> mapper;

        public ReusedOffSiteCreateSiteController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IObligatedReusedSiteRequestCreator requestCreator,
            IMap<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer, ReusedOffSiteCreateSiteViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.requestCreator = requestCreator;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId, Guid? siteId)
        {
            using (var client = apiClient())
            {
                var sites = await client.SendAsync(User.GetAccessToken(), new GetAatfSite(aatfId, returnId));
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var viewModel = mapper.Map(new SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer()
                {
                    AatfId = aatfId,
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    SiteId = siteId,
                    ReturnedSites = sites,
                    Countries = countries
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
        public virtual async Task<ActionResult> Index(ReusedOffSiteCreateSiteViewModel viewModel)
        {
            return await AatfSitePostAction(viewModel);
        }

        private async Task<ActionResult> AatfSitePostAction(ReusedOffSiteCreateSiteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    return AatfRedirect.ReusedOffSiteSummaryList(viewModel.ReturnId, viewModel.AatfId, viewModel.OrganisationId);
                }
            }

            using (var client = apiClient())
            {
                viewModel.AddressData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
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
            breadcrumb.AatfDisplayInfo = DisplayHelper.ReportingOnValue(aatfInfo.Name, aatfInfo.ApprovalNumber, quarter);
        }
    }
}