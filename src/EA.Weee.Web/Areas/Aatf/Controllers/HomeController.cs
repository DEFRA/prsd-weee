namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class HomeController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel> mapper;

        public HomeController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client, IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel> mapper)
        {
            this.apiClient = client;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, bool isAE)
        {
            using (var client = apiClient())
            {
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(organisationId));

                var model = mapper.Map(new AatfDataToHomeViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = organisationId, IsAE = isAE });

                if (model.AatfList.Count == 1)
                {
                    return RedirectToAction("Index", "ViewAatfContactDetails", new { organisationId = organisationId, aatfId = model.AatfList[0].Id, isAE = isAE });
                }

                if (isAE)
                {
                    await SetBreadcrumb(model.OrganisationId, "View AE contact details", false);
                }
                else
                {
                    await SetBreadcrumb(model.OrganisationId, "View AATF contact details", false);
                }

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsAE)
                {
                    return RedirectToAction("Index", "ViewAatfContactDetails", new { area = "Aatf", organisationId = model.OrganisationId, aatfId = model.SelectedAeId, isAE = model.IsAE });
                }
                else
                {
                    return RedirectToAction("Index", "ViewAatfContactDetails", new { area = "Aatf", organisationId = model.OrganisationId, aatfId = model.SelectedAatfId, isAE = model.IsAE });
                }
            }

            using (var client = apiClient())
            {
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(model.OrganisationId));

                model = mapper.Map(new AatfDataToHomeViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = model.OrganisationId, IsAE = model.IsAE });
            }

            if (!model.ModelValidated)
            {
                ModelState.RunCustomValidation(model);
            }

            ModelState.ApplyCustomValidationSummaryOrdering(HomeViewModel.ValidationMessageDisplayOrder);

            await SetBreadcrumb(model.OrganisationId, "AATF return", false);

            return View(model);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, bool setScheme = true)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            if (setScheme)
            {
                breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
            }
        }
    }
}