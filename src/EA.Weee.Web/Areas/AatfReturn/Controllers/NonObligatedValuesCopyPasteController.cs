namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateReturnCreatedActionFilter]
    public class NonObligatedValuesCopyPasteController : AatfReturnBaseController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Func<IWeeeClient> apiClient;

        public NonObligatedValuesCopyPasteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, bool dcf)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                var typeHeading = dcf == false ? "Non-obligated WEEE" : "Non-obligated WEEE retained by a DCF";

                var viewModel = new NonObligatedValuesCopyPasteViewModel()
                {
                    ReturnId = returnId,
                    OrganisationId = @return.OrganisationData.Id,
                    Dcf = dcf,
                    Typeheading = typeHeading
                };

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn);

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

            return await Task.Run<ActionResult>(() => AatfRedirect.NonObligated(viewModel.ReturnId, viewModel.Dcf));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}