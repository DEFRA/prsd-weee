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
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Interfaces;
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

            return View(viewModel);
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
            await SetBreadcrumb();
            
            RemovePCSRecordsFilterViewModel model = new RemovePCSRecordsFilterViewModel();

            using (var client = apiClient())
            {
                // Get the list of years for the SelectList and add an "All items" option
                var allYearsStrings = await GetAllYears(client);

                // Get the selected item for the Years SelectList or null if the "All items" option is selected
                var selectedYear = int.TryParse(selYear, out int parsedValue) ? parsedValue : (int?)null;

                // Get the list of Schemes for the SelectList and add an "All items" option
                var schemes = await GetSchemesForYear(client, selectedYear);

                // Get the selected item for the Schemes SelectList or null if the "All items" option is selected
                var selectedName = ((string.IsNullOrEmpty(selScheme)) || (selScheme == AllPCSs)) ? null : selScheme;

                // Get the data for the results table using the selected items and sort the results
                var searchResults = await GetSortedResults(client, selectedYear, selectedName);

                // Get the selected items for the SelectLists
                var stringYear = selectedYear == null ? AllYears : selectedYear.ToString();
                var stringName = String.IsNullOrEmpty(selectedName) ? AllPCSs : selectedName;

                // Populate the model
                model = PopulateRemovePCSRecordsFilterViewModel(model, searchResults, schemes, allYearsStrings, page, stringYear, stringName);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePCS(RemovePCSRecordsFilterViewModel model, int page = 1)
        {
            await SetBreadcrumb();

            string selScheme = model.SelectedScheme;
            string selYear = model.SelectedYear;

            using (var client = apiClient())
            {
                // Get the list of years for the SelectList and add an "All items" option
                var allYearsStrings = await GetAllYears(client);

                // Get the selected item for the Years SelectList or null if the "All items" option is selected
                var selectedYear = int.TryParse(selYear, out int parsedValue) ? parsedValue : (int?)null;

                // Get the list of Schemes for the SelectList and add an "All items" option
                var schemes = await GetSchemesForYear(client, selectedYear);

                // Get the selected item for the Schemes SelectList or null if the "All items" option is selected
                var selectedName = ((string.IsNullOrEmpty(selScheme)) || (selScheme == AllPCSs)) ? null : selScheme;

                // Get the data for the results table using the selected items and sort the results
                var searchResults = await GetSortedResults(client, selectedYear, selectedName);

                // Populate the model
                model = PopulateRemovePCSRecordsFilterViewModel(model, searchResults, schemes, allYearsStrings, page, selYear, selScheme);
            }

            return View(model);
        }

        internal IGetAllYears GetAllYearsStub;
        private async Task<List<string>> GetAllYears(IWeeeClient client)
        {
            if (GetAllYearsStub != null)
            {
                return GetAllYearsStub.GetAllYears(client);
            }
            
            List<int> allYears = await client.SendAsync(User.GetAccessToken(), new GetSchemeComplianceYearsExceedingRetentionPeriod());
            List<string> allYearsStrings = allYears.ConvertAll(i => i.ToString());
            allYearsStrings.Insert(0, AllYears);

            return allYearsStrings;
        }

        internal IGetSchemesForYear GetSchemesForYearStub;
        private async Task<List<SchemeData>> GetSchemesForYear(IWeeeClient client, int? selectedYear)
        {
            if (GetSchemesForYearStub != null)
            {
                return GetSchemesForYearStub.GetSchemesForYear(client, selectedYear);
            }
            
            GetSchemesForComplianceYear getSchemesRequest = new GetSchemesForComplianceYear(selectedYear);
            List<SchemeData> schemes = await client.SendAsync(User.GetAccessToken(), getSchemesRequest);
            schemes.Insert(0, new SchemeData { Id = Guid.Empty, SchemeName = AllPCSs });

            return schemes;
        }

        internal IGetSortedResults GetSortedResultsStub;
        private async Task<List<SchemeDataExceedingRetentionPeriod>> GetSortedResults(IWeeeClient client, int? selectedYear, string selectedName)
        {
            if (GetSortedResultsStub != null)
            {
                return GetSortedResultsStub.GetSortedResults(client, selectedYear, selectedName);
            }
            
            var request = new GetSchemeDataExceedingRetentionPeriod(selectedYear, selectedName);
            var searchResults = await client.SendAsync(User.GetAccessToken(), request);
            searchResults = searchResults.OrderByDescending(r => r.ComplianceYear)
                                         .ThenBy(r => r.SchemeName)
                                         .ToList();

            return searchResults;
        }

        internal IPopulateRemovePCSRecordsFilterViewModel PopulateRemovePCSRecordsFilterViewModelStub;
        private RemovePCSRecordsFilterViewModel PopulateRemovePCSRecordsFilterViewModel(RemovePCSRecordsFilterViewModel model,
            List<SchemeDataExceedingRetentionPeriod> data, 
            List<SchemeData> schemes, 
            List<string> allYearsStrings, 
            int page, 
            string selectedYear,
            string selectedScheme)
        {
            if (PopulateRemovePCSRecordsFilterViewModelStub != null)
            {
                return PopulateRemovePCSRecordsFilterViewModelStub.PopulateRemovePCSRecordsFilterViewModel(model, data, schemes, allYearsStrings, page, selectedYear, selectedScheme);
            }
            
            var results = new RemovePCSRecordsListViewModel
            {
                SelectedYear = selectedYear,
                SelectedSchemeName = selectedScheme,
                SchemeData = data.ToPagedList(page - 1, DefaultPageSize, data.Count())
            };

            model.ComplianceYears = new SelectList(allYearsStrings);
            model.SchemeNames = new SelectList(schemes, "SchemeName", "SchemeName");
            model.SelectedYear = selectedYear;
            model.SelectedScheme = selectedScheme;
            model.Results = results;

            return model;
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

                    if (result == -1)
                    {
                        throw new Exception("There was a problem with the deletion.");
                    }
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    ModelState.AddModelError(String.Empty, ex);
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "There was a problem with the deletion.");
                    return View(model);
                }
            }

            return RedirectToAction("Deleted", new { id = model.SchemeId, complianceYear = model.ComplianceYear });
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