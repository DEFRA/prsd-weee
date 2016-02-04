namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Services;
    using Services.Caching;
    using ViewModels.Scheme;
    using ViewModels.Scheme.Overview;
    using ViewModels.Scheme.Overview.ContactDetails;
    using ViewModels.Scheme.Overview.MembersData;
    using ViewModels.Scheme.Overview.OrganisationDetails;
    using ViewModels.Scheme.Overview.PcsDetails;
    using Web.ViewModels.Shared.Scheme;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
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
        public async Task<ActionResult> ManageSchemes()
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
        public async Task<ActionResult> Overview(Guid schemeId, OverviewDisplayOption? overviewDisplayOption = null)
        {
            await Task.Yield();

            if (overviewDisplayOption == null)
            {
                overviewDisplayOption = OverviewDisplayOption.PcsDetails;
            }

            switch (overviewDisplayOption.Value)
            {
                case OverviewDisplayOption.MembersData:
                    // TODO: Replace with call to get members data
                    return View("Overview/MembersDataOverview", new MembersDataOverviewViewModel(schemeId, string.Empty, schemeId, string.Empty)
                    {
                        DownloadsByYear = new List<YearlyDownloads>
                        {
                            new YearlyDownloads
                            {
                                Year = 2016, 
                                IsMembersDownloadAvailable = true,
                                IsDataReturnsDownloadAvailable = true
                            }
                        }
                    });
                case OverviewDisplayOption.OrganisationDetails:
                // TODO:  Replace with call to get organisation details, and select correct view based on organisation type
                    return View("Overview/RegisteredCompanyDetailsOverview",
                        new RegisteredCompanyDetailsOverviewViewModel(schemeId, string.Empty));
                case OverviewDisplayOption.ContactDetails:
                    // TODO: Replace with call to get contact details
                    return View("Overview/ContactDetailsOverview",
                        new ContactDetailsOverviewViewModel(schemeId, string.Empty));
                default:
                    // TODO:  Replace with call to get PCS details
                    return View("Overview/PcsDetailsOverview", new PcsDetailsOverviewViewModel(schemeId, string.Empty));
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditScheme(Guid? schemeId)
        {
            if (schemeId.HasValue)
            {
                using (var client = apiClient())
                {
                    var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId.Value));

                    List<int> years = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(scheme.OrganisationId));

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
                        IsUnchangeableStatus = scheme.SchemeStatus == SchemeStatus.Approved || scheme.SchemeStatus == SchemeStatus.Rejected,
                        OrganisationId = scheme.OrganisationId,
                        SchemeId = schemeId.Value,
                        ComplianceYears = years
                    };

                    await SetBreadcrumb(schemeId);
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("ManageSchemes");
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

        [HttpGet]
        public async Task<ActionResult> GetProducerCSV(Guid orgId, int complianceYear, string approvalNumber)
        {
            using (var client = apiClient())
            {
                var csvFileName = approvalNumber + "_fullmemberlist_" + complianceYear + "_" + DateTime.Now.ToString("ddMMyyyy_HHmm") + ".csv";
                
                var producerCSVData = await client.SendAsync(User.GetAccessToken(),
                    new GetProducerCSV(orgId, complianceYear));

                byte[] data = new UTF8Encoding().GetBytes(producerCSVData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(csvFileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ManageContactDetails(Guid schemeId, Guid orgId)
        {
            await SetBreadcrumb(schemeId);

            var model = new ManageContactDetailsViewModel();
            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(orgId));
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                model.OrganisationAddress = organisationData.OrganisationAddress;
                model.Contact = organisationData.Contact;
                model.OrganisationAddress.Countries = countries;
                model.SchemeId = schemeId;
                model.OrgId = orgId;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageContactDetails(ManageContactDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId);

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                }
                return View(model);
            }

            using (var client = apiClient())
            {
                var orgData = new OrganisationData
                {
                    Id = model.OrgId,
                    Contact = model.Contact,
                    OrganisationAddress = model.OrganisationAddress,
                };
                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationContactDetails(orgData));
            }

            return RedirectToAction("EditScheme", new { schemeId = model.SchemeId });
        }

        [HttpGet]
        public async Task<ActionResult> ViewOrganisationDetails(Guid schemeId, Guid orgId)
        {
            await SetBreadcrumb(schemeId);

            using (var client = apiClient())
            {
                var orgDetails = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(orgId));

                var model = new ViewOrganisationDetailsViewModel
                {
                    OrganisationData = orgDetails
                };

                return View("ViewOrganisationDetails", model);
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

            if (model.SelectedValue == ConfirmSchemeRejectionOptions.No)
            {
                return RedirectToAction("EditScheme", new { schemeId });
            }

            if (model.SelectedValue == ConfirmSchemeRejectionOptions.Yes)
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
            breadcrumb.InternalActivity = "Manage PCSs";

            if (schemeId.HasValue)
            {
                breadcrumb.InternalOrganisation = await cache.FetchSchemeName(schemeId.Value);
            }
        }
    }
}