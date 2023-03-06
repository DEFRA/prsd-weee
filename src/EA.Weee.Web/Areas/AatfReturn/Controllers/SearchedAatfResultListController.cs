namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Aatf;
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [ValidateReturnCreatedActionFilter]
    public class SearchedAatfResultListController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer, SearchedAatfResultListViewModel> mapper;
        private readonly ICreateWeeeSentOnAatfRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public SearchedAatfResultListController(Func<IWeeeClient> apiClient,
                                                IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer,
                                                SearchedAatfResultListViewModel> mapper,
                                                ICreateWeeeSentOnAatfRequestCreator requestCreator,
                                                BreadcrumbService breadcrumb,
                                                IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
            this.requestCreator = requestCreator;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId, Guid selectedAatfId, string selectedAatfName)
        {
            using (var client = apiClient())
            {
                List<AatfData> aatfDatas = new List<AatfData>();

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                if (selectedAatfId != Guid.Empty)
                {
                    var aatfResult = await client.SendAsync(this.User.GetAccessToken(), new GetAatfByIdExternalSearch(selectedAatfId));
                    if (aatfResult != null)
                    {
                        aatfDatas.Add(aatfResult);
                    }

                    var model = mapper.Map(new ReturnAndAatfToSearchedAatfViewModelMapTransfer()
                    {
                        AatfId = aatfId,
                        ReturnId = returnId,
                        OrganisationId = organisationId,
                        AatfDataList = aatfDatas,
                        AatfName = selectedAatfName,
                        SelectedAatfId = selectedAatfId,
                        SelectedAatfName = selectedAatfName,
                        SelectedSiteName = aatfResult.SiteAddress.Name
                    });

                    return View(model);
                }
                else
                {
                    var resultData = await client.SendAsync(User.GetAccessToken(), new GetSearchAatfAddress(selectedAatfName, aatfId, returnId, true));
                    if (resultData != null && resultData.Count > 0)
                    {
                        foreach (var item in resultData)
                        {
                            var aatfData = await client.SendAsync(User.GetAccessToken(), new GetAatfByIdExternalSearch(item.SearchTermId));
                            if (aatfData != null)
                            {
                                aatfDatas.Add(aatfData);
                            }
                        }

                        var model = mapper.Map(new ReturnAndAatfToSearchedAatfViewModelMapTransfer()
                        {
                            AatfId = aatfId,
                            ReturnId = returnId,
                            OrganisationId = organisationId,
                            AatfDataList = aatfDatas,
                            AatfName = selectedAatfName,
                            SelectedAatfId = selectedAatfId,
                            SelectedAatfName = selectedAatfName
                        });

                        return View(model);
                    }
                    else
                    {
                        return await Task.Run<ActionResult>(() => RedirectToAction("Index", "NoResultsFound", new { area = "AatfReturn", returnId = returnId, aatfId = aatfId, aatfName = selectedAatfName, isCanNotFindLinkClick = false }));
                    }
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SearchedAatfResultListViewModel searchedAatfModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var aatf = await client.SendAsync(User.GetAccessToken(), new Weee.Requests.Aatf.GetAatfById(searchedAatfModel.SelectedAatfId));
                    var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOn(searchedAatfModel.AatfId, searchedAatfModel.ReturnId, null));

                    var isAlreadyExists = weeeSentOn.Find(x => x.SiteAddressId == aatf.SiteAddress.Id && x.OperatorAddressId == aatf.Organisation.BusinessAddress.Id);
                    if (isAlreadyExists != null)
                    {
                        return AatfRedirect.ObligatedSentOn(searchedAatfModel.SelectedSiteName, searchedAatfModel.OrganisationId, searchedAatfModel.AatfId, searchedAatfModel.ReturnId, isAlreadyExists.WeeeSentOnId, false, true);
                    }
                    else
                    {
                        var viewModel = new CreateWeeeSentOnViewModel()
                        {
                            AatfId = searchedAatfModel.AatfId,
                            OrganisationId = searchedAatfModel.OrganisationId,
                            ReturnId = searchedAatfModel.ReturnId,
                            SelectedAatfId = searchedAatfModel.SelectedAatfId
                        };

                        var request = requestCreator.ViewModelToRequest(viewModel);
                        var result = await client.SendAsync(User.GetAccessToken(), request);

                        return AatfRedirect.ObligatedSentOn(searchedAatfModel.SelectedSiteName, searchedAatfModel.OrganisationId, searchedAatfModel.AatfId, searchedAatfModel.ReturnId, result, false, true);
                    }
                }
            }
            else
            {
                using (var client = apiClient())
                {
                    var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(searchedAatfModel.ReturnId, false));

                    await SetBreadcrumb(@return.OrganisationData.Id, BreadCrumbConstant.AatfReturn, searchedAatfModel.AatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));

                    List<AatfData> aatfDatas = new List<AatfData>();
                    var resultData = await client.SendAsync(User.GetAccessToken(), new GetSearchAatfAddress(searchedAatfModel.SelectedAatfName, searchedAatfModel.AatfId, searchedAatfModel.ReturnId, true));
                    if (resultData != null && resultData.Count > 0)
                    {
                        foreach (var item in resultData)
                        {
                            var aatfData = await client.SendAsync(User.GetAccessToken(), new GetAatfByIdExternalSearch(item.SearchTermId));
                            if (aatfData != null)
                            {
                                aatfDatas.Add(aatfData);
                            }
                        }
                    }

                    searchedAatfModel = mapper.Map(new ReturnAndAatfToSearchedAatfViewModelMapTransfer()
                    {
                        AatfId = searchedAatfModel.AatfId,
                        ReturnId = searchedAatfModel.ReturnId,
                        OrganisationId = searchedAatfModel.OrganisationId,
                        AatfDataList = aatfDatas,
                        AatfName = searchedAatfModel.SelectedAatfName,
                        SelectedAatfId = searchedAatfModel.SelectedAatfId,
                        SelectedAatfName = searchedAatfModel.SelectedAatfName
                    });

                    return View(searchedAatfModel);
                }
            }
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