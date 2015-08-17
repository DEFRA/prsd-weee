namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
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

            return RedirectToAction("EditScheme", new { id = viewModel.Selected.Value });
        }

        private async Task<List<SchemeData>> GetSchemes()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetSchemes());
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditScheme(Guid id)
        {
            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(id));

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

                return View("EditScheme", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditScheme(Guid id, SchemeViewModel model)
        {
            if (model.Status == SchemeStatus.Rejected)
            {
                return RedirectToAction("ConfirmRejection", "Scheme", new { id });
            }

            model.CompetentAuthorities = await GetCompetentAuthorities();

            if (!ModelState.IsValid)
            {
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
                        return View("EditScheme", model);
                    }
                }

                await
                    client.SendAsync(User.GetAccessToken(),
                        new UpdateSchemeInformation(id, model.SchemeName, model.ApprovalNumber,
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
        public ActionResult ConfirmRejection(Guid id)
        {
            var model = new ConfirmRejectionViewModel();
            model.SchemeId = id;
            return View("ConfirmRejection", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmRejection(ConfirmRejectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ConfirmRejectionOptions.SelectedValue == ConfirmSchemeRejection.No)
            {
                return RedirectToAction("EditScheme", "Scheme", new { id = model.SchemeId });
            }

            if (model.ConfirmRejectionOptions.SelectedValue == ConfirmSchemeRejection.Yes)
            {
                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), new SetSchemeStatus(model.SchemeId, SchemeStatus.Rejected));
                }
            }
            return RedirectToAction("ManageSchemes", "Scheme");
        }
    }
}