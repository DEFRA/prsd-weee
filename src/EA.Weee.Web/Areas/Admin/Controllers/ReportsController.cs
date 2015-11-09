namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using ViewModels.Home;
    using ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Shared;

    public class ReportsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;

        public ReportsController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        // GET: Admin/Reports
        public async Task<ActionResult> Index()
        {
            using (var client = apiClient())
            {
                var userStatus = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                switch (userStatus)
                {
                    case UserStatus.Active:
                        return RedirectToAction("ChooseReport", "Reports");
                    case UserStatus.Inactive:
                    case UserStatus.Pending:
                    case UserStatus.Rejected:
                        return RedirectToAction("InternalUserAuthorisationRequired", "Account", new { userStatus });
                    default:
                        throw new NotSupportedException(
                            string.Format("Cannot determine result for user with status '{0}'", userStatus));
                }
            }
        }

        [HttpGet]
        public ActionResult ChooseReport()
        {
            var model = new ChooseReportViewModel();
            return View("ChooseReport", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseReport(ChooseReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.SelectedValue)
            {
                case Reports.ProducerDetails:
                    return RedirectToAction("ProducerDetails", "Reports");

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProducerDetails()
        {
            using (var client = apiClient())
            {
                try
                {
                    var allYears = await client.SendAsync(User.GetAccessToken(), new GetAllComplianceYears());
                    var allSchemes = await client.SendAsync(User.GetAccessToken(), new GetAllApprovedSchemes());
                    var appropriateauthorities = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());

                    List<SelectListItem> years = new List<SelectListItem>();
                    years.Add(new SelectListItem
                    {
                        Text = "All",
                        Value = "All"
                    });

                    years.AddRange(allYears.Select(item => new SelectListItem
                    {
                        Value = item.ToString(), Text = item.ToString()
                    }));

                    List<SelectListItem> schemes = new List<SelectListItem>();
                    schemes.Add(new SelectListItem
                    {
                        Text = "All",
                        Value = "All"
                    });

                    schemes.AddRange(allSchemes.Select(item => new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.SchemeName
                    }));

                    List<SelectListItem> aas = new List<SelectListItem>();
                    aas.Add(new SelectListItem
                    {
                        Text = "All",
                        Value = "All"
                    });

                    aas.AddRange(appropriateauthorities.Select(item => new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name
                    }));

                    ProducerDetailsViewModel model = new ProducerDetailsViewModel
                    {
                        ComplianceYears = years,
                        SchemeNames = schemes,
                        AppropriateAuthorities = aas
                    };
                    return View("ProducerDetails", model);
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);
                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    return View("ProducerDetails");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerDetails(ProducerDetailsViewModel model)
        {
            using (var client = apiClient())
            {
                var allYears = await client.SendAsync(User.GetAccessToken(), new GetAllComplianceYears());
                var allSchemes = await client.SendAsync(User.GetAccessToken(), new GetAllApprovedSchemes());
                var appropriateauthorities = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());

                List<SelectListItem> years = new List<SelectListItem>();
                years.Add(new SelectListItem
                {
                    Text = "All",
                    Value = "All"
                });

                years.AddRange(allYears.Select(item => new SelectListItem
                {
                    Value = item.ToString(),
                    Text = item.ToString()
                }));

                List<SelectListItem> schemes = new List<SelectListItem>();
                schemes.Add(new SelectListItem
                {
                    Text = "All",
                    Value = "All"
                });

                schemes.AddRange(allSchemes.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.SchemeName
                }));

                List<SelectListItem> aas = new List<SelectListItem>();
                aas.Add(new SelectListItem
                {
                    Text = "All",
                    Value = "All"
                });

                aas.AddRange(appropriateauthorities.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                }));

                model.ComplianceYears = years;
                model.SchemeNames = schemes;
                model.AppropriateAuthorities = aas;
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return View(model);
        }
    }
}