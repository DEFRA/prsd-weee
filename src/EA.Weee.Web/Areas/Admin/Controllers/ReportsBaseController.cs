namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Admin;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Services;
    using ViewModels.Home;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Shared;

    public class ReportsBaseController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public ReportsBaseController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
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
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetPanAreas());
            }
        }

        protected async Task<IList<LocalAreaData>> FetchLocalAreas()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetLocalAreas());
            }
        }

        protected async Task<IList<UKCompetentAuthorityData>> FetchAuthorities()
        {
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());
            }
        }

        protected async Task<List<SchemeData>> FetchSchemes()
        {
            var request = new GetSchemes(GetSchemes.FilterType.ApprovedOrWithdrawn);
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        protected async Task<List<int>> FetchComplianceYearsForAatf()
        {
            var request = new GetAatfAeActiveComplianceYears();
            using (var client = apiClient())
            {
                var items = await client.SendAsync(User.GetAccessToken(), request);
                return items;
            }
        }

        protected async Task<List<int>> FetchComplianceYearsForAatfReturns()
        {
            var request = new GetAatfReturnsActiveComplianceYears();
            using (var client = apiClient())
            {
                var items = await client.SendAsync(User.GetAccessToken(), request);
                return items;
            }
        }

        protected void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ViewReports;
        }

        protected async Task<ActionResult> CheckUserStatus(string redirectController, string redirectAction)
        {
            using (var client = apiClient())
            {
                var userStatus = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                switch (userStatus)
                {
                    case UserStatus.Active:
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