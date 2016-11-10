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
    using Core.DataReturns;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Scheme.ViewModels;
    using Services;
    using Services.Caching;
    using ViewModels.Submissions;
    using Web.ViewModels.Shared.Submission;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Admin.GetDataReturnSubmissionChanges;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Shared;
    using GetSchemes = Weee.Requests.Admin.GetSchemes;
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

        [HttpGet]
        public async Task<ActionResult> ChooseSubmissionType()
        {
            using (var client = apiClient())
            {   
                var model = new ChooseSubmissionTypeViewModel
                {
                    PossibleValues = new List<string>
                    {
                        SubmissionType.MemberRegistrations,
                        SubmissionType.EeeOrWeeeDataReturns
                    }
                };

                await SetBreadcrumb();

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseSubmissionType(ChooseSubmissionTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedValue == SubmissionType.EeeOrWeeeDataReturns)
                {
                    return RedirectToAction("DataReturnSubmissionHistory");
                }
                else if (viewModel.SelectedValue == SubmissionType.MemberRegistrations)
                {
                    return RedirectToAction("SubmissionsHistory");
                }
            }

            await SetBreadcrumb();
            viewModel.PossibleValues = new List<string>
            {
                SubmissionType.MemberRegistrations,
                SubmissionType.EeeOrWeeeDataReturns
            };

            return View(viewModel);
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
                    var allYears = await client.SendAsync(User.GetAccessToken(), new GetMemberRegistrationsActiveComplianceYears());

                    GetSchemes getSchemesRequest = new GetSchemes(GetSchemes.FilterType.ApprovedOrWithdrawn);
                    List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);

                    SubmissionsHistoryViewModel model = new SubmissionsHistoryViewModel
                    {
                        ComplianceYears = new SelectList(allYears),
                        SchemeNames = new SelectList(schemes, "Id", "SchemeName"),
                        SelectedYear = allYears.FirstOrDefault(),
                        SelectedScheme = schemes.Count > 0 ? schemes.First().Id : Guid.Empty
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
        public Task<ActionResult> FetchSubmissionResults(int year, Guid schemeId)
        {
            return RetrieveSubmissionResults(year, schemeId, SubmissionsHistoryOrderBy.SubmissionDateDescending);
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="schemeId"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult> GetSubmissionResults(int year, Guid schemeId, SubmissionsHistoryOrderBy orderBy)
        {
            return RetrieveSubmissionResults(year, schemeId, orderBy);
        }

        private async Task<ActionResult> RetrieveSubmissionResults(int year, Guid schemeId, SubmissionsHistoryOrderBy orderBy)
        {
            if (Request != null &&
                !Request.IsAjaxRequest())
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

                    var request = new GetSubmissionsHistoryResults(schemeId, schemeData.OrganisationId,
                        year, orderBy);

                    var searchResults = await client.SendAsync(User.GetAccessToken(), request);

                    var model = new SubmissionsResultsViewModel
                    {
                        Year = year,
                        Scheme = schemeId,
                        Results = searchResults.Data,
                        OrderBy = orderBy
                    };

                    return PartialView("_submissionsResults", model);
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
                IEnumerable<ErrorData> errors =
                    (await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(schemeId, memberUploadId)))
                    .OrderByDescending(e => e.ErrorLevel);

                CsvWriter<ErrorData> csvWriter = csvWriterFactory.Create<ErrorData>();
                csvWriter.DefineColumn("Description", e => e.Description);

                var schemePublicInfo = await cache.FetchSchemePublicInfo(schemeId);
                var csvFileName = string.Format("{0}_memberregistration_{1}_warnings_{2}.csv", schemePublicInfo.ApprovalNo, year, submissionDateTime.ToString("ddMMyyyy_HHmm"));

                string csv = csvWriter.Write(errors);
                byte[] fileContent = new UTF8Encoding().GetBytes(csv);
                return File(fileContent, "text/csv", CsvFilenameFormat.FormatFileName(csvFileName));
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> DataReturnSubmissionHistory()
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb();

                try
                {
                    //Get all the compliance years currently in database and set it to latest one.
                    //Get all the approved PCSs
                    var allYears = await client.SendAsync(User.GetAccessToken(), new GetDataReturnsActiveComplianceYears());

                    GetSchemes getSchemesRequest = new GetSchemes(GetSchemes.FilterType.ApprovedOrWithdrawn);
                    List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);

                    DataReturnSubmissionsHistoryViewModel model = new DataReturnSubmissionsHistoryViewModel
                    {
                        ComplianceYears = new SelectList(allYears),
                        SchemeNames = new SelectList(schemes, "Id", "SchemeName"),
                        SelectedYear = allYears.FirstOrDefault(),
                        SelectedScheme = schemes.Count > 0 ? schemes.First().Id : Guid.Empty
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
        public Task<ActionResult> FetchDataReturnSubmissionResults(int year, Guid schemeId)
        {
            return RetrieveDataReturnsSubmissionResults(year, schemeId, DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending);
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="schemeId"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult> GetDataReturnSubmissionResults(int year, Guid schemeId, DataReturnSubmissionsHistoryOrderBy orderBy)
        {
            return RetrieveDataReturnsSubmissionResults(year, schemeId, orderBy);
        }

        private async Task<ActionResult> RetrieveDataReturnsSubmissionResults(int year, Guid schemeId, DataReturnSubmissionsHistoryOrderBy orderBy)
        {
            if (Request != null &&
                !Request.IsAjaxRequest())
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

                    var getDataReturnSubmissionsHistoryResults = new GetDataReturnSubmissionsHistoryResults(
                        schemeId, schemeData.OrganisationId, year, ordering: orderBy, includeSummaryData: true, compareEeeOutputData: true);

                    DataReturnSubmissionsHistoryResult searchResults = await client.SendAsync(User.GetAccessToken(), getDataReturnSubmissionsHistoryResults);

                    var model = new DataReturnSubmissionsResultsViewModel
                    {
                        Year = year,
                        Scheme = schemeId,
                        Results = searchResults.Data,
                        OrderBy = orderBy
                    };

                    return PartialView("_dataReturnSubmissionsResults", model);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadDataReturnSubmissionEeeChanges(Guid currentSubmission, Guid previousSubmission)
        {
            CSVFileData csvFileData;
            using (IWeeeClient client = apiClient())
            {
                var request = new GetDataReturnSubmissionEeeChangesCsv(currentSubmission, previousSubmission);
                csvFileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            byte[] data = new UTF8Encoding().GetBytes(csvFileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(csvFileData.FileName));
        }

        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "Submissions history";

            await Task.Yield();
        }
    }
}