namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    public class ObligatedSentOnController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel> mapper;

        public ObligatedSentOnController(IWeeeCache cache,
            BreadcrumbService breadcrumb,
            Func<IWeeeClient> apiClient,
            IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel> mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid returnId, Guid aatfId, Guid weeeSentOnId)
        {
            using (var client = apiClient())
            {
                var weeeSentOnOperator = await client.SendAsync(User.GetAccessToken(), new GetSentOnOperatorSite(weeeSentOnId));

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                var model = mapper.Map(new ReturnToObligatedViewModelMapTransfer()
                {
                    AatfId = aatfId,
                    OrganisationId = @return.ReturnOperatorData.OrganisationId,
                    ReturnId = returnId,
                    ReturnData = @return
                }) as ObligatedSentOnViewModel;

                model.OperatorName = weeeSentOnOperator.Name;

                return View(model);
            }
        }
    }
}