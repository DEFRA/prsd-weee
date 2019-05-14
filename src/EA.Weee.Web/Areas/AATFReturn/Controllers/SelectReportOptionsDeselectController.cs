namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
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

    public class SelectReportOptionsDeselectController : AatfReturnBaseController
    { 
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSelectReportOptionsRequestCreator requestCreator;
        private const string pcsQuestion = "PCS";

        public SelectReportOptionsDeselectController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);
                var oldModel = TempData["viewModel"] as SelectReportOptionsViewModel;
                var viewModel = new SelectReportOptionsDeselectViewModel()
                {
                    ReturnId = oldModel.ReturnId,
                    ReportOnQuestions = oldModel.ReportOnQuestions,
                    OrganisationId = oldModel.OrganisationId,
                    SelectedOptions = oldModel.SelectedOptions,
                    DcfSelectedValue = oldModel.DcfSelectedValue
                };
                TempData["viewModel"] = oldModel;

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectReportOptionsDeselectViewModel viewModel)
        {
            SetSelected(viewModel);
            var oldModel = TempData["viewModel"] as SelectReportOptionsViewModel;
            var newViewModel = new SelectReportOptionsDeselectViewModel()
            {
                ReturnId = oldModel.ReturnId,
                ReportOnQuestions = oldModel.ReportOnQuestions,
                OrganisationId = oldModel.OrganisationId,
                SelectedOptions = oldModel.SelectedOptions,
                DcfSelectedValue = oldModel.DcfSelectedValue,
                SelectedValue = viewModel.SelectedValue
            };
            SetSelected(newViewModel);
            TempData["viewModel"] = oldModel;

            if (ModelState.IsValid)
            {
                if (newViewModel.SelectedValue == "Yes")
                {
                    if (newViewModel.HasSelectedOptions)
                    {
                        using (var client = apiClient())
                        {
                            var request = requestCreator.ViewModelToRequest(newViewModel);

                            await client.SendAsync(User.GetAccessToken(), request);
                        }
                    }

                    if (newViewModel.ReportOnQuestions.First(r => r.Question == pcsQuestion).Selected)
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

            await SetBreadcrumb(newViewModel.OrganisationId, BreadCrumbConstant.AatfReturn);

            return View(newViewModel);
        }

        private void SetSelected(SelectReportOptionsDeselectViewModel viewModel)
        {
            if (viewModel.HasSelectedOptions)
            {
                foreach (var option in viewModel.SelectedOptions)
                {
                    viewModel.ReportOnQuestions.First(r => r.Id == option).Selected = true;
                }
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}