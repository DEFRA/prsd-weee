namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Admin;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Services;
    using Services.Caching;
    using ViewModels.Submissions;
    using Weee.Requests.Admin;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using GetSubmissionsHistoryResults = Weee.Requests.Shared.GetSubmissionsHistoryResults;

    public class SubmissionsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly CsvWriterFactory csvWriterFactory;

        public SubmissionsController(BreadcrumbService breadcrumb, Func<IWeeeClient> client, IWeeeCache cache, CsvWriterFactory csvWriterFactory)
        {
            this.breadcrumb = breadcrumb;
            this.apiClient = client;
            this.cache = cache;
            this.csvWriterFactory = csvWriterFactory;
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
                        SelectedYear = allYears.FirstOrDefault(),
                        SelectedScheme = allSchemes.Count > 0 ? allSchemes.First().Id : Guid.Empty
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
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FetchSubmissionResults(int year, Guid schemeId)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            if (!ModelState.IsValid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            using (var client = apiClient())
            {
                try
                {
                    var schemeData = await client.SendAsync(User.GetAccessToken(), new GetSchemeById(schemeId));

                    IList<SubmissionsHistorySearchResult> searchResults = await client.SendAsync(User.GetAccessToken(), new GetSubmissionsHistoryResults(schemeId, schemeData.OrganisationId, year));
                    return PartialView("_submissionsResults", searchResults);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadCSV(Guid schemeId, int year, Guid memberUploadId, DateTime submissionDateTime)
        {
            using (var client = apiClient())
            {
                IEnumerable<UploadErrorData> errors =
                    (await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(schemeId, memberUploadId)))
                    .OrderByDescending(e => e.ErrorLevel);

                CsvWriter<UploadErrorData> csvWriter = csvWriterFactory.Create<UploadErrorData>();
                csvWriter.DefineColumn("Description", e => e.Description);

                var schemePublicInfo = await cache.FetchSchemePublicInfo(schemeId);
                var csvFileName = string.Format("{0}_memberregistration_{1}_warnings_{2}.csv", schemePublicInfo.ApprovalNo, year, submissionDateTime.ToString("ddMMyyyy_HHmm"));

                string csv = csvWriter.Write(errors);
                byte[] fileContent = new UTF8Encoding().GetBytes(csv);
                return File(fileContent, "text/csv", CsvFilenameFormat.FormatFileName(csvFileName));
            }
        }

        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "View submissions history";

            await Task.Yield();
        }
    }
}