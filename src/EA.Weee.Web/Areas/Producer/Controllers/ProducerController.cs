namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using iText.Kernel.XMP.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Mvc;

    [AuthorizeRouteClaims("directRegistrantId", WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerController : ExternalSiteController
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public ProducerController(
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IMapper mapper)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            return View();
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

            var yearParam = year ?? years.First();

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

            var source = new SmallProducerSubmissionMapperData
            {
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

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

            var source = new SmallProducerSubmissionMapperData
            {
                SmallProducerSubmissionData = SmallProducerSubmissionData
            };

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

        private Task SetViewBreadcrumb() => SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.ViewOrganisation);
        private Task SetHistoricBreadcrumb() => SetBreadcrumb(SmallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.HistoricProducerRegistrationSubmission);

        private Task SetTabsCrumb(int? year = null) => year.HasValue ? SetHistoricBreadcrumb() : SetViewBreadcrumb();

        private IEnumerable<int> YearsDropdownData(SmallProducerSubmissionData data)
        {
            return data.SubmissionHistory.OrderByDescending(x => x.Key).Select(x => x.Key);
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