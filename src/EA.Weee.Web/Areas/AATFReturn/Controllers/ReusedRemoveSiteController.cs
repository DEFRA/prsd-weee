namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public class ReusedRemoveSiteController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer, ReusedRemoveSiteViewModel> mapper;

        public ReusedRemoveSiteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMap<ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer, ReusedRemoveSiteViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId, Guid siteId)
        {
            using (var client = apiClient())
            {
                var sites = await client.SendAsync(User.GetAccessToken(), new GetAatfSite(aatfId, returnId));

                if (sites.AddressData.Count == 0 || sites.AddressData.Count(p => p.Id == siteId) == 0)
                {
                    return AatfRedirect.ReusedOffSiteSummaryList(returnId, aatfId, organisationId);
                }

                var site = sites.AddressData.Select(s => s).Where(s => s.Id == siteId).Single();

                var viewModel = mapper.Map(new ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer()
                {
                    SiteAddress = GenerateAddress(site),
                    Site = site,
                    SiteId = site.Id,
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = organisationId
                });

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReusedRemoveSiteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    if (viewModel.SelectedValue == "Yes")
                    {
                        await client.SendAsync(User.GetAccessToken(), new RemoveAatfSite(viewModel.SiteId));
                    }

                    return await Task.Run<ActionResult>(() =>
                        RedirectToAction("Index", "ReusedOffSiteSummaryList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId, aatfId = viewModel.AatfId }));
                }
            }

            return View(viewModel);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }

        public virtual string GenerateAddress(AddressData address)
        {
            var siteAddressLong = address.Name + "<br/>" + address.Address1;

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