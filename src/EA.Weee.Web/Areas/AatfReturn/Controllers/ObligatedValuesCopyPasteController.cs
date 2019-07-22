namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateReturnCreatedActionFilter]
    public class ObligatedValuesCopyPasteController : AatfReturnBaseController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Func<IWeeeClient> apiClient;

        public ObligatedValuesCopyPasteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId, Guid schemeId, ObligatedType obligatedType)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));
                var viewModel = new ObligatedValuesCopyPasteViewModel()
                {
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = @return.OrganisationData.Id,
                    AatfName = (await cache.FetchAatfData(@return.OrganisationData.Id, aatfId)).Name,
                    Type = obligatedType
                };

                if (obligatedType == ObligatedType.Received)
                {
                    viewModel.SchemeId = schemeId;
                    viewModel.SchemeName = Task.Run(() => cache.FetchSchemePublicInfoBySchemeId(schemeId)).Result.Name;
                }
                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ObligatedValuesCopyPasteViewModel viewModel, string cancel)
        {
            if (string.IsNullOrEmpty(cancel))
            {
                var b2bContent = viewModel.B2bPastedValues.First();
                var b2cContent = viewModel.B2cPastedValues.First();

                if (!string.IsNullOrEmpty(b2bContent) || !string.IsNullOrEmpty(b2cContent))
                {
                    TempData["pastedValues"] = new ObligatedCategoryValue() { B2B = b2bContent, B2C = b2cContent };
                }
            }

            if (viewModel.Type == ObligatedType.Reused)
            {
                return await Task.Run<ActionResult>(() => AatfRedirect.ObligatedReused(viewModel.ReturnId, viewModel.AatfId));
            }

            return await Task.Run<ActionResult>(() => AatfRedirect.ObligatedReceived(viewModel.ReturnId, viewModel.AatfId, viewModel.SchemeId));
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