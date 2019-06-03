namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme;
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
        public async Task<ActionResult> EditSoleTraderOrIndividualOrganisationDetails(Guid? schemeId, Guid orgId, Guid? aatfId)
        {
            await SetBreadcrumb(schemeId, aatfId, orgId);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(orgId));
                if (!organisationData.CanEditOrganisation)
                {
                    return new HttpForbiddenResult();
                }
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var model = new EditSoleTraderOrIndividualOrganisationDetailsViewModel
                {
                    OrganisationType = organisationData.OrganisationType,
                    BusinessTradingName = organisationData.TradingName,
                    BusinessAddress = organisationData.BusinessAddress,
                    SchemeId = schemeId,
                    OrgId = orgId,
                    AatfId = aatfId
                };

                model.BusinessAddress.Countries = countries;
                
                return View("EditSoleTraderOrIndividualOrganisationDetails", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditRegisteredCompanyOrganisationDetails(Guid? schemeId, Guid orgId, Guid? aatfId)
        {
            await SetBreadcrumb(schemeId, aatfId, orgId);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(orgId));
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
                    AatfId = aatfId
                };

                model.BusinessAddress.Countries = countries;

                return View("EditRegisteredCompanyOrganisationDetails", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRegisteredCompanyOrganisationDetails(EditRegisteredCompanyOrganisationDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId, model.AatfId, model.OrgId);

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
        public async Task<ActionResult> EditSoleTraderOrIndividualOrganisationDetails(EditSoleTraderOrIndividualOrganisationDetailsViewModel model)
        {
            await SetBreadcrumb(model.SchemeId, model.AatfId, model.OrgId);

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

        private async Task SetBreadcrumb(Guid? schemeId, Guid? aatfId, Guid organisationId)
        {
            breadcrumb.InternalActivity = string.Empty;
            breadcrumb.InternalOrganisation = string.Empty;

            if (schemeId.HasValue && !aatfId.HasValue)
            {
                breadcrumb.InternalActivity = InternalUserActivity.ManageScheme;
                breadcrumb.InternalOrganisation = await cache.FetchSchemeName(schemeId.Value);
            }
            if (!schemeId.HasValue && aatfId.HasValue)
            {
                breadcrumb.InternalActivity = InternalUserActivity.ManageAatfs;
                breadcrumb.InternalOrganisation = (await cache.FetchAatfData(organisationId, aatfId.Value)).Name;
            }
        }
    }
}