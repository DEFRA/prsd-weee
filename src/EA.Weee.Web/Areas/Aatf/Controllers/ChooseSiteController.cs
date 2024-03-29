﻿namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AatfEvidence.Controllers;
    using Api.Client;
    using Constant;
    using Infrastructure;
    using Mappings.ToViewModel;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Weee.Requests.AatfReturn;

    public class ChooseSiteController : AatfEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;

        public ChooseSiteController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client, IMapper mapper, ConfigurationService configurationService)
        {
            this.apiClient = client;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
            this.configurationService = configurationService;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId)
        {
            var model = await GenerateSelectYourAatfViewModel(organisationId);

            if (model.AatfList.Count == 0)
            {
                await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.AatfManageEvidence);
                return View("IndexNoAatf", model);
            }
            if (model.AatfList.Count == 1)
            {
                return RedirectToAction("Index", "ManageEvidenceNotes", new { organisationId = model.OrganisationId, aatfId = model.AatfList[0].Id });
            }
            
            await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.AatfManageEvidence);
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SelectYourAatfViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "ManageEvidenceNotes", new { organisationId = model.OrganisationId, aatfId = model.SelectedId });
            }

            model = await GenerateSelectYourAatfViewModel(model.OrganisationId);

            await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.AatfManageEvidence);

            return View(model);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }

        private async Task<SelectYourAatfViewModel> GenerateSelectYourAatfViewModel(Guid organisationId)
        {
            var allAatfsAndAes = await cache.FetchAatfDataForOrganisationData(organisationId);

            return mapper.Map<SelectYourAatfViewModel>(new AatfEvidenceToSelectYourAatfViewModelMapTransfer()
            {
                AatfList = allAatfsAndAes, 
                OrganisationId = organisationId
            });
        }
    }
}