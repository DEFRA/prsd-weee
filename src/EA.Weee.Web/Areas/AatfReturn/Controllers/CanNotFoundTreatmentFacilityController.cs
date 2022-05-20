namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
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
    public class CanNotFoundTreatmentFacilityController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer, CanNotFoundTreatmentFacilityViewModel> mapper;

        public CanNotFoundTreatmentFacilityController(Func<IWeeeClient> apiClient,
                                                      BreadcrumbService breadcrumb,
                                                      IWeeeCache cache,
                                                      IMap<ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer, CanNotFoundTreatmentFacilityViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId, string aatfName, bool? isCanNotFindLinkClick = false)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                var viewModel = mapper.Map(new ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer()
                {
                    Return = @return,
                    AatfId = aatfId,
                    IsCanNotFindLinkClick = isCanNotFindLinkClick.Value,
                    AatfName = aatfName,
                });

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;

                return View(viewModel);
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