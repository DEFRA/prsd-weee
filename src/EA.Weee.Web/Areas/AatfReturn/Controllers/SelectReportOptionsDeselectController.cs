namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateOrganisationActionFilter]
    [ValidateReturnCreatedActionFilter]
    public class SelectReportOptionsDeselectController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private readonly IMap<SelectReportOptionsViewModel, SelectReportOptionsDeselectViewModel> mapper;

        public SelectReportOptionsDeselectController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IAddSelectReportOptionsRequestCreator requestCreator,
            IMap<SelectReportOptionsViewModel, SelectReportOptionsDeselectViewModel> mapper)
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
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                var selectReportOptionsViewModel = TempData["viewModel"] as SelectReportOptionsViewModel;

                var viewModel = mapper.Map(selectReportOptionsViewModel);

                TempData["viewModel"] = selectReportOptionsViewModel;
                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectReportOptionsDeselectViewModel viewModel)
        {
            var selectReportOptionsViewModel = TempData["viewModel"] as SelectReportOptionsViewModel;

            var deselectViewModel = mapper.Map(selectReportOptionsViewModel);

            deselectViewModel.SelectedValue = viewModel.SelectedValue;

            TempData["viewModel"] = selectReportOptionsViewModel;

            if (ModelState.IsValid)
            {
                if (deselectViewModel.SelectedValue == viewModel.YesValue)
                {
                    using (var client = apiClient())
                    {
                        var request = requestCreator.ViewModelToRequest(deselectViewModel);

                        await client.SendAsync(User.GetAccessToken(), request);
                    }

                    if (IfWeeeReceivedChangedToSelected(selectReportOptionsViewModel))
                    {
                        return AatfRedirect.SelectPcs(deselectViewModel.OrganisationId, deselectViewModel.ReturnId);
                    }

                    return AatfRedirect.TaskList(deselectViewModel.ReturnId);
                }
                else
                {
                    return AatfRedirect.SelectReportOptions(deselectViewModel.OrganisationId, deselectViewModel.ReturnId);
                }
            }

            await SetBreadcrumb(deselectViewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

            return View(deselectViewModel);
        }

        private bool IfWeeeReceivedChangedToSelected(SelectReportOptionsViewModel model)
        {
            return model.ReportOnQuestions.First(r => r.Id == (int)ReportOnQuestionEnum.WeeeReceived).ReSelected;
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