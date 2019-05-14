namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
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
    [ValidateReturnActionFilter]
    public class SelectReportOptionsController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private readonly ISelectReportOptionsViewModelValidatorWrapper validator;
        private readonly IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel> mapper;

        public SelectReportOptionsController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IAddSelectReportOptionsRequestCreator requestCreator,
            ISelectReportOptionsViewModelValidatorWrapper validator,
            IMap<ReportOptionsToSelectReportOptionsViewModelMapTransfer, SelectReportOptionsViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.requestCreator = requestCreator;
            this.validator = validator;
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
                    ReturnData = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId))
                });

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectReportOptionsViewModel viewModel)
        {
            SetSelected(viewModel);

            if (ModelState.IsValid)
            {
                if (viewModel.HasSelectedOptions)
                {
                    using (var client = apiClient())
                    {
                        viewModel.ReturnData = await client.SendAsync(User.GetAccessToken(), new GetReturn(viewModel.ReturnId));
                        if (CheckHasDeselectedOptions(viewModel))
                        {
                            TempData["viewModel"] = viewModel;
                            return AatfRedirect.SelectReportOptionDeselect(viewModel.OrganisationId, viewModel.ReturnId);
                        }
                        var request = requestCreator.ViewModelToRequest(viewModel);

                        await client.SendAsync(User.GetAccessToken(), request);
                    }
                }

                if (viewModel.ReportOnQuestions.First(r => r.Id == (int)ReportOnQuestionEnum.WeeeReceived).Selected)
                {
                    return AatfRedirect.SelectPcs(viewModel.OrganisationId, viewModel.ReturnId);
                }

                return AatfRedirect.TaskList(viewModel.ReturnId);
            }

            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);

            viewModel.HasDcfError = GetDcfModelStateError();

            return View(viewModel);
        }

        private void SetSelected(SelectReportOptionsViewModel viewModel)
        {
            if (viewModel.HasSelectedOptions)
            {
                foreach (var option in viewModel.SelectedOptions)
                {
                    viewModel.ReportOnQuestions.First(r => r.Id == option).Selected = true;
                }
            }
        }

        private bool CheckHasDeselectedOptions(SelectReportOptionsViewModel viewModel)
        {
            var oldReturnOptions = viewModel.ReturnData.ReturnReportOns.Select(r => r.ReportOnQuestionId).ToList();
            if (oldReturnOptions.Count != 0)
            {
                var deselectedOptions = oldReturnOptions.Where(s => viewModel.SelectedOptions.All(s2 => s2 != s)).ToList();
                if (deselectedOptions != null && deselectedOptions.Count != 0)
                {
                    foreach (var option in deselectedOptions)
                    {
                        viewModel.ReportOnQuestions.First(r => r.Id == option).Deselected = true;
                    }

                    return true;
                }
            }
            return false;
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }

        private bool GetDcfModelStateError()
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

            return allErrors != null && allErrors.Count(p => p.ErrorMessage == "You must tell us whether any of the non-obligated WEEE was retained by a DCF") > 0;
        }
    }
}