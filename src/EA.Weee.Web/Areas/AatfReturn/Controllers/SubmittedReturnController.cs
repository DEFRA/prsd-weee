namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Attributes;
    using Constant;
    using EA.Weee.Requests.AatfReturn;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;

    public class SubmittedReturnController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public SubmittedReturnController(Func<IWeeeClient> apiClient,
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
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId));

                await SetBreadcrumb(@return.ReturnOperatorData.OrganisationId, BreadCrumbConstant.AatfReturn);

                return View("Index", mapper.Map<SubmittedReturnViewModel>(@return));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(SubmittedReturnViewModel model)
        {
            return await Task.Run<ActionResult>(() =>
                RedirectToAction("ChooseActivity", "Home", new { area = "Scheme", pcsId = model.OrgansationId }));
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}
