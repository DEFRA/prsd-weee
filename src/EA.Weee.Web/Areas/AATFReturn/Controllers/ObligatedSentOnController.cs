namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ObligatedSentOnController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IObligatedSentOnWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel> mapper;

        public ObligatedSentOnController(IWeeeCache cache,
            BreadcrumbService breadcrumb,
            Func<IWeeeClient> apiClient,
            IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel> mapper,
            IObligatedSentOnWeeeRequestCreator requestCreator)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
            this.requestCreator = requestCreator;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid organisationId, Guid weeeSentOnId, Guid aatfId, string operatorName)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var model = mapper.Map(new ReturnToObligatedViewModelMapTransfer()
                {
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    ReturnData = @return,
                    AatfId = aatfId,
                    OperatorName = operatorName,
                    WeeeSentOnId = weeeSentOnId
                });

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

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

                    return RedirectToAction("Index", "Holding", new { organisationId = viewModel.OrganisationId });
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