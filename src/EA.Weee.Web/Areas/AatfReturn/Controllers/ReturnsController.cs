namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Api.Client;
    using Attributes;
    using Constant;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Organisations;

    [ValidateOrganisationActionFilterAttribute]
    public class ReturnsController : AatfReturnBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper; 

        public ReturnsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IWeeeCache cache, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturns(organisationId, FacilityType.Aatf));

                var viewModel = mapper.Map<ReturnsViewModel>(@return);

                viewModel.OrganisationName = (await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId))).OrganisationName;
                viewModel.OrganisationId = organisationId;

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateReturnEditActionFilter]
        [Route("aatf-return/returns/{organisationId:Guid}/copy/{returnId:Guid}")]
        public virtual async Task<ActionResult> Copy(Guid returnId, Guid organisationId)
        {
            using (var client = apiClient())
            {
                var newId = await client.SendAsync(User.GetAccessToken(), new CopyReturn(returnId));

                return AatfRedirect.TaskList(newId);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReturnsViewModel model)
        {
            using (var client = apiClient())
            {
                AddReturn request = new AddReturn()
                {
                    OrganisationId = model.OrganisationId,
                    FacilityType = FacilityType.Aatf
                };

                var aatfReturnId = await client.SendAsync(User.GetAccessToken(), request);

                return AatfRedirect.SelectReportOptions(model.OrganisationId, aatfReturnId);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}