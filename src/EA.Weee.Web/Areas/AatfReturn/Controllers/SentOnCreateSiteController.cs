﻿namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [ValidateReturnCreatedActionFilter]
    public class SentOnCreateSiteController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSentOnAatfSiteRequestCreator requestCreator;
        private readonly IMap<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer, SentOnCreateSiteViewModel> mapper;

        public SentOnCreateSiteController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IAddSentOnAatfSiteRequestCreator requestCreator, IMap<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer, SentOnCreateSiteViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.requestCreator = requestCreator;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId, Guid? weeeSentOnId, bool? isEditDetails = false, bool? isEditTonnage = false, bool? isAddDetails = false)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));
                WeeeSentOnData weeeSentOn = null;

                if (weeeSentOnId != null)
                {
                    weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOnById(weeeSentOnId.Value));
                }

                var countryData = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));

                var viewModel = mapper.Map(new ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer()
                {
                    CountryData = countryData,
                    Return = @return,
                    AatfId = aatfId,
                    WeeeSentOnData = weeeSentOn
                });

                viewModel.IsEditDetails = isEditDetails;
                viewModel.IsEditTonnage = isEditTonnage;
                viewModel.IsAddDetails = isAddDetails;

                if (isEditTonnage.Value)
                {
                    var request = requestCreator.ViewModelToRequest(viewModel);

                    var result = await client.SendAsync(User.GetAccessToken(), request);

                    return AatfRedirect.ObligatedSentOn(viewModel.SiteAddressData.Name, viewModel.OrganisationId, viewModel.AatfId, viewModel.ReturnId, result);
                }

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                TempData["currentQuarter"] = @return.Quarter;
                TempData["currentQuarterWindow"] = @return.QuarterWindow;

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SentOnCreateSiteViewModel viewModel, bool? noJavascriptCopy, bool? isEditDetails = false, bool? isEditTonnage = false)
        {
            if (NoJavascriptCopy(noJavascriptCopy))
            {
                CopySiteAddressToOperatorAddress(viewModel);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var client = apiClient())
                    {
                        if (isEditDetails.Value)
                        {
                            var request = requestCreator.ViewModelToRequest(viewModel);

                            await client.SendAsync(User.GetAccessToken(), request);

                            return AatfRedirect.SentOnSummaryList(viewModel.OrganisationId, viewModel.ReturnId, viewModel.AatfId);
                        }
                        else
                        {
                            var request = requestCreator.ViewModelToRequest(viewModel);

                            var result = await client.SendAsync(User.GetAccessToken(), request);

                            return AatfRedirect.ObligatedSentOn(viewModel.SiteAddressData.Name, viewModel.OrganisationId, viewModel.AatfId, viewModel.ReturnId, result);
                        }
                    }
                }
            }

            using (var client = apiClient())
            {
                viewModel.SiteAddressData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
                viewModel.OperatorAddressData.Countries = await client.SendAsync(User.GetAccessToken(), new GetCountries(false));
            }

            await SetBreadcrumb(viewModel.OrganisationId, BreadCrumbConstant.AatfReturn, viewModel.AatfId, DisplayHelper.YearQuarterPeriodFormat(TempData["currentQuarter"] as Quarter, TempData["currentQuarterWindow"] as QuarterWindow));

            return View(viewModel);
        }

        private void CopySiteAddressToOperatorAddress(SentOnCreateSiteViewModel viewModel)
        {
            viewModel.OperatorAddressData.Address1 = viewModel.SiteAddressData.Address1;
            viewModel.OperatorAddressData.Address2 = viewModel.SiteAddressData.Address2;
            viewModel.OperatorAddressData.TownOrCity = viewModel.SiteAddressData.TownOrCity;
            viewModel.OperatorAddressData.CountyOrRegion = viewModel.SiteAddressData.CountyOrRegion;
            viewModel.OperatorAddressData.Postcode = viewModel.SiteAddressData.Postcode;
            viewModel.OperatorAddressData.CountryId = viewModel.SiteAddressData.CountryId;
            viewModel.OperatorAddressData.CountryName = viewModel.SiteAddressData.CountryName;
            ModelState.Clear();
        }

        private static bool NoJavascriptCopy(bool? javascriptCopy)
        {
            return javascriptCopy.HasValue && javascriptCopy.Value;
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity, Guid aatfId, string quarter)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
            var aatfInfo = await cache.FetchAatfData(organisationId, aatfId);
            breadcrumb.QuarterDisplayInfo = quarter;
            breadcrumb.AatfDisplayInfo = DisplayHelper.ReportingOnValue(aatfInfo.Name, aatfInfo.ApprovalNumber);
        }
    }
}