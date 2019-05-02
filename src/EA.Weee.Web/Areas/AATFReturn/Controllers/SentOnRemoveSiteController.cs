﻿namespace EA.Weee.Web.Areas.AatfReturn.Controllers
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

    public class SentOnRemoveSiteController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel> mapper;

        public SentOnRemoveSiteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId, Guid weeeSentOnId)
        {
            using (var client = apiClient())
            {
                var weeeSentOnList = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOn(aatfId, returnId, weeeSentOnId));
                var weeeSentOn = weeeSentOnList[0];

                var siteAddress = GenerateAddress(weeeSentOn.SiteAddress);
                var operatorAddress = GenerateAddress(weeeSentOn.OperatorAddress);

                var viewModel = mapper.Map(new ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
                {
                    ReturnId = returnId,
                    AatfId = aatfId,
                    OrganisationId = organisationId,
                    WeeeSentOn = weeeSentOn,
                    SiteAddress = siteAddress,
                    OperatorAddress = operatorAddress
                });
                /*
                var model = new SentOnRemoveSiteViewModel()
                {
                    WeeeSentOn = weeeSentOn,
                    OrganisationId = organisationId,
                    ReturnId = returnId,
                    AatfId = aatfId,
                    SiteAddress = siteAddress,
                    OperatorAddress = operatorAddress,
                    TonnageB2B = 0.000m,
                    TonnageB2C = 0.000m
                };

                foreach (var category in weeeSentOn.Tonnages)
                {
                    if (category.B2B != null)
                    {
                        model.TonnageB2B += category.B2B;
                    }

                    if (category.B2C != null)
                    {
                        model.TonnageB2C += category.B2C;
                    }
                }
                */
                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SentOnRemoveSiteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    if (viewModel.SelectedValue == "Yes")
                    {
                        var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new RemoveWeeeSentOn(viewModel.WeeeSentOnId));
                    }

                    return await Task.Run<ActionResult>(() =>
                        RedirectToAction("Index", "SentOnSiteSummaryList", new { area = "AatfReturn", organisationId = viewModel.OrganisationId, returnId = viewModel.ReturnId, aatfId = viewModel.AatfId }));
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

        private string GenerateAddress(AatfAddressData address)
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

            siteAddressLong += "<br/>" + address.CountryName + "<br/>" + address.Postcode;

            return siteAddressLong;
        }
    }
}