namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    
    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public class EditOrganisationController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;

        public EditOrganisationController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public async Task<ActionResult> EditSoleTraderOrganisationDetails(Guid? schemeId, Guid orgId, Guid? aatfId, FacilityType? facilityType)
        {
            await SetBreadcrumb(schemeId, aatfId, orgId, facilityType);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetInternalOrganisation(orgId));
                if (!organisationData.CanEditOrganisation)
                {
                    return new HttpForbiddenResult();
                }
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var model = new EditSoleTraderOrganisationDetailsViewModel
                {
                    OrganisationType = organisationData.OrganisationType,
                    BusinessTradingName = organisationData.TradingName,
                    CompanyName = organisationData.Name,
                    BusinessAddress = organisationData.BusinessAddress,
                    SchemeId = schemeId,
                    OrgId = orgId,
                    AatfId = aatfId,
                    FacilityType = facilityType.GetValueOrDefault()
                };

                model.BusinessAddress.Countries = countries;
                
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditPartnershipOrganisationDetails(Guid? schemeId, Guid orgId, Guid? aatfId, FacilityType? facilityType)
        {
            await SetBreadcrumb(schemeId, aatfId, orgId, facilityType);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetInternalOrganisation(orgId));
                if (!organisationData.CanEditOrganisation)
                {
                    return new HttpForbiddenResult();
                }
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var model = new EditPartnershipOrganisationDetailsViewModel
                {
                    OrganisationType = organisationData.OrganisationType,
                    BusinessTradingName = organisationData.TradingName,
                    BusinessAddress = organisationData.BusinessAddress,
                    SchemeId = schemeId,
                    OrgId = orgId,
                    AatfId = aatfId,
                    FacilityType = facilityType.GetValueOrDefault()
                };

                model.BusinessAddress.Countries = countries;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditRegisteredCompanyOrganisationDetails(Guid? schemeId, Guid orgId, Guid? aatfId, FacilityType? facilityType)
        {
            await SetBreadcrumb(schemeId, aatfId, orgId, facilityType);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetInternalOrganisation(orgId));
                if (!organisationData.CanEditOrganisation)
                {
                    return new HttpForbiddenResult();
                }
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var model = new EditRegisteredCompanyOrganisationDetailsViewModel
                {
                    OrganisationType = organisationData.OrganisationType,
                    CompanyName = organisationData.OrganisationName,
                    BusinessTradingName = organisationData.TradingName,
                    CompaniesRegistrationNumber = organisationData.CompanyRegistrationNumber,
                    BusinessAddress = organisationData.BusinessAddress,
                    SchemeId = schemeId,
                    OrgId = orgId,
                    AatfId = aatfId,
                    FacilityType = facilityType.GetValueOrDefault()
                };

                model.BusinessAddress.Countries = countries;

                return View("EditRegisteredCompanyOrganisationDetails", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRegisteredCompanyOrganisationDetails(EditRegisteredCompanyOrganisationDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId, model.AatfId, model.OrgId, model.FacilityType);

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model.BusinessAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
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
                    BusinessAddress = model.BusinessAddress,
                };

                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationDetails(orgData));
            }

            if (model.SchemeId.HasValue)
            {
                return RedirectScheme(model.SchemeId.Value);
            }

            if (model.AatfId.HasValue)
            {
                return RedirectToAatf(model.AatfId.Value);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSoleTraderOrganisationDetails(EditSoleTraderOrganisationDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId, model.AatfId, model.OrgId, model.FacilityType);

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model.BusinessAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
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
                    Name = model.CompanyName,
                    BusinessAddress = model.BusinessAddress,
                };

                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationDetails(orgData));
            }

            if (model.SchemeId.HasValue)
            {
                return RedirectScheme(model.SchemeId.Value);
            }

            if (model.AatfId.HasValue)
            {
                return RedirectToAatf(model.AatfId.Value);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPartnershipOrganisationDetails(EditPartnershipOrganisationDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId, model.AatfId, model.OrgId, model.FacilityType);

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model.BusinessAddress.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
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
                    BusinessAddress = model.BusinessAddress,
                };

                await client.SendAsync(User.GetAccessToken(), new UpdateOrganisationDetails(orgData));
            }

            if (model.SchemeId.HasValue)
            {
                return RedirectScheme(model.SchemeId.Value);
            }

            if (model.AatfId.HasValue)
            {
                return RedirectToAatf(model.AatfId.Value);
            }

            return View(model);
        }

        private RedirectToRouteResult RedirectScheme(Guid schemeId)
        {
            return RedirectToAction("Overview", "Scheme", new { schemeId = schemeId, overviewDisplayOption = OverviewDisplayOption.OrganisationDetails });
        }

        private ActionResult RedirectToAatf(Guid aatfId)
        {
            return Redirect(Url.Action("Details", new { controller = "Aatf", area = "Admin", Id = aatfId }) + "#organisationDetails");
        }

        private async Task SetBreadcrumb(Guid? schemeId, Guid? aatfId, Guid organisationId, FacilityType? facilityType)
        {
            if (schemeId.HasValue && !aatfId.HasValue)
            {
                breadcrumb.InternalActivity = InternalUserActivity.ManageScheme;
                breadcrumb.InternalScheme = await cache.FetchSchemeName(schemeId.Value);
            }
            if (!schemeId.HasValue && aatfId.HasValue)
            {
                switch (facilityType)
                {
                    case FacilityType.Aatf:
                        breadcrumb.InternalActivity = InternalUserActivity.ManageAatfs;
                        breadcrumb.InternalAatf = (await cache.FetchAatfData(organisationId, aatfId.Value)).Name;
                        break;
                    case FacilityType.Ae:
                        breadcrumb.InternalActivity = InternalUserActivity.ManageAes;
                        breadcrumb.InternalAe = (await cache.FetchAatfData(organisationId, aatfId.Value)).Name;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}