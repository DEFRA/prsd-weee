namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
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
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;

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
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                var model = mapper.Map(new ReturnToNonObligatedValuesViewModelMapTransfer()
                {
                    ReturnId = returnId,
                    Dcf = dcf,
                    ReturnData = @return,
                    OrganisationId = @return.OrganisationData.Id,
                    PastedData = TempData["pastedValues"] as String
                });

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));
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
                    var returnId = viewModel.ReturnId;

                    var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                    var model = mapper.Map(new ReturnToNonObligatedValuesViewModelMapTransfer()
                    {
                        ReturnId = returnId,
                        Dcf = !viewModel.Dcf,
                        ReturnData = @return,
                        OrganisationId = @return.OrganisationData.Id,
                        PastedData = TempData["pastedValues"] as String
                    });
                    await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));
                    TempData["currentQuarter"] = @return.Quarter;
                    TempData["currentQuarterWindow"] = @return.QuarterWindow;

                    var error = false;
                    foreach (var categoryValue in viewModel.CategoryValues)
                    {
                        var inputTonnage = Convert.ToDecimal(string.IsNullOrEmpty(categoryValue.Tonnage) ? "0" : categoryValue.Tonnage);
                        var existingNotDcfTonnage = Convert.ToDecimal(string.IsNullOrEmpty(model.CategoryValues[categoryValue.CategoryId - 1].Tonnage) ? "0" : model.CategoryValues[categoryValue.CategoryId - 1].Tonnage);

                        if (viewModel.Dcf && inputTonnage > existingNotDcfTonnage)
                        {
                            ModelState.AddModelError($"CategoryValues[{categoryValue.CategoryId - 1}].Tonnage", $"Catagory {categoryValue.CategoryId} tonnage must be less than or equal to {existingNotDcfTonnage} tonnes");
                            error = true;
                        }
                        else if (!viewModel.Dcf && inputTonnage < existingNotDcfTonnage)
                        {
                            ModelState.AddModelError(string.Empty, $"Catagory {categoryValue.CategoryId} tonnage must be more than or equal to {existingNotDcfTonnage} tonnes");
                            error = true;
                        }
                    }
                    if (error)
                    {
                        return View(viewModel);
                    }

                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("Index", "AatfTaskList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId });
                }

                await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

                return View(viewModel);
            }
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