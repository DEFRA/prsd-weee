namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Helper;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.Requests;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Requests;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared.Aatf;
    using EA.Weee.Web.ViewModels.Shared.Aatf.Mapping;
    using Weee.Requests.Admin.Aatf;

    public class AatfController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly IEditFacilityDetailsRequestCreator detailsRequestCreator;
        private readonly IEditAatfContactRequestCreator contactRequestCreator;
        private readonly IWeeeCache cache;
        private readonly IFacilityViewModelBaseValidatorWrapper validationWrapper;

        public AatfController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IMapper mapper,
            IEditFacilityDetailsRequestCreator detailsRequestCreator,
            IEditAatfContactRequestCreator contactRequestCreator,
            IWeeeCache cache,
            IFacilityViewModelBaseValidatorWrapper validationWrapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
            this.detailsRequestCreator = detailsRequestCreator;
            this.contactRequestCreator = contactRequestCreator;
            this.cache = cache;
            this.validationWrapper = validationWrapper;
        }

        [HttpGet]
        public async Task<ActionResult> Details(Guid id, string selectedTab = null)
        {
            using (var client = apiClient())
            {
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                var years = await client.SendAsync(User.GetAccessToken(), new GetAatfComplianceYearsByAatfId(aatf.AatfId));

                var associatedAatfs = await client.SendAsync(User.GetAccessToken(), new GetAatfsByOrganisationId(aatf.Organisation.Id));

                var associatedSchemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesByOrganisationId(aatf.Organisation.Id));

                var submissionHistory = await client.SendAsync(User.GetAccessToken(), new GetAatfSubmissionHistory(id));

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                var viewModel = mapper.Map<AatfDetailsViewModel>(new AatfDataToAatfDetailsViewModelMapTransfer(aatf)
                {
                    OrganisationString = GenerateSharedAddress(aatf.Organisation.BusinessAddress),
                    AssociatedAatfs = associatedAatfs,
                    AssociatedSchemes = associatedSchemes,
                    SubmissionHistory = submissionHistory,
                    ComplianceYearList = years,
                    CurrentDate = currentDate
                });

                viewModel.SelectedTab = selectedTab;

                SetBreadcrumb(aatf.FacilityType, aatf.Name);

                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<ActionResult> FetchDetails(Guid aatfId, int selectedComplianceYear)
        {
            using (var client = apiClient())
            {               
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfIdByComplianceYear(aatfId, selectedComplianceYear));

                return RedirectToAction("Details", new { Id = aatf });
            }
        }

        [HttpGet]
        public async Task<ActionResult> ManageAatfs(FacilityType facilityType)
        {
            SetBreadcrumb(facilityType, null);

            return View(nameof(ManageAatfs), new ManageAatfsViewModel { FacilityType = facilityType, AatfDataList = await GetAatfs(facilityType), CanAddAatf = IsUserInternalAdmin(), Filter = new FilteringViewModel { FacilityType = facilityType, CompetentAuthorityOptions = await GetCompetentAuthoritiesList() } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageAatfs(ManageAatfsViewModel viewModel)
        {
            SetBreadcrumb(viewModel.FacilityType, null);

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    viewModel = new ManageAatfsViewModel
                    {
                        AatfDataList = await GetAatfs(viewModel.FacilityType, viewModel.Filter),
                        CanAddAatf = IsUserInternalAdmin(),
                        FacilityType = viewModel.FacilityType,
                        Filter = viewModel.Filter
                    };
                    return View(viewModel);
                }
            }
            else
            {
                return RedirectToAction("Details", new { Id = viewModel.Selected.Value });
            }
        }

        private bool IsUserInternalAdmin()
        {
            var claimsPrincipal = new ClaimsPrincipal(this.User);

            return claimsPrincipal.HasClaim(p => p.Value == Claims.InternalAdmin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ApplyFilter(FilteringViewModel filter)
        {
            SetBreadcrumb(filter.FacilityType, null);
            return View(nameof(ManageAatfs), new ManageAatfsViewModel { AatfDataList = await GetAatfs(filter.FacilityType, filter), CanAddAatf = IsUserInternalAdmin(), Filter = filter, FacilityType = filter.FacilityType });
        }

        [HttpGet]
        public async Task<ActionResult> ClearFilter(FacilityType facilityType)
        {
            return await ManageAatfs(facilityType);
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> ManageAatfDetails(Guid id)
        {
            using (var client = apiClient())
            {
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                if (!aatf.CanEdit || !IsValidRecordToEdit(currentDate, aatf.ComplianceYear))
                {
                    return new HttpForbiddenResult();
                }

                SetBreadcrumb(aatf.FacilityType, aatf.Name);

                switch (aatf.FacilityType)
                {
                    case FacilityType.Aatf:
                        return View(await PopulateAndReturnViewModel<AatfEditDetailsViewModel>(aatf, client));
                    case FacilityType.Ae:
                        return View(await PopulateAndReturnViewModel<AeEditDetailsViewModel>(aatf, client));
                    default:
                        throw new ArgumentOutOfRangeException(nameof(aatf.FacilityType));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> ManageAatfDetails(AatfEditDetailsViewModel viewModel)
        {
            return await ManageFacilityDetails(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> ManageAeDetails(AeEditDetailsViewModel viewModel)
        {
            return await ManageFacilityDetails(viewModel);
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> ManageContactDetails(Guid id, FacilityType facilityType)
        {
            using (var client = apiClient())
            {
                var contact = await client.SendAsync(User.GetAccessToken(), new GetAatfContact(id));

                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                var countries = await client.SendAsync(this.User.GetAccessToken(), new GetCountries(false));

                if (!contact.CanEditContactDetails || !IsValidRecordToEdit(currentDate, aatf.ComplianceYear))
                {
                    return new HttpForbiddenResult();
                }

                var model = this.mapper.Map<AatfEditContactAddressViewModel>(new AatfEditContactTransfer() { AatfData = aatf, Countries = countries, CurrentDate = currentDate });

                SetBreadcrumb(facilityType, aatf.Name);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageContactDetails(AatfEditContactAddressViewModel viewModel)
        {
            SetBreadcrumb(viewModel.AatfData.FacilityType, viewModel.AatfData.Name);

            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = contactRequestCreator.ViewModelToRequest(viewModel);
                    request.SendNotification = false;

                    await client.SendAsync(User.GetAccessToken(), request);

                    return Redirect(Url.Action("Details", new { area = "Admin", Id = viewModel.Id }) + "#contactDetails");
                }
            }

            using (var client = apiClient())
            {
                viewModel.ContactData.AddressData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id, Guid organisationId, FacilityType facilityType)
        {
            using (var client = apiClient())
            {
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                SetBreadcrumb(facilityType, aatf.Name);

                var canDelete = await client.SendAsync(User.GetAccessToken(), new CheckAatfCanBeDeleted(id));

                var viewModel = new DeleteViewModel()
                {
                    AatfId = id,
                    OrganisationId = organisationId,
                    FacilityType = facilityType,
                    DeletionData = canDelete,
                    AatfName = aatf.Name,
                    OrganisationName = await cache.FetchOrganisationName(organisationId)
                };

                return View(viewModel);
            }
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> UpdateApproval(Guid id)
        {
            using (var client = apiClient())
            {
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                SetBreadcrumb(aatf.FacilityType, aatf.Name);

                var request = (EditAatfDetails)TempData["aatfRequest"];

                var approvalDateFlags = await client.SendAsync(User.GetAccessToken(), new CheckAatfApprovalDateChange(id, request.Data.ApprovalDate.Value));

                var viewModel = mapper.Map<UpdateApprovalViewModel>(new UpdateApprovalDateViewModelMapTransfer() { AatfData = aatf, CanApprovalDateBeChangedFlags = approvalDateFlags, Request = request });

                TempData["aatfRequest"] = request;

                return View(viewModel);
            }
        }

        [HttpPost]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateApproval(UpdateApprovalViewModel model)
        {
            SetBreadcrumb(model.FacilityType, model.AatfName);

            if (ModelState.IsValid)
            {
                if (model.SelectedValue.Equals("Yes"))
                {
                    using (var client = apiClient())
                    {
                        await client.SendAsync(User.GetAccessToken(), model.Request);

                        await cache.InvalidateAatfCache(model.OrganisationId);

                        return Redirect(Url.Action("Details", new { id = model.AatfId }));
                    }
                }
                else
                {
                    return RedirectToAction(nameof(ManageAatfDetails), new { id = model.AatfId });
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DeleteViewModel viewModel)
        {
            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new DeleteAnAatf(viewModel.AatfId, viewModel.OrganisationId));

                await cache.InvalidateOrganisationSearch();

                await cache.InvalidateAatfCache(viewModel.OrganisationId);

                return RedirectToAction("ManageAatfs", new { facilityType = viewModel.FacilityType });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Download(Guid returnId, int complianceYear, int quarter, Guid aatfId)
        {
            CSVFileData fileData;

            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), new GetAatfObligatedData(complianceYear, quarter, returnId, aatfId));
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        private async Task<List<AatfDataList>> GetAatfs(FacilityType facilityType, FilteringViewModel filter = null)
        {
            using (var client = apiClient())
            {
                var mappedFilter = filter != null ? mapper.Map<AatfFilter>(filter) : null;
                return await client.SendAsync(User.GetAccessToken(), new GetAatfs(facilityType, mappedFilter));
            }
        }

        private async Task<List<UKCompetentAuthorityData>> GetCompetentAuthoritiesList()
        {
            using (var client = apiClient())
            {
                var authorities = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
                return authorities.ToList();
            }
        }

        private async Task<T> PopulateAndReturnViewModel<T>(AatfData facility, IWeeeClient client)
            where T : FacilityViewModelBase
        {
            var viewModel = mapper.Map<T>(facility);

            var accessToken = User.GetAccessToken();
            viewModel.CompetentAuthoritiesList = await client.SendAsync(accessToken, new GetUKCompetentAuthorities());
            viewModel.PanAreaList = await client.SendAsync(accessToken, new GetPanAreas());
            viewModel.LocalAreaList = await client.SendAsync(accessToken, new GetLocalAreas());
            viewModel.SiteAddressData.Countries = await client.SendAsync(accessToken, new GetCountries(false));

            return viewModel;
        }

        private async Task<ActionResult> ManageFacilityDetails(FacilityViewModelBase viewModel)
        {
            PreventSiteAddressNameValidationErrors();
            SetBreadcrumb(viewModel.FacilityType, null);

            using (var client = apiClient())
            {
                var doesApprovalNumberExist = false;

                var existingAatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(viewModel.Id));

                if (existingAatf.ApprovalNumber != viewModel.ApprovalNumber)
                {
                    var result = await validationWrapper.Validate(User.GetAccessToken(), viewModel);

                    if (!result.IsValid)
                    {
                        doesApprovalNumberExist = true;
                    }
                }

                if (ModelState.IsValid && !doesApprovalNumberExist)
                {
                    viewModel.CompetentAuthoritiesList = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
                    viewModel.PanAreaList = await client.SendAsync(User.GetAccessToken(), new GetPanAreas());
                    viewModel.LocalAreaList = await client.SendAsync(User.GetAccessToken(), new GetLocalAreas());

                    var request = detailsRequestCreator.ViewModelToRequest(viewModel);

                    if (existingAatf.ApprovalDate != viewModel.ApprovalDate)
                    {
                        var approvalDateFlags = await client.SendAsync(User.GetAccessToken(),
                            new CheckAatfApprovalDateChange(existingAatf.Id, viewModel.ApprovalDate.Value));

                        if (approvalDateFlags.HasFlag(CanApprovalDateBeChangedFlags.DateChanged))
                        {
                            TempData["aatfRequest"] = request;

                            return RedirectToAction(nameof(UpdateApproval), new { id = existingAatf.Id, organisationId = existingAatf.Organisation.Id });
                        }
                    }

                    await client.SendAsync(User.GetAccessToken(), request);

                    await cache.InvalidateAatfCache(existingAatf.Organisation.Id);

                    return Redirect(Url.Action("Details", new { area = "Admin", viewModel.Id }));
                }

                if (!viewModel.ModelValidated)
                {
                    ModelState.RunCustomValidation(viewModel);
                }

                var accessToken = User.GetAccessToken();
                viewModel.StatusList = Enumeration.GetAll<AatfStatus>();
                viewModel.SizeList = Enumeration.GetAll<AatfSize>();
                viewModel.CompetentAuthoritiesList = await client.SendAsync(accessToken, new GetUKCompetentAuthorities());
                viewModel.PanAreaList = await client.SendAsync(accessToken, new GetPanAreas());
                viewModel.LocalAreaList = await client.SendAsync(accessToken, new GetLocalAreas());
                viewModel.SiteAddressData.Countries = await client.SendAsync(accessToken, new GetCountries(false));

                ModelState.ApplyCustomValidationSummaryOrdering(FacilityViewModelBase.ValidationMessageDisplayOrder);

                if (doesApprovalNumberExist)
                {
                    ModelState.AddModelError("ApprovalNumber", Constants.ApprovalNumberExistsError);
                }

                return View(nameof(ManageAatfDetails), viewModel);
            }
        }

        public virtual string GenerateSharedAddress(Core.Shared.AddressData address)
        {
            var siteAddressLong = address.Address1;

            if (address.Address2 != null)
            {
                siteAddressLong += "<br/>" + address.Address2;
            }

            siteAddressLong += "<br/>" + address.TownOrCity;

            if (address.CountyOrRegion != null)
            {
                siteAddressLong += "<br/>" + address.CountyOrRegion;
            }

            if (address.Postcode != null)
            {
                siteAddressLong += "<br/>" + address.Postcode;
            }

            siteAddressLong += "<br/>" + address.CountryName;

            return siteAddressLong;
        }

        private void SetBreadcrumb(FacilityType type, string name)
        {
            switch (type)
            {
                case FacilityType.Aatf:
                    breadcrumb.InternalActivity = InternalUserActivity.ManageAatfs;
                    breadcrumb.InternalAatf = name;
                    break;
                case FacilityType.Ae:
                    breadcrumb.InternalActivity = InternalUserActivity.ManageAes;
                    breadcrumb.InternalAe = name;
                    break;
                default:
                    break;
            }
        }

        private bool IsValidRecordToEdit(DateTime currentDate, int complianceYear)
        {
            return currentDate.Year > 1 && ComplianceYearHelper.FetchCurrentComplianceYears(currentDate, true).Any(x => x.Equals(complianceYear)) ? true : false;
        }
    }
}