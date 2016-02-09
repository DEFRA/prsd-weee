namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core.Helpers;
    using Prsd.Core.Mapper;
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
        private readonly IMapper mapper;

        public SchemeController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
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

            return RedirectToAction("Overview", new { schemeId = viewModel.Selected.Value });
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
            await SetBreadcrumb(schemeId);

            if (overviewDisplayOption == null)
            {
                overviewDisplayOption = OverviewDisplayOption.PcsDetails;
            }

            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId));

                switch (overviewDisplayOption.Value)
                {
                    case OverviewDisplayOption.MembersData:

                        // TODO: Extend GetSchemeById request (above) to include WEEE/EEE as well as members download information, and create a mapping
                        var membersDataModel = new MembersDataOverviewViewModel();
                        membersDataModel.SchemeId = schemeId;
                        membersDataModel.SchemeName = scheme.SchemeName;
                        return View("Overview/MembersDataOverview", membersDataModel);

                    case OverviewDisplayOption.OrganisationDetails:

                        var orgDetails = await client.SendAsync(User.GetAccessToken(), new OrganisationBySchemeId(schemeId));
                        switch (orgDetails.OrganisationType)
                        {
                            case OrganisationType.SoleTraderOrIndividual:
                                var soleTraderModel = mapper.Map<SoleTraderDetailsOverviewViewModel>(orgDetails);
                                soleTraderModel.SchemeId = schemeId;
                                soleTraderModel.SchemeName = scheme.SchemeName;
                                return View("Overview/SoleTraderDetailsOverview", soleTraderModel);

                            case OrganisationType.Partnership:
                                var partnershipModel = mapper.Map<PartnershipDetailsOverviewViewModel>(orgDetails);
                                partnershipModel.SchemeId = schemeId;
                                partnershipModel.SchemeName = scheme.SchemeName;
                                return View("Overview/PartnershipDetailsOverview", partnershipModel);

                            case OrganisationType.RegisteredCompany:
                            default:
                                var registeredCompanyModel = mapper.Map<RegisteredCompanyDetailsOverviewViewModel>(orgDetails);
                                registeredCompanyModel.SchemeId = schemeId;
                                registeredCompanyModel.SchemeName = scheme.SchemeName;
                                return View("Overview/RegisteredCompanyDetailsOverview", registeredCompanyModel);
                        }

                    case OverviewDisplayOption.ContactDetails:

                        var organisationData =
                            await client.SendAsync(User.GetAccessToken(), new OrganisationBySchemeId(schemeId));
                        var contactDetailsModel = mapper.Map<ContactDetailsOverviewViewModel>(organisationData);
                        contactDetailsModel.SchemeName = scheme.SchemeName;
                        contactDetailsModel.SchemeId = scheme.Id;
                        return View("Overview/ContactDetailsOverview", contactDetailsModel);

                    case OverviewDisplayOption.PcsDetails:
                    default:

                        return View("Overview/PcsDetailsOverview", mapper.Map<PcsDetailsOverviewViewModel>(scheme));
                }
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
                        IbisCustomerReference = scheme.IbisCustomerReference,
                        CompetentAuthorityId = scheme.CompetentAuthorityId ?? Guid.Empty,
                        SchemeName = scheme.SchemeName,
                        ObligationType = scheme.ObligationType,
                        Status = scheme.SchemeStatus,
                        IsChangeableStatus = scheme.SchemeStatus != SchemeStatus.Rejected && scheme.SchemeStatus != SchemeStatus.Withdrawn,
                        OrganisationId = scheme.OrganisationId,
                        SchemeId = schemeId.Value,
                        ComplianceYears = years
                    };

                    if (scheme.SchemeStatus == SchemeStatus.Pending)
                    {
                        var statuses = EnumHelper.GetValues(typeof(SchemeStatus));
                        statuses.Remove((int)SchemeStatus.Withdrawn);
                        model.StatusSelectList = new SelectList(statuses, "Key", "Value");
                    }

                    if (scheme.SchemeStatus == SchemeStatus.Approved)
                    {
                        var statuses = EnumHelper.GetValues(typeof(SchemeStatus));
                        statuses.Remove((int)SchemeStatus.Pending);
                        statuses.Remove((int)SchemeStatus.Rejected);
                        model.StatusSelectList = new SelectList(statuses, "Key", "Value");
                    }

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

            if (model.Status == SchemeStatus.Withdrawn)
            {
                return RedirectToAction("ConfirmWithdrawn", new { schemeId });
            }

            model.CompetentAuthorities = await GetCompetentAuthorities();

            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(schemeId);
                model.CompetentAuthorities = await GetCompetentAuthorities();
                return View(model);
            }

            UpdateSchemeInformationResult result;
            using (var client = apiClient())
            {
                UpdateSchemeInformation request = new UpdateSchemeInformation(
                    schemeId,
                    model.SchemeName,
                    model.ApprovalNumber,
                    model.IbisCustomerReference,
                    model.ObligationType.Value,
                    model.CompetentAuthorityId,
                    model.Status);

                result = await client.SendAsync(User.GetAccessToken(), request);
            }

            switch (result.Result)
            {
                case UpdateSchemeInformationResult.ResultType.Success:
                    return RedirectToAction("Overview", new { schemeId });

                case UpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure:
                    {
                        ModelState.AddModelError("ApprovalNumber", "Approval number already exists.");

                        await SetBreadcrumb(schemeId);
                        model.CompetentAuthorities = await GetCompetentAuthorities();
                        return View(model);
                    }

                case UpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure:
                    {
                        string errorMessage = string.Format(
                            "Billing reference \"{0}\" already exists for scheme \"{1}\" ({2}).",
                            result.IbisCustomerReferenceUniquenessFailure.IbisCustomerReference,
                            result.IbisCustomerReferenceUniquenessFailure.OtherSchemeName,
                            result.IbisCustomerReferenceUniquenessFailure.OtherSchemeApprovalNumber);

                        ModelState.AddModelError("IbisCustomerReference", errorMessage);

                        await SetBreadcrumb(schemeId);
                        model.CompetentAuthorities = await GetCompetentAuthorities();
                        return View(model);
                    }

                default:
                    throw new NotSupportedException();
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

            return RedirectToAction("Overview", new { schemeId = model.SchemeId, overviewDisplayOption = OverviewDisplayOption.ContactDetails });
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

        [HttpGet]
        public async Task<ActionResult> EditSoleTraderOrIndividualOrganisationDetails(Guid schemeId, Guid orgId)
        {
            await SetBreadcrumb(schemeId);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(orgId));
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var model = new EditSoleTraderOrIndividualOrganisationDetailsViewModel
                {
                    OrganisationType = organisationData.OrganisationType,
                    BusinessTradingName = organisationData.TradingName,
                    BusinesAddress = organisationData.BusinessAddress
                };

                model.BusinesAddress.Countries = countries;
                model.SchemeId = schemeId;
                model.OrgId = orgId;

                return View("EditSoleTraderOrIndividualOrganisationDetails", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditRegisteredCompanyOrganisationDetails(Guid schemeId, Guid orgId)
        {
            await SetBreadcrumb(schemeId);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(orgId));
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var model = new EditRegisteredCompanyOrganisationDetailsViewModel
                {
                    OrganisationType = organisationData.OrganisationType,
                    CompanyName = organisationData.OrganisationName,
                    BusinessTradingName = organisationData.TradingName,
                    CompaniesRegistrationNumber = organisationData.CompanyRegistrationNumber,
                    BusinesAddress = organisationData.BusinessAddress
                };

                model.BusinesAddress.Countries = countries;
                model.SchemeId = schemeId;
                model.OrgId = orgId;

                return View("EditRegisteredCompanyOrganisationDetails", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRegisteredCompanyOrganisationDetails(EditRegisteredCompanyOrganisationDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId);

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model.BusinesAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                }
                return View(model);
            }

            using (var client = apiClient())
            {
                var orgData = new OrganisationData
                {
                    Id = model.OrgId,
                    OrganisationType = model.OrganisationType,
                    CompanyRegistrationNumber = model.CompaniesRegistrationNumber,
                    TradingName = model.BusinessTradingName,
                    Name = model.CompanyName,
                    BusinessAddress = model.BusinesAddress,
                };
                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationDetails(orgData));
            }

            return RedirectToAction("Overview", new { schemeId = model.SchemeId, overviewDisplayOption = OverviewDisplayOption.OrganisationDetails });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSoleTraderOrIndividualOrganisationDetails(EditSoleTraderOrIndividualOrganisationDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId);

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model.BusinesAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                }
                return View(model);
            }

            using (var client = apiClient())
            {
                var orgData = new OrganisationData
                {
                    Id = model.OrgId,
                    OrganisationType = model.OrganisationType,
                    TradingName = model.BusinessTradingName,
                    BusinessAddress = model.BusinesAddress,
                };
                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationDetails(orgData));
            }

            return RedirectToAction("Overview", new { schemeId = model.SchemeId, overviewDisplayOption = OverviewDisplayOption.OrganisationDetails });
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

            return RedirectToAction("Overview", new { schemeId });
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmWithdrawn(Guid schemeId)
        {
            var model = new ConfirmWithdrawnViewModel();
            await SetBreadcrumb(schemeId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmWithdrawn(Guid schemeId, ConfirmWithdrawnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(schemeId);
                return View(model);
            }

            if (model.SelectedValue == ConfirmSchemeWithdrawOptions.No)
            {
                return RedirectToAction("EditScheme", new { schemeId });
            }

            if (model.SelectedValue == ConfirmSchemeWithdrawOptions.Yes)
            {
                using (var client = apiClient())
                {
                    await client.SendAsync(User.GetAccessToken(), new SetSchemeStatus(schemeId, SchemeStatus.Withdrawn));
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