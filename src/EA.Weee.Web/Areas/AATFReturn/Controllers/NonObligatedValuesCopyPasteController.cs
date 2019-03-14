namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public class NonObligatedValuesCopyPasteController : AatfReturnBaseController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IPasteProcessor pasteProcessor;

        public NonObligatedValuesCopyPasteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IPasteProcessor pasteProcessor)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.pasteProcessor = pasteProcessor;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, bool dcf)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));
                var viewModel = new NonObligatedValuesCopyPasteViewModel()
                {
                    ReturnId = returnId,
                    OrganisationId = @return.ReturnOperatorData.OrganisationId,
                    Dcf = dcf
                };
                await SetBreadcrumb(@return.ReturnOperatorData.OrganisationId, BreadCrumbConstant.AatfReturn);
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(NonObligatedValuesCopyPasteViewModel viewModel, string cancel)
        {
            if (string.IsNullOrEmpty(cancel))
            {
                var pastedContent = viewModel.PastedValues.First();

                if (!string.IsNullOrEmpty(pastedContent))
                {
                    TempData["pastedValues"] = pastedContent;
                }
            }

            return await Task.Run<ActionResult>(() => RedirectToAction("Index", "NonObligated",
                    new { area = "AatfReturn", returnId = viewModel.ReturnId, dcf = viewModel.Dcf }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}