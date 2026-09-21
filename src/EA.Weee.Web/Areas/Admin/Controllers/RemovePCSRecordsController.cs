namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Shared.Paging;
    using EA.Weee.Requests;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemovePCSRecords;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class RemovePCSRecordsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private const int pageSize = 10;
        private readonly BreadcrumbService breadcrumb;

        public RemovePCSRecordsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int page = 1, string selScheme = null, string selYear = null)
        {
            await SetBreadcrumb();

            using (var client = apiClient())
            {
                var complianceYears = await GetAllYears(client);
                var selectedYear = selYear == "0" || selYear == null ? (int?)null : Convert.ToInt32(selYear);
                var selectedYearValue = selectedYear?.ToString() ?? "0";

                var schemeList = await GetSchemesForYear(client, null);
                var selectedName = string.IsNullOrEmpty(selScheme) || selScheme == "All PCSs" ? null : selScheme;
                var selectedSchemeValue = selectedName ?? "All PCSs";

                var searchResults = await GetPCSResults(client,
                                                        selectedYear,
                                                        selectedName,
                                                        page);

                var model = new RemovePCSRecordsViewModel(selectedYearValue,
                                                          selectedSchemeValue,
                                                          complianceYears,
                                                          schemeList,
                                                          searchResults);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RemovePCSRecordsViewModel model, int page = 1)
        {
            await SetBreadcrumb();

            using (var client = apiClient())
            {
                var complianceYears = await GetAllYears(client);
                var selectedYear = model.SelectedYear == "0" ? (int?)null : Convert.ToInt32(model.SelectedYear);

                var selectedName = string.IsNullOrEmpty(model.SelectedScheme) || model.SelectedScheme == "All PCSs" ? null : model.SelectedScheme;
                var schemeList = await GetSchemesForYear(client, null);

                var searchResults = await GetPCSResults(client,
                                                        selectedYear,
                                                        selectedName,
                                                        page);

                model = new RemovePCSRecordsViewModel(model.SelectedYear,
                                                      model.SelectedScheme,
                                                      complianceYears,
                                                      schemeList,
                                                      searchResults);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmDeletion(Guid schemeId, string schemeName, string approvalNumber, int complianceYear)
        {
            await SetBreadcrumb();

            using (var client = apiClient())
            {
                var model = new RemovePCSRecordConfirmDeletionViewModel
                {
                    SchemeId = schemeId,
                    PCSName = schemeName,
                    ApprovalNumber = approvalNumber,
                    ComplianceYear = complianceYear
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeletion(RemovePCSRecordConfirmDeletionViewModel model)
        {
            using (var client = apiClient())
            {
                var request = new GetReturnValueFromRemovingPCSRecords(model.SchemeId, model.ComplianceYear);
                var result = await client.SendAsync(User.GetAccessToken(), request);

                if (result == 0)
                {
                    return RedirectToAction("Deleted", new
                    {
                        schemeId = model.SchemeId,
                        schemeName = model.PCSName,
                        approvalNumber = model.ApprovalNumber,
                        complianceYear = model.ComplianceYear
                    });
                }
                else
                {
                    return RedirectToAction("DeleteFailure", new
                    {
                        schemeId = model.SchemeId,
                        schemeName = model.PCSName,
                        approvalNumber = model.ApprovalNumber,
                        complianceYear = model.ComplianceYear
                    });
                }
            }
        }

        [HttpGet]
        public ActionResult Deleted(Guid schemeId, string schemeName, string approvalNumber, int complianceYear)
        {
            var model = GenerateRemovePCSRecordConfirmDeletion(schemeId, schemeName, approvalNumber, complianceYear);

            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteFailure(Guid schemeId, string schemeName, string approvalNumber, int complianceYear)
        {
            var model = GenerateRemovePCSRecordConfirmDeletion(schemeId, schemeName, approvalNumber, complianceYear);

            return View(model);
        }

        private RemovePCSRecordConfirmDeletionViewModel GenerateRemovePCSRecordConfirmDeletion(Guid schemeId, string schemeName, string approvalNumber, int complianceYear)
        {
            var model = new RemovePCSRecordConfirmDeletionViewModel
            {
                SchemeId = schemeId,
                ComplianceYear = complianceYear,
                PCSName = schemeName,
                ApprovalNumber = approvalNumber
            };

            return model;
        }

        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "Remove PCS records";

            await Task.Yield();
        }

        private async Task<List<SelectListItem>> GetAllYears(IWeeeClient client)
        {
            var request = new GetSchemeComplianceYearsExceedingRetentionPeriod();
            var complianceYears = await client.SendAsync(User.GetAccessToken(), request);

            return complianceYears.Select(x => new SelectListItem { Value = x.ToString(), Text = x.ToString() })
                                  .Prepend(new SelectListItem { Value = "0", Text = "All Years" })
                                  .ToList();
        }

        private async Task<List<SelectListItem>> GetSchemesForYear(IWeeeClient client, int? selectedYear)
        {
            var request = new GetSchemesForComplianceYear(selectedYear);
            var schemesList = await client.SendAsync(User.GetAccessToken(), request);

            return schemesList.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() })
                              .Prepend(new SelectListItem { Value = "All PCSs", Text = "All PCSs" })
                              .OrderBy(x => x.Text)
                              .ToList();
        }

        private async Task<IPagedList<RemovePCSRowViewModel>> GetPCSResults(IWeeeClient client, int? selectedYear, string selectedName, int pageNumber)
        {
            var request = new GetSchemeDataExceedingRetentionPeriodRequest(selectedYear, selectedName);
            var searchResults = await client.SendAsync(User.GetAccessToken(), request);

            var result = searchResults.OrderBy(r => r.ComplianceYear)
                                      .ThenBy(r => r.SchemeName)
                                      .Select(r => new RemovePCSRowViewModel
                                      {
                                          SchemeId = r.SchemeId,
                                          SchemeName = r.SchemeName,
                                          ApprovalNumber = r.ApprovalNumber,
                                          ComplianceYear = r.ComplianceYear
                                      })
                                      .ToList();

            return result.ToPagedList(pageNumber - 1, pageSize, result.Count);
        }
    }
}