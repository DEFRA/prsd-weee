namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using Core.Shared;
    using Infrastructure;
    using ViewModels;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;

    public class SchemeController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;

        public SchemeController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ViewResult> ManageSchemes()
        {
            return View(new ManageSchemesViewModel { Schemes = await GetSchemes() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageSchemes(ManageSchemesViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
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
                    SchemeId = schemeId,
                    ApprovalNumber = scheme.ApprovalName,
                    OldApprovalNumber = scheme.ApprovalName,
                    IbisCustomerReference = scheme.IbisCustomerReference,
                    CompetentAuthorityId = scheme.CompetentAuthorityId ?? Guid.Empty,
                    SchemeName = scheme.SchemeName,
                    ObligationType = scheme.ObligationType
                };

                return View("EditScheme", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditScheme(SchemeViewModel model)
        {
            model.CompetentAuthorities = await GetCompetentAuthorities();
            if (!ModelState.IsValid)
            {
                return View("EditScheme", model);
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
                        ModelState.AddModelError(string.Empty, "Approval number already exists.");
                        return View("EditScheme", model);
                    }
                }

                await
                    client.SendAsync(User.GetAccessToken(),
                        new UpdateSchemeInformation(model.SchemeId, model.SchemeName, model.ApprovalNumber,
                            model.IbisCustomerReference,
                            model.ObligationType.Value, model.CompetentAuthorityId));

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
    }
}