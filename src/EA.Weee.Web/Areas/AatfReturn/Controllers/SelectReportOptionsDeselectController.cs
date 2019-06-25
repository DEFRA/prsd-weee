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
        private static string confirmSelectedValue = "Yes";

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

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(@return.Quarter, @return.QuarterWindow));
                var oldModel = TempData["viewModel"] as SelectReportOptionsViewModel;
                var viewModel = mapper.Map(oldModel);
                TempData["viewModel"] = oldModel;
                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;
                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectReportOptionsDeselectViewModel viewModel)
        {
            var oldModel = TempData["viewModel"] as SelectReportOptionsViewModel;

            var newViewModel = mapper.Map(oldModel);
            newViewModel.SelectedValue = viewModel.SelectedValue;
            newViewModel.DeselectedOptions = oldModel.ReportOnQuestions.Where(d => d.Deselected == true).Select(d => d.Id).ToList();
            SetSelected(newViewModel);
            TempData["viewModel"] = oldModel;

            if (ModelState.IsValid)
            {
                if (newViewModel.SelectedValue == confirmSelectedValue)
                {
                    using (var client = apiClient())
                    {
                        var request = requestCreator.ViewModelToRequest(newViewModel);

                        await client.SendAsync(User.GetAccessToken(), request);
                    }

                    if (newViewModel.ReportOnQuestions.First(r => r.Id == (int)ReportOnQuestionEnum.WeeeReceived).Selected && !oldModel.ReportOnQuestions.First(r => r.Id == (int)ReportOnQuestionEnum.WeeeReceived).Selected)
                    {
                        return AatfRedirect.SelectPcs(newViewModel.OrganisationId, newViewModel.ReturnId);
                    }

                    return AatfRedirect.TaskList(newViewModel.ReturnId);
                }
                else
                {
                    return AatfRedirect.SelectReportOptions(newViewModel.OrganisationId, newViewModel.ReturnId);
                }
            }

            await SetBreadcrumb(newViewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

            return View(newViewModel);
        }

        private void SetSelected(SelectReportOptionsDeselectViewModel viewModel)
        {
            foreach (var option in viewModel.ReportOnQuestions)
            {
                if (option.Deselected)
                {
                    option.Selected = false;
                }
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