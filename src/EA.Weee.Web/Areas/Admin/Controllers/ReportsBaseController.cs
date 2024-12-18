namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Admin;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Core.Constants;
    using EA.Weee.Web.Constant;
    using Infrastructure;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Home;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Admin.Reports;
    using Weee.Requests.Shared;

    public class ReportsBaseController : AdminController
    {
        protected readonly Func<IWeeeClient> ApiClient;
        private readonly BreadcrumbService breadcrumb;

        public ReportsBaseController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb)
        {
            this.ApiClient = apiClient;
            this.breadcrumb = breadcrumb;
        }

        protected async Task<SelectList> PatAreaList()
        {
            return new SelectList(await FetchPatAreas(), "Id", "Name");
        }

        protected async Task<SelectList> LocalAreaList()
        {
            return new SelectList(await FetchLocalAreas(), "Id", "Name");
        }

        protected async Task<SelectList> CompetentAuthoritiesList()
        {
            return new SelectList(await FetchAuthorities(), "Id", "Abbreviation");
        }

        protected async Task<IList<PanAreaData>> FetchPatAreas()
        {
            using (var client = ApiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetPanAreas());
            }
        }

        protected async Task<IList<LocalAreaData>> FetchLocalAreas()
        {
            using (var client = ApiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetLocalAreas());
            }
        }

        protected async Task<IList<UKCompetentAuthorityData>> FetchAuthorities()
        {
            using (var client = ApiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
            }
        }

        protected async Task<List<SchemeData>> FetchSchemes(bool includeDirectRegistrant = false, bool includeAllSchemes = false)
        {
            var request = new GetSchemes(GetSchemes.FilterType.ApprovedOrWithdrawn);
            List<SchemeData> schemesList;

            using (var client = ApiClient())
            {
                schemesList = await client.SendAsync(User.GetAccessToken(), request);
            }

            if (includeDirectRegistrant)
            {
                schemesList.Insert(0, new SchemeData() { SchemeName = ReportsFixedIdConstant.AllDirectRegistrants, Id = ReportsFixedIdConstant.AllDirectRegistrantFixedId });
            }

            if (includeAllSchemes)
            {
                schemesList.Insert(0, new SchemeData() { SchemeName = ReportsFixedIdConstant.AllSchemes, Id = ReportsFixedIdConstant.AllSchemeFixedId });
            }

            return schemesList;
        }

        protected async Task<List<SchemeData>> FetchSchemesWithObligationOrEvidence(int complianceYear)
        {
            if (complianceYear > 0)
            {
                var request = new GetSchemesWithObligationOrEvidence(complianceYear);
                using (var client = ApiClient())
                {
                    return await client.SendAsync(User.GetAccessToken(), request);
                }
            }

            return new List<SchemeData>();
        }

        protected async Task<List<int>> FetchComplianceYearsForAatf()
        {
            var request = new GetAatfAeActiveComplianceYears();
            using (var client = ApiClient())
            {
                var items = await client.SendAsync(User.GetAccessToken(), request);
                return items;
            }
        }

        protected async Task<List<int>> FetchComplianceYearsForAatfReturns()
        {
            var request = new GetAatfReturnsActiveComplianceYears();
            using (var client = ApiClient())
            {
                var items = await client.SendAsync(User.GetAccessToken(), request);
                return items;
            }
        }

        protected void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ViewReports;
        }

        protected async Task<ActionResult> CheckUserStatus(string redirectController, string redirectAction, Func<Task<ActionResult>> viewResult = null)
        {
            using (var client = ApiClient())
            {
                var userStatus = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                switch (userStatus)
                {
                    case UserStatus.Active:
                        if (viewResult != null)
                        {
                           return await viewResult.Invoke();
                        }

                        return RedirectToAction(redirectAction, redirectController);
                    case UserStatus.Inactive:
                    case UserStatus.Pending:
                    case UserStatus.Rejected:
                        return RedirectToAction("InternalUserAuthorisationRequired", "Account", new { userStatus });
                    default:
                        throw new NotSupportedException(
                            $"Cannot determine result for user with status '{userStatus}'");
                }
            }
        }
    }
}