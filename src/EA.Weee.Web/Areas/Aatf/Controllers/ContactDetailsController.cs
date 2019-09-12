namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Requests;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared.Aatf;
    using EA.Weee.Web.ViewModels.Shared.Aatf.Mapping;

    public class ContactDetailsController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IEditAatfContactRequestCreator contactRequestCreator;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ContactDetailsController(IWeeeCache cache, BreadcrumbService breadcrumb, Func<IWeeeClient> client, IMapper mapper, IEditAatfContactRequestCreator contactRequestCreator)
        {
            this.apiClient = client;
            this.mapper = mapper;
            this.contactRequestCreator = contactRequestCreator;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid aatfId, FacilityType facilityType)
        {
            using (var client = this.apiClient())
            {
                var aatf = await client.SendAsync(this.User.GetAccessToken(), new GetAatfByIdExternal(aatfId));

                var model = new ContactDetailsViewModel() { OrganisationId = organisationId, AatfId = aatfId, Contact = aatf.Contact, AatfName = aatf.Name, FacilityType = facilityType };

                await this.SetBreadcrumb(aatf);
               
                return this.View(model);
            }
        }

        [HttpGet]
        public virtual async Task<ActionResult> Edit(Guid id)
        {
            using (var client = this.apiClient())
            {
                var aatf = await client.SendAsync(this.User.GetAccessToken(), new GetAatfByIdExternal(id));

                var countries = await client.SendAsync(this.User.GetAccessToken(), new GetCountries(false));

                var currentDate = await client.SendAsync(this.User.GetAccessToken(), new GetApiUtcDate());

                var model = this.mapper.Map<AatfEditContactAddressViewModel>(new AatfEditContactTransfer() { AatfData = aatf, Countries = countries, CurrentDate = currentDate });

                await this.SetBreadcrumb(aatf);

                return this.View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(AatfEditContactAddressViewModel model)
        {
            await this.SetBreadcrumb(model.AatfData);

            if (this.ModelState.IsValid)
            {
                using (var client = this.apiClient())
                {
                    var request = this.contactRequestCreator.ViewModelToRequest(model);
                    request.SendNotification = true;

                    await client.SendAsync(this.User.GetAccessToken(), request);

                    return this.RedirectToAction(
                        "Index",
                        new { organisationId = model.OrganisationId, aatfId = model.Id, facilityType = model.AatfData.FacilityType });
                }
            }

            using (var client = this.apiClient())
            {
                model.ContactData.AddressData.Countries = await client.SendAsync(this.User.GetAccessToken(), new GetCountries(false));
            }

            return this.View(model);
        }

        private async Task SetBreadcrumb(AatfData aatf)
        {
            this.breadcrumb.ExternalOrganisation = await this.cache.FetchOrganisationName(aatf.Organisation.Id);
            this.breadcrumb.ExternalAatf = aatf;
            this.breadcrumb.ExternalActivity = string.Format(AatfAction.ManageAatfContactDetails, aatf.FacilityType.ToDisplayString());
            this.breadcrumb.OrganisationId = aatf.Organisation.Id;
        }
    }
}