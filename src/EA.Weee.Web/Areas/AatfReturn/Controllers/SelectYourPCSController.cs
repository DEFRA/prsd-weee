namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
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

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectYourPcsViewModel viewModel, bool reselect = false)
        {
            if (ModelState.IsValid)
            {
                if (reselect)
                {
                    return await Reselect(viewModel);
                }

                using (var client = apiClient())
                {
                    var requests = requestCreator.ViewModelToRequest(viewModel);

                    foreach (var request in requests)
                    {
                        await client.SendAsync(User.GetAccessToken(), request);
                    }
                }

                return AatfRedirect.TaskList(viewModel.ReturnId);
            }
            else
            {
                await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);
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
                    SelectedSchemes = existing.SchemeDataItems.Select(p => p.Id).ToList(),
                    Reselect = true
                };

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View("Reselect", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task<ActionResult> Reselect(SelectYourPcsViewModel viewModel)
        {
            using (var client = apiClient())
            {
                var existing = await client.SendAsync(User.GetAccessToken(), new GetReturnScheme(viewModel.ReturnId));

                if (HaveSchemesBeenRemoved(viewModel, existing.SchemeDataItems.ToList()))
                {
                    PcsRemovedViewModel model = new PcsRemovedViewModel()
                    {
                        RemovedSchemeList = viewModel.SchemeList.Where(p => viewModel.SelectedSchemes.Contains(p.Id) == false).ToList(),
                        ReturnId = viewModel.ReturnId
                    };

                    return View("PcsRemoved", model);
                }

                foreach (var scheme in existing.SchemeDataItems)
                {
                    if (viewModel.SelectedSchemes.Contains(scheme.Id))
                    {
                        viewModel.SelectedSchemes.Remove(scheme.Id);
                    }
                }

                var requests = requestCreator.ViewModelToRequest(viewModel);

                foreach (var request in requests)
                {
                    await client.SendAsync(User.GetAccessToken(), request);
                }
            }

            return AatfRedirect.TaskList(viewModel.ReturnId);
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

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}