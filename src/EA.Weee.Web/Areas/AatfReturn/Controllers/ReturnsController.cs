namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using Api.Client;
    using Attributes;
    using Constant;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Web.ViewModels.Returns;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Organisations;

    [ValidateOrganisationActionFilterAttribute(FacilityType = FacilityType.Aatf)]
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
        public virtual async Task<ActionResult> Index(Guid organisationId, int? selectedComplianceYear, string selectedQuarter)
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturns(organisationId, FacilityType.Aatf));

                var viewModel = mapper.Map<ReturnsViewModel>(new ReturnToReturnsViewModelTransfer() { ReturnsData = @return, SelectedQuarter = selectedQuarter, SelectedComplianceYear = selectedComplianceYear });
                viewModel.OrganisationId = organisationId;

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReturnsViewModel model)
        {
            using (var client = apiClient())
            {
                var aatfReturnId = await client.SendAsync(User.GetAccessToken(), new AddReturn()
                {
                    OrganisationId = model.OrganisationId,
                    Year = model.ComplianceYear,
                    Quarter = model.Quarter,
                    FacilityType = FacilityType.Aatf
                });

                return AatfRedirect.SelectReportOptions(model.OrganisationId, aatfReturnId);
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

                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturn(returnId, false));

                if (@return.NilReturn)
                {
                    return AatfRedirect.SelectReportOptions(organisationId, newId);
                }

                return AatfRedirect.TaskList(newId);
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