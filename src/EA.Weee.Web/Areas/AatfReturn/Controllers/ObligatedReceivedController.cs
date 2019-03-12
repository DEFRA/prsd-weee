﻿namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Mappings.ToViewModel;
    using Prsd.Core.Mapper;
    using Weee.Requests.AatfReturn;

    public class ObligatedReceivedController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IObligatedReceivedWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToObligatedViewModelTransfer, ObligatedViewModel> mapper;

        public ObligatedReceivedController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> apiClient, IObligatedReceivedWeeeRequestCreator requestCreator, IMap<ReturnToObligatedViewModelTransfer, ObligatedViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.requestCreator = requestCreator;
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId, Guid schemeId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var model = mapper.Map(new ReturnToObligatedViewModelTransfer() { AatfId = aatfId, OrganisationId = @return.ReturnOperatorData.OrganisationId, ReturnId = returnId, SchemeId = schemeId, ReturnData = @return });

                if (TempData["pastedValues"] != null)
                {
                    model.CategoryValues = TempData["pastedValues"] as IList<ObligatedCategoryValue>;
                }

                await SetBreadcrumb(@return.ReturnOperatorData.OrganisationId, BreadCrumbConstant.AatfReturn);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ObligatedViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    return AatfRedirect.TaskList(viewModel.ReturnId);
                }
            }

            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn);

            return View(viewModel);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}