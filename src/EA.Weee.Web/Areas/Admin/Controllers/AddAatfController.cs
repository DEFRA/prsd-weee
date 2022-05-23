namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Helper;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public class AddAatfController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IFacilityViewModelBaseValidatorWrapper validationWrapper;

        public AddAatfController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IFacilityViewModelBaseValidatorWrapper validationWrapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.validationWrapper = validationWrapper;
        }

        [HttpGet]
        public async Task<ActionResult> Add(Guid organisationId, FacilityType facilityType)
        {
            SetBreadcrumb(facilityType);

            switch (facilityType)
            {
                case FacilityType.Aatf:
                    return View(await PopulateAndReturnViewModel(new AddAatfViewModel(), organisationId));
                case FacilityType.Ae:
                    return View(await PopulateAndReturnViewModel(new AddAeViewModel(), organisationId));
                default:
                    throw new ArgumentOutOfRangeException(nameof(facilityType));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAatf(AddAatfViewModel viewModel)
        {
            return await AddFacility(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAe(AddAeViewModel viewModel)
        {
            return await AddFacility(viewModel);
        }

        private async Task<T> PopulateAndReturnViewModel<T>(T viewModel, Guid organisationId)
            where T : AddFacilityViewModelBase
        {
            viewModel.OrganisationId = organisationId;
            return await PopulateViewModelLists(viewModel);
        }

        private async Task<T> PopulateViewModelLists<T>(T viewModel)
            where T : AddFacilityViewModelBase
        {
            using (var client = apiClient())
            {
                var countries = await GetCountries();
                var organisation = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(viewModel.OrganisationId));

                viewModel.ContactData.AddressData.Countries = countries;
                viewModel.OrganisationName = organisation.OrganisationName;
                viewModel = await PopulateFacilityViewModelLists(viewModel, countries, client, User.GetAccessToken());

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());
                viewModel.ComplianceYearList = new SelectList(ComplianceYearHelper.FetchCurrentComplianceYears(currentDate));
            }

            return viewModel;
        }

        private async Task<T> PopulateFacilityViewModelLists<T>(T viewModel, IList<CountryData> countries, IWeeeClient client, string accessToken)
        where T : FacilityViewModelBase
        {
            viewModel.SiteAddressData.Countries = countries;
            viewModel.CompetentAuthoritiesList = await client.SendAsync(accessToken, new GetUKCompetentAuthorities());
            viewModel.PanAreaList = await client.SendAsync(accessToken, new GetPanAreas());
            viewModel.LocalAreaList = await client.SendAsync(accessToken, new GetLocalAreas());
            viewModel.SizeList = Enumeration.GetAll<AatfSize>();
            viewModel.StatusList = Enumeration.GetAll<AatfStatus>();

            return viewModel;
        }

        private async Task<IList<CountryData>> GetCountries()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }
        }

        private async Task<ActionResult> AddFacility(AddFacilityViewModelBase viewModel)
        {
            PreventSiteAddressNameValidationErrors();
            SetBreadcrumb(viewModel.FacilityType);
            viewModel = await PopulateViewModelLists(viewModel);

            if (!ModelState.IsValid)
            {
                if (!viewModel.ModelValidated)
                {
                    ModelState.RunCustomValidation(viewModel);
                }

                ModelState.ApplyCustomValidationSummaryOrdering(AddFacilityViewModelBase.ValidationMessageDisplayOrder);
                return View(nameof(Add), viewModel);
            }

            using (var client = apiClient())
            {
                var result = await validationWrapper.Validate(User.GetAccessToken(), viewModel);

                if (!result.IsValid)
                {
                    ModelState.AddModelError("ApprovalNumber", Constants.ApprovalNumberExistsError);
                    return View(nameof(Add), viewModel);
                }

                var request = new AddAatf()
                {
                    Aatf = AatfHelper.CreateFacilityData(viewModel),
                    AatfContact = viewModel.ContactData,
                    OrganisationId = viewModel.OrganisationId,
                };

                await client.SendAsync(User.GetAccessToken(), request);

                await cache.InvalidateAatfCache(request.OrganisationId);

                await client.SendAsync(User.GetAccessToken(), new CompleteOrganisationAdmin() { OrganisationId = viewModel.OrganisationId });
                await cache.InvalidateOrganisationSearch();

                return RedirectToAction("ManageAatfs", "Aatf", new { viewModel.FacilityType });
            }
        }

        private void SetBreadcrumb(FacilityType type)
        {
            switch (type)
            {
                case FacilityType.Aatf:
                    SetBreadcrumb(InternalUserActivity.ManageAatfs);
                    break;
                case FacilityType.Ae:
                    SetBreadcrumb(InternalUserActivity.ManageAes);
                    break;
                default:
                    break;
            }
        }

        private void SetBreadcrumb(string activity)
        {
            breadcrumb.InternalActivity = activity;
        }
    }
}
