namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
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
        public virtual async Task<ActionResult> Index(ObligatedValuesCopyPasteViewModel viewModel, string cancel)
        {
            if (string.IsNullOrEmpty(cancel))
            {
                var b2bContent = viewModel.B2bPastedValues.First();
                var b2cContent = viewModel.B2cPastedValues.First();

                if (!string.IsNullOrEmpty(b2bContent) || !string.IsNullOrEmpty(b2cContent))
                {
                    var obligatedPastedValues = new ObligatedPastedValues();

                    obligatedPastedValues.B2B = pasteProcessor.BuildModel(b2bContent);
                    obligatedPastedValues.B2C = pasteProcessor.BuildModel(b2cContent);
                    
                    var categoryValues = pasteProcessor.ParseObligatedPastedValues(obligatedPastedValues);

                    TempData["pastedValues"] = categoryValues;
                }
            }

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