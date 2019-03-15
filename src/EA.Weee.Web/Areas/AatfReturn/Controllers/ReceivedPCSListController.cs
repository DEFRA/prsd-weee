namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;

    public class ReceivedPcsListController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel> mapper;

        public ReceivedPcsListController(Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb,
            IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid returnId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                var schemeList = await client.SendAsync(User.GetAccessToken(), new GetReturnScheme(returnId));

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var viewModel = mapper.Map(new ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer()
                {
                    AatfId = aatfId,
                    ReturnId = returnId,
                    OrganisationId = @return.ReturnOperatorData.OrganisationId,
                    AatfName = (await cache.FetchAatfData(schemeList.OperatorData.OrganisationId, aatfId)).Name,
                    ReturnData = @return,
                    SchemeDataItems = schemeList.SchemeDataItems.ToList()
                });

                //var viewModel = new ReceivedPcsListViewModel
                //{
                //    AatfName = (await cache.FetchAatfData(schemeList.OperatorData.OrganisationId, aatfId)).Name,
                //    OrganisationId = schemeList.OperatorData.OrganisationId,
                //    ReturnId = returnId,
                //    AatfId = aatfId,
                //    SchemeList = schemeList.SchemeDataItems.ToList()
                //};

                await SetBreadcrumb(schemeList.OperatorData.OrganisationId, BreadCrumbConstant.AatfReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReceivedPcsListViewModel viewModel)
        {
            return await Task.Run(() => RedirectToAction("Index", "AatfTaskList", new { area = "AatfReturn", returnId = viewModel.ReturnId, organisationid = viewModel.OrganisationId }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}