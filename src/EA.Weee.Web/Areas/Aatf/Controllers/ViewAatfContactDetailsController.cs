namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ViewAatfContactDetailsController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ViewAatfContactDetailsController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client)
        {
            this.apiClient = client;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid aatfId, bool isAE)
        {
            using (var client = apiClient())
            {
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfByIdExternal(aatfId, organisationId));

                var model = new ViewAatfContactDetailsViewModel() { OrganisationId = organisationId, AatfId = aatfId, Contact = aatf.Contact, AatfName = aatf.Name, IsAE = isAE };

                if (isAE)
                {
                    await SetBreadcrumb(model.OrganisationId, "View AE contact details", aatf.Name, aatf.ApprovalNumber, aatf.FacilityType, aatf);
                }
                else
                {
                    await SetBreadcrumb(model.OrganisationId, "View AATF contact details", aatf.Name, aatf.ApprovalNumber, aatf.FacilityType, aatf);
                }

                return View(model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, string aatfName, string aatfApproval, string aatfFacility, AatfDataExternal aatf)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalAatf = aatf;
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}