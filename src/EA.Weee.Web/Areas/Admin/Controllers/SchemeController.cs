namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using Core.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using ViewModels.Scheme;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;

    public class SchemeController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;

        public SchemeController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public async Task<ViewResult> ManageSchemes()
        {
            await SetBreadcrumb(null);
            return View(new ManageSchemesViewModel { Schemes = await GetSchemes() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageSchemes(ManageSchemesViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(null);
                return View(new ManageSchemesViewModel { Schemes = await GetSchemes() });
            }

            return RedirectToAction("EditScheme", new { schemeId = viewModel.Selected.Value });
        }

        private async Task<List<SchemeData>> GetSchemes()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetSchemes());
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditScheme(Guid schemeId)
        {
            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId));

                var model = new SchemeViewModel
                {
                    CompetentAuthorities = await GetCompetentAuthorities(),
                    ApprovalNumber = scheme.ApprovalName,
                    OldApprovalNumber = scheme.ApprovalName,
                    IbisCustomerReference = scheme.IbisCustomerReference,
                    CompetentAuthorityId = scheme.CompetentAuthorityId ?? Guid.Empty,
                    SchemeName = scheme.SchemeName,
                    ObligationType = scheme.ObligationType,
                    Status = scheme.SchemeStatus,
                    IsUnchangeableStatus = scheme.SchemeStatus == SchemeStatus.Approved || scheme.SchemeStatus == SchemeStatus.Rejected
                };

                await SetBreadcrumb(schemeId);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditScheme(Guid schemeId, SchemeViewModel model)
        {
            if (model.Status == SchemeStatus.Rejected)
            {
                return RedirectToAction("ConfirmRejection", new { schemeId });
            }

            model.CompetentAuthorities = await GetCompetentAuthorities();

            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(schemeId);
                return View(model);
            }

            using (var client = apiClient())
            {
                if (model.OldApprovalNumber != model.ApprovalNumber)
                {
                    var approvalNumberExists = await
                        client.SendAsync(User.GetAccessToken(),
                            new VerifyApprovalNumberExists(model.ApprovalNumber));

                    if (approvalNumberExists)
                    {
                        ModelState.AddModelError("ApprovalNumber", "Approval number already exists.");
                        await SetBreadcrumb(schemeId);
                        return View(model);
                    }
                }

                await
                    client.SendAsync(User.GetAccessToken(),
                        new UpdateSchemeInformation(schemeId, model.SchemeName, model.ApprovalNumber,
                            model.IbisCustomerReference,
                            model.ObligationType.Value, model.CompetentAuthorityId, model.Status));

                return RedirectToAction("ManageSchemes");
            }
        }

        private async Task<IEnumerable<UKCompetentAuthorityData>> GetCompetentAuthorities()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmRejection(Guid schemeId)
        {
            var model = new ConfirmRejectionViewModel();
            await SetBreadcrumb(schemeId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmRejection(Guid schemeId, ConfirmRejectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(schemeId);
                return View(model);
            }

            if (model.ConfirmRejectionOptions.SelectedValue == ConfirmSchemeRejectionOptions.No)
            {
                return RedirectToAction("EditScheme", new { schemeId });
            }

            if (model.ConfirmRejectionOptions.SelectedValue == ConfirmSchemeRejectionOptions.Yes)
            {
                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), new SetSchemeStatus(schemeId, SchemeStatus.Rejected));
                }
            }

            return RedirectToAction("ManageSchemes");
        }

        private async Task SetBreadcrumb(Guid? schemeId)
        {
            breadcrumb.InternalActivity = "Manage schemes";

            if (schemeId.HasValue)
            {
                breadcrumb.InternalOrganisation = await cache.FetchSchemeName(schemeId.Value);
            }
        }
    }
}