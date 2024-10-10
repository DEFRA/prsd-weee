namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;

        public ProducerController(
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IMapper mapper,
            IMvcTemplateExecutor templateExecutor,
            IPdfDocumentProvider pdfDocumentProvider)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
            this.templateExecutor = templateExecutor;
            this.pdfDocumentProvider = pdfDocumentProvider;
        }

        //[SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> RepresentedCompanies(Guid organisationId)
        {
            var possibleValues = new List<(Guid DirectRegistrantId, Guid RepresentedCompanyId)>
            {
                (new Guid("425216C0-4AD7-4CE9-9235-82C3C1E26FE7"), new Guid("3091CD16-329D-4111-B1F7-B1F000DAA5E1")),
                (new Guid("324D2CD9-618B-46CD-B272-B1EE00BB0918"), new Guid("8A0C6A16-C218-4620-8CD0-B1EE00BB0918"))
            };

            var model = new RepresentedCompaniesViewModel
            {
                PossibleValues = possibleValues.Select(v => $"{v.DirectRegistrantId}|{v.RepresentedCompanyId}").ToList(),
                OrganisationId = organisationId
            };

            // need to retrieve the direct registrants represented companies by organisation id here 
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RepresentedCompanies(RepresentedCompaniesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var selectedIds = model.SelectedValue.Split('|');
                if (selectedIds.Length == 2 && Guid.TryParse(selectedIds[0], out var directRegistrantId) && Guid.TryParse(selectedIds[1], out var representedCompanyId))
                {
                    return RedirectToAction(nameof(ProducerController.TaskList),
                        typeof(ProducerController).GetControllerName(),
                        new
                        {
                            area = "Producer",
                            organisationId = model.OrganisationId,
                            directRegistrantId
                        });
                }
            }

            return View(model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> TaskList()
        {
            var submission = SmallProducerSubmissionData.CurrentSubmission;

            var model = new TaskListViewModel()
            {
                OrganisationId = SmallProducerSubmissionData.OrganisationData.Id,
                ProducerTaskModels = new List<ProducerTaskModel>
                {
                    new ProducerTaskModel
                    {
                        TaskLinkName = "Organisation details",
                        Complete = submission.OrganisationDetailsComplete,
                        Action = nameof(ProducerSubmissionController.EditOrganisationDetails)
                    },
                    new ProducerTaskModel
                    {
                        TaskLinkName = "Contact details",
                        Complete = submission.ContactDetailsComplete,
                        Action = nameof(ProducerSubmissionController.EditContactDetails)
                    },
                    new ProducerTaskModel
                    {
                        TaskLinkName = "Service of notice",
                        Complete = submission.ServiceOfNoticeComplete,
                        Action = nameof(ProducerSubmissionController.ServiceOfNotice)
                    },
                }
            };

            if (SmallProducerSubmissionData.HasAuthorisedRepresentitive)
            {
                model.ProducerTaskModels.Add(new ProducerTaskModel
                {
                    TaskLinkName = "Represented organisation details",
                    Complete = submission.RepresentingCompanyDetailsComplete,
                    Action = nameof(ProducerSubmissionController.EditRepresentedOrganisationDetails)
                });
            }

            model.ProducerTaskModels.Add(new ProducerTaskModel
            {
                TaskLinkName = "EEE details",
                Complete = submission.EEEDetailsComplete,
                Action = nameof(ProducerSubmissionController.EditEeeeData)
            });

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public async Task<ActionResult> CheckAnswers()
        {
            var source = new SmallProducerSubmissionMapperData()
            {
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>(source);

            await SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);

            return View(model);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> Submissions(int? year = null)
        {
            await SetTabsCrumb(year);

            var years = YearsDropdownData(SmallProducerSubmissionData);

            int? yearParam = year ?? (years.FirstOrDefault() == 0 ? (int?)null : years.First());

            return await OrganisationDetails(yearParam);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> OrganisationDetails(int? year = null)
        {
            await SetTabsCrumb(year);

            var years = YearsDropdownData(SmallProducerSubmissionData);

            var organisationVM = MapDetailsSubmissionYearModel<OrganisationViewModel>(year);

            var vm = new OrganisationDetailsTabsViewModel
            {
                Years = years,
                Year = year,
                ActiveOption = OrganisationDetailsDisplayOption.OrganisationDetails,
                OrganisationViewModel = organisationVM,
                SmallProducerSubmissionData = this.SmallProducerSubmissionData
            };

            return View("ViewOrganisation/OrganisationDetails", vm);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ContactDetails(int? year = null)
        {
            await SetTabsCrumb(year);

            var years = YearsDropdownData(SmallProducerSubmissionData);

            var contactVm = MapDetailsSubmissionYearModel<ContactDetailsViewModel>(year);

            var vm = new OrganisationDetailsTabsViewModel
            {
                Years = years,
                Year = year,
                ActiveOption = OrganisationDetailsDisplayOption.ContactDetails,
                SmallProducerSubmissionData = this.SmallProducerSubmissionData,
                ContactDetailsViewModel = contactVm
            };

            return View("ViewOrganisation/ContactDetails", vm);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> ServiceOfNoticeDetails(int? year = null)
        {
            await SetTabsCrumb(year);

            var years = YearsDropdownData(SmallProducerSubmissionData);

            var serviceOfNoticeViewModel = MapDetailsSubmissionYearModel<ServiceOfNoticeViewModel>(year);

            var vm = new OrganisationDetailsTabsViewModel
            {
                Years = years,
                Year = year,
                ActiveOption = OrganisationDetailsDisplayOption.ServiceOfNoticeDetails,
                SmallProducerSubmissionData = this.SmallProducerSubmissionData,
                ServiceOfNoticeViewModel = serviceOfNoticeViewModel
            };

            return View("ViewOrganisation/ServiceOfNoticeDetails", vm);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> RepresentedOrganisationDetails(int? year = null)
        {
            await SetTabsCrumb(year);

            var years = YearsDropdownData(SmallProducerSubmissionData);

            var representingCompanyDetailsViewModel = MapDetailsSubmissionYearModel<RepresentingCompanyDetailsViewModel>(year);

            var vm = new OrganisationDetailsTabsViewModel
            {
                Years = years,
                Year = year,
                ActiveOption = OrganisationDetailsDisplayOption.RepresentedOrganisationDetails,
                SmallProducerSubmissionData = this.SmallProducerSubmissionData,
                RepresentingCompanyDetailsViewModel = representingCompanyDetailsViewModel
            };

            return View("ViewOrganisation/RepresentedOrganisationDetails", vm);
        }

        [SmallProducerSubmissionContext]
        [HttpGet]
        public async Task<ActionResult> TotalEEEDetails(int? year = null)
        {
            await SetTabsCrumb(year);

            var years = YearsDropdownData(SmallProducerSubmissionData);

            var editEeeDataViewModel = MapDetailsSubmissionYearModel<EditEeeDataViewModel>(year);

            var vm = new OrganisationDetailsTabsViewModel
            {
                Years = years,
                Year = year,
                ActiveOption = OrganisationDetailsDisplayOption.TotalEEEDetails,
                SmallProducerSubmissionData = this.SmallProducerSubmissionData,
                EditEeeDataViewModel = editEeeDataViewModel
            };

            return View("ViewOrganisation/TotalEEEDetails", vm);
        }

        [HttpGet]
        public ActionResult SubmitRegistration()
        {
            return View("SubmitRegistration");
        }

        [HttpGet]
        public ActionResult RegistrationSubmissions()
        {
            return View("SubmitRegistration");
        }

        [HttpGet]
        [SmallProducerSubmissionContext]
        public ActionResult DownloadSubmission()
        {
                var source = new SmallProducerSubmissionMapperData()
                {
                    SmallProducerSubmissionData = SmallProducerSubmissionData
                };

                var model = mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>(source);

                model.IsPdfDownload = true;

                var content = templateExecutor.RenderRazorView(ControllerContext, "DownloadSubmission", model);

                var pdf = pdfDocumentProvider.GeneratePdfFromHtml(content);

                var timestamp = SystemTime.Now;
                var fileName = $"producer_submission{timestamp.ToString(DateTimeConstants.SubmissionTimestamp)}.pdf";

                return File(pdf, "application/pdf", fileName);
        }

        private Task SetViewBreadcrumb() => SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.ViewOrganisation);
        private Task SetHistoricBreadcrumb() => SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.HistoricProducerRegistrationSubmission);

        private Task SetTabsCrumb(int? year = null) => year.HasValue ? SetHistoricBreadcrumb() : SetViewBreadcrumb();

        private IEnumerable<int> YearsDropdownData(SmallProducerSubmissionData data)
        {
            return data.SubmissionHistory
                .Where(x => x.Value.Status == SubmissionStatus.Submitted)
                .OrderByDescending(x => x.Key)
                .Select(x => x.Key);
        }

        private T MapDetailsSubmissionYearModel<T>(int? year)
        {
            return mapper.Map<SubmissionsYearDetails, T>(
               new SubmissionsYearDetails
               {
                   Year = year,
                   SmallProducerSubmissionData = this.SmallProducerSubmissionData
               });
        }
    }
}