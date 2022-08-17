﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Attributes;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Filters;
    using Infrastructure;
    using Prsd.Core.Helpers;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Mappings.ToViewModel;
    using ViewModels.Scheme;
    using ViewModels.Scheme.Overview;
    using ViewModels.Scheme.Overview.ContactDetails;
    using ViewModels.Scheme.Overview.MembersData;
    using ViewModels.Scheme.Overview.OrganisationDetails;
    using ViewModels.Scheme.Overview.PcsDetails;
    using ViewModels.Shared;
    using Web.ViewModels.Shared.Scheme;
    using Weee.Requests.Admin;
    using Weee.Requests.DataReturns;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Shared;
    using GetSchemes = Weee.Requests.Scheme.GetSchemes;

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

        private bool IsUserInternalAdmin()
        {
            var claimsPrincipal = new ClaimsPrincipal(this.User);

            return claimsPrincipal.HasClaim(p => p.Value == Claims.InternalAdmin);
        }

        [HttpGet]
        public async Task<ActionResult> ManageSchemes()
        {
            await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
            return View(new ManageSchemesViewModel { Schemes = await GetSchemes(), CanAddPcs = IsUserInternalAdmin() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageSchemes(ManageSchemesViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
                return View(new ManageSchemesViewModel { Schemes = await GetSchemes(), CanAddPcs = IsUserInternalAdmin() });
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
        public async Task<ActionResult> Overview(Guid schemeId, OverviewDisplayOption? overviewDisplayOption = null, string clickedTab = null)
        {
            await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);

            if (overviewDisplayOption == null)
            {
                overviewDisplayOption = OverviewDisplayOption.PcsDetails;
            }

            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId));

                var associatedAatfs = await client.SendAsync(User.GetAccessToken(), new GetAatfsByOrganisationId(scheme.OrganisationId));

                var associatedSchemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesByOrganisationId(scheme.OrganisationId));

                switch (overviewDisplayOption.Value)
                {
                    case OverviewDisplayOption.MembersData:

                        var membersDataModel = mapper.Map<MembersDataOverviewViewModel>(scheme);
                        return View("Overview/MembersDataOverview", membersDataModel);

                    case OverviewDisplayOption.OrganisationDetails:

                        var orgDetails = await client.SendAsync(User.GetAccessToken(), new OrganisationBySchemeId(schemeId));
                        var associatedEntities = mapper.Map<AssociatedEntitiesViewModel>(new AssociatedEntitiesViewModelTransfer()
                        {
                            AssociatedAatfs = associatedAatfs, AssociatedSchemes = associatedSchemes, SchemeId = schemeId
                        });

                        switch (orgDetails.OrganisationType)
                        {
                            case OrganisationType.SoleTraderOrIndividual:
                                var soleTraderModel = mapper.Map<SoleTraderDetailsOverviewViewModel>(orgDetails);
                                soleTraderModel.CanEditOrganisation = orgDetails.CanEditOrganisation;
                                soleTraderModel.SchemeId = schemeId;
                                soleTraderModel.SchemeName = scheme.SchemeName;
                                soleTraderModel.AssociatedEntities = associatedEntities;
                                return View("Overview/SoleTraderDetailsOverview", soleTraderModel);

                            case OrganisationType.Partnership:
                                var partnershipModel = mapper.Map<PartnershipDetailsOverviewViewModel>(orgDetails);
                                partnershipModel.CanEditOrganisation = orgDetails.CanEditOrganisation;
                                partnershipModel.SchemeId = schemeId;
                                partnershipModel.SchemeName = scheme.SchemeName;
                                partnershipModel.AssociatedEntities = associatedEntities;
                                return View("Overview/PartnershipDetailsOverview", partnershipModel);

                            case OrganisationType.RegisteredCompany:
                            default:
                                var registeredCompanyModel = mapper.Map<RegisteredCompanyDetailsOverviewViewModel>(orgDetails);
                                registeredCompanyModel.CanEditOrganisation = orgDetails.CanEditOrganisation;
                                registeredCompanyModel.SchemeId = schemeId;
                                registeredCompanyModel.SchemeName = scheme.SchemeName;
                                registeredCompanyModel.AssociatedEntities = associatedEntities;
                                return View("Overview/RegisteredCompanyDetailsOverview", registeredCompanyModel);
                        }

                    case OverviewDisplayOption.ContactDetails:
                        var contactDetailsModel = mapper.Map<ContactDetailsOverviewViewModel>(scheme);
                        contactDetailsModel.SchemeName = scheme.SchemeName;
                        contactDetailsModel.SchemeId = scheme.Id;
                        contactDetailsModel.CanEditContactDetails = scheme.CanEdit;
                        return View("Overview/ContactDetailsOverview", contactDetailsModel);

                    case OverviewDisplayOption.PcsDetails:
                    default:
                        return View("Overview/PcsDetailsOverview", mapper.Map<PcsDetailsOverviewViewModel>(scheme));
                }
            }
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> EditScheme(Guid? schemeId)
        {
            if (schemeId.HasValue)
            {
                using (var client = apiClient())
                {
                    var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId.Value));

                    if (!scheme.CanEdit)
                    {
                        return new HttpForbiddenResult();
                    }

                    List<int> years = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(scheme.OrganisationId));

                    var model = new SchemeViewModelBase
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

                    model.StatusSelectList = new SelectList(GetStatuses(scheme.SchemeStatus), "Key", "Value");

                    await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("ManageSchemes");
            }
        }

        private Dictionary<int, string> GetStatuses(SchemeStatus currentSchemeStatus)
        {
            var statuses = EnumHelper.GetValues(typeof(SchemeStatus));
            if (currentSchemeStatus == SchemeStatus.Pending)
            {
                statuses.Remove((int)SchemeStatus.Withdrawn);
            }

            if (currentSchemeStatus == SchemeStatus.Approved)
            {
                statuses.Remove((int)SchemeStatus.Pending);
                statuses.Remove((int)SchemeStatus.Rejected);
            }
            return statuses;
        }

        private async Task<SchemeViewModelBase> SetSchemeStatusAndCompetentAuthorities(Guid schemeId, SchemeViewModelBase model)
        {
            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId));
                model.StatusSelectList = new SelectList(GetStatuses(scheme.SchemeStatus), "Key", "Value");
                model.CompetentAuthorities = await GetCompetentAuthorities();
            }

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> EditScheme(Guid schemeId, SchemeViewModelBase model)
        {
            await cache.InvalidateSchemeCache(schemeId);
            await cache.InvalidateSchemePublicInfoCache(model.OrganisationId);
            await cache.InvalidateOrganisationSearch();

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
                model = await SetSchemeStatusAndCompetentAuthorities(schemeId, model);
                await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
                return View(model);
            }

            CreateOrUpdateSchemeInformationResult result;
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
                case CreateOrUpdateSchemeInformationResult.ResultType.Success:
                    return RedirectToAction("Overview", new { schemeId });

                case CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure:
                    {
                        ModelState.AddModelError("ApprovalNumber", "Approval number already exists");

                        await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
                        model = await SetSchemeStatusAndCompetentAuthorities(schemeId, model);
                        return View(model);
                    }

                case CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure:
                    {
                        string errorMessage = string.Format(
                            "Billing reference \"{0}\" already exists for scheme \"{1}\" ({2})",
                            result.IbisCustomerReferenceUniquenessFailure.IbisCustomerReference,
                            result.IbisCustomerReferenceUniquenessFailure.OtherSchemeName,
                            result.IbisCustomerReferenceUniquenessFailure.OtherSchemeApprovalNumber);

                        ModelState.AddModelError("IbisCustomerReference", errorMessage);

                        await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
                        model = await SetSchemeStatusAndCompetentAuthorities(schemeId, model);
                        return View(model);
                    }

                case CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure:
                    ModelState.AddModelError("IbisCustomerReference", "Enter a customer billing reference");

                    await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
                    model = await SetSchemeStatusAndCompetentAuthorities(schemeId, model);
                    return View(model);

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [ValidateOrganisationHasNoSchemeFilterAttribute]
        public async Task<ActionResult> AddScheme(Guid organisationId)
        {
            await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
            using (var client = apiClient())
            {
                AddSchemeViewModel viewModel = new AddSchemeViewModel()
                {
                    CompetentAuthorities = await GetCompetentAuthorities(),
                    ComplianceYears = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(organisationId)),
                    OrganisationId = organisationId,
                    OrganisationName = (await cache.FetchOrganisationName(organisationId)),
                    IsChangeableStatus = true,
                    OrganisationAddress = new AddressData()
                };

                viewModel.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> AddScheme(AddSchemeViewModel model)
        {
            using (var client = apiClient())
            {
                if (!ModelState.IsValid)
                {
                    model.CompetentAuthorities = await GetCompetentAuthorities();
                    model.ComplianceYears = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(model.OrganisationId));
                    model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                    await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
                    return View(model);
                }

                CreateScheme request = new CreateScheme(
                    model.OrganisationId,
                    model.SchemeName,
                    model.ApprovalNumber,
                    model.IbisCustomerReference,
                    model.ObligationType.Value,
                    model.CompetentAuthorityId,
                    SchemeStatus.Approved);

                CreateOrUpdateSchemeInformationResult result = await client.SendAsync(User.GetAccessToken(), request);

                await cache.InvalidateOrganisationSearch();

                switch (result.Result)
                {
                    case CreateOrUpdateSchemeInformationResult.ResultType.Success:

                        SchemeData orgData = new SchemeData()
                        {
                            OrganisationId = model.OrganisationId,
                            Contact = model.Contact,
                            Address = model.OrganisationAddress,
                        };

                        await client.SendAsync(User.GetAccessToken(), new UpdateSchemeContactDetails(orgData));

                        return RedirectToAction("ManageSchemes");

                    case CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure:
                        {
                            ModelState.AddModelError("ApprovalNumber", "Approval number already exists");

                            await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
                            model.CompetentAuthorities = await GetCompetentAuthorities();
                            model.ComplianceYears = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(model.OrganisationId));
                            model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                            return View(model);
                        }

                    case CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure:
                        {
                            string errorMessage = string.Format(
                                "Billing reference \"{0}\" already exists for scheme \"{1}\" ({2})",
                                result.IbisCustomerReferenceUniquenessFailure.IbisCustomerReference,
                                result.IbisCustomerReferenceUniquenessFailure.OtherSchemeName,
                                result.IbisCustomerReferenceUniquenessFailure.OtherSchemeApprovalNumber);

                            ModelState.AddModelError("IbisCustomerReference", errorMessage);

                            await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
                            model.CompetentAuthorities = await GetCompetentAuthorities();
                            model.ComplianceYears = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(model.OrganisationId));
                            model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                            return View(model);
                        }

                    case CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure:
                        ModelState.AddModelError("IbisCustomerReference", "Enter a customer billing reference");

                        await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
                        model.CompetentAuthorities = await GetCompetentAuthorities();
                        model.ComplianceYears = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(model.OrganisationId));
                        model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                        return View(model);

                    case CreateOrUpdateSchemeInformationResult.ResultType.SchemeAlreadyExists:
                    {
                        ModelState.AddModelError("SchemeExists", "Scheme has already been created for the organisation");

                        await SetBreadcrumb(null, InternalUserActivity.ManageScheme);
                        model.CompetentAuthorities = await GetCompetentAuthorities();
                        model.ComplianceYears = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(model.OrganisationId));
                        model.OrganisationAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                        return View(model);
                    }

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetProducerCsv(Guid orgId, int complianceYear, string approvalNumber)
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
        public async Task<ActionResult> GetEeeWeeeCsv(Guid organisationId, int complianceYear)
        {
            FileInfo file;
            using (IWeeeClient client = apiClient())
            {
                var fetchSummaryCsv = new FetchSummaryCsv(organisationId, complianceYear);
                file = await client.SendAsync(User.GetAccessToken(), fetchSummaryCsv);
            }

            return File(file.Data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> ManageContactDetails(Guid schemeId, Guid orgId)
        {
            await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);

            var model = new ManageContactDetailsViewModel();
            using (var client = apiClient())
            {
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId));
                if (!scheme.CanEdit)
                {
                    return new HttpForbiddenResult();
                }

                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                model.OrganisationAddress = scheme.Address;
                model.Contact = scheme.Contact;
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
            await SetBreadcrumb(model.SchemeId, InternalUserActivity.ManageScheme);

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
                var orgData = new SchemeData()
                {
                    OrganisationId = model.OrgId,
                    Contact = model.Contact,
                    Address = model.OrganisationAddress,
                };
                await client.SendAsync(User.GetAccessToken(), new UpdateSchemeContactDetails(orgData));
            }

            return RedirectToAction("Overview", new { schemeId = model.SchemeId, overviewDisplayOption = OverviewDisplayOption.ContactDetails });
        }

        [HttpGet]
        public async Task<ActionResult> ViewOrganisationDetails(Guid schemeId, Guid orgId)
        {
            await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);

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
            await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmRejection(Guid schemeId, ConfirmRejectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
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
                    await cache.InvalidateOrganisationSearch();
                }
            }

            return RedirectToAction("Overview", new { schemeId });
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmWithdrawn(Guid schemeId)
        {
            var model = new ConfirmWithdrawnViewModel();
            await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmWithdrawn(Guid schemeId, ConfirmWithdrawnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await SetBreadcrumb(schemeId, InternalUserActivity.ManageScheme);
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
                    await cache.InvalidateOrganisationSearch();
                }
            }

            return RedirectToAction("ManageSchemes");
        }

        private async Task SetBreadcrumb(Guid? schemeId, string activity)
        {
            breadcrumb.InternalActivity = activity;

            if (schemeId.HasValue)
            {
                breadcrumb.InternalScheme = await cache.FetchSchemeName(schemeId.Value);
            }
        }
    }
}