namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;

    public class CheckYourReturnController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        //private readonly IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReturnViewModel> mapper;

        public CheckYourReturnController(Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb,
            IMapper mapper)    
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId)
        { 
            using (var client = apiClient())
            {
                //var schemeList = await client.SendAsync(User.GetAccessToken(), new GetReturnScheme(returnId));
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var viewModel = mapper.Map<ReturnViewModel>(@return);

                //var viewModel = mapper.Map(new ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer()
                //{
                //    ReturnId = returnId,
                //    OrganisationId = schemeList.OperatorData.OrganisationId,
                //    ReturnData = @return,
                //    SchemeDataItems = schemeList.SchemeDataItems
                //});

                await SetBreadcrumb(@return.ReturnOperatorData.OrganisationId, BreadCrumbConstant.AatfReturn);

                return View("Index", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(CheckYourReturnViewModel viewModel)
        {
            return await Task.Run<ActionResult>(() => AatfRedirect.SubmittedReturn(viewModel.ReturnId));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}
