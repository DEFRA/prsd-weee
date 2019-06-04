namespace EA.Weee.Web.Areas.AeReturn.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Areas.AeReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ReturnsController : AeReturnBaseController
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
                var @returns = await client.SendAsync(User.GetAccessToken(), new GetReturns(organisationId, FacilityType.Ae));

                var viewModel = mapper.Map<ReturnsViewModel>(@returns);

                viewModel.OrganisationName = (await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId))).OrganisationName;
                viewModel.OrganisationId = organisationId;

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AeReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ReturnsViewModel viewModel)
        {
            return AeRedirect.ExportedWholeWeee(viewModel.OrganisationId);
        }

        [HttpGet]
        public async Task<ActionResult> ExportedWholeWeee(Guid organisationId)
        {
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AeReturn);

            ExportedWholeWeeeViewModel model = new ExportedWholeWeeeViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportedWholeWeee(Guid organisationId, ExportedWholeWeeeViewModel viewModel)
        {
            if (viewModel.WeeeSelectedValue == "yes")
            {
                return AeRedirect.ReturnsList(organisationId);
            }

            return AeRedirect.NilReturn(organisationId);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}