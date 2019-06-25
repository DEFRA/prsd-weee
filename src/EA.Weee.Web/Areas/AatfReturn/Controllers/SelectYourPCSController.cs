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
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, bool reselect = false)
        {
            if (reselect)
            {
                return await Reselect(organisationId, returnId);
            }

            using (var client = apiClient())
            {
                var viewModel = new SelectYourPcsViewModel
                {
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    SchemeList = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal())
                };

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(@return.Quarter, @return.QuarterWindow));

                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectYourPcsViewModel viewModel, bool reselect = false)
        {
            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

            if (ModelState.IsValid)
            {
                if (reselect)
                {
                    return await Reselect(viewModel);
                }

                using (var client = apiClient())
                {
                    var requests = requestCreator.ViewModelToRequest(viewModel);
                    await client.SendAsync(User.GetAccessToken(), requests);
                }

                return AatfRedirect.TaskList(viewModel.ReturnId);
            }
            else
            {
                return View(viewModel);
            }
        }

        private async Task<ActionResult> Reselect(Guid organisationId, Guid returnId)
        {
            using (var client = apiClient())
            {
                GetReturnScheme request = new GetReturnScheme(returnId);

                var existing = await client.SendAsync(User.GetAccessToken(), request);

                SelectYourPcsViewModel viewModel = new SelectYourPcsViewModel
                {
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    SchemeList = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal()),
                    Reselect = true
                };

                if (TempData.ContainsKey("selectedSchemes"))
                {
                    viewModel.SelectedSchemes = TempData["selectedSchemes"] as List<Guid>;
                    TempData.Remove("selectedSchemes");
                }
                else
                {
                    viewModel.SelectedSchemes = existing.SchemeDataItems.Select(p => p.Id).ToList();
                }

                await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, DisplayHelper.FormatQuarter(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

                return View("Reselect", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task<ActionResult> Reselect(SelectYourPcsViewModel viewModel)
        {
            using (var client = apiClient())
            {
                SchemeDataList existing = await client.SendAsync(User.GetAccessToken(), new GetReturnScheme(viewModel.ReturnId));

                if (HaveSchemesBeenRemoved(viewModel, existing.SchemeDataItems.ToList()))
                {
                    PcsRemovedViewModel model = new PcsRemovedViewModel()
                    {
                        RemovedSchemeList = viewModel.SchemeList.Where(p => viewModel.SelectedSchemes.Contains(p.Id) == false).ToList(),
                        SelectedSchemes = viewModel.SelectedSchemes,
                        RemovedSchemes = viewModel.SchemeList.Select(p => p.Id).Where(q => !viewModel.SelectedSchemes.Contains(q) && existing.SchemeDataItems.Any(e => e.Id == q)).ToList(),
                        ReturnId = viewModel.ReturnId,
                        OrganisationId = viewModel.OrganisationId
                    };

                    return View("PcsRemoved", model);
                }

                return await SaveAndContinue(existing, viewModel.SelectedSchemes, viewModel.ReturnId);
            }
        }

        private async Task<ActionResult> SaveAndContinue(SchemeDataList existingSchemes, List<Guid> schemeIdsToAdd, Guid returnId)
        {
            using (var client = apiClient())
            {
                foreach (SchemeData scheme in existingSchemes.SchemeDataItems)
                {
                    if (schemeIdsToAdd.Contains(scheme.Id))
                    {
                        schemeIdsToAdd.Remove(scheme.Id);
                    }
                }

                var request = new AddReturnScheme
                {
                    ReturnId = returnId,
                    SchemeIds = schemeIdsToAdd
                };
                await client.SendAsync(User.GetAccessToken(), request);

                return AatfRedirect.TaskList(returnId);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PcsRemoved(PcsRemovedViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedValue == "Yes")
                {
                    using (var client = apiClient())
                    {
                        SchemeDataList existing = await client.SendAsync(User.GetAccessToken(), new GetReturnScheme(viewModel.ReturnId));
                        await client.SendAsync(User.GetAccessToken(), new RemoveReturnScheme() { SchemeIds = viewModel.RemovedSchemes });
                        return await SaveAndContinue(existing, viewModel.SelectedSchemes, viewModel.ReturnId);
                    }
                }
                else
                {
                    TempData["selectedSchemes"] = viewModel.SelectedSchemes;
                    return AatfRedirect.SelectPcs(viewModel.OrganisationId, viewModel.ReturnId, true);
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        private bool HaveSchemesBeenRemoved(SelectYourPcsViewModel model, List<SchemeData> alreadySelected)
        {
            foreach (SchemeData scheme in alreadySelected)
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
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            breadcrumb.AatfDisplayInfo = DisplayHelper.ReportingOnValue(string.Empty, string.Empty, quarter);
        }
    }
}