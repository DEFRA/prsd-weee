namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateOrganisationActionFilter]
    [ValidateReturnCreatedActionFilter]
    public class SelectReportOptionsController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private readonly IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel> mapper;

        public SelectReportOptionsController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IAddSelectReportOptionsRequestCreator requestCreator,
            IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.requestCreator = requestCreator;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId)
        {
            using (var client = apiClient())
            {
                var viewModel = mapper.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer()
                {
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    ReportOnQuestions = await client.SendAsync(User.GetAccessToken(), new GetReportOnQuestion()),
                    ReturnData = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false))
                });

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(viewModel.ReturnData.Quarter, viewModel.ReturnData.QuarterWindow));

                TempData["currentQuarter"] = viewModel.ReturnData.Quarter;
                TempData["currentQuarterWindow"] = viewModel.ReturnData.QuarterWindow;

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectReportOptionsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    viewModel.ReturnData = await client.SendAsync(User.GetAccessToken(), new GetReturn(viewModel.ReturnId, false));

                    if (viewModel.DeSelectedOptions.Any())
                    {
                        if (!viewModel.HasSelectedOptions)
                        {
                            return AatfRedirect.SelectReportOptionsNil(viewModel.OrganisationId, viewModel.ReturnId);
                        }

                        TempData["viewModel"] = viewModel;

                        return AatfRedirect.SelectReportOptionDeselect(viewModel.OrganisationId, viewModel.ReturnId);
                    }
                    
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    if (IfNotPreviouslySelectedPcs(viewModel))
                    {
                        return AatfRedirect.SelectPcs(viewModel.OrganisationId, viewModel.ReturnId);
                    }
                }

                return AatfRedirect.TaskList(viewModel.ReturnId);
            }
            else
            {
                RemoveDcfSelectedValueModelState(viewModel);
            }

            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

            return View(viewModel);
        }

        private void RemoveDcfSelectedValueModelState(SelectReportOptionsViewModel viewModel)
        {
            if (!viewModel.NonObligatedQuestionSelected)
            {
                ModelState.Remove("DcfSelectedValue");
            }
        }

        private static bool IfNotPreviouslySelectedPcs(SelectReportOptionsViewModel viewModel)
        {
            return viewModel.ReportOnQuestions.First(r => r.Id == (int)ReportOnQuestionEnum.WeeeReceived).Selected
                   && !viewModel.ReturnData.SchemeDataItems.Any();
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, string quarter)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            breadcrumb.QuarterDisplayInfo = quarter;
        }
    }
}