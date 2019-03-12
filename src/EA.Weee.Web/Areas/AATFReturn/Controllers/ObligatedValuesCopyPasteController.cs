namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public class ObligatedValuesCopyPasteController : AatfReturnBaseController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IPasteProcessor pasteProcessor;

        public ObligatedValuesCopyPasteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IPasteProcessor pasteProcessor)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.pasteProcessor = pasteProcessor;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId, Guid schemeId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));
                var viewModel = new ObligatedValuesCopyPasteViewModel()
                {
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = @return.ReturnOperatorData.OrganisationId,
                    SchemeId = schemeId
                };
                await SetBreadcrumb(@return.ReturnOperatorData.OrganisationId, BreadCrumbConstant.AatfReturn);
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ObligatedValuesCopyPasteViewModel viewModel)
        {
            var modelValues = new ObligatedCategoryValue();

            if (viewModel.B2cPastedValues != null)
            {
                modelValues.B2C = viewModel.B2cPastedValues[0];
            }

            if (viewModel.B2bPastedValues != null)
            {
                modelValues.B2B = viewModel.B2bPastedValues[0];
            }

            TempData["pastedValues"] =  pasteProcessor.BuildModel(modelValues);

            return await Task.Run<ActionResult>(() => RedirectToAction("Index", "ObligatedReceived",
                    new { area = "AatfReturn", schemeId = viewModel.SchemeId, returnId = viewModel.ReturnId, aatfId = viewModel.AatfId }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}