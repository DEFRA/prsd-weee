namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Attributes;
    using Constant;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Infrastructure;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using ViewModels.Validation;

    [ValidateReturnCreatedActionFilter]
    public class NonObligatedController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly INonObligatedWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToNonObligatedValuesViewModelMapTransfer, NonObligatedValuesViewModel> mapper;

        public NonObligatedController(IWeeeCache cache,
            BreadcrumbService breadcrumb,
            Func<IWeeeClient> apiClient,
            INonObligatedWeeeRequestCreator requestCreator,
            IMap<ReturnToNonObligatedValuesViewModelMapTransfer,
            NonObligatedValuesViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.requestCreator = requestCreator;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, bool dcf)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var model = mapper.Map(new ReturnToNonObligatedValuesViewModelMapTransfer()
                {
                    ReturnId = returnId,
                    Dcf = dcf,
                    ReturnData = @return,
                    OrganisationId = @return.OrganisationData.Id,
                    PastedData = TempData["pastedValues"] as String
                });

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(@return.Quarter, @return.QuarterWindow));
                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(NonObligatedValuesViewModel viewModel)
        {
            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("Index", "AatfTaskList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId });
                }

                await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

                return View(viewModel);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, string quarter)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            breadcrumb.AatfDisplayInfo = DisplayHelper.ReportingOnValue(string.Empty, string.Empty, quarter);
        }
    }
}