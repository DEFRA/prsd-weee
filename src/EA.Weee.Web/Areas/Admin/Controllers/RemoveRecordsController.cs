namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using AutoMapper;
    using Base;
    using EA.Prsd.Core.Web.ApiClient;
    using EA.Prsd.Core.Web.Mvc.Extensions;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Admin.GetActiveComplianceYears;
    using EA.Weee.Requests.Admin.Reports;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;
    using EA.Weee.Web.Areas.Admin.ViewModels.Submissions;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using Security;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class RemoveRecordsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAppConfiguration configuration;
        private const int DefaultPageSize = 5;
        private readonly BreadcrumbService breadcrumb;
        private const string AllPCSs = "All PCSs";

        public RemoveRecordsController(
            Func<IWeeeClient> apiClient,
            IAppConfiguration configuration,
            BreadcrumbService breadcrumb)
        {
            this.configuration = configuration;
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public ActionResult ChooseActivity()
        {
            RemoveRecordsViewModel viewModel = new RemoveRecordsViewModel();
            PopulateViewModelPossibleValues(viewModel);
            return View("ChooseActivity", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(RemoveRecordsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewModelPossibleValues(viewModel);
                return View(viewModel);
            }

            switch (viewModel.SelectedValue)
            {
                case InternalRemoveRecordsActivity.RemovePCS:
                    return RedirectToAction("RemovePCS");

                //case InternalRemoveRecordsActivity.RemoveAATF:
                //    return RedirectToAction("RemoveAATF");

                default:
                    throw new NotSupportedException();
            }
        }

        private void PopulateViewModelPossibleValues(RemoveRecordsViewModel viewModel)
        {
            viewModel.PossibleValues = new List<string>();

            viewModel.PossibleValues.Add(InternalRemoveRecordsActivity.RemovePCS);
            //viewModel.PossibleValues.Add(InternalRemoveRecordsActivity.RemoveAATF);
        }

        /// <summary>
        /// This method is used by both JS and non-JS users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> RemovePCS()
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb();

                try
                {
                    //Get all the compliance years currently in database and set it to latest one.
                    //Get all the approved PCSs
                    List<int> allYears = await client.SendAsync(User.GetAccessToken(), new GetSchemeComplianceYearsExceedingRetentionPeriod());
                    GetSchemesForComplianceYear getSchemesRequest = new GetSchemesForComplianceYear(FilterType.ApprovedOrWithdrawn, allYears[0]);
                    List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
                    List<string> allYearsStrings = allYears.ConvertAll(i => i.ToString());
                    schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = AllPCSs });
                    schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = String.Empty });
                    allYearsStrings.Insert(0, null);

                    RemovePCSRecordsFilterViewModel model = new RemovePCSRecordsFilterViewModel
                    {
                        ComplianceYears = new SelectList(allYearsStrings),
                        SchemeNames = new SelectList(schemes, "SchemeName", "SchemeName"),
                        SelectedYear = allYearsStrings.FirstOrDefault(),
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> FetchSchemeForComplainceYear(int complianceYear)
        {
            using (var client = apiClient())
            {
                try
                {
                    GetSchemesForComplianceYear getSchemesRequest = new GetSchemesForComplianceYear(FilterType.ApprovedOrWithdrawn, complianceYear);
                    List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
                    schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = AllPCSs });
                    schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = String.Empty });
                    IEnumerable<SelectListItem> schemeNames = new SelectList(schemes, "SchemeName", "SchemeName");

                    return Json(schemeNames, JsonRequestBehavior.AllowGet);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return Json(new { string.Empty });
                }
            }
        }

        /// <summary>
        /// This method is called using AJAX by JS-users.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="name"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> FetchPCSRecordsList(int? year, string name)
        {
            return RetrievePCSRecordsList(year, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="name"></param>
        /// <returns>ActionResult</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ApiBadRequestException"></exception>
        private async Task<ActionResult> RetrievePCSRecordsList(int? year, string name)
        {
            if (Request != null && !Request.IsAjaxRequest())
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
                    if (year == 0)
                    {
                        year = null;
                    }

                    if (name == Guid.Empty.ToString() || String.IsNullOrEmpty(name) || name == AllPCSs)
                    {
                        name = null;
                    }

                    var request = new GetSchemeDataExceedingRetentionPeriod(year, name);
                    var searchResults = await client.SendAsync(User.GetAccessToken(), request);

                    var model = new RemovePCSRecordsListViewModel
                    {
                        SelectedYear = year,
                        SelectedSchemeName = name,
                        SchemeData = searchResults
                    };

                    return PartialView("_removePCSResults", model);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        private async Task<ActionResult> ConfirmDeletion(Guid id, int complianceYear)
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "PCS Submissions history";

            await Task.Yield();
        }
    }
}