namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Attributes;

    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [ValidateOrganisationActionFilter]
    [ValidateReturnCreatedActionFilter]
    public class SelectYourPcsController : AatfReturnBaseController
    {
        private const string RemovedSchemeList = "RemovedSchemeList";
        private const string SelectedSchemes = "SelectedSchemes";
        private const string RemovedSchemes = "RemovedSchemes";

        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IAddReturnSchemeRequestCreator requestCreator;
        private readonly IWeeeCache cache;

        public SelectYourPcsController(Func<IWeeeClient> apiclient, BreadcrumbService breadcrumb, IWeeeCache cache, IAddReturnSchemeRequestCreator requestCreator)
        {
            this.apiClient = apiclient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.requestCreator = requestCreator;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, bool reselect = false, bool copyPrevious = false, bool clearSelections = false)
        {
            using (var client = this.apiClient())
            {
                var viewModel = new SelectYourPcsViewModel
                {
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    SchemeList = await client.SendAsync(this.User.GetAccessToken(), new GetSchemesExternal()),
                    PreviousQuarterData = await client.SendAsync(this.User.GetAccessToken(), new GetPreviousQuarterSchemes(organisationId, returnId))
                };

                if (reselect)
                {
                    var request = new GetReturnScheme(returnId);
                    var existing = await client.SendAsync(this.User.GetAccessToken(), request);

                    viewModel.SelectedSchemes = existing.SchemeDataItems.Select(p => p.Id).ToList();
                    viewModel.Reselect = true;
                }

                if (copyPrevious)
                {
                    viewModel.SelectedSchemes = viewModel.PreviousQuarterData.PreviousSchemes;
                    viewModel.CopyPrevious = true;
                }

                if (clearSelections)
                {
                    viewModel.SelectedSchemes = new List<Guid>();
                }

                var @return = await client.SendAsync(this.User.GetAccessToken(), new GetReturn(returnId, false));

                await this.SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                this.TempData["currentQuarter"] = @return.Quarter;
                this.TempData["currentQuarterWindow"] = @return.QuarterWindow;

                return this.View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectYourPcsViewModel viewModel, bool reselect = false)
        {
            await this.SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(this.TempData["currentQuarter"] as Quarter, this.TempData["currentQuarterWindow"] as QuarterWindow));

            if (this.ModelState.IsValid)
            {
                if (reselect)
                {
                    return await this.Reselect(viewModel);
                }

                using (var client = this.apiClient())
                {
                    var requests = this.requestCreator.ViewModelToRequest(viewModel);
                    await client.SendAsync(this.User.GetAccessToken(), requests);
                }

                return AatfRedirect.TaskList(viewModel.ReturnId);
            }

            using (var client = this.apiClient())
            {
                viewModel.PreviousQuarterData = await client.SendAsync(
                                                    this.User.GetAccessToken(),
                                                    new GetPreviousQuarterSchemes(viewModel.OrganisationId, viewModel.ReturnId));
            }

            return this.View(viewModel);
        }

        private async Task<ActionResult> Reselect(SelectYourPcsViewModel viewModel)
        {
            using (var client = this.apiClient())
            {
                var existing = await client.SendAsync(this.User.GetAccessToken(), new GetReturnScheme(viewModel.ReturnId));

                if (this.HaveSchemesBeenRemoved(viewModel, existing.SchemeDataItems.ToList()))
                {
                    this.TempData[RemovedSchemeList] = viewModel.SchemeList
                        .Where(q => !viewModel.SelectedSchemes.Contains(q.Id) && existing.SchemeDataItems.Any(e => e.Id == q.Id)).ToList();
                    this.TempData[SelectedSchemes] = viewModel.SelectedSchemes;
                    this.TempData[RemovedSchemes] = viewModel.SchemeList.Select(p => p.Id)
                        .Where(q => !viewModel.SelectedSchemes.Contains(q) && existing.SchemeDataItems.Any(e => e.Id == q)).ToList();

                    return RedirectToRoute(AatfRedirect.RemovedPcsRouteName, new { returnId = viewModel.ReturnId, organisationId = viewModel.OrganisationId });
                }

                return await this.SaveAndContinue(existing, viewModel.SelectedSchemes, viewModel.ReturnId);
            }
        }

        private async Task<ActionResult> SaveAndContinue(SchemeDataList existingSchemes, List<Guid> schemeIdsToAdd, Guid returnId)
        {
            using (var client = this.apiClient())
            {
                foreach (var scheme in existingSchemes.SchemeDataItems)
                {
                    if (schemeIdsToAdd.Contains(scheme.Id))
                    {
                        schemeIdsToAdd.Remove(scheme.Id);
                    }
                }

                if (schemeIdsToAdd != null && schemeIdsToAdd.Count > 0)
                {
                    var request = new AddReturnScheme
                    {
                        ReturnId = returnId,
                        SchemeIds = schemeIdsToAdd
                    };
                    await client.SendAsync(this.User.GetAccessToken(), request);
                }

                return AatfRedirect.TaskList(returnId);
            }
        }

        [HttpGet]
        public async Task<ActionResult> PcsRemoved(Guid organisationId, Guid returnId)
        {
            var removedSchemeList = this.TempData[RemovedSchemeList] as List<SchemeData>;
            var selectedSchemes = this.TempData[SelectedSchemes] as List<Guid>;
            var removedSchemes = this.TempData[RemovedSchemes] as List<Guid>;

            var model = new PcsRemovedViewModel()
            {
                RemovedSchemeList = removedSchemeList,
                SelectedSchemes = selectedSchemes,
                RemovedSchemes = removedSchemes,
                ReturnId = returnId,
                OrganisationId = organisationId
            };

            this.TempData[RemovedSchemeList] = removedSchemeList;
            this.TempData[SelectedSchemes] = selectedSchemes;
            this.TempData[RemovedSchemes] = removedSchemes;

            using (var client = this.apiClient())
            {
                var @return = await client.SendAsync(this.User.GetAccessToken(), new GetReturn(returnId, false));

                await this.SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));
            }
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PcsRemoved(PcsRemovedViewModel viewModel)
        {
            await this.SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.YearQuarterPeriodFormat(this.TempData["currentQuarter"] as Quarter, this.TempData["currentQuarterWindow"] as QuarterWindow));

            if (this.ModelState.IsValid)
            {
                if (viewModel.SelectedValue == "Yes")
                {
                    using (var client = this.apiClient())
                    {
                        var existing = await client.SendAsync(this.User.GetAccessToken(), new GetReturnScheme(viewModel.ReturnId));

                        await client.SendAsync(this.User.GetAccessToken(), new RemoveReturnScheme() { SchemeIds = viewModel.RemovedSchemes, ReturnId = viewModel.ReturnId });

                        return await this.SaveAndContinue(existing, viewModel.SelectedSchemes, viewModel.ReturnId);
                    }
                }
                else
                {
                    return AatfRedirect.SelectPcs(viewModel.OrganisationId, viewModel.ReturnId, true);
                }
            }
            else
            {
                return this.View(viewModel);
            }
        }

        private bool HaveSchemesBeenRemoved(SelectYourPcsViewModel model, List<SchemeData> alreadySelected)
        {
            foreach (var scheme in alreadySelected)
            {
                if (model.SelectedSchemes.Contains(scheme.Id) == false)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, string quarter)
        {
            this.breadcrumb.ExternalOrganisation = await this.cache.FetchOrganisationName(organisationId);
            this.breadcrumb.ExternalActivity = activity;
            this.breadcrumb.OrganisationId = organisationId;
            this.breadcrumb.QuarterDisplayInfo = quarter;
        }
    }
}