namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Attributes;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
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
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [ValidateReturnCreatedActionFilter]
    public class SearchedAatfResultListController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer, SearchedAatfResultListViewModel> mapper;
        private readonly ICreateWeeeSentOnAatfRequestCreator requestCreator;

        public SearchedAatfResultListController(Func<IWeeeClient> apiClient,
                                                BreadcrumbService breadcrumb,
                                                IWeeeCache cache, IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer,
                                                SearchedAatfResultListViewModel> mapper,
                                                ICreateWeeeSentOnAatfRequestCreator requestCreator)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
            this.requestCreator = requestCreator;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId, Guid returnId, Guid aatfId, Guid selectedAatfId, string selectedAatfName)
        {
            using (var client = apiClient())
            {
                var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOn(aatfId, returnId, null));
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn, aatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));
                var aatfAddressList = await client.SendAsync(User.GetAccessToken(), new GetAatfAddressBySearchId(selectedAatfId));

                var model = mapper.Map(new ReturnAndAatfToSearchedAatfViewModelMapTransfer()
                {
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = organisationId,
                    Sites = aatfAddressList,
                    AatfName = selectedAatfName,
                    SelectedAatfId = selectedAatfId,
                    SelectedAatfName = selectedAatfName
                });

                if (aatfAddressList != null && aatfAddressList.Count > 0)
                {
                    return View(model);
                }
                else
                {
                    return await Task.Run<ActionResult>(() =>
                    RedirectToAction("Index", "CanNotFoundTreatmentFacility", new { area = "AatfReturn", returnId = returnId, aatfId = aatfId, aatfName = selectedAatfName, isCanNotFindLinkClick = false }));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SearchedAatfResultListViewModel searchedAatfModel)
        {
            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var weeeSentOn = await client.SendAsync(User.GetAccessToken(), new GetWeeeSentOn(searchedAatfModel.AatfId, searchedAatfModel.ReturnId, null));
                    var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(searchedAatfModel.ReturnId, false));

                    await SetBreadcrumb(searchedAatfModel.OrganisationId, BreadCrumbConstant.AatfReturn, searchedAatfModel.AatfId, DisplayHelper.YearQuarterPeriodFormat(@return.Quarter, @return.QuarterWindow));
                    var aatfAddressList = await client.SendAsync(User.GetAccessToken(), new GetAatfAddressBySearchId(searchedAatfModel.SelectedAatfId));
                    searchedAatfModel.Sites = aatfAddressList;

                    return View(searchedAatfModel);
                }
            }
            else
            {
                using (var client = apiClient())
                {
                    var viewModel = new CreateWeeeSentOnViewModel()
                    {
                        AatfId = searchedAatfModel.AatfId,
                        OrganisationId = searchedAatfModel.OrganisationId,
                        ReturnId = searchedAatfModel.ReturnId,
                        SelectedWeeeSentOnId = searchedAatfModel.SelectedWeeeSentOnId.Value
                    };

                    var request = requestCreator.ViewModelToRequest(viewModel);
                    var result = await client.SendAsync(User.GetAccessToken(), request);

                    return AatfRedirect.ObligatedSentOn(searchedAatfModel.SelectedSiteName, searchedAatfModel.OrganisationId, searchedAatfModel.AatfId, searchedAatfModel.ReturnId, result, false, true);
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