namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using EA.Weee.Api.Client;
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
                var aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfByIdExternal(aatfId));

                var model = new ViewAatfContactDetailsViewModel() { OrganisationId = organisationId, AatfId = aatfId, Contact = aatf.Contact, AatfName = aatf.Name, IsAE = isAE };

                await SetBreadcrumb(model.OrganisationId, "AATF contact details", aatf.Name, aatf.ApprovalNumber, aatf.FacilityType);

                return View(model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, string aatfName, string aatfApproval, string aatfFacility)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            breadcrumb.ExternalAatfName = aatfName;
            breadcrumb.ExternalAatfApprovalNumber = aatfApproval;
            breadcrumb.ExternalAatfFacilityType = aatfFacility;
        }
    }
}