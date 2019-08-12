namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public class CopyAatfController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly IWeeeCache cache;

        public CopyAatfController(
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IMapper mapper,
            IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult> CopyAatfDetails(Guid id)
        {
            using (var client = apiClient())
            {
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                if (!aatf.CanEdit)
                {
                    return new HttpForbiddenResult();
                }

                SetBreadcrumb(aatf.FacilityType, aatf.Name);

                switch (aatf.FacilityType)
                {
                    case FacilityType.Aatf:
                        return View("Copy", await PopulateAndReturnViewModel<CopyAatfViewModel>(aatf));
                    case FacilityType.Ae:
                        return View("Copy", await PopulateAndReturnViewModel<CopyAeViewModel>(aatf));
                    default:
                        throw new ArgumentOutOfRangeException(nameof(aatf.FacilityType));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CopyAatfDetails(CopyAatfViewModel viewModel)
        {
            return await CopyFacilityDetails(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CopyAeDetails(CopyAeViewModel viewModel)
        {
            return await CopyFacilityDetails(viewModel);
        }

        private async Task<T> PopulateAndReturnViewModel<T>(AatfData facility)
    where T : CopyFacilityViewModelBase
        {
            var viewModel = mapper.Map<T>(facility);
            viewModel.OrganisationId = facility.Organisation.Id;
            viewModel.AatfId = facility.AatfId;
            viewModel = await PopulateViewModelLists(viewModel);

            return viewModel;
        }

        private async Task<T> PopulateViewModelLists<T>(T viewModel)
            where T : CopyFacilityViewModelBase
        {
            using (var client = apiClient())
            {
                var accessToken = User.GetAccessToken();
                var countries = await client.SendAsync(accessToken, new GetCountries(false));

                viewModel.ContactData.AddressData.Countries = countries;
                viewModel = await AddAatfController.PopulateFacilityViewModelLists(viewModel, countries, client, User.GetAccessToken());
            }

            return viewModel;
        }

        private async Task<ActionResult> CopyFacilityDetails(CopyFacilityViewModelBase viewModel)
        {
            PreventSiteAddressNameValidationErrors();
            SetBreadcrumb(viewModel.FacilityType, null);
            viewModel = await PopulateViewModelLists(viewModel);
            using (var client = apiClient())
            {
                if (!ModelState.IsValid)
                {
                    if (!viewModel.ModelValidated)
                    {
                        ModelState.RunCustomValidation(viewModel);
                    }

                    ModelState.ApplyCustomValidationSummaryOrdering(CopyFacilityViewModelBase.ValidationMessageDisplayOrder);
                    return View("Copy", viewModel);
                }

                if (ModelState.IsValid)
                {
                    var request = new CopyAatf()
                    {
                        Aatf = AddAatfController.CreateFacilityData(viewModel),
                        OrganisationId = viewModel.OrganisationId,
                        AatfId = viewModel.AatfId,
                        AatfContact = viewModel.ContactData
                    };

                    await client.SendAsync(User.GetAccessToken(), request);

                    await cache.InvalidateAatfCache(request.OrganisationId);
                }

                return RedirectToAction("ManageAatfs", "Aatf", new { viewModel.FacilityType });
            }
        }

        private void SetBreadcrumb(FacilityType type, string name)
        {
            switch (type)
            {
                case FacilityType.Aatf:
                    breadcrumb.InternalActivity = InternalUserActivity.CopyAatf;
                    breadcrumb.InternalAatf = name;
                    break;
                case FacilityType.Ae:
                    breadcrumb.InternalActivity = InternalUserActivity.CopyAe;
                    breadcrumb.InternalAe = name;
                    break;
                default:
                    break;
            }
        }
    }
}