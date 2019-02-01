namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.AatfReturn;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Web.OAuth;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;

    public class NonObligatedController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly INonObligatedWeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public NonObligatedController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> apiClient, INonObligatedWeeRequestCreator requestCreator)
        {
            this.apiClient = apiClient;
            this.requestCreator = requestCreator;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, bool dcf)
        {
            var viewModel = new NonObligatedValuesViewModel(new NonObligatedCategoryValues()) { OrganisationId = organisationId, ReturnId = returnId, Dcf = dcf };

            await SetBreadcrumb(organisationId, "AATF Return");

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(NonObligatedValuesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    await client.SendAsync(User.GetAccessToken(), request);

                    if (viewModel.Dcf)
                    {
                        return RedirectToAction("Index", "Holding", new { area = "AatfReturn" });
                    }

                    return RedirectToAction("Index", "NonObligated", new { area = "AatfReturn", dcf = true });
                }
            }

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