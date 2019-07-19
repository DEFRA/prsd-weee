namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateReturnCreatedActionFilter]
    public class ObligatedSentOnValuesCopyPasteController : AatfReturnBaseController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Func<IWeeeClient> apiClient;

        public ObligatedSentOnValuesCopyPasteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid organisationId, Guid weeeSentOnId, Guid aatfId, string siteName)
        {
            using (IWeeeClient client = apiClient())
            {
                ReturnData returnData = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));
                ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel()
                {
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = returnData.OrganisationData.Id,
                    WeeeSentOnId = weeeSentOnId,
                    SiteName = siteName,
                };
                await SetBreadcrumb(returnData.OrganisationData.Id, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(returnData.Quarter, returnData.QuarterWindow));
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ObligatedSentOnValuesCopyPasteViewModel viewModel, string cancel)
        {
            if (string.IsNullOrEmpty(cancel))
            {
                string b2bContent = viewModel.B2bPastedValues.First();
                string b2cContent = viewModel.B2cPastedValues.First();

                if (!string.IsNullOrEmpty(b2bContent) || !string.IsNullOrEmpty(b2cContent))
                {
                    TempData["pastedValues"] = new ObligatedCategoryValue() { B2B = b2bContent, B2C = b2cContent };
                }
            }

            return await Task.Run<ActionResult>(() => AatfRedirect.ObligatedSentOn(viewModel.SiteName, viewModel.OrganisationId, viewModel.AatfId, viewModel.ReturnId, viewModel.WeeeSentOnId));
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