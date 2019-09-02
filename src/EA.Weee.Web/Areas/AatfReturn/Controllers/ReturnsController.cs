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
        public virtual async Task<ActionResult> Index(Guid organisationId)
        {
            ReturnsViewModel viewModel = await PrepareViewModel(organisationId, null);

            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

            return View(viewModel);
        }

        private async Task<ReturnsViewModel> PrepareViewModel(Guid organisationId, int? complianceYear, string quarter = "All")
        {
            using (var client = apiClient())
            {
                var @return = await client.SendAsync(User.GetAccessToken(), new GetReturns(organisationId, FacilityType.Aatf));

                var complianceYearList = @return.ReturnsList.Select(x => x.Quarter.Year).Distinct().ToList();
                var latestComplianceYear = complianceYearList.OrderByDescending(x => x).FirstOrDefault();

                //Need to do the below 2 lines when compliance year drop down changes, so that the quarter dropdown dynamically changes
                var quartersForLatestComplianceYear = @return.ReturnsList.Select(x => x.Quarter).ToList();
                var filteredQuarterList = quartersForLatestComplianceYear.Where(x => x.Year == latestComplianceYear).Select(a => a.Q.ToString()).Distinct().ToList();

                var viewModel = mapper.Map<ReturnsViewModel>(@return);

                viewModel.QuarterList = filteredQuarterList;
                viewModel.ComplianceYearList = complianceYearList;
                
                viewModel.OrganisationName = (await client.SendAsync(User.GetAccessToken(), new GetOrganisationInfo(organisationId))).OrganisationName;
                viewModel.OrganisationId = organisationId;

                if (complianceYear != null)
                {
                    viewModel.SelectedComplianceYear = complianceYear.GetValueOrDefault();
                    viewModel.Returns = viewModel.Returns.Where(p => p.ReturnViewModel.Year == complianceYear.ToString()).ToList();
                }
                else
                {
                    viewModel.SelectedComplianceYear = latestComplianceYear;
                }

                if (quarter != "All")
                {
                    viewModel.Returns = viewModel.Returns.Where(p => p.ReturnViewModel.Quarter == quarter).ToList();
                    viewModel.SelectedQuarter = quarter;
                }
                else
                {
                    viewModel.SelectedQuarter = "All";
                }

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfReturn);

                return viewModel;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(ReturnsViewModel model, bool applyFilter = false)
        {
            if (applyFilter)
            {
                ReturnsViewModel viewModel = await PrepareViewModel(model.OrganisationId, model.SelectedComplianceYear, model.SelectedQuarter);

                return View(viewModel);
            }

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