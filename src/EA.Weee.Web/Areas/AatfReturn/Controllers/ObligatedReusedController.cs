namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Weee.Requests.AatfReturn;

    public class ObligatedReusedController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IObligatedReusedWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel> mapper;

        public ObligatedReusedController(IWeeeCache cache,
            BreadcrumbService breadcrumb,
            Func<IWeeeClient> apiClient,
            IObligatedReusedWeeeRequestCreator requestCreator, 
            IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.requestCreator = requestCreator;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var model = mapper.Map(new ReturnToObligatedViewModelMapTransfer()
                {
                    AatfId = aatfId,
                    OrganisationId = @return.ReturnOperatorData.OrganisationId,
                    ReturnId = returnId,
                    ReturnData = @return,
                    PastedData = TempData["pastedValues"] as ObligatedCategoryValue
                });

                await SetBreadcrumb(@return.ReturnOperatorData.OrganisationId, BreadCrumbConstant.AatfReturn);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ObligatedViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    if (viewModel.Edit)
                    {
                        return AatfRedirect.ReusedOffSiteSummaryList(viewModel.ReturnId, viewModel.AatfId, viewModel.OrganisationId);
                    }

                    return AatfRedirect.ReusedOffSite(viewModel.ReturnId, viewModel.AatfId, viewModel.OrganisationId);
                }
            }

            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);
            return View(viewModel);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}