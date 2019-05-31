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
using EA.Weee.Web.Areas.Admin.Controllers.Base;
using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
using EA.Weee.Web.Areas.Admin.ViewModels.Scheme;
using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
using EA.Weee.Web.Infrastructure;
using EA.Weee.Web.Services;
using EA.Weee.Web.Services.Caching;

namespace EA.Weee.Web.Areas.Admin.Controllers
{
    public class OrganisationController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public OrganisationController(Func<IWeeeClient> apiClient, IWeeeCache cache, BreadcrumbService breadcrumb, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> EditSoleTraderOrIndividualOrganisationDetails(Guid schemeId, Guid orgId)
        {
            await SetBreadcrumb(schemeId);

            using (var client = apiClient())
            {
                var organisationData = await client.SendAsync(User.GetAccessToken(), new OrganisationBySchemeId(schemeId));
                if (!organisationData.CanEditOrganisation)
                {
                    return new HttpForbiddenResult();
                }
                var countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var model = new EditSoleTraderOrIndividualOrganisationDetailsViewModel
                {
                    OrganisationType = organisationData.OrganisationType,
                    BusinessTradingName = organisationData.TradingName,
                    BusinessAddress = organisationData.BusinessAddress
                };

                model.BusinessAddress.Countries = countries;
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
                var organisationData = await client.SendAsync(User.GetAccessToken(), new OrganisationBySchemeId(schemeId));
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
                    BusinessAddress = organisationData.BusinessAddress
                };

                model.BusinessAddress.Countries = countries;
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

            return RedirectToAction("Overview", new { schemeId = model.SchemeId, overviewDisplayOption = OverviewDisplayOption.OrganisationDetails });
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