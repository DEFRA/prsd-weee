namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.Requests;
    using EA.Weee.Web.Areas.Admin.ViewModels.Ae;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public class AeController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public AeController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> ManageAes()
        {
            SetBreadcrumb();
            return View(new ManageAesViewModel { AeDataList = await GetAes() });
        }

        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            SetBreadcrumb();

            using (var client = apiClient())
            {
                AatfData aatf = await client.SendAsync(User.GetAccessToken(), new GetAatfById(id));

                List<AatfDataList> associatedAatfs = await client.SendAsync(User.GetAccessToken(), new GetAatfsByOperatorId(aatf.Operator.Id));

                List<Core.Scheme.SchemeData> associatedSchemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesByOrganisationId(aatf.Operator.OrganisationId));

                AeDetailsViewModel viewModel = mapper.Map<AeDetailsViewModel>(new AatfDataToAeDetailsViewModelMapTransfer(aatf)
                {
                    OrganisationString = GenerateAddress(aatf.Operator.Organisation.BusinessAddress),
                    AssociatedAatfs = associatedAatfs,
                    AssociatedSchemes = associatedSchemes
                });

                return View(viewModel);
            }
        }

        private async Task<List<AatfDataList>> GetAes()
        {
            using (var client = apiClient())
            {
                var list = await client.SendAsync(User.GetAccessToken(), new GetAatfs(FacilityType.Ae));
                return list;
            }
        }

        private void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ManageAes;
        }

        public virtual string GenerateAddress(Core.Shared.AddressData address)
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
    }
}