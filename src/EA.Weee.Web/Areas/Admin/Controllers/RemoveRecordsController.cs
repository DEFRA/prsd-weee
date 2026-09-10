namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using EA.Prsd.Core.Web.ApiClient;
    using EA.Prsd.Core.Web.Mvc.Extensions;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared.Paging;
    using EA.Weee.Requests;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class RemoveRecordsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAppConfiguration configuration;
        private const int DefaultPageSize = 5;
        private readonly BreadcrumbService breadcrumb;
        private const string AllPCSs = "All PCSs";
        private const string AllYears = "All Years";

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

                default:
                    throw new NotSupportedException();
            }
        }

        private void PopulateViewModelPossibleValues(RemoveRecordsViewModel viewModel)
        {
            viewModel.PossibleValues = new List<string>() { InternalRemoveRecordsActivity.RemovePCS, InternalRemoveRecordsActivity.RemoveAATF };
        }

        [HttpGet]
        public async Task<ActionResult> RemovePCS(int page = 1, string selScheme = null, string selYear = null)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb();

                List<int> allYears = await client.SendAsync(User.GetAccessToken(), new GetSchemeComplianceYearsExceedingRetentionPeriod());
                List<string> allYearsStrings = allYears.ConvertAll(i => i.ToString());
                allYearsStrings.Insert(0, AllYears);
                int? selectedYear = int.TryParse(selYear, out int parsedValue) ? parsedValue : (int?)null;

                GetSchemesForComplianceYear getSchemesRequest = new GetSchemesForComplianceYear(null);
                List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
                schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = AllPCSs });

                var selectedName = ((string.IsNullOrEmpty(selScheme)) || (selScheme == Guid.Empty.ToString()) || (selScheme == "AllPCSs")) ? null : selScheme;

                var request = new GetSchemeDataExceedingRetentionPeriod(selectedYear, selectedName);
                var searchResults = await client.SendAsync(User.GetAccessToken(), request);
                var totalRecords = searchResults.Count();

                searchResults = searchResults.OrderBy(r => r.ComplianceYear)
                                             .ThenBy(r => r.SchemeName)
                                             .ToList();

                var results = new RemovePCSRecordsListViewModel
                {
                    SelectedYear = AllYears,
                    SelectedSchemeName = AllPCSs,
                    SchemeData = searchResults.ToPagedList(page - 1, DefaultPageSize, totalRecords)
                };

                RemovePCSRecordsFilterViewModel model = new RemovePCSRecordsFilterViewModel
                {
                    ComplianceYears = new SelectList(allYearsStrings),
                    SchemeNames = new SelectList(schemes, "Id", "SchemeName"),
                    SelectedYear = allYearsStrings.FirstOrDefault(),
                    SelectedScheme = "AllPCSs",
                    Results = results
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePCS(RemovePCSRecordsFilterViewModel model, int page = 1)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb();

                List<int> allYears = await client.SendAsync(User.GetAccessToken(), new GetSchemeComplianceYearsExceedingRetentionPeriod());
                List<string> allYearsStrings = allYears.ConvertAll(i => i.ToString());
                allYearsStrings.Insert(0, AllYears);
                int? selectedYear = int.TryParse(model.SelectedYear, out int parsedValue) ? parsedValue : (int?)null;

                GetSchemesForComplianceYear getSchemesRequest = new GetSchemesForComplianceYear(selectedYear);
                List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
                schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = AllPCSs });

                var selectedName = ((string.IsNullOrEmpty(model.SelectedScheme)) || (model.SelectedScheme == Guid.Empty.ToString()) || (model.SelectedScheme == "AllPCSs")) ? null : model.SelectedScheme;

                model.ComplianceYears = new SelectList(allYearsStrings);
                model.SchemeNames = new SelectList(schemes, "Id", "SchemeName");
                model.SelectedYear = selectedYear.ToString();
                model.SelectedScheme = selectedName;

                var request = new GetSchemeDataExceedingRetentionPeriod(selectedYear, selectedName);
                var searchResults = await client.SendAsync(User.GetAccessToken(), request);
                var totalRecords = searchResults.Count();

                searchResults = searchResults.OrderBy(r => r.ComplianceYear)
                                             .ThenBy(r => r.SchemeName)
                                             .ToList();

                if (selectedName == null)
                {
                    selectedName = AllPCSs;
                }

                var results = new RemovePCSRecordsListViewModel
                {
                    SelectedYear = selectedYear.ToString() ?? AllYears,
                    SelectedSchemeName = selectedName,
                    SchemeData = searchResults.ToPagedList(page - 1, DefaultPageSize, totalRecords)
                };

                model.SelectedYear = selectedYear.ToString();
                model.SelectedScheme = selectedName;
                model.Results = results;

                return View(model);
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
                    GetSchemesForComplianceYear getSchemesRequest = new GetSchemesForComplianceYear(complianceYear);
                    List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
                    schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = AllPCSs });
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

        [HttpGet]
        public async Task<ActionResult> ConfirmDeletion(Guid id, int complianceYear)
        {
            List<SchemeData> schemes = null;

            using (var client = apiClient())
            {
                await SetBreadcrumb();

                GetSchemesForComplianceYear getSchemesRequest = new GetSchemesForComplianceYear(complianceYear);
                schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
            }

            var pcsName = schemes.Where(s => s.Id == id).Select(s => s.SchemeName).FirstOrDefault();
            var approvalNumber = schemes.Where(s => s.Id == id).Select(s => s.ApprovalName).FirstOrDefault();

            RemovePCSRecordsConfirmDeletionViewModel model = new RemovePCSRecordsConfirmDeletionViewModel
            {
                SchemeId = id,
                ComplianceYear = complianceYear,
                PCSName = pcsName,
                ApprovalNumber = approvalNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeletion(RemovePCSRecordsConfirmDeletionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                try
                {
                    var request = new GetReturnValueFromRemovingPCSRecords(model.SchemeId, model.ComplianceYear);
                    var result = await client.SendAsync(User.GetAccessToken(), request);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    return View(model);
                }
            }

            return RedirectToAction("Deleted",
                new { id = model.SchemeId, complianceYear = model.ComplianceYear });
        }

        [HttpGet]
        public async Task<ActionResult> Deleted(Guid id, int complianceYear)
        {
            List<SchemeData> schemes = null;

            using (var client = apiClient())
            {
                await SetBreadcrumb();

                GetSchemes getSchemesRequest = new GetSchemes(FilterType.ApprovedOrWithdrawn);
                schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
            }

            var pcsName = schemes.Where(s => s.Id == id).Select(s => s.SchemeName).FirstOrDefault();
            var approvalNumber = schemes.Where(s => s.Id == id).Select(s => s.ApprovalName).FirstOrDefault();

            RemovePCSRecordsConfirmDeletionViewModel model = new RemovePCSRecordsConfirmDeletionViewModel
            {
                SchemeId = id,
                ComplianceYear = complianceYear,
                PCSName = pcsName,
                ApprovalNumber = approvalNumber
            };

            return View(model);
        }

        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "Remove PCS records";

            await Task.Yield();
        }
    }
}