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
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Services;
    using Services.Caching;
    using ViewModels.Submissions;
    using Weee.Requests.Admin;

    public class SubmissionsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private const int DefaultPageSize = 25;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;

        public SubmissionsController(BreadcrumbService breadcrumb, Func<IWeeeClient> client, IWeeeCache cache)
        {
            this.breadcrumb = breadcrumb;
            this.apiClient = client;
            this.cache = cache;
        }

        /// <summary>
        /// This method is used by both JS and non-JS users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> SubmissionsHistory()
        {
           using (var client = apiClient())
            {
                await SetBreadcrumb();

                try
                {
                    //Get all the compliance years currently in database and set it to latest one.
                    //Get all the approved PCSs
                    var allYears = await client.SendAsync(User.GetAccessToken(), new GetAllComplianceYears());
                    var allSchemes = await client.SendAsync(User.GetAccessToken(), new GetAllApprovedSchemes());
                    SubmissionsHistoryViewModel model = new SubmissionsHistoryViewModel
                    {
                        ComplianceYears = new SelectList(allYears),
                        SchemeNames = new SelectList(allSchemes, "Id", "SchemeName"),
                        SelectedYear = allYears.First(),
                        SelectedScheme = allSchemes.First().SchemeName
                    };
                    return View(model);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return View();
                }
            }
        }

        /// <summary>
        /// This method is used by non-JS users to retrieve search results.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmissionsHistory(SubmissionsHistoryViewModel viewModel)
        {
            await SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            return RedirectToAction("ChooseActivity", "Home");
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> FetchSearchResultsJson(int year, string schemeName)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            //TODO
            IList<SubmissionsHistorySearchResult> searchResults = new List<SubmissionsHistorySearchResult>();

            return Json(searchResults, JsonRequestBehavior.AllowGet);
        }

       private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "View submissions history";

            await Task.Yield();
        }
    }
}