﻿namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
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
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [ValidateReturnCreatedActionFilter]
    public class SearchedAatfResultListController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToSearchedAnAatfViewModelMapTransfer, ReturnToSearchedAnAatfViewModelMap> mapper;

        public SearchedAatfResultListController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMap<ReturnToSearchedAnAatfViewModelMapTransfer, ReturnToSearchedAnAatfViewModelMap> mapper)
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

                var model = mapper.Map(new ReturnToSearchedAnAatfViewModelMapTransfer()
                {                    
                    AatfId = aatfId                    
                });

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                return View(model);
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